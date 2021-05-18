using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XServer.XMessages;

namespace XClient.XSocket
{
    public delegate void OnMessageReceived(ChatCommand cc);
    class OurSocket
    {
        Socket m_ServerSocket;
        IPEndPoint m_ServerEP;
        public OnMessageReceived m_OnMessageReceived;
        byte[] m_Buffer = new byte[1024];

        public OurSocket(IPEndPoint nServerEP)
        {
            this.m_ServerEP = nServerEP;
            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void StartConnect()
        {
            m_ServerSocket.BeginConnect(m_ServerEP,new AsyncCallback(OnConnect),null);

        }
        void OnConnect(IAsyncResult ar)
        {
            m_ServerSocket.EndConnect(ar);

            m_ServerSocket.BeginReceive(m_Buffer, 0, m_Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
        }

        void OnReceive(IAsyncResult ar)
        {
            int length = m_ServerSocket.EndReceive(ar);
            if (length <= 0)
            {
                return;
            }
            
            m_ServerSocket.BeginReceive(m_Buffer, 0, m_Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
        }

        private void ExtractBuffer(byte[] m_Buffer, int length)
        {
            byte[] SizedBuffer = new byte[length];
            Array.Copy(m_Buffer, 0, SizedBuffer, 0, SizedBuffer.Length);

            if (SizedBuffer[0] == (byte)XMessageProtocols.HEADER)
            {
                XMessageProtocols xmp = (XMessageProtocols)SizedBuffer[1];
                switch (xmp)
                {
                    case XMessageProtocols.CHAT_EVENT:
                        if (m_OnMessageReceived != null)
                            m_OnMessageReceived(ChatCommand.ParseFrom(GetCommand(SizedBuffer)));
                        break;
                    case XMessageProtocols.INFO_EVENT:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Console.WriteLine("invalid buffer received");
            }
            
        }
        byte[] GetCommand(byte[] data)
        {
            byte[] temp = new byte[data.Length - 2];
            Array.Copy(data, 2, temp, 0, temp.Length);
            return temp;
        }

        public void SendChat(string nCommand)
        {
            ChatCommand.Builder ccb = new ChatCommand.Builder();
            ccb.SetSender("Benim ödev uyglamam");
            ccb.SetCommand(nCommand);
            ccb.SetUsertype(UserType.ADMIN);

            byte[] senddata = CreateCommand(ccb.Build().ToByteArray(), XMessageProtocols.CHAT_EVENT);

            SendBytes(senddata);
        }

        private void SendBytes(byte[] senddata)
        {
            m_ServerSocket.BeginSend(senddata, 0, senddata.Length, SocketFlags.None, new AsyncCallback(OnSend), null);

        }
        void OnSend(IAsyncResult ar)
        {
            int length = m_ServerSocket.EndSend(ar);
            if (length <= 0)
                return;
            
        }
        private byte[] CreateCommand(byte[] vs, XMessageProtocols cHAT_EVENT)
        {
            byte[] senddata = new byte[vs.Length + 2];
            senddata[0] = (byte)XMessageProtocols.HEADER;
            senddata[1] = (byte)cHAT_EVENT;

            Array.Copy(vs, 0, senddata, 2, vs.Length);

            return senddata;
        }
    }
}
