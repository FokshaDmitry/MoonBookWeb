using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ClientConnect
    {
        public event Action<string>? onAccess;
        public event Action<string>? onError;

        public IPAddress ip = IPAddress.Any;
        public int port = 3456;

        private ClientConnect() { }
        private static ClientConnect? _instance;
        public static ClientConnect getInstance()
        {
            if (ClientConnect._instance == null) _instance = new ClientConnect();
            return _instance;
        }

        TcpListener tcpListener;
        Thread threadListener;

        public void Connect()
        {
            if (tcpListener != null)
            {
                onError?.Invoke(" Socket is bisy");
                return;
            }
            if (threadListener != null)
            {
                onError?.Invoke(" Thread is bisy");
                return;
            }

            tcpListener = new TcpListener(ip, port);
            threadListener = new Thread(MyListener);
            threadListener.Start();
        }

        public void Disconnect()
        {
            tcpListener.Stop();
            isListener = false;
            onError?.Invoke(" Listener Stop ");

        }

        bool isListener = true;
        public static List<ClientOperations> clients = new List<ClientOperations>();
        public void MyListener()
        {
            try
            {
                tcpListener.Start();
                onError?.Invoke(" Listener Start ");
                while (isListener)
                {
                    clients.Add(new ClientOperations(tcpListener.AcceptTcpClient()));
                }
            }
            catch (Exception ex)
            {
                onError?.Invoke($"Err: {ex.Message}");
                tcpListener.Stop();
                throw;
            }

        }
    }
}
