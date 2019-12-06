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
    public partial class MainForm : Form, IWriteText
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private RegistryHandler RegistryHandler { get; set; }

        private delegate void WriteTextDelegate(string text, bool errorTxt = false);
        private WriteTextDelegate md;

        /// <summary>
        /// 是否可寫
        /// </summary>
        public bool IsWritable => WindowState != FormWindowState.Minimized;

        private void BackToNormalForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.notifyIcon1.Visible = false;
            RegistryHandler.DequeueOutputText();
        }

        private void BackToNormal_Click(object sender, EventArgs e)
        {
            BackToNormalForm();
        }

        /// <summary>
        /// IWriteText介面
        /// </summary>
        /// <param name="text"></param>
        /// <param name="errorTxt"></param>
        public void WriteText(string text, bool errorTxt = false)
        {
            this.Invoke(md, new object[] { text, errorTxt });
        }

        private void DoWriteText(string text, bool errorTxt = false)
        {
            if (richTextBox1.Text != "")
                richTextBox1.AppendText(Environment.NewLine);

            var color = errorTxt ? Color.Red : Color.Chartreuse;
            richTextBox1.SelectionColor = color;
            richTextBox1.AppendText(text);

            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.ScrollToCaret();
        }

        /// <summary>
        /// 表單關閉中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState != FormWindowState.Normal)
                return;
            
            var result = MessageBox.Show("是否縮小到工具列", "確認視窗", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                this.WindowState = FormWindowState.Minimized;        //決定視窗大小
                this.ShowInTaskbar = false;                          //決定是否出現在工作列
                this.notifyIcon1.Visible = true;                     //決定使否顯示notifyIcon1
                e.Cancel = true;
            }            
        }

        /// <summary>
        /// 表單關閉按鈕點擊事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// icon雙擊事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            BackToNormalForm();
        }

        /// <summary>
        /// 表單載入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            RegistryHandler = new RegistryHandler(this);
            md = new WriteTextDelegate(DoWriteText);
            WriteText("Start timer");
        }
    }
}
