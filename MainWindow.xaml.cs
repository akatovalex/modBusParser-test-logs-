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


namespace modBusParse {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch (result) {
                case System.Windows.Forms.DialogResult.OK:
                    var file = fileDialog.FileName;
                    TxtFile.Text = file;
                    TxtFile.ToolTip = file;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    TxtFile.Text = null;
                    TxtFile.ToolTip = null;
                    break;
            }
        }

        private void BtnFileOpen_Click(object sender, RoutedEventArgs e) {

        }

        private void Rb_JSON_Checked(object sender, RoutedEventArgs e) {

        }

        private void Rb_XML_Checked(object sender, RoutedEventArgs e) {

        }

        private void Rb_TXT_Checked(object sender, RoutedEventArgs e) {

        }
    }
}
