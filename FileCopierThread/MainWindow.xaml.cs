using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace FileCopierThread
{
    public partial class MainWindow : Window
    {
        private readonly Thread thread;
        private string[] pathes;

        public MainWindow()
        {
            InitializeComponent();
            pathes = new string[2];
            thread = new Thread(CopyMethod);
        }

        private void btnFrom_Click(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text Files |*.txt";

            var result = dialog.ShowDialog();

            if(result == true)
            {
                pathes[0] = dialog.FileName;
                txtFrom.Text = dialog.FileName;
                return;
            }


        }

        private void btnTo_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog= new VistaFolderBrowserDialog();

            var result = dialog.ShowDialog();

            if (result == true)
            {
                pathes[1] = dialog.SelectedPath;
                txtTo.Text = dialog.SelectedPath;
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtFrom.Text) || string.IsNullOrWhiteSpace(txtTo.Text))
            {
                MessageBox.Show("Please enter all information");
                return;
            }    

            if(!File.Exists(txtFrom.Text) || !Directory.Exists(txtTo.Text)) 
            {
                MessageBox.Show("Entered wrong file or folder path");
                return;
            }


            thread.Start();

        }


        private async void CopyMethod()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                FileInfo file = new FileInfo(txtFrom.Text);
                DirectoryInfo dir = new DirectoryInfo(txtTo.Text);
                using (FileStream fsRead = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream fsWrite = new FileStream($"{dir.FullName}/{file.Name}", FileMode.Create, FileAccess.Write))
                    {
                        var len = 10;
                        var fileSize = fsRead.Length;
                        byte[] buffer = new byte[len];

                        do
                        {
                            len = fsRead.Read(buffer, 0, buffer.Length); // 8
                            fsWrite.Write(buffer, 0, len);

                            fileSize -= len;

                            Thread.Sleep(100);

                        } while (len != 0);

                    }
                }
            });

            
        }
    }
}
