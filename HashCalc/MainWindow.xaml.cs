using Microsoft.Win32;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;

namespace HashCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

            foreach (string item in CalculateTab_InputFileListBox.Items)
            {
                outputDict[item] = new Dictionary<string, string>();

                if (CalculateTab_ScanMD2CheckBox.IsChecked ?? false)
                    outputDict[item]["md2"] = CalculateHash<MD2Digest>(item);
                else
                    outputDict[item]["md2"] = "";

                if (CalculateTab_ScanMD4CheckBox.IsChecked ?? false)
                    outputDict[item]["md4"] = CalculateHash<MD4Digest>(item);
                else
                    outputDict[item]["md4"] = "";

                if (CalculateTab_ScanMD5CheckBox.IsChecked ?? false)
                    outputDict[item]["md5"] = CalculateHash<MD5Digest>(item);
                else
                    outputDict[item]["md5"] = "";

                if (CalculateTab_ScanSHA1CheckBox.IsChecked ?? false)
                    outputDict[item]["sha1"] = CalculateHash<Sha1Digest>(item);
                else
                    outputDict[item]["sha1"] = "";

                if (CalculateTab_ScanSHA256CheckBox.IsChecked ?? false)
                    outputDict[item]["sha256"] = CalculateHash<Sha256Digest>(item);
                else
                    outputDict[item]["sha256"] = "";

                if (CalculateTab_ScanSHA384CheckBox.IsChecked ?? false)
                    outputDict[item]["sha384"] = CalculateHash<Sha384Digest>(item);
                else
                    outputDict[item]["sha384"] = "";

                if (CalculateTab_ScanSHA512CheckBox.IsChecked ?? false)
                    outputDict[item]["sha512"] = CalculateHash<Sha512Digest>(item);
                else
                    outputDict[item]["sha512"] = "";

                CalculateTab_ScannedFileListBox.Items.Add(item);
            }

            CalculateTab_InputFileListBox.Items.Clear();
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
