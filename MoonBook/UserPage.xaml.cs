using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
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

namespace MoonBook
{
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        ServerConnect server;
        public Guid idUser;
        MainWindow main;
        BitmapImage imgsource;
        private OpenFileDialog openFileDialog1;
        private byte[]? ImageByte;
        string title;
        LibProtocol.Online tmp;
        public UserPage(Guid guid, string? name, string? surname, byte[] photo, bool online, MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            idUser = guid;
            Name.Text = $"{name} {surname}";
            title = "";
            imgsource = new BitmapImage();
            if (online) OnlineStatus.Fill = System.Windows.Media.Brushes.LightGreen;
            else OnlineStatus.Fill = System.Windows.Media.Brushes.LightGray;
            if (photo != null)
            {
                imgsource.BeginInit();
                imgsource.StreamSource = new MemoryStream(photo);
                imgsource.EndInit();
                Photo.Fill = new ImageBrush(imgsource);
                minPhoto.Fill = new ImageBrush(imgsource);
            }
            tmp = new LibProtocol.Online();
            server = new ServerConnect();
            server.onError += mess => MessageBox.Show(mess);
            openFileDialog1 = new OpenFileDialog()
            {
                Filter = "Image files (*.BMP, *.JPG, *.GIF, *.TIF, *.PNG, *.ICO, *.EMF, *.WMF)|*.bmp;*.jpg;*.gif; *.tif; *.png; *.ico; *.emf; *.wmf",
                Title = "Open image file"
            };
            server = new ServerConnect();
            server.onError += mess => MessageBox.Show(mess);
            Task.Run(() => Online(idUser));
            Task.Run(() => Check());
        }
        public void OpenFileDialogForm()
        {
            Dispatcher.Invoke(() => openFileDialog1.ShowDialog());
            if (openFileDialog1.FileName != "")
            {
                try
                {
                    Dispatcher.Invoke(() => CheckImg.Fill = new ImageBrush(new BitmapImage(new Uri(openFileDialog1.FileName))));
                    Dispatcher.Invoke(() => Path.Text = openFileDialog1.FileName);
                    ImageByte = File.ReadAllBytes(openFileDialog1.FileName);
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }
        public void Check()
        {
            while (true)
            {
                Thread.Sleep(1500);
                int num = 0;
                int tmp = 0;
                server.Connect();
                Dispatcher.Invoke(() => server.chekOnline(idUser));
                Dispatcher.Invoke(() => server.waitResponse((res) => 
                {
                    foreach (Post item in VewPost.Items)
                    {
                        num += item.ListComments.Items.Count;
                        num += Convert.ToInt32(item.Like.Text);
                        num += Convert.ToInt32(item.Dislike.Text);
                    }
                    num += VewPost.Items.Count;
                    tmp = (int)res.data;

                }));
                if (tmp != num)
                    Task.Run(() => Online(idUser));
            }
        }
        public void Online(Guid id)
        {
            server.Connect();
            Dispatcher.Invoke(() => server.monOnline(id));
            Dispatcher.Invoke(() =>server.waitResponse((res) => tmp = (LibProtocol.Online)res.data));
            Dispatcher.Invoke(() => 
            {
                VewPost.Items.Clear();
                foreach (var post in tmp.posts.Join(tmp.users, p => p.IdUser, u => u.Id, (p, u) => new { pos = p, use = u }).Distinct())
                {
                    VewPost.Items.Add(new Post($"{post.use.Name} {post.use.Surname}", post.use.Phpto, post.pos.Text, post.pos.Title, post.pos.Image, post.pos.Date, post.pos.Like, post.pos.Dislike, post.pos.Id, idUser, tmp));
                }
            });
            
        }
        public void NewPost()
        {
            server.Connect();
            Dispatcher.Invoke(()=> server.addPost(idUser ,Name.Text, title, NewText.Text, ImageByte));
            Dispatcher.Invoke(() => 
            {
                NewText.Text = "";
                Path.Text = "";
                title = "";
                CheckImg.Fill = null;
            });
        }
        public void Search()
        {
            server.Connect();
            Dispatcher.Invoke(() => server.Search(new LibProtocol.Models.User {Id = idUser, Name = SeachText.Text}));
            Dispatcher.Invoke(() => server.waitResponse((res) => 
            { 
                tmp = (LibProtocol.Online)res.data;
                foreach (var user in tmp.subscriptions.Join(tmp.users, s => s.IdFreand, u => u.Id, (s,u) => new {Sub = s, Use = u}))
                {
                    ListUser.Items.Add(new User(idUser, $"{user.Use.Name} {user.Use.Surname}", user.Use.Phpto, user.Use.Online, true, user.Use.Id));
                    tmp.users.Remove(user.Use);
                }
                foreach (var user in tmp.users)
                {
                    ListUser.Items.Add(new User(idUser, $"{user.Name} {user.Surname}", user.Phpto, user.Online, false, user.Id));

                }
            }));
            
        }
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            NewText.Text.Replace($">{title}<", "");
            Task.Run(()=>NewPost());
        }

        private void Image_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => OpenFileDialogForm());
        }

        private void Title_Click(object sender, RoutedEventArgs e)
        {
            title = NewText.SelectedText;
            NewText.SelectedText = $">{NewText.SelectedText.ToUpper()}<";
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Search());
        }
    }
}
