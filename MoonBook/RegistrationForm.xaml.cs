using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MoonBook
{
    /// <summary>
    /// Interaction logic for RegistrationForm.xaml
    /// </summary>
    public partial class RegistrationForm : Window
    {
        private OpenFileDialog openFileDialog1;
        private byte[]? Photo;
        private ServerConnect server;
        public RegistrationForm()
        {
            InitializeComponent();
            openFileDialog1 = new OpenFileDialog()
            {
                Filter = "Image files (*.BMP, *.JPG, *.GIF, *.TIF, *.PNG, *.ICO, *.EMF, *.WMF)|*.bmp;*.jpg;*.gif; *.tif; *.png; *.ico; *.emf; *.wmf",
                Title = "Open image file"
            };
            server = new ServerConnect();
            server.onError += mess => MessageBox.Show(mess);
        }
        public void OpenFileDialogForm()
        {
            Dispatcher.Invoke(new Action(() => openFileDialog1.ShowDialog()));
            if (openFileDialog1.FileName != "")
            {
                try
                {
                    Dispatcher.Invoke(new Action(() => Elips.Fill = new ImageBrush(new BitmapImage(new Uri(openFileDialog1.FileName)))));
                    Photo = File.ReadAllBytes(openFileDialog1.FileName);
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }
        public void AddAccaunt()
        {
            
            if (Dispatcher.Invoke(()=>LogName.Text) == "")
            {
                MessageBox.Show("Enter Name");
                return;
            }
            if (Dispatcher.Invoke(() => LogSurname.Text) == "")
            {
                MessageBox.Show("Enter Surname");
                return;
            }
            if (Dispatcher.Invoke(() => LogDate.Text) == "")
            {
                MessageBox.Show("Enter Date");
                return;
            }
            if (Dispatcher.Invoke(() => LogLogin.Text) == "")
            {
                MessageBox.Show("Enter Login");
                return;
            }
            if (Dispatcher.Invoke(() => LogPass.Password) == "")
            {
                MessageBox.Show("Enter Password");
                return;
            }
            if (Dispatcher.Invoke(() => LogConPass.Password) == "")
            {
                MessageBox.Show("Enter Confirm Password");
                return;
            }
            if (Dispatcher.Invoke(() => LogPass.Password != LogConPass.Password))
            {
                MessageBox.Show("Password dont confirm");
                return;
            }
            server.Connect();
            Dispatcher.Invoke(()=>server.newAccaunt(LogName.Text, LogSurname.Text, LogDate.DisplayDate, LogLogin.Text, LogPass.Password, Photo));
            server.waitResponse((res)=> 
            {
                if(res.succces)
                {
                    MessageBox.Show(res.StatusTxt);
                    Dispatcher.Invoke(()=> this.Close());
                }
                else
                {
                    MessageBox.Show(res.StatusTxt);
                }
            });

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(()=>AddAccaunt());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Task.Run(() => OpenFileDialogForm());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
