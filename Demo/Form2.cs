using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RegLib;

namespace Demo
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label1.Visible = false;
            if (!File.Exists(Application.StartupPath + "\\reg.key"))
            {
                button1.Enabled = true;
            }
            else
            {
                clsRegInfo info = RegLib.EncryptDecrypt.LoadRegFile(Application.StartupPath + "\\reg.key");
                string localID = RegLib.EncryptDecrypt.GetID();
                if (info == null || info.ID != localID)
                {
                    button1.Enabled = true;
                }
                else
                {
                    button1.Enabled = false;
                    label1.Visible = true;
                    DateTime dt = new DateTime(info.TimeTo);
                    label1.Text = "授权时间到：" + dt.ToString();
                }
            }
            textBox1.Text = RegLib.EncryptDecrypt.GetID();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "reg.key";
            openFileDialog1.Filter = "密匙文件|reg.key";
            openFileDialog1.Title = "选择注册文件";
            //openFileDialog1.ShowDialog();
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                clsRegInfo info = RegLib.EncryptDecrypt.LoadRegFile(openFileDialog1.FileName);
                string localID = RegLib.EncryptDecrypt.GetID();
                if (info == null || info.ID != localID)
                {
                    MessageBox.Show("注册文件非法！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    clsPublic.isReg = false;
                }
                else
                {
                    long timeNow = DateTime.Now.Ticks;
                    if (timeNow > info.TimeTo)
                    {
                        MessageBox.Show("注册文件已经失效！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        clsPublic.isReg = false;
                    }
                    else
                    {
                        File.Copy(openFileDialog1.FileName, Application.StartupPath + "\\reg.key");
                        clsPublic.isReg = true;
                        label1.Text = "授权时间到：" + new DateTime(info.TimeTo);
                        MessageBox.Show("注册成功！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}
