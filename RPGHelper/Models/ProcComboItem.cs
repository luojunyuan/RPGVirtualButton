using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace RPGHelper.Models
{
    class ProcComboItem
    {
        public Process proc = null!;

        public BitmapImage Icon { get; set; } = null!;

        public string Title { get; set; } = string.Empty;
    }
}
