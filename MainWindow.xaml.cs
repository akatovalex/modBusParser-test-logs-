using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Forms;
using System.IO;
using System.Xml;


namespace modBusParse {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        string outputFormat;
        private Commands commands;
        private MBExceptions exceptions;

        public MainWindow() {
            InitializeComponent();

            commands = new Commands(@"dictionaries\commands.vcb");
            exceptions = new MBExceptions(@"dictionaries\exceptions.vcb");
        }


        private void BtnFileOpen_Click(object sender, RoutedEventArgs e) {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch (result) {
                case System.Windows.Forms.DialogResult.OK: {
                    var file = fileDialog.FileName;
                    label_FilePath.Content = file;
                    label_FilePath.ToolTip = file;
                    btn_CreateLog.IsEnabled = true;
                    break;
                }
                case System.Windows.Forms.DialogResult.Cancel:
                default: {
                    label_FilePath.Content = null;
                    label_FilePath.ToolTip = null;
                    btn_CreateLog.IsEnabled = false;
                    break;
                }
            }
        }

        private void Rb_JSON_Checked(object sender, RoutedEventArgs e) {
            outputFormat = ".json";
        }

        private void Rb_XML_Checked(object sender, RoutedEventArgs e) {
            outputFormat = ".xml";
        }

        private void Rb_TXT_Checked(object sender, RoutedEventArgs e) {
            outputFormat = ".txt";
        }

        private void Btn_CreateLog_Click(object sender, RoutedEventArgs e) {
            string resultLogs = MBParse.Parse(File.ReadAllText(label_FilePath.Content.ToString()), this.exceptions, this.commands, this.outputFormat);
            string fileName = "output";
            File.WriteAllText(fileName + this.outputFormat, resultLogs);
            System.Windows.MessageBox.Show("File path: " + System.IO.Directory.GetCurrentDirectory() + "\\" + fileName + this.outputFormat
                , "File added successfully", new System.Windows.MessageBoxButton());
        }

        private void Btn_CreateLogFromRawData_Click(object sender, RoutedEventArgs e) {
            string resultLogs = MBParse.Parse(new TextRange(RichTextBoxData.Document.ContentStart, RichTextBoxData.Document.ContentEnd).Text
                , this.exceptions, this.commands, this.outputFormat);
            string fileName = "output";
            File.WriteAllText(fileName + this.outputFormat, resultLogs);
            System.Windows.MessageBox.Show("File path: " + System.IO.Directory.GetCurrentDirectory() + "\\" + fileName + this.outputFormat
                ,"File added successfully",new System.Windows.MessageBoxButton());
        }


        private void TxtFile_TextChanged(object sender, TextChangedEventArgs e) {

            if (new TextRange(RichTextBoxData.Document.ContentStart, RichTextBoxData.Document.ContentEnd).Text.Length > 10) {
                btn_CreateLogFromRawData.IsEnabled = true;
            }
            else {
                btn_CreateLogFromRawData.IsEnabled = false;
            }
        }

    }
}
