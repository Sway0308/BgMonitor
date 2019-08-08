using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegistryMonitor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            WriteText("Start timer", false);
            timer.Start();
        }

        private void BackToNormal_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.notifyIcon1.Visible = false;
        }

        private void DeleteRegistryKey()
        {
            //讀取Registry Key位置
            using (var RegK = Registry.LocalMachine.OpenSubKey(@"Software\Policies\Google\Chrome", RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                var value = RegK.GetValue("RemoteAccessHostFirewallTraversal")?.ToString();
                if (value != null)
                {
                    WriteText($"Registry Value = {value}");
                    WriteText($"Start delete registry");
                    RegK.DeleteValue("RemoteAccessHostFirewallTraversal", true);
                    WriteText($"finish delete registry");
                }
                else
                {
                    WriteText("No Registry");
                }
            }
        }

        private void WriteText(string text, bool newLine = true)
        {
            if (newLine)
                richTextBox1.AppendText(Environment.NewLine);
            richTextBox1.AppendText(text);
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.ScrollToCaret();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer.Interval = 10 * 60 * 1000; //每10分鐘檢查一次
            WriteText(DateTime.Now.ToString());
            DeleteRegistryKey();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                var result = MessageBox.Show("是否縮小到工具列", "確認視窗", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    this.WindowState = FormWindowState.Minimized;        //決定視窗大小
                    this.ShowInTaskbar = false;                          //決定是否出現在工作列
                    this.notifyIcon1.Visible = true;                     //決定使否顯示notifyIcon1
                    e.Cancel = true;
                }
            }
        }

        private void FormClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSetTimer_Click(object sender, EventArgs e)
        {
            timer.Enabled = !timer.Enabled;
            btnSetTimer.Text = timer.Enabled ? "停止計時器" : "啟動計時器";
        }
    }
}
