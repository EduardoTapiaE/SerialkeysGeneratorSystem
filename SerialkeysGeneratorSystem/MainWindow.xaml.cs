using KeysLibrary.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace SerialkeysGeneratorSystem
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

        private void BtnGenerateKey_Click(object sender, RoutedEventArgs e)
        {
            IKeysServices keyService = new KeysServices();
            string path = Directory.GetCurrentDirectory();
            var public_key = keyService.GeneratePublicKey();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path+"\\clave.txt", false))
            {
                file.WriteLine(public_key);
            }
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(path + "\\clave.txt")
            {
                UseShellExecute = true
            };
            p.Start();
        }
    }
}
