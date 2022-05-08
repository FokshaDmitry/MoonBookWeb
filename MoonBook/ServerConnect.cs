using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoonBook
{
    public class ServerConnect
    {
        public IPAddress ip = IPAddress.Parse("127.0.0.1");
        public int port = 3456;

        public event Action<string>? onError;
        TcpClient tpcClient;
        Thread threadClient;
        NetworkStream stream;
        UdpClient udpClient;

        BinaryFormatter bf = new BinaryFormatter();

        public void Connect()
        {
            try
            {
                tpcClient = new TcpClient();
                tpcClient.Connect(new IPEndPoint(ip, port));
                stream = tpcClient.GetStream();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
        public void newAccaunt(string name, string surname, DateTime date, string login, string password, byte[] photo)
        {
            LibProtocol.Request request = new LibProtocol.Request();
            request.command = LibProtocol.Command.Registration;
            request.data = new LibProtocol.Models.User { Id = Guid.NewGuid(), Name = name, Surname = surname, DateOfBith = date, Login = login, Password = password, Phpto = photo };
            try
            {
                bf.Serialize(stream, request);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
        public void addPost(Guid idU, string name, string title, string text, byte[] img)
        {
            LibProtocol.Request request = new LibProtocol.Request();
            request.command = LibProtocol.Command.addPost;
            request.data = new LibProtocol.Models.Posts { Id = Guid.NewGuid(), Date = DateTime.Now, Title = title, Text = text, Image = img, Like = 0, Dislike = 0, IdUser = idU };
            try
            {
                bf.Serialize(stream, request);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
        public void addComent(DateTime date, string text, Guid idpost, Guid iduser)
        {
            LibProtocol.Request request = new LibProtocol.Request();
            request.command = LibProtocol.Command.Comment;
            request.data = new LibProtocol.Models.Comments { Id = Guid.NewGuid(), Date = date, Text = text, idPost = idpost, idUser = iduser };
            try
            {
                bf.Serialize(stream, request);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
        public void LoginAccaunt(string log, string pass)
        {
            LibProtocol.Request request = new LibProtocol.Request();
            request.command = LibProtocol.Command.Login;
            request.data = new LibProtocol.Models.User { Login = log, Password = pass };
            try
            {
                bf.Serialize(stream, request);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
        public void addReaction(int reaction, Guid idpost, Guid iduser)
        {
            LibProtocol.Request request = new LibProtocol.Request();
            request.command = LibProtocol.Command.Reaction;
            request.data = new LibProtocol.Models.Reactions { Id = Guid.NewGuid(), IdUser = iduser, Reaction = reaction, IdPost = idpost };
            try
            {
                bf.Serialize(stream, request);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
        public void Subscribe(Guid iduser, Guid idfreand)
        {
            LibProtocol.Request request = new LibProtocol.Request();
            request.command = LibProtocol.Command.Subscription;
            request.data = new LibProtocol.Models.Subscriptions { Id = Guid.NewGuid(), IdUser = iduser, IdFreand = idfreand };
            try
            {
                bf.Serialize(stream, request);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
        public void Delete(Guid id)
        {
            LibProtocol.Request request = new LibProtocol.Request();
            request.command = LibProtocol.Command.Remove;
            request.data = id;
            try
            {
                bf.Serialize(stream, request);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
        public void chekOnline(Guid id)
        {
            LibProtocol.Request request = new LibProtocol.Request();
            request.command = LibProtocol.Command.Chek;
            request.data = id;
            try
            {
                bf.Serialize(stream, request);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
        public void Search(LibProtocol.Models.User user)
        {
            LibProtocol.Request request = new LibProtocol.Request();
            request.command = LibProtocol.Command.Search;
            request.data = user;
            try
            {
                bf.Serialize(stream, request);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
        public void monOnline(Guid iduser)
        {
            LibProtocol.Request request = new LibProtocol.Request();
            request.command = LibProtocol.Command.Online;
            request.data = iduser;
            try
            {
                bf.Serialize(stream, request);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
        public void waitResponse(Action<LibProtocol.Response> onOk)
        {
            LibProtocol.Response response = (LibProtocol.Response)bf.Deserialize(stream);
            if (response.succces)
            {
                onOk(response);
            }
            else
            {
                onError?.Invoke(response.StatusTxt);
            }
        }
    }
}
