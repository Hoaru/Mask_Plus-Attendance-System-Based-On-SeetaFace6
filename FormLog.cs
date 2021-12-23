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
    public partial class FormLog : Form
    {
        public FormLog()
        {
            InitializeComponent();
        }

        private bool login()
        {
            if (textBox1.Text == "" || textBox2.Text == "" || comboBox1.Text == "")
            {
                MessageBox.Show("输入不完整请重新输入", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                //else return true;
                if (comboBox1.Text == "学生")
                {
                    string sql = "select * from Student where Id = '" + textBox1.Text + "' and Password = '" + textBox2.Text + "'";
                    //string sql = "select * from Student where id = '" + textBox1.Text + "' ";
                    Dao dao = new Dao();
                    IDataReader reader = dao.read(sql);//生成对象，但还未读取
                    if (reader.Read())
                        //!!!未读取到
                        return true;
                    else
                        return false;
                }
                else if (comboBox1.Text == "教师")
                {
                    string sql = "select * from Teacher where Id = '" + textBox1.Text + "' and Password = '" + textBox2.Text + "'";
                    Dao dao = new Dao();
                    IDataReader reader = dao.read(sql);
                    if (reader.Read())
                        return true;
                    else
                        return false;
                }
                else if (comboBox1.Text == "管理员")
                {
                    if (textBox1.Text == "admin" && textBox2.Text == "admin")
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            if (login())
            {
                 if (comboBox1.Text == "管理员")
                {
                    FormAdm fa = new FormAdm();
                    fa.Show();
                    this.Hide();
                }

                else if (comboBox1.Text == "教师")
                {
                    string teacher_id = textBox1.Text;
                    string teacher_password = textBox2.Text;
                    string sql = "select * from Teacher where id = '" + teacher_id + "' and password = '" + teacher_password + "'";
                    Dao dao = new Dao();
                    IDataReader reader = dao.read(sql);
                    reader.Read();
                    string id = reader["id"].ToString();
                    FormTea ft = new FormTea(id);
                    ft.Show();
                    this.Hide();
                }

                else if (comboBox1.Text == "学生")
                {
                    string student_id = textBox1.Text;
                    string student_password = textBox2.Text;
                    string sql = "select * from Student where id = '" + student_id + "' and password = '" + student_password + "'";
                    //string sql = "select * from Student where id = '" + student_id + "'";
                    Dao dao = new Dao();
                    IDataReader reader = dao.read(sql);
                    reader.Read();
                    string id = reader["id"].ToString();
                    FormStu fs = new FormStu(id);
                    fs.Show();
                    this.Hide();
                    //this.Close();
                    //不可使用该函数，因为Program.cs中Application.Run(new FormLog())打开了FormLog窗体，
                    //之后的Form窗体都是在FormTea窗体的基础上建立的。如果调用该函数，则所有窗体都会关闭
                }
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
