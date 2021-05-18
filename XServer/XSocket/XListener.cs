using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XServer.XMessages;

namespace XServer.XSocket
{
    class XListener
    {
        Socket m_ListenerSocket;
        int m_port;

        List<XClient> m_ClientList = new List<XClient>();
        public XListener(int nPort)
        {
            this.m_port = nPort;
            m_ListenerSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

        }
        public void StartListen()
        {
            m_ListenerSocket.Bind(new IPEndPoint(IPAddress.Any, this.m_port));
            m_ListenerSocket.Listen(10);

            m_ListenerSocket.BeginAccept(new AsyncCallback(OnAccept), null);
        }
        void OnAccept(IAsyncResult ar)
        {
            Socket temp=m_ListenerSocket.EndAccept(ar);
            XClient xc = new XClient(temp);
            xc.m_OnMessageReceived += new OnMessageReceived(OnMessageReceived);
            xc.StartRelay();

            m_ClientList.Add(xc);
        }
        void OnMessageReceived(ChatCommand cc)
        {
            Console.WriteLine("A chat command Received from  "+cc.Usertype +" Client " + cc.Sender);
            Console.WriteLine("Message: " + cc.Command);
        }

    }
}
