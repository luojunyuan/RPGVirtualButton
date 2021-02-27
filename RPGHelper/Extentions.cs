using RPGHelper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGHelper
{
    static class Extentions
    {
        public static string GetMainModulePath(this Process process)
        {
            var buffer = new StringBuilder(1024);

            IntPtr hprocess = NativeMethods.OpenProcess(NativeMethods.PROCESS_QUERY_LIMITED_INFORMATION,
                                                        false, process.Id);

            if (hprocess != IntPtr.Zero)
            {
                try
                {
                    int size = buffer.Capacity + 1;
                    if (NativeMethods.QueryFullProcessImageName(hprocess, 0, buffer, out size))
                    {
                        return buffer.ToString();
                    }
                }
                finally
                {
                    NativeMethods.CloseHandle(hprocess);
                }
            }

            return string.Empty;
        }

        public static bool Contain(this ObservableCollection<ProcComboItem> data, ProcComboItem tar)
        {
            foreach (var item in data)
            {
                if (item.proc.Id == tar.proc.Id)
                    return true;
            }
            return false;
        }

        public static void Debug(this object obj)
        {
            System.Diagnostics.Debug.WriteLine(obj.ToString());
        }
    }
}
