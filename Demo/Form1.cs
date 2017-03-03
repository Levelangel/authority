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
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            if (!File.Exists(Application.StartupPath + "\\reg.key"))
            {
                clsPublic.isReg = false;
            }
            else
            {
                clsRegInfo info = RegLib.EncryptDecrypt.LoadRegFile(Application.StartupPath + "\\reg.key");
                string localID = RegLib.EncryptDecrypt.GetID();
                if (info == null || info.ID != localID)
                {
                    clsPublic.isReg = false;
                }
            }
            MessageBox.Show("软件未注册！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            label1.Visible = true;
        }

        private void 注册ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frmReg = new Form2();
            frmReg.ShowDialog();
        }
    }
}
