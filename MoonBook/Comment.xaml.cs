using System;
using System.Collections.Generic;
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

namespace MoonBook
{
    /// <summary>
    /// Interaction logic for Comment.xaml
    /// </summary>
    public partial class Comment : Grid
    {
        Guid IdPost;
        Guid IdUser;
        public Comment(string name, DateTime date, byte[] photo, string text, Guid idpost, Guid iduser)
        {
            InitializeComponent();
            NameComment.Text = name;
            DataComent.Text = date.ToString();
            TextComment.Text = text;
            IdPost = idpost;
            IdUser = iduser;
            if (photo != null)
            {
                BitmapImage imgsource = new BitmapImage();
                imgsource.BeginInit();
                imgsource.StreamSource = new MemoryStream(photo);
                imgsource.EndInit();
                PhotoComent.Fill = new ImageBrush(imgsource);
            }
        }
    }
}
