using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPlistener
{
    class handleMessage
    {
        public string Message { get; private set; }
        private int zone { get; set; }
  
        public handleMessage(string message)
        {
            Message = message;
        }

        public string Compare(string bericht, Stream stream)
        {
            
            string assemblemessage = "";
            if (bericht.Equals("%Hello$"))
            {
                string message = ACK(bericht);
          
                assemblemessage += "ACK: " + message + Environment.NewLine;
            }
            else if (bericht.Equals("%Lijst ophalen$"))
            {
               
                List<RFID> list = new List<RFID>();
                list = DatabaseQueries.LoadAllFromDatabase();
                foreach (RFID rfid in list)
                {
                  
                    string message = "RFID nummer:" + rfid.Nummer + ", RFID Snelheid:" + rfid.Snelheid + ",";

                    assemblemessage += "Lijst: " + message + Environment.NewLine;
                }
                 
               
            }
            else if (bericht.StartsWith("%Voeg RFID Toe"))
            {
               
                string message = AddRFID(bericht);

                assemblemessage +="AddingRFID: "+ message + Environment.NewLine;
            }
             else if (bericht.StartsWith("%Zone"))
            {
               
                zone = ZoneParse(bericht);
             
                 
                assemblemessage +="Zone: "+ zone + " doorgegeven" + Environment.NewLine;
            }
            else if (bericht.StartsWith("%Synchroniseer:"))
            {
                int time = TimestampParse(bericht);
                List<RFID> list = new List<RFID>();
                list = DatabaseQueries.LoadAllFromDatabasewithcertainTimestamp(time);
                foreach (RFID rfid in list)
                {

                    string message = "RFID nummer:" + rfid.Nummer + ", RFID Snelheid:" + rfid.Snelheid + ",";

                    assemblemessage += "Lijst: " + message + Environment.NewLine;
                }
            }
            return assemblemessage;
        }

        private string ACK(string bericht)
        {
            if (bericht.Equals("%Hello$"))
            {
                Message = "%Goodbye$";
            }
            else
            {
                Message = "%What?$";
            }
            return Message;

        }

        private string AddRFID(string bericht)
        {
           /* int count = RFID.GetCountFromDatabase() + 1;*/
            string[] woorden = bericht.Split(':');
            if (woorden[0] == "%Voeg RFID Toe")
            {
                string[] insert = woorden[1].Split(',');
                string snelheid = insert[1].TrimEnd('$');
                RFID rfid = new RFID(Convert.ToString(insert[0]), Convert.ToInt32(snelheid));
                DatabaseQueries.SaveToDatabase(rfid.Nummer, rfid.Snelheid,zone);
                Message = "%RFID toegevoegd$";
            }
            return Message;
        }

        private int ZoneParse(string bericht)
        {
            int zonepar = 0;
            string[] woorden = bericht.Split(':');
            if (woorden[0] == "%Zone")
            {
                zonepar = Convert.ToInt32(woorden[1]);
            }
            return zonepar;
        }
        private int TimestampParse(string bericht)
        {
            int timestamp = 0;
            string[] woorden = bericht.Split(':');
            if (woorden[0] == "%Synchroniseer")
            {
                timestamp = Convert.ToInt32(woorden[1]);
            }
            return timestamp;

        }

    }
}
