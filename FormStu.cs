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
    public partial class FormStu : Form
    {
        string self_id;

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

        // 取消令牌
        CancellationTokenSource Token { get; set; }

        // 指示是否应关闭窗体
        bool IsClose = false;
        public FormStu(string id)
        {
            InitializeComponent();
            VideoPlayer.Visible = false; // 隐藏摄像头画面控件

            //先赋值，后table。顺序不能变
            self_id = id;
            TableStu();
            timer1.Start();
            textBox2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            dataGridView1.Visible = false;
        }

        private void FormStu_Load(object sender, EventArgs e)
        {
            VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            comboBox1.Items.Clear();

            foreach (FilterInfo info in VideoDevices)
            { comboBox1.Items.Add(info.Name); }

            if (comboBox1.Items.Count > 0)
            { comboBox1.SelectedIndex = 0; }
        }

        //窗体关闭时，关闭摄像头
        private void FormStu_Closing(object sender, FormClosingEventArgs e)
        {
            Token?.Cancel();
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

        public void TableStu()
        {
            string sql = "select * from Student where Id = '" + self_id + "'";
            Dao dao = new Dao();
            IDataReader reader = dao.read(sql);
            reader.Read();
            label5.Text = reader["Id"].ToString();
            label6.Text = reader["Name"].ToString();
            label7.Text = reader["Class"].ToString();
            label8.Text = reader["FeatureIndex"].ToString();
            reader.Close();//关闭连接
        }

        private void btn_input_Click(object sender, EventArgs e)
        {
            if (VideoPlayer.IsRunning)//首先检查是否处于运行状态，如果不是则显示”打开摄像头并识别人脸“
            {
                Token?.Cancel();
                btn_input.Text = "录入/更改人脸信息";

                string varchar64_new = Format.FloatsToVarcharMax_new(Face);//将人脸信息转化为字符串
                UpdateSelfInfo(varchar64_new);//将字符串形式的人脸信息 存入数据库
            }
            else//如果是则显示”关闭摄像头“，并调用摄像头
            {
                if (comboBox1.SelectedIndex == -1)
                    return;
                FilterInfo info = VideoDevices[comboBox1.SelectedIndex];//设备信息
                VideoCaptureDevice videoCapture = new VideoCaptureDevice(info.MonikerString);
                VideoPlayer.VideoSource = videoCapture;
                VideoPlayer.Start();
                btn_input.Text = "确认";
                Token = new CancellationTokenSource();//用于控制是否线程的运行与终止
                StartDetector(Token.Token);//Token.Token，第一个Token是对象名称，第二个Token是对象的属性
            }
        }

        //更新数据库中人脸信息
        private void UpdateSelfInfo(string varchar64_new)
        {
            string sql_update = "update Student set FeatureIndex = '" + self_id + "' , CroppedFace =  '" + varchar64_new + "' where Id = '" + self_id + "'";
            //MessageBox.Show(sql_update);
            Dao dao = new Dao();
            int flag_update = dao.execute(sql_update);//返回受影响的行数
            if (flag_update > 0)
            {
                //MessageBox.Show("录入/修改成功");
                TableStu();
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

                        /*Task<float[]> ffloat = ViewFace.ExtractAsync(bitmap, await fmp);
                        Face.Add(await ffloat);*/


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

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();//结束整个程序
        }

        private void 修改密码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            FormStuPas fps = new FormStuPas(self_id);
            fps.Show();
        }

        private void 查看考勤记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Visible = true;
            string sql_select1 = "select * from RecordAttend where IdStudent =  '" + self_id + "'";
            Dao dao_select1 = new Dao();
            IDataReader reader_select1 = dao_select1.read(sql_select1);
            while (reader_select1.Read())
            {
                /*
                 Course:
                    Id,
                    NameCourse,
                    Credit,
                    NameTeacher

                RecordAttend:
                    IdStudent,
                    IdCourse,
                    TimesCourse,
                    StatusAbsence,
                    SumAbsence
                 */
                string Ids, Idc, Tco, Sta, Sum;
                Ids = reader_select1["IdStudent"].ToString();
                Idc = reader_select1["IdCourse"].ToString();
                Tco = reader_select1["TimesCourse"].ToString();
                Sta = reader_select1["StatusAbsence"].ToString();
                Sum = reader_select1["SumAbsence"].ToString();

                //课次，string转int
                int Tco_int;
                Tco_int = int.Parse(Tco);

                string status = "缺席";
                for (int i = 0; i < Tco_int; i++)
                {
                    if (Sta.Substring(i, 1) == "0")
                    {
                        int certain_i = i + 1;//某一缺席课次，例如first，second，third...
                        string Abt = certain_i.ToString();//转化为string型

                        string sql_select2 = "select * from Course where Id =  '" + Idc + "'";
                        Dao dao_select2 = new Dao();
                        IDataReader reader_select2 = dao_select2.read(sql_select2);
                        if (reader_select2.Read())
                        {
                            string Nco, Nte;
                            Nco = reader_select2["NameCourse"].ToString();
                            Nte = reader_select2["NameTeacher"].ToString();
                            string[] str = { Idc, Nco, Nte, Abt, status };
                            dataGridView1.Rows.Add(str);
                        }
                        else
                        {
                            MessageBox.Show("查无此课程！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        reader_select2.Close();//关闭连接
                    }
                }
            }
            reader_select1.Close();//关闭连接
        }

        private void 系统ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {}
    }
}
