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
    public partial class FormAdm : Form
    {
        public FormAdm()
        {
            InitializeComponent();
            TableTea();
            timer1.Start();
            textBox2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public void TableTea()
        {
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;
            dataGridView1.Rows.Clear();

            添加教师ToolStripMenuItem.Visible = true;
            修改教师ToolStripMenuItem.Visible = true;
            删除教师ToolStripMenuItem.Visible = true;

            添加学生ToolStripMenuItem.Visible = false;
            修改学生ToolStripMenuItem.Visible = false;
            删除学生ToolStripMenuItem.Visible = false;

            添加课程ToolStripMenuItem.Visible = false;
            修改课程ToolStripMenuItem.Visible = false;
            删除课程ToolStripMenuItem.Visible = false;

            string sql = "select * from Teacher";
            Dao dao = new Dao();
            IDataReader reader = dao.read(sql);
            while (reader.Read())
            {
                string Idt, Nte, Ran, Pte;
                Idt = reader["Id"].ToString();
                Nte = reader["Name"].ToString();
                Ran = reader["Rank"].ToString();
                Pte = reader["Password"].ToString();
                string[] str = { Idt, Nte, Ran, Pte };
                dataGridView1.Rows.Add(str);
            }
            reader.Close();//关闭连接
        }

        private void 显示教师ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableTea();
        }

        private void 添加教师ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAdmTea fat = new FormAdmTea(this);
            fat.Show(this);
        }

        private void 修改教师ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] str =
            {
                dataGridView1.SelectedCells[0].Value.ToString(),
                dataGridView1.SelectedCells[1].Value.ToString(),
                dataGridView1.SelectedCells[2].Value.ToString()
            };
            FormAdmTea fat = new FormAdmTea(str, this);
            fat.Show();
        }

        private void 删除教师ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确定要删除吗？", "提示", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                string selected_id, selected_name;
                selected_id = dataGridView1.SelectedCells[0].Value.ToString();
                selected_name = dataGridView1.SelectedCells[1].Value.ToString();
                string sql = " delete from Teacher where Id = '" + selected_id + "'and Name = '" + selected_name + "'";
                //MessageBox.Show(sql);
                Dao dao = new Dao();
                dao.execute(sql);
                TableTea();
            }
        }

        public void TableStu()
        {
            dataGridView2.Visible = true;
            dataGridView1.Visible = false;
            dataGridView3.Visible = false;
            dataGridView2.Rows.Clear();

            添加教师ToolStripMenuItem.Visible = false;
            修改教师ToolStripMenuItem.Visible = false;
            删除教师ToolStripMenuItem.Visible = false;

            添加学生ToolStripMenuItem.Visible = true;
            修改学生ToolStripMenuItem.Visible = true;
            删除学生ToolStripMenuItem.Visible = true;

            添加课程ToolStripMenuItem.Visible = false;
            修改课程ToolStripMenuItem.Visible = false;
            删除课程ToolStripMenuItem.Visible = false;

            string sql = "select * from Student";
            Dao dao = new Dao();
            IDataReader reader = dao.read(sql);
            while (reader.Read())
            {
                string Ids, Nst, Cla, Pas, Fin;
                Ids = reader["Id"].ToString();
                Nst = reader["Name"].ToString();
                Cla = reader["Class"].ToString();
                Pas = reader["Password"].ToString();
                Fin = reader["FeatureIndex"].ToString();
                string[] str = { Ids, Nst, Cla, Pas, Fin };
                dataGridView2.Rows.Add(str);
            }
            reader.Close();//关闭连接
        }

        private void 显示学生ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableStu();
        }

        private void 添加学生ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAdmStu fas = new FormAdmStu(this);
            fas.Show(this);
        }

        private void 修改学生ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] str =
            {
                dataGridView2.SelectedCells[0].Value.ToString(),
                dataGridView2.SelectedCells[1].Value.ToString(),
                dataGridView2.SelectedCells[2].Value.ToString(),
                dataGridView2.SelectedCells[4].Value.ToString(),
            };
            FormAdmStu fas = new FormAdmStu(str, this);
            fas.Show();
        }

        private void 删除学生ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确定要删除吗？", "提示", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                string selected_id, selected_name;
                selected_id = dataGridView2.SelectedCells[0].Value.ToString();
                selected_name = dataGridView2.SelectedCells[1].Value.ToString();
                string sql = " delete from Student where Id = '" + selected_id + "'and Name = '" + selected_name + "'";
                //MessageBox.Show(sql);
                Dao dao = new Dao();
                dao.execute(sql);
                TableStu();
            }
        }

        public void TableCou()
        {
            dataGridView3.Visible = true;
            dataGridView2.Visible = false;
            dataGridView1.Visible = false;
            dataGridView3.Rows.Clear();

            添加教师ToolStripMenuItem.Visible = false;
            修改教师ToolStripMenuItem.Visible = false;
            删除教师ToolStripMenuItem.Visible = false;

            添加学生ToolStripMenuItem.Visible = false;
            修改学生ToolStripMenuItem.Visible = false;
            删除学生ToolStripMenuItem.Visible = false;

            添加课程ToolStripMenuItem.Visible = true;
            修改课程ToolStripMenuItem.Visible = true;
            删除课程ToolStripMenuItem.Visible = true;

            string sql = "select * from Course";
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
                dataGridView3.Rows.Add(str);
            }
            reader.Close();//关闭连接
        }

        private void 显示课程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableCou();
        }

        private void 添加课程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAdmCou fac = new FormAdmCou(this);
            fac.Show(this);
        }

        private void 修改课程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] str =
            {
                dataGridView3.SelectedCells[0].Value.ToString(),
                dataGridView3.SelectedCells[1].Value.ToString(),
                dataGridView3.SelectedCells[2].Value.ToString(),
                dataGridView3.SelectedCells[3].Value.ToString()
            };
            FormAdmCou fac = new FormAdmCou(str, this);
            fac.Show();
        }

        private void 删除课程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确定要删除吗？", "提示", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                string selected_id, selected_namecourse;
                selected_id = dataGridView3.SelectedCells[0].Value.ToString();
                selected_namecourse = dataGridView3.SelectedCells[1].Value.ToString();
                string sql = " delete from Course where Id = '" + selected_id + "'and NameCourse = '" + selected_namecourse + "'";
                //MessageBox.Show(sql);
                Dao dao = new Dao();
                dao.execute(sql);
                TableCou();
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();//结束整个程序
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
