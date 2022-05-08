using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ClientOperations
    {
        TcpClient? tcpClient;
        Thread? threadClient;

        public static event Action<string>? onRun;

        BinaryFormatter bf = new BinaryFormatter();


        public ClientOperations(TcpClient client)
        {
            this.tcpClient = client;
            threadClient = new Thread(RunBin);
            threadClient.Start();
        }


        public void RunBin()
        {
            try
            {

                LibProtocol.Request req = (LibProtocol.Request)bf.Deserialize(tcpClient.GetStream());

                LibProtocol.Response response = new LibProtocol.Response();

                switch (req.command)
                {
                    case LibProtocol.Command.Registration:
                        Commands.Registration r = new Commands.Registration((LibProtocol.Models.User)req.data);
                        if (r.ChekLogin())
                        {
                            r.buildResponce(ref response);
                        }
                        else
                        {
                            response.succces = false;
                            response.code = LibProtocol.ResponseCode.Ok;
                            response.StatusTxt = "Vendor alredy exist";
                        }
                        break;
                    case LibProtocol.Command.Login:
                        Commands.Login l = new Commands.Login((LibProtocol.Models.User)req.data);
                        l.buildResponce(ref response);
                        break;
                    case LibProtocol.Command.addPost:
                        CommandServer.AddPost p = new CommandServer.AddPost((LibProtocol.Models.Posts)req.data);
                        p.buildResponce(ref response);
                        break;
                    case LibProtocol.Command.Online:
                        CommandServer.Online o = new CommandServer.Online((Guid)req.data);
                        o.buildResponce(ref response);
                        break;
                    case LibProtocol.Command.Reaction:
                        CommandServer.Reaction reac = new CommandServer.Reaction((LibProtocol.Models.Reactions)req.data);
                        reac.buildResponce(ref response);
                        break;
                    case LibProtocol.Command.Comment:
                        CommandServer.AddComent c = new CommandServer.AddComent((LibProtocol.Models.Comments)req.data);
                        c.buildResponce(ref response);
                        break;
                    case LibProtocol.Command.Chek:
                        Commands.Check ch = new Commands.Check((Guid)req.data);
                        ch.buildResponce(ref response);
                        break;
                    case LibProtocol.Command.Remove:
                        Commands.Remove rem = new Commands.Remove((Guid)req.data);
                        rem.buildResponce(ref response);
                        break;
                    case LibProtocol.Command.Subscription:
                        Commands.Subscription s = new Commands.Subscription((LibProtocol.Models.Subscriptions)req.data);
                        s.buildResponce(ref response);
                        break;
                    case LibProtocol.Command.Search:
                        Commands.Search ser = new Commands.Search((LibProtocol.Models.User)req.data);
                        ser.buildResponce(ref response);
                        break;
                    default:
                        response.succces = false;
                        response.code = LibProtocol.ResponseCode.Error;
                        response.StatusTxt = "Command not Found";
                        break;
                }
                bf.Serialize(tcpClient.GetStream(), response);
                Close();
            }
            catch (Exception ex)
            {
                onRun?.Invoke("Err: " + ex.Message);
                Close();
            }
        }
        void Close()
        {
            tcpClient?.Close();
            ClientConnect.clients.Remove(this);
        }
    }
}
