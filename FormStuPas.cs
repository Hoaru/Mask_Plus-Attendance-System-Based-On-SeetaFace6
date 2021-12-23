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
    public partial class FormStuPas : Form
    {
        string self_id;
        public FormStuPas(string id)
        {
            InitializeComponent();
            self_id = id;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string p0 = textBox1.Text;
            string p1 = textBox2.Text;
            string p2 = textBox3.Text;
            //如果有空项则提示重新输入
            if (p0 == "" || p1 == "" || p2 == "")
            {
                MessageBox.Show("输入不完整请重新输入", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string sql_select = "select * from Student where Id = '" + self_id + "'";
                Dao dao_select = new Dao();
                IDataReader reader_select = dao_select.read(sql_select);
                //如果能读到个人信息
                if (reader_select.Read())
                {
                    string p;
                    p = reader_select["Password"].ToString();
                    //如果所输入原密码正确
                    if (p == p0)
                    {
                        //如果两次新密码一致
                        if (p1 == p2)
                        {
                            string sql_update = "update Student set Password = '" + p2 + "' where Id = '" + self_id + "'";
                            //MessageBox.Show(sql_update);
                            Dao dao_update = new Dao();
                            int flag_update = dao_update.execute(sql_update);//返回受影响的行数
                            if (flag_update > 0)
                            {
                                MessageBox.Show("修改成功");
                                this.Hide();
                            }
                        }
                        //如果两次新密码不一致
                        else
                        {
                            MessageBox.Show("两次输入的密码不一致，请重新输入", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            textBox1.Text = "";
                            textBox2.Text = "";
                            textBox3.Text = "";
                        }
                    }
                    //如果所输入原密码错误
                    else
                    {
                        MessageBox.Show("原密码错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox1.Text = "";
                        textBox2.Text = "";
                        textBox3.Text = "";
                    }
                    reader_select.Close();
                }
                //如果不能读到个人信息
                else
                {
                    MessageBox.Show("由于未知错误未查询到此人", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                }
                
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
