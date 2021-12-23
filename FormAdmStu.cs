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
    public partial class FormAdmStu : Form
    {

        string[] self_str = new string[4];
        public FormAdm self_fa;
        //添加界面初始化
        public FormAdmStu(FormAdm f)
        {
            InitializeComponent();
            self_fa = f;
            button1.Visible = true;
            button3.Visible = false;
            //修改界面中显示保存按钮，隐藏修改 按钮
        }

        //修改界面初始化
        public FormAdmStu(string[] new_str, FormAdm f)
        {
            InitializeComponent();
            self_fa = f;
            button1.Visible = false;
            button3.Visible = true;
            //修改界面中显示修改按钮，隐藏保存按钮
            for (int i = 0; i < 4; i++)
                self_str[i] = new_str[i];
            textBox1.Text = self_str[0];
            textBox2.Text = self_str[1];
            textBox3.Text = self_str[2];
            textBox4.Text = self_str[3];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("输入不完整请重新输入", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string sql_select = "select * from Student where Id = '" + textBox1.Text + "' ";
                Dao dao_select = new Dao();
                IDataReader reader_select = dao_select.read(sql_select);
                if (reader_select.Read())
                {
                    MessageBox.Show("该学号已存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    string sql = "insert into Student values('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','123456','','" + textBox4.Text + "')";
                    //MessageBox.Show(sql);
                    Dao dao = new Dao();
                    int flag = dao.execute(sql);//返回受影响的行数
                    if (flag > 0)
                    {
                        MessageBox.Show("添加成功");
                        self_fa.TableStu();
                        this.Hide();
                    }
                }
                reader_select.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("输入不完整请重新输入", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string sql_delete = "delete from Student where Id ='" + self_str[0] + "' and Name = '" + self_str[1] + "'and Class = '" + self_str[2] + "'and FeatureIndex = '" + self_str[3] + "'  ";
                string sql_add = "insert into Student values('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','123456','','" + textBox4.Text + "')";
                //注：在此不直接使用update语句的原因是，假如课程的4个属性全部产生变化，即where判断条件也发生变化，则可能会导致冲突
                //string sql = "Delete:\n" + sql_delete + "\n\nAdd:\n" + sql_add;
                //MessageBox.Show(sql);
                Dao dao = new Dao();
                int flag_delete = dao.execute(sql_delete);//返回受影响的行数
                int flag_add = dao.execute(sql_add);
                if (flag_delete > 0 && flag_add > 0)
                {
                    MessageBox.Show("修改成功");
                    self_fa.TableStu();
                    this.Hide();
                }
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
