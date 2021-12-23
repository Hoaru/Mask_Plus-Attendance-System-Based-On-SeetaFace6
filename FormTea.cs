using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MFAsys
{
    public partial class FormTea : Form
    {
        string self_IdTeacher;
        string self_NameTeacher;
        string self_IdCourse;
        string self_NameCourse;
        string self_TimesCourse;

        public FormTea(string id)
        {
            InitializeComponent();
            self_IdTeacher = id;
            self_NameTeacher = SelectTeacherName(self_IdTeacher);
            TableCou();
            dataGridView1.Visible = true;//课程
            dataGridView2.Visible = false;//学生
            textBox2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public void TableCou()
        {
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
            dataGridView1.Rows.Clear();

            /*添加教师ToolStripMenuItem.Visible = false;
            修改教师ToolStripMenuItem.Visible = false;
            删除教师ToolStripMenuItem.Visible = false;*/

            string sql = "select * from Course where NameTeacher = '" + self_NameTeacher + "'";
            Dao dao = new Dao();
            IDataReader reader = dao.read(sql);
            while (reader.Read())
            {
                string Idc, Nco, Cre, Nte;
                Idc = reader["Id"].ToString();
                Nco = reader["NameCourse"].ToString();
                Cre = reader["Credit"].ToString();
                Nte = reader["NameTeacher"].ToString();
                string[] str = { Idc, Nco, Cre, Nte };
                dataGridView1.Rows.Add(str);
            }
            reader.Close();//关闭连接
        }

        public void TableStu()
        {
            dataGridView2.Visible = true;
            dataGridView1.Visible = false;
            dataGridView2.Rows.Clear();
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            string sql_select1 = "select * from RecordAttend where IdCourse = '" + self_IdCourse + "'";
            Dao dao1 = new Dao();
            IDataReader reader_select1 = dao1.read(sql_select1);
            while (reader_select1.Read())
            {
                string Ids;
                Ids = reader_select1["IdStudent"].ToString();
                string sql_select2 = "select * from Student where Id = '" + Ids + "'";
                Dao dao_select2 = new Dao();
                IDataReader reader_select2 = dao_select2.read(sql_select2);
                if (reader_select2.Read())
                {
                    string Nst, Cla, Pas, Fin;
                    Nst = reader_select2["Name"].ToString();
                    Cla = reader_select2["Credit"].ToString();
                    Pas = reader_select2["Password"].ToString();
                    Fin = reader_select2["FeatureIndex"].ToString();
                    string[] str = { Ids, Nst, Cla, Pas, Fin };
                    dataGridView2.Rows.Add(str);
                }
                else
                {
                    MessageBox.Show("未找到该学生", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                reader_select2.Close();//关闭连接
            }
            reader_select1.Close();//关闭连接
        }

        public string SelectTeacherName(string id)
        {
            string sql = "select * from Teacher where Id = '" + id + "'";
            Dao dao = new Dao();
            IDataReader reader = dao.read(sql);
            if (reader.Read())
            {
                string Nam;
                Nam = reader["Name"].ToString();
                return Nam;
            }
            else
            {
                MessageBox.Show("未找到该教师", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                string Null = "";
                return Null;
            }
            reader.Close();//关闭连接
        }

        public string SelectTimesCourse(string id)
        {
            string sql = "select * from RecordAttend where IdCourse = '" + id + "'";
            Dao dao = new Dao();
            IDataReader reader = dao.read(sql);
            if (reader.Read())
            {
                string Tco;
                Tco = reader["TimesCourse"].ToString();
                return Tco;
            }
            else
            {
                MessageBox.Show("未找到该课程", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                string Null = "";
                return Null;
            }
            reader.Close();//关闭连接
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 课程目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
            self_IdCourse = "";
            self_NameCourse = "";
            self_TimesCourse = "";
            TableCou();
        }

        private void 签到ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            self_IdCourse = dataGridView1.SelectedCells[0].Value.ToString();
            self_NameCourse = dataGridView1.SelectedCells[1].Value.ToString();
            self_TimesCourse = SelectTimesCourse(self_IdCourse);
            FormTeaAtt fta = new FormTeaAtt(this, self_IdCourse, self_NameCourse);
            fta.Show();
        }

        private void 查看学生ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView2.Visible = true;
            dataGridView1.Visible = false;
            dataGridView2.Rows.Clear();
            self_IdCourse = dataGridView1.SelectedCells[0].Value.ToString();
            self_NameCourse = dataGridView1.SelectedCells[1].Value.ToString();
            self_TimesCourse = SelectTimesCourse(self_IdCourse);
            string sql_select1 = "select * from RecordAttend where IdCourse = '" + self_IdCourse + "' ";
            Dao dao_select1 = new Dao();
            IDataReader reader_select1 = dao_select1.read(sql_select1);
            while (reader_select1.Read())
            {
                string Ids = reader_select1["IdStudent"].ToString();
                string sql_select2 = "select * from Student where Id = '" + Ids + "'";
                Dao dao_select2 = new Dao();
                IDataReader reader_select2 = dao_select2.read(sql_select2);
                if (reader_select2.Read())
                {
                    string Nst, Cla, Pas, Fin;
                    Nst = reader_select2["Name"].ToString();
                    Cla = reader_select2["Class"].ToString();
                    Pas = reader_select2["Password"].ToString();
                    Fin = reader_select2["FeatureIndex"].ToString();
                    string[] str = { Ids, Nst, Cla, Pas, Fin };
                    dataGridView2.Rows.Add(str);
                }
                reader_select2.Close();
            }
            reader_select1.Close();//关闭连接
        }

        private void 查看本次缺勤人员名单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            self_IdCourse = dataGridView1.SelectedCells[0].Value.ToString();
            self_NameCourse = dataGridView1.SelectedCells[1].Value.ToString();
            self_TimesCourse = SelectTimesCourse(self_IdCourse);
            string times = SelectTimesCourse(self_IdCourse);
            if(times != self_TimesCourse)//如果签到结束，发生了课次加1的情况
            {
                dataGridView2.Visible = true;
                dataGridView1.Visible = false;
                dataGridView2.Rows.Clear();
                string sql_select1 = " select * from RecordAttend where IdCourse = '" + self_IdCourse + "' ";
                Dao dao_select1 = new Dao();
                IDataReader reader_select1 = dao_select1.read(sql_select1);
                while (reader_select1.Read())
                {
                    string Sta = reader_select1["StatusAbsence"].ToString();
                    string Ids = reader_select1["IdStudent"].ToString();
                    int times_int = int.Parse(times);
                    char single_sta = Sta[times_int - 1];//当前课程出勤情况
                    if(single_sta == '0')
                    {
                        string sql_select2 = " select * from Student where IdCourse = '" + Ids + "' ";
                        Dao dao_select2 = new Dao();
                        IDataReader reader_select2 = dao_select2.read(sql_select2);
                        if (reader_select2.Read())
                        {
                            string Nst, Cla, Pas, Fin;
                            Nst = reader_select2["Name"].ToString();
                            Cla = reader_select2["Class"].ToString();
                            Pas = reader_select2["Passsword"].ToString();
                            Fin = reader_select2["FeatureIndex"].ToString();
                            string[] str = { Ids, Nst, Cla, Pas, Fin };
                            dataGridView2.Rows.Add(str);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("请在完成考勤后查看", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
