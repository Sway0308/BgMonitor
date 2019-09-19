using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryMonitor
{
    public interface IWriteText
    {
        bool IsWritable { get; }
        void WriteText(string text, bool errorTxt = false);
    }
}
