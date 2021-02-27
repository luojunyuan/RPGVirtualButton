using RPGHelper.Models;
using System;
using System.Windows;

namespace RPGHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel ViewModel { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
            InjectButton.IsEnabled = false;
            _ = ViewModel.GetProcessAsync();
        }

        private async void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            await ViewModel.GetProcessAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var procComboItem = ProcComboBox.SelectedItem as ProcComboItem;
            var process = procComboItem!.proc;
            if (process.HasExited)
            {
                ViewModel.Applications.Remove(procComboItem);
                return;
            }

            new GameWindow(process).Show();
            Close();
        }

        private void ProcComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ProcComboBox.SelectedItem is null)
            {
                InjectButton.IsEnabled = false;
            }
            else
            {
                InjectButton.IsEnabled = true;
            }
        }
    }
}
