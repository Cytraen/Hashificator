using Microsoft.Win32;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace HashCalc
{
    public partial class MainWindow : Window
    {
        [DllImport("user32")] public static extern int FlashWindow(IntPtr hwnd, bool bInvert);

        private Dictionary<string, Dictionary<string, string>> outputDict;

        public MainWindow()
        {
            InitializeComponent();
        }

        private string CalculateHash<T>(string path) where T : IDigest, new()
        {
            IDigest hash = new T();

            byte[] result = new byte[hash.GetDigestSize()];

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite))
            {
                byte[] buffer = new byte[4092];
                int bytesRead;

                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    hash.BlockUpdate(buffer, 0, bytesRead);
                }

                hash.DoFinal(result, 0);
            }
            return BitConverter.ToString(result).Replace("-", "");
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
            CalculateTab_ScannedFileListBox.Items.Clear();

            outputDict = new Dictionary<string, Dictionary<string, string>>();

            if (CalculateTab_InputFileListBox.Items.Count == 0)
            {
                MessageBox.Show("Please add at least one file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var inputList = new string[CalculateTab_InputFileListBox.Items.Count];
            CalculateTab_InputFileListBox.Items.CopyTo(inputList, 0);

            var checkBoxes = new Dictionary<string, bool>
            {
                ["md2"] = CalculateTab_ScanMD2CheckBox.IsChecked ?? false,
                ["md4"] = CalculateTab_ScanMD4CheckBox.IsChecked ?? false,
                ["md5"] = CalculateTab_ScanMD5CheckBox.IsChecked ?? false,
                ["sha1"] = CalculateTab_ScanSHA1CheckBox.IsChecked ?? false,
                ["sha256"] = CalculateTab_ScanSHA256CheckBox.IsChecked ?? false,
                ["sha384"] = CalculateTab_ScanSHA384CheckBox.IsChecked ?? false,
                ["sha512"] = CalculateTab_ScanSHA512CheckBox.IsChecked ?? false
            };

            if (checkBoxes.Values.Where(e => e.Equals(true)).Count() == 0)
            {
                MessageBox.Show("Please check at least one box.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var worker = new BackgroundWorker();

            worker.DoWork += workerWork;

            void workerWork(object obj, DoWorkEventArgs e)
            {
                Dispatcher.BeginInvoke((Action)(() => EnableAllCalculateTabButtons(false)));

                foreach (var item in inputList)
                {
                    outputDict[item] = new Dictionary<string, string>();

                    if (checkBoxes["md2"])
                        outputDict[item]["md2"] = CalculateHash<MD2Digest>(item);
                    else
                        outputDict[item]["md2"] = "";

                    if (checkBoxes["md4"])
                        outputDict[item]["md4"] = CalculateHash<MD4Digest>(item);
                    else
                        outputDict[item]["md4"] = "";

                    if (checkBoxes["md5"])
                        outputDict[item]["md5"] = CalculateHash<MD5Digest>(item);
                    else
                        outputDict[item]["md5"] = "";

                    if (checkBoxes["sha1"])
                        outputDict[item]["sha1"] = CalculateHash<Sha1Digest>(item);
                    else
                        outputDict[item]["sha1"] = "";

                    if (checkBoxes["sha256"])
                        outputDict[item]["sha256"] = CalculateHash<Sha256Digest>(item);
                    else
                        outputDict[item]["sha256"] = "";

                    if (checkBoxes["sha384"])
                        outputDict[item]["sha384"] = CalculateHash<Sha384Digest>(item);
                    else
                        outputDict[item]["sha384"] = "";

                    if (checkBoxes["sha512"])
                        outputDict[item]["sha512"] = CalculateHash<Sha512Digest>(item);
                    else
                        outputDict[item]["sha512"] = "";

                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        CalculateTab_ScannedFileListBox.Items.Add(item);
                        CalculateTab_InputFileListBox.Items.Remove(item);
                        CalculateTab_ProgressBar.Value += 100 / inputList.Count();
                    }));
                }

                Dispatcher.BeginInvoke((Action)(() => EnableAllCalculateTabButtons(true)));
            }

            worker.RunWorkerAsync();
        }

        private void EnableAllCalculateTabButtons(bool yes)
        {
            CalculateTab_ProgressBar.Value = 0;
            CalculateTab_ProgressBar.IsIndeterminate = yes ? false : true;
            TaskbarItemInfo.ProgressState = yes ? System.Windows.Shell.TaskbarItemProgressState.None : System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
            CalculateTab_AddButton.IsEnabled = yes;
            CalculateTab_RemoveButton.IsEnabled = yes;
            CalculateTab_CalculateButton.IsEnabled = yes;
            CalculateTab_ScanMD2CheckBox.IsEnabled = yes;
            CalculateTab_ScanMD4CheckBox.IsEnabled = yes;
            CalculateTab_ScanMD5CheckBox.IsEnabled = yes;
            CalculateTab_ScanSHA1CheckBox.IsEnabled = yes;
            CalculateTab_ScanSHA256CheckBox.IsEnabled = yes;
            CalculateTab_ScanSHA384CheckBox.IsEnabled = yes;
            CalculateTab_ScanSHA512CheckBox.IsEnabled = yes;

            if (yes)
            {
                FlashWindow(new WindowInteropHelper(this).Handle, true);
                MessageBox.Show("Job's done!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CalculateTab_ScannedFileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                var refDict = outputDict[(string)e.AddedItems[0]];
                CalculateTab_MD2Result.Text = refDict["md2"];
                CalculateTab_MD4Result.Text = refDict["md4"];
                CalculateTab_MD5Result.Text = refDict["md5"];
                CalculateTab_SHA1Result.Text = refDict["sha1"];
                CalculateTab_SHA256Result.Text = refDict["sha256"];
                CalculateTab_SHA384Result.Text = refDict["sha384"];
                CalculateTab_SHA512Result.Text = refDict["sha512"];
            }
            else
            {
                CalculateTab_MD2Result.Text = "";
                CalculateTab_MD4Result.Text = "";
                CalculateTab_MD5Result.Text = "";
                CalculateTab_SHA1Result.Text = "";
                CalculateTab_SHA256Result.Text = "";
                CalculateTab_SHA384Result.Text = "";
                CalculateTab_SHA512Result.Text = "";
            }
        }
    }
}