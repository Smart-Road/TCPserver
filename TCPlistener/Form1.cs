using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace TCPlistener
{
    public partial class Form1 : Form
    {
        private SocketHelper helper = new SocketHelper();
        private BackgroundWorker bw = new BackgroundWorker();
        ClientAdministration c = new ClientAdministration();
        public Form1()
        {
            InitializeComponent();

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            Database.PrepareConnection();
        }
        static string output = "";
        private void Start_Click(object sender, EventArgs e)
        {
            MessageRecieve.Start();
            startwork();
        }
        private void startwork()
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
                connect.Enabled = false;
            }
            

        }
        private void ConnectToClient()
        {
            try
            {
                c.ConnectToClient();
            }
            catch (Exception ex)
            {
                output = "Error: " + ex.ToString();
                MessageBox.Show(output);
            }
            finally
            {
                if (c.tcpListener != null)
                    c.tcpListener.Stop();
            }

        }
        private void Stop_Click(object sender, EventArgs e)
        {
            connect.Enabled = true;
            if (bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
            }
            MessageRecieve.Stop();
            
            if (c.tcpClient != null || c.tcpListener != null)
            {
               
                
                c.tcpListener.Stop();
            }

        }
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            ConnectToClient();
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            connect.Enabled = true;
            if ((e.Cancelled == true))
            {
                tbProgress.Text = "Canceled!";
            }

            else if (!(e.Error == null))
            {
                tbProgress.Text = ("Error: " + e.Error.Message);
            }

            else
            {
                tbProgress.Text = "Done!";
            }
            listBox1.Items.Add(helper.LastMessage);
        }
        private void MessageRecieve_Tick(object sender, EventArgs e)
        {
         
            if (c.tcpClient != null)
            {
                int status = c.messagerecieve();
                if (status == -1)
                {
                    listBox1.Items.Add("geen bericht gekregen");
                }
                else
                {
                    listBox1.Items.Add("bericht ontvangen");
                }
            }
            else
            {
                c.ConnectToClient();   
            }
            

         
        }
    }
}
