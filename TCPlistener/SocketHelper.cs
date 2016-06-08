using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace TCPlistener
{
    class SocketHelper
    {
        TcpClient mscClient;
        string mstrMessage;
        string mstrResponse;
        byte[] bytesSent;
        private string messBytes;

        public String LastMessage { get; private set; }

        public void processMsg(TcpClient client, NetworkStream stream, byte[] bytesReceived)
        {
            // Handle the message received and  
            // sends a response back to the client.
            mstrMessage = Encoding.ASCII.GetString(bytesReceived, 0, bytesReceived.Length);
            mscClient = client;
            int count = 0;
            string message = Convert.ToString(mstrMessage);
            count = message.IndexOf("$")+1;
            message = message.Substring(0, count);
            messBytes = message;
            handleMessage hen = new handleMessage(messBytes);
            mstrMessage = hen.Compare(message, stream);
            LastMessage = mstrMessage;
            mstrResponse = hen.Message;
            
            bytesSent = Encoding.UTF8.GetBytes(mstrMessage);
            stream.Write(bytesSent, 0, bytesSent.Length);
        }

    }

}
