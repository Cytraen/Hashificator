using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HashCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CalculateTab_AddButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Select file(s)",
                Filter = "All files (*.*) | *.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    CalculateTab_InputFileListBox.Items.Add(fileName);
                }
            }
        }

        private void CalculateTab_RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in CalculateTab_InputFileListBox.SelectedItems.Cast<object>().ToArray())
            {
                CalculateTab_InputFileListBox.Items.Remove(item);
            }
        }

        private void CalculateTab_CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in CalculateTab_InputFileListBox.Items)
            {
                if (CalculateTab_ScanMD2CheckBox.IsChecked ?? false)
                {
                    
                }
                if (CalculateTab_ScanMD4CheckBox.IsChecked ?? false)
                {

                }
                if (CalculateTab_ScanMD5CheckBox.IsChecked ?? false)
                {
                    using var hash = MD5.Create();
                    using var stream = File.OpenRead((string)item);
                    MessageBox.Show(BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", ""));

                }
                if (CalculateTab_ScanSHA1CheckBox.IsChecked ?? false)
                {
                    using var hash = SHA1.Create();
                    using var stream = File.OpenRead((string)item);
                    MessageBox.Show(BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", ""));
                }
                if (CalculateTab_ScanSHA256CheckBox.IsChecked ?? false)
                {
                    using var hash = SHA256.Create();
                    using var stream = File.OpenRead((string)item);
                    MessageBox.Show(BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", ""));
                }
                if (CalculateTab_ScanSHA384CheckBox.IsChecked ?? false)
                {
                    using var hash = SHA384.Create();
                    using var stream = File.OpenRead((string)item);
                    MessageBox.Show(BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", ""));
                }
                if (CalculateTab_ScanSHA512CheckBox.IsChecked ?? false)
                {
                    using var hash = SHA512.Create();
                    using var stream = File.OpenRead((string)item);
                    MessageBox.Show(BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", ""));
                }
            }
        }
    }
}
