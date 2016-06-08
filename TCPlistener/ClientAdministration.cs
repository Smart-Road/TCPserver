using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPlistener
{
    class ClientAdministration
    {
        private SocketHelper helper = new SocketHelper();
        public TcpListener tcpListener;
        private NetworkStream stream;
        public TcpClient tcpClient;
        private int zone { get; set; }
        public ClientAdministration()
        {

        }
        public int ConnectToClient()
        {
            try
            {

                IPAddress IpAddress = IPAddress.Any;
                tcpListener = new TcpListener(IpAddress, 13);
                tcpListener.Start();
            }
            catch
            {
                return -1;

            }
               tcpClient = tcpListener.AcceptTcpClient();
            if (tcpClient != null)
            {
                MessageBox.Show("verbonden");
            }
           
            stream = tcpClient.GetStream();
          
            
            return 0;

        }

        public int messagerecieve()
        {
            byte[] bytes = new byte[256];
            stream.Read(bytes, 0, bytes.Length);
            if (bytes.Length == 0)
            {
                return -1;
            }
            helper.processMsg(tcpClient, stream, bytes);
            return 0;

        }
    }
}
