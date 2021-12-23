using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using View.Core.Extensions;
using ViewFaceCore.Sharp;

namespace MFAsys
{
    public partial class FormTeaAtt : Form
    {
        string self_CourseName;//课程名称
        string self_CourseId;
        string self_Times;//课次
        public FormTea self_ft;

        //摄像头设备信息集合
        FilterInfoCollection VideoDevices;

        //人脸位置信息集合
        List<Rectangle> FaceRectangles = new List<Rectangle>();

        //人脸对应的年龄集合
        List<int> Ages = new List<int>();

        //性别集合
        List<string> Gender = new List<string>();

        //单人人脸信息
        float[] Face;

        //人脸识别库
        ViewFace ViewFace = new ViewFace();

        // detect取消令牌
        CancellationTokenSource Token_detect { get; set; }

        // check取消令牌
        CancellationTokenSource Token_check { get; set; }

        // 指示是否应关闭窗体
        bool IsClose = false;

        public FormTeaAtt(FormTea f, string CourseId, string CourseName)
        {
            InitializeComponent();
            VideoPlayer.Visible = false; // 隐藏摄像头画面控件

            self_CourseName = CourseName;
            self_CourseId = CourseId;
            self_Times = SelectCourseTimes(self_CourseId);
            self_ft = f;
            //签到状态图片显示
            pictureBox2.Visible = false;
            pictureBox3.Visible = true;
            textBox2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void FormTeaAtt_Load(object sender, EventArgs e)
        {
            VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            comboBox1.Items.Clear();

            foreach (FilterInfo info in VideoDevices)
            { comboBox1.Items.Add(info.Name); }

            if (comboBox1.Items.Count > 0)
            { comboBox1.SelectedIndex = 0; }
        }

        //窗体关闭时，关闭摄像头
        private void FormTeaAtt_Closing(object sender, FormClosingEventArgs e)
        {
            Token_detect?.Cancel();
            Token_check?.Cancel();
            if (!IsClose && VideoPlayer.IsRunning)// 若摄像头开启时，点击关闭是暂不关闭，并设置关闭窗口的标识，待摄像头等设备关闭后，再关闭窗体。
            {
                e.Cancel = true;
                IsClose = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public string SelectCourseTimes(string id)
        {
            string sql = "select * from RecordAttend where IdCourse = '" + id + "'";
            Dao dao = new Dao();
            IDataReader reader = dao.read(sql);
            if (reader.Read())
            {
                string times;
                times = reader["TimesCourse"].ToString();
                return times;
            }
            else
            {
                MessageBox.Show("未找到该课程", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                string Null = "";
                return Null;
            }
            reader.Close();//关闭连接
        }

        private void btn_start_Click(object sender, EventArgs e)
        {

            if (VideoPlayer.IsRunning)//首先检查是否处于运行状态，如果不是则显示”打开摄像头并识别人脸“
            {
                Token_detect?.Cancel();
                btn_start.Text = "下一个";
                /*for(int i = 0;i < Face.Length;i++)
                    Console.WriteLine(Face[i]);*/
                string varchar64_new = Format.FloatsToVarcharMax_new(Face);//将人脸信息转化为字符串
                Token_check = new CancellationTokenSource();//用于控制是否线程的运行与终止
                StartChecker(Token_detect.Token, varchar64_new);//从数据库中查找相似的人脸信息
                //UpdateSelfInfo(varchar64_new);//将字符串形式的人脸信息 存入数据库
            }
            else//如果是则显示”确认“，并调用摄像头
            {
                Token_check?.Cancel();//@@@@@@@@@@@@@@@@@@@@@@
                if (comboBox1.SelectedIndex == -1)
                    return;
                FilterInfo info = VideoDevices[comboBox1.SelectedIndex];//设备信息
                VideoCaptureDevice videoCapture = new VideoCaptureDevice(info.MonikerString);
                VideoPlayer.VideoSource = videoCapture;
                VideoPlayer.Start();
                btn_start.Text = "确认";
                Token_detect = new CancellationTokenSource();//用于控制是否线程的运行与终止
                StartDetector(Token_detect.Token);//Token_detect.Token，第一个Token_detect是对象名称，第二个Token是对象的属性
            }
        }

        private void btn_end_Click(object sender, EventArgs e)
        {
            //记录未签到同学
            string sql_create = "create view CertainClassAbsentStudent as select * from RecordAttend where StatusCertainAbsence = '1' ";
            Dao dao_create = new Dao();
            dao_create.execute(sql_create);//返回受影响的行数
            string sql_select = "select * from CertainClassAbsentStudent";
            Dao dao_select = new Dao();
            IDataReader reader_select = dao_select.read(sql_select);
            while (reader_select.Read())
            {
                string Ids, Sta, Tco, Sum;
                Ids = reader_select["IdStudent"].ToString();
                Sta = reader_select["StatusAbsence"].ToString();
                Tco = reader_select["TimesCourse"].ToString();
                Sum = reader_select["SumAbsence"].ToString();
                int Tco_int = int.Parse(Tco);
                string Tco_int_plus1 = (Tco_int + 1).ToString();
                int Sum_int = int.Parse(Sum);
                string Sum_int_plus1 = (Sum_int + 1).ToString();

                //为未签到同学记录，直接更改x[Tco_int]
                //Sta = Sta.Remove(Tco_int, 1).Insert(Tco_int, "0");//因为数据库中已将签到记录所有位置0，因此不需要将缺席状态0更新
                //string sql_update2 = "update CertainClassAbsentStudent set StatusAbsence = '" + Sta + "', TimesCourse = '" + Tco_int_plus1 + "', SumAbsence = '" + Sum_int_plus1 + "'  where IdStudent = '" + Ids + "'";
                string sql_update2 = "update CertainClassAbsentStudent set TimesCourse = '" + Tco_int_plus1 + "', SumAbsence = '" + Sum_int_plus1 + "'，StatusCertainAbsence = '0'  where IdStudent = '" + Ids + "'";
                Dao dao_update2 = new Dao();
                if (dao_update2.execute(sql_update2) != 0)//返回受影响的行数
                {
                    MessageBox.Show("登记完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        //更新数据库中学生考勤信息
        private void UpdateStudentInfo(string Ids)
        {
            //label显示
            label2.Text = self_CourseName;
            label4.Text = self_Times;
            label6.Text = Ids;

            //签到状态图片显示
            pictureBox2.Visible = true;
            pictureBox3.Visible = false;

            //记录考勤
            string sql_select2 = "select * from CertainClassStudent where IdStudent = '" + Ids + "'";
            Dao dao_select2 = new Dao();
            IDataReader reader_select2 = dao_select2.read(sql_select2);
            if (reader_select2.Read())
            {
                string Sta, Tco;
                Sta = reader_select2["StatusAbsence"].ToString();
                Tco = reader_select2["TimesCourse"].ToString();
                int Tco_int = int.Parse(Tco);
                string Tco_int_plus1 = (Tco_int + 1).ToString();

                //为已签到同学记录，直接更改x[Tco_int]
                Sta = Sta.Remove(Tco_int, 1).Insert(Tco_int, "1");
                //StatusCertainAbsence属性用于表示签到处理状态，0表示没有问题，1表示有问题，将进行缺勤人员登记前的缺勤人员标记
                string sql_update1 = "update CertainClassStudent set StatusAbsence = '" + Sta + "', TimesCourse = '" + Tco_int_plus1 + "'  where IdStudent = '" + Ids + "'";
                Dao dao_update1 = new Dao();
                if (dao_update1.execute(sql_update1) != 0)//返回受影响的行数
                {
                    string sql_delete1 = "delete from CertainClassStudent where IdStudent = '" + Ids + "'";
                    Dao dao_delete1 = new Dao();
                    if (dao_delete1.execute(sql_delete1) != 0)//返回受影响的行数
                    {
                        MessageBox.Show("更新出席成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        string sql_update2 = "update CertainClassStudent set  StatusCertainAbsence = '1'";
                        Dao dao_update2 = new Dao();
                        if (dao_update2.execute(sql_update2) != 0)
                        {
                            MessageBox.Show("更新缺席成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        string sql_drop = "drop view CertainClassStudent";
                        Dao dao_drop = new Dao();
                        if (dao_drop.execute(sql_drop) != 0)
                            MessageBox.Show("删除视图成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                reader_select2.Close();
            }
        }

        //持续检测一次人脸，直到停止。
        private async void StartDetector(CancellationToken token)
        {

            //token.IsCancellationRequested：获取是否已请求取消此标记。如果此令牌已请求取消，则为 true；否则为 false。
            while (VideoPlayer.IsRunning && !token.IsCancellationRequested)
            {


                // 获取摄像头画面 
                Bitmap bitmap = VideoPlayer.GetCurrentVideoFrame();
                if (bitmap != null)//如果有画面
                {
                    FaceRectangles.Clear(); // 人脸信息集合（年龄、性别、id）
                    Ages.Clear(); //年龄
                    //Pids.Clear();//id

                    var infos = await ViewFace.FaceTrackAsync(bitmap); // 识别一个画面中的全部人脸
                    if (infos.Length > 1)
                    {
                        MessageBox.Show("请确保每次仅一人进行人脸信息采集", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    foreach (var info in infos)//因为可以在一个画面中检测到多张人脸，所以用foreach
                    {
                        FaceRectangles.Add(info.Location);
                        //Pids.Add(info.Pid);//Pid一直在增加

                        ViewFaceCore.Sharp.Model.FaceInfo fin = new ViewFaceCore.Sharp.Model.FaceInfo() { Location = info.Location, Score = info.Score };

                        Task<ViewFaceCore.Sharp.Model.FaceMarkPoint[]> fmp = ViewFace.FaceMarkAsync(bitmap, fin);

                        Task<int> age = ViewFace.FaceAgePredictorAsync(bitmap, await fmp);
                        Ages.Add(await age);

                        Task<ViewFaceCore.Sharp.Model.Gender> gender = ViewFace.FaceGenderPredictorAsync(bitmap, await fmp);
                        Gender.Add((await gender).ToDescription());

                        Face = ViewFace.Extract(bitmap, await fmp);

                    }

                    //using:当在某个代码段中使用了类的实例，而希望无论因为什么原因，只要离开了这个代码段就自动调用这个类实例的Dispose
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        // 如果有人脸
                        if (FaceRectangles.Any())
                        {
                            g.DrawRectangles(new Pen(Color.DeepSkyBlue, 4), FaceRectangles.ToArray());//在 bitmap 上绘制出人脸的位置信息

                            for (int i = 0; i < FaceRectangles.Count; i++)
                            {
                                g.DrawString($"{Ages[i]} 岁 | {Gender[i]} ", new Font("微软雅黑", 24), Brushes.DeepSkyBlue, new PointF(FaceRectangles[i].X + FaceRectangles[i].Width + 24, FaceRectangles[i].Y));
                            }
                        }

                    }
                }
                //如果没有画面，对应122左右
                else
                { await Task.Delay(10); }
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = bitmap;
            }

            VideoPlayer?.SignalToStop();
            VideoPlayer?.WaitForStop();
            if (IsClose)
            {
                Close();
            }
        }

        //持续检测一次人脸，直到停止。
        private void StartChecker(CancellationToken token, string varchar64_new)
        {
            float[] new_face = Format.VarcharMaxToFloats_new(varchar64_new);


            string sql_drop = "drop view CertainClassStudent";
            Dao dao_drop = new Dao();
            if (dao_drop.execute(sql_drop) != 0)
                MessageBox.Show("删除视图成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);



            //创建只包含特定课程学生记录的  视图
            string sql_create = "create view CertainClassStudent as select * from RecordAttend where IdCourse = '" + self_CourseId + "'";
            Dao dao_create = new Dao();
            dao_create.execute(sql_create);//返回受影响的行数

            //token.IsCancellationRequested：获取是否已请求取消此标记。如果此令牌已请求取消，则为 true；否则为 false。
            while (!token.IsCancellationRequested)
            {
                string sql_select1 = "select * from Student";//必须是Student，因为只有Student才有CroppedFace
                Dao dao_select1 = new Dao();
                IDataReader reader_select1 = dao_select1.read(sql_select1);

                while (reader_select1.Read())
                {
                    string Cfa = reader_select1["CroppedFace"].ToString();
                    float[] old_face = Format.VarcharMaxToFloats_new(Cfa);
                    bool flag = ViewFace.IsSelf(ViewFace.Similarity(old_face, new_face));
                    if (!flag)
                        continue;
                    else
                    {
                        string Ids = reader_select1["Id"].ToString();
                        UpdateStudentInfo(Ids);
                    }
                }
                reader_select1.Close();
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        { }
    }
}
