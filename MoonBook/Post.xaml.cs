using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

namespace MoonBook
{
    /// <summary>
    /// Interaction logic for Post.xaml
    /// </summary>
    public partial class Post : Grid
    {
        public Guid Id;
        public Guid IdUser;
        ServerConnect server;
        public Post(string name, byte[] photo, string text, string title, byte[] img, DateTime date, int like, int dislike, Guid IdPost, Guid iduser, LibProtocol.Online online)
        {
            InitializeComponent();
            Name.Text = name;
            TextBlog.Text = text;
            Title.Text = title;
            Data.Text = date.ToString();
            Like.Text = like.ToString();
            Dislike.Text = dislike.ToString();
            IdUser = iduser;
            Id = IdPost;
            server = new ServerConnect();
            if (img != null)
            {
                BitmapImage imgsource = new BitmapImage();
                imgsource.BeginInit();
                imgsource.StreamSource = new MemoryStream(img);
                imgsource.EndInit();
                Image.Source = imgsource;
            }
            if (photo != null)
            {
                BitmapImage imgsource = new BitmapImage();
                imgsource.BeginInit();
                imgsource.StreamSource = new MemoryStream(photo);
                imgsource.EndInit();
                PostPhoto.Fill = new ImageBrush(imgsource);
            }
            foreach (var coment in online.comments.Where(c => c.idPost == Id).OrderByDescending(c => c.Date).Join(online.users, c => c.idUser, u => u.Id, (c, u) => new { comm = c, use = u }))
            {
                ListComments.Items.Add(new Comment($"{coment.use.Name} {coment.use.Surname}", coment.comm.Date, coment.use.Phpto, coment.comm.Text, coment.comm.idPost, coment.use.Id));
            }
        }
        public void Reaction(int Reakt)
        {
            server.Connect();
            Dispatcher.Invoke(()=> server.addReaction(Reakt, Id, IdUser));
        }
        public void Remove()
        {
            server.Connect();
            Dispatcher.Invoke(() => server.Delete(Id));
        }
        public void SendComment()
        {
            server.Connect();
            Dispatcher.Invoke(() => server.addComent(DateTime.Now, CommentText.Text, Id, IdUser));
        }
        private void Like_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(()=> Reaction(1));
        }

        private void Dislike_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Reaction(2));
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(()=> SendComment());
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Remove());
        }
    }
}
