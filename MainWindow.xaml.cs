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


namespace modBusParse {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        int outputFormat;

        public MainWindow() {
            InitializeComponent();

            Commands commands = new Commands(@"dictionaries\commands.vcb");
            MBExceptions exceptions = new MBExceptions(@"dictionaries\exceptions.vcb");
            //Source source = new Source();
            

            //TxtFile.Text = commands.Command["44"];
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
            outputFormat = 0;
        }

        private void Rb_XML_Checked(object sender, RoutedEventArgs e) {
            outputFormat = 1;
        }

        private void Rb_TXT_Checked(object sender, RoutedEventArgs e) {
            outputFormat = 2;
        }

        private void Btn_CreateLog_Click(object sender, RoutedEventArgs e) {
            switch (this.outputFormat) {
                case 0: {
                    //JSON
                    string abc = MBParse.Parse(File.ReadAllText(label_FilePath.Content.ToString()));

                    File.WriteAllText(@"output.txt", abc);
                    break;
                }
                case 1: {
                    //File.Create(@"output.xml");
                    break;
                }
                default: {
                    //File.Create(@"output.txt");
                    break;
                }
            }
        }

        private void Btn_CreateLogFromRawData_Click(object sender, RoutedEventArgs e) {
            switch (this.outputFormat) {
                case 0: {



                    //File.Create(@"output.json");

                    //последние два байта записаны в обратном порядке, при нахождении контрольной суммы вместе с ними выходит 0x0
                    //byte[] b = { 0xFE, 0x46, 0x00, 0x3B, 0x01, 0xCA, 0x6C };              - обратный      (0)
                    //byte[] b = { 0x68, 0x46, 0x02, 0x03, 0x12, 0x71, 0xBC };              - обратный      (0)
                    //byte[] b = { 0x68, 0x04, 0x00, 0x10, 0x00, 0x10, 0xF9, 0x3A };        - обратный      (0)
                    //byte[] b = { 0x68, 0x04, 0x04, 0x0E, 0x08, 0x0F, 0x10 };              - обратный      КС = (0x85, 0x94) - тест без неё
                    //byte[] b = { 0x68, 0x03, 0x00, 0x02, 0xFF, 0xFF};           //          - верный ответ в ОБЫЧНОМ порядке 0x83, 0xEC
                    //byte[] b = { 0x68, 0x03, 0x00, 0xFF, 0xFF, 0xFF, 0x02, 0xFF, 0xFF, 0x83, 0xEC };
                    //TxtFile.Text = string.Format("0x{0:X}", CheckSum.CRC16(b, b.Length));
                    break;
                }
                case 1: {
                    //File.Create(@"output.xml");
                    break;
                }
                default: {
                    //File.Create(@"output.txt");
                    break;
                }
            }
        }


        private void TxtFile_TextChanged(object sender, TextChangedEventArgs e) {
            if (TxtFile.Text != "") {
                btn_CreateLogFromRawData.IsEnabled = true;
            }
            else {
                btn_CreateLogFromRawData.IsEnabled = false;
            }
        }

    }
}
