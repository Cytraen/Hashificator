using Hashificator.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Hashificator.WPFApp
{
    public partial class MainWindow : Window
    {
        [DllImport("user32")] private static extern int FlashWindow(IntPtr hwnd, bool bInvert);

        private Dictionary<string, HashCollection> _outputDict;

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
                    _ = CalculateTab_InputFileListBox.Items.Add(fileName);
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
            CalculateTab_ScannedFileListBox.Items.Clear();

            _outputDict = new Dictionary<string, HashCollection>();

            if (CalculateTab_InputFileListBox.Items.Count == 0)
            {
                MessageBox.Show("Please add at least one file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var inputList = new string[CalculateTab_InputFileListBox.Items.Count];
            CalculateTab_InputFileListBox.Items.CopyTo(inputList, 0);

            var checkBoxes = new HashSelection
            {
                MD2 = CalculateTab_ScanMD2CheckBox.IsChecked ?? false,
                MD4 = CalculateTab_ScanMD4CheckBox.IsChecked ?? false,
                MD5 = CalculateTab_ScanMD5CheckBox.IsChecked ?? false,

                Sha1 = CalculateTab_ScanSha1CheckBox.IsChecked ?? false,
                Sha224 = CalculateTab_ScanSha224CheckBox.IsChecked ?? false,
                Sha256 = CalculateTab_ScanSha256CheckBox.IsChecked ?? false,
                Sha384 = CalculateTab_ScanSha384CheckBox.IsChecked ?? false,
                Sha512 = CalculateTab_ScanSha512CheckBox.IsChecked ?? false,

                Sha3_224 = CalculateTab_ScanSha3_224CheckBox.IsChecked ?? false,
                Sha3_256 = CalculateTab_ScanSha3_256CheckBox.IsChecked ?? false,
                Sha3_384 = CalculateTab_ScanSha3_384CheckBox.IsChecked ?? false,
                Sha3_512 = CalculateTab_ScanSha3_512CheckBox.IsChecked ?? false
            };

            int threadCount;

            if (!int.TryParse(MaxThreadsTextBox.Text, out threadCount))
            {
                threadCount = Math.Min(Environment.ProcessorCount / 4, 1);
            }

            uint bufferSize;

            if (!uint.TryParse(BufferSizeTextBox.Text, out bufferSize))
            {
                bufferSize = 64;
            }

            var worker = new BackgroundWorker();

            worker.DoWork += workerWork;

            void workerWork(object obj, DoWorkEventArgs e)
            {
                _ = Dispatcher.BeginInvoke((Action)(() => EnableAllCalculateTabButtons(false)));


                var startTime = DateTime.UtcNow;
                foreach (var item in inputList)
                {
                    var hashCollection = Crypto.CalculateHashes(item, checkBoxes, threadCount, bufferSize * 1024);

                    _outputDict[item] = hashCollection;

                    _ = Dispatcher.BeginInvoke((Action)(() =>
                      {
                          _ = CalculateTab_ScannedFileListBox.Items.Add(item);
                          CalculateTab_InputFileListBox.Items.Remove(item);
                      }));
                }
                var finishTime = DateTime.UtcNow;

                _ = MessageBox.Show($"Job's done!\nTook {Math.Round((finishTime - startTime).TotalSeconds, 2)} seconds.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                _ = Dispatcher.BeginInvoke((Action)(() => EnableAllCalculateTabButtons(true)));
            }

            worker.RunWorkerAsync();
        }

        private void EnableAllCalculateTabButtons(bool yes)
        {
            CalculateTab_ProgressBar.Value = 0;
            CalculateTab_ProgressBar.IsIndeterminate = !yes;
            TaskbarItemInfo.ProgressState = yes ? System.Windows.Shell.TaskbarItemProgressState.None : System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
            CalculateTab_AddButton.IsEnabled = yes;
            CalculateTab_RemoveButton.IsEnabled = yes;
            CalculateTab_CalculateButton.IsEnabled = yes;

            CalculateTab_ScanMD2CheckBox.IsEnabled = yes;
            CalculateTab_ScanMD4CheckBox.IsEnabled = yes;
            CalculateTab_ScanMD5CheckBox.IsEnabled = yes;

            CalculateTab_ScanSha1CheckBox.IsEnabled = yes;
            CalculateTab_ScanSha224CheckBox.IsEnabled = yes;
            CalculateTab_ScanSha256CheckBox.IsEnabled = yes;
            CalculateTab_ScanSha384CheckBox.IsEnabled = yes;
            CalculateTab_ScanSha512CheckBox.IsEnabled = yes;

            CalculateTab_ScanSha3_224CheckBox.IsEnabled = yes;
            CalculateTab_ScanSha3_256CheckBox.IsEnabled = yes;
            CalculateTab_ScanSha3_384CheckBox.IsEnabled = yes;
            CalculateTab_ScanSha3_512CheckBox.IsEnabled = yes;

            if (yes)
            {
                _ = FlashWindow(new WindowInteropHelper(this).Handle, true);
            }
        }

        private void CalculateTab_ScannedFileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                var hashes = _outputDict[(string)e.AddedItems[0]];

                CalculateTab_MD2Result.Text = hashes.MD2;
                CalculateTab_MD4Result.Text = hashes.MD4;
                CalculateTab_MD5Result.Text = hashes.MD5;

                CalculateTab_Sha1Result.Text = hashes.Sha1;
                CalculateTab_Sha224Result.Text = hashes.Sha224;
                CalculateTab_Sha256Result.Text = hashes.Sha256;
                CalculateTab_Sha384Result.Text = hashes.Sha384;
                CalculateTab_Sha512Result.Text = hashes.Sha512;

                CalculateTab_Sha3_224Result.Text = hashes.Sha3_224;
                CalculateTab_Sha3_256Result.Text = hashes.Sha3_256;
                CalculateTab_Sha3_384Result.Text = hashes.Sha3_384;
                CalculateTab_Sha3_512Result.Text = hashes.Sha3_512;
            }
            else
            {
                CalculateTab_MD2Result.Text = string.Empty;
                CalculateTab_MD4Result.Text = string.Empty;
                CalculateTab_MD5Result.Text = string.Empty;

                CalculateTab_Sha1Result.Text = string.Empty;
                CalculateTab_Sha224Result.Text = string.Empty;
                CalculateTab_Sha256Result.Text = string.Empty;
                CalculateTab_Sha384Result.Text = string.Empty;
                CalculateTab_Sha512Result.Text = string.Empty;

                CalculateTab_Sha3_224Result.Text = string.Empty;
                CalculateTab_Sha3_256Result.Text = string.Empty;
                CalculateTab_Sha3_384Result.Text = string.Empty;
                CalculateTab_Sha3_512Result.Text = string.Empty;
            }
        }
    }
}