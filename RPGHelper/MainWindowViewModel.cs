using RPGHelper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RPGHelper
{
    class MainWindowViewModel
    {
        public ObservableCollection<ProcComboItem> Applications { get; set; } = new();

        internal async Task GetProcessAsync()
        {
            await Task.Run(() =>
            {
                ObservableCollection<ProcComboItem> tmpCollection = new();
                foreach (var process in ProcessEnumerable())
                {
                    try
                    {
                        var item = new ProcComboItem()
                        {
                            proc = process,
                            Title = process.MainModule!.FileVersionInfo.FileDescription ?? string.Empty,
                            Icon = PEIcon2BitmapImage(process.GetMainModulePath())
                        };
                        tmpCollection.Add(item);
                        if (!Applications.Contain(item))
                        {
                            Application.Current.Dispatcher.Invoke(() => Applications.Add(item));
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                foreach (var item in Applications.ToList())
                {
                    if (!tmpCollection.Contain(item))
                    {
                        Application.Current.Dispatcher.Invoke(() => Applications.Remove(item));
                    }
                }
            });
        }

        private IEnumerable<Process> ProcessEnumerable()
        {
            foreach (var proc in Process.GetProcesses())
            {
                if (proc.MainWindowHandle != IntPtr.Zero && !proc.MainWindowTitle.Equals(string.Empty))
                {
                    if (uselessProcess.Contains(proc.ProcessName))
                        continue;

                    yield return proc;
                }
            }
        }

        private readonly List<string> uselessProcess = new List<string>
        {
            "TextInputHost", "ApplicationFrameHost", "Calculator", "Video.UI", "WinStore.App", "SystemSettings",
            "PaintStudio.View", "ShellExperienceHost", "commsapps", "Music.UI", "HxOutlook", "Maps"
        };

        private static BitmapImage PEIcon2BitmapImage(string fullPath)
        {
            var result = new BitmapImage();
            Stream stream = new MemoryStream();

            var iconBitmap = Icon.ExtractAssociatedIcon(fullPath)!.ToBitmap();
            iconBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            iconBitmap.Dispose();
            result.BeginInit();
            result.StreamSource = stream;
            result.EndInit();
            result.Freeze();

            return result;
        }
    }
}
