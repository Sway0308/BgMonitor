using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RegistryMonitor
{
    public class RegistryHandler
    {
        public RegistryHandler(IWriteText writeText)
        {
            WriteText = writeText;
            Timer.Start();
            Timer.Elapsed += Timer_Elapsed;
        }

        private Timer Timer { get; } = new Timer(3000);
        private IWriteText WriteText { get; }
        private Queue<OutputText> OutputTextQueue { get; } = new Queue<OutputText>();

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Timer.Interval != 10 * 60 * 1000)
                Timer.Interval = 10 * 60 * 1000; //每10分鐘檢查一次
            DoCheckRegistry();
        }

        private void DoCheckRegistry()
        {
            OutputTextQueue.Enqueue(new OutputText { Text = DateTime.Now.ToString() });
            DeleteRegistryKey();
            DequeueOutputText();
        }

        private void DeleteRegistryKey()
        {
            //讀取Registry Key位置
            using (var RegK = Registry.LocalMachine.OpenSubKey(@"Software\Policies\Google\Chrome", RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                var value = RegK.GetValue("RemoteAccessHostFirewallTraversal")?.ToString();
                if (value != null)
                {
                    OutputTextQueue.Enqueue(new OutputText { Text = $"Registry Value = {value}", IsError = true });
                    OutputTextQueue.Enqueue(new OutputText { Text = "Start delete registry", IsError = true });
                    RegK.DeleteValue("RemoteAccessHostFirewallTraversal", true);
                    OutputTextQueue.Enqueue(new OutputText { Text = "finish delete registry", IsError = true });
                }
                else
                {
                    OutputTextQueue.Enqueue(new OutputText { Text = "No Registry" });
                }
            }
        }

        public void DequeueOutputText()
        {
            if (!WriteText.IsWritable)
                return;

            while (OutputTextQueue.Count > 0)
            {
                var item = OutputTextQueue.Dequeue();
                WriteText.WriteText(item.Text, item.IsError);
            }
        }
    }
}
