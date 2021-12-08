﻿using CryptoLibrary;
using KeysLibrary.Services;
using SerialkeysGeneratorSystem.Helpers;
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

            TxtPublicKey.Text = public_key;
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(path + "\\clave.txt")
            {
                UseShellExecute = true
            };
            p.Start();
        }

        private void BtnGenerateSerial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var public_key = TxtPublicKey.Text;
                if (public_key == string.Empty)
                    throw new Exception("Debe generar la llave publica primero. ");
                AES aes_crypto = new AES();
                aes_crypto.SetPrivateKey("ghdy65et73564gdt");
                aes_crypto.SetPublicKey(public_key);

                DateTime time_now = AppHelper.GetNetworkTime();

                var encrypted_data = aes_crypto.Encrypt("Hola mundo!");
                TxtSerial.Text = encrypted_data;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnValidateSerial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IKeysServices keyService = new KeysServices();
                var public_key = keyService.GeneratePublicKey();
                AES aes_crypto = new AES();
                aes_crypto.SetPrivateKey("ghdy65et73564gdt");
                aes_crypto.SetPublicKey(public_key);
                var serial = TxtValidateSerial.Text;
                var decrypted_data = aes_crypto.Decrypt(serial);
                MessageBox.Show("Serial valido!");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Serial no valido!");
            }
        }
    }
}
