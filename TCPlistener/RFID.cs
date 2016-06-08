using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TCPlistener
{
    [Serializable]
    public class RFID
    {
        public  string Nummer { get;  set; }
        public int Snelheid { get;  set; }
        
        public RFID(string nummer, int snelheid)
        {
            this.Nummer = nummer;
            this.Snelheid = snelheid;
        }      
    }
}

