using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
namespace TCPlistener
{
    static class DatabaseQueries
    {
        public static List<RFID> LoadAllFromDatabase()
        {
            // Voer een select-query uit om alle kunsten uit te lezen
            Database.Query = "SELECT * FROM RFIDS";
            Database.OpenConnection();

            // De resultaten worden nu opgeslagen in een "reader": deze wordt in de while-loop
            // verderop gebruikt om nieuwe instanties van kunsten aan te maken
            SQLiteDataReader reader = Database.Command.ExecuteReader();

            // Onderstaande list bevat alle kunsten die uitgelezen worden
            List<RFID> RFID = new List<RFID>();
            while (reader.Read())
            {
                RFID.Add(new RFID(Convert.ToString(reader["nummer"]), Convert.ToInt32(reader["snelheid"])));
            }
            Database.CloseConnection();
            return RFID;
        }
        public static List<RFID> LoadAllFromDatabasewithcertainTimestamp(int timestamp)
        {
            Database.Query = "SELECT * FROM RFIDS WHERE Timestamp >" + timestamp;
            Database.OpenConnection();

            // De resultaten worden nu opgeslagen in een "reader": deze wordt in de while-loop
            // verderop gebruikt om nieuwe instanties van kunsten aan te maken
            SQLiteDataReader reader = Database.Command.ExecuteReader();

            // Onderstaande list bevat alle kunsten die uitgelezen worden
            List<RFID> RFID = new List<RFID>();
            while (reader.Read())
            {
                RFID.Add(new RFID(Convert.ToString(reader["nummer"]), Convert.ToInt32(reader["snelheid"])));
            }
            Database.CloseConnection();
            return RFID;
        }
        public static int GetCountFromDatabase()
        {
            // Zet een nieuwe verbinding op met de database
            SQLiteConnection connection = new SQLiteConnection("Data Source=" + Database.DatabaseFilename + ";Version=3");

            // Bouw de query op om het aantal studenten op te vragen
            string sql = "SELECT COUNT(*) FROM RFIDS";
            SQLiteCommand command = new SQLiteCommand(sql, connection);

            // Open de database-verbinding
            connection.Open();

            // Foutafhandling is hier achterwege gelaten. We slaan het resultaat van de
            // query tijdelijk op in een variabele omdat we eerst nog de database-verbinding
            // willen sluiten voordat we uit de functie stappen.

            int count = Convert.ToInt32(command.ExecuteScalar());

            // Sluit de database-verbinding, en retourneer het resultaat
            connection.Close();
            return count;
        }
        public static bool SaveToDatabase(string nummer, int snelheid, int zone)
        {
            // Bouw de insert-query op met de gegeven informatie

            DateTimeOffset dto = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            int timestamp = Convert.ToInt32(dto);
            var naam = SQLiteConvert.ToUTF8(nummer);
            Database.Query = "INSERT INTO RFIDS (Timestamp, nummer,snelheid) values (" + timestamp + ", " + naam
                 + ", " + snelheid + "," + zone + ")";

            Database.OpenConnection();

            bool success = false;
            try
            {
                // ExecuteNonQuery wordt gebruikt als we geen gegevens verwachten van de query
                Database.Command.ExecuteNonQuery();
                success = true;
            }
            catch (SQLiteException e)
            {
                // Code 19 geeft aan dat een veld wat uniek moet zijn in de database, dit door
                // deze insert niet meer zou zijn. Het is dus niet toegevoegd. Aangezien in deze
                // applicatie deze constraint alleen op het Kunstnummer staat, kunnen we de
                // foutmelding heel specifiek weergeven.
                if (e.ErrorCode == 19)
                {
                    return success;
                }
            }

            Database.CloseConnection();
            return success;
        }
        public static bool ReplaceExistingdatabase(List<RFID> replacList)
        {
            Database.Query = "DELETE * FROM RFIDS";
            Database.OpenConnection();
            bool success = false;

            try
            {
                // ExecuteNonQuery wordt gebruikt als we geen gegevens verwachten van de query
                Database.Command.ExecuteNonQuery();
            }
            catch (SQLiteException)
            {
                return success;
            }
            int count = DatabaseQueries.GetCountFromDatabase() + 1;
            foreach (RFID r in replacList)
            {
                Database.Query = "INSERT INTO RFIDS (count,nummer,snelheid) values (" + count + ", " + r.Nummer
                                 + ", " + r.Snelheid + ")";

                Database.OpenConnection();


                try
                {
                    // ExecuteNonQuery wordt gebruikt als we geen gegevens verwachten van de query
                    Database.Command.ExecuteNonQuery();
                    success = true;
                }
                catch (SQLiteException e)
                {
                    // Code 19 geeft aan dat een veld wat uniek moet zijn in de database, dit door
                    // deze insert niet meer zou zijn. Het is dus niet toegevoegd. Aangezien in deze
                    // applicatie deze constraint alleen op het Kunstnummer staat, kunnen we de
                    // foutmelding heel specifiek weergeven.
                    if (e.ErrorCode == 19)
                    {
                        return success;
                    }
                }
            }
            Database.CloseConnection();
            return success;
        }
        public static bool DeleteSelectionFromDatabase(RFID RF)
        {
            bool success = false;
            // Voer een select-query uit om alle kunsten uit te lezen
            Database.Query = "SELECT * FROM RFIDS";
            Database.OpenConnection();

            // De resultaten worden nu opgeslagen in een "reader": deze wordt in de while-loop
            // verderop gebruikt om nieuwe instanties van kunsten aan te maken
            SQLiteDataReader reader = Database.Command.ExecuteReader();

            // Onderstaande list bevat alle kunsten die uitgelezen worden
            List<RFID> listRFID = new List<RFID>();
            while (reader.Read())
            {
                listRFID.Add(new RFID(Convert.ToString(reader["nummer"]), Convert.ToInt32("snelheid")));
            }
            Database.CloseConnection();
            foreach (RFID rfid in listRFID)
            {
                if (rfid.Nummer == RF.Nummer)
                {
                    listRFID.Remove(rfid);
                }
            }
            foreach (RFID r in listRFID)
            {
                Database.Query = "INSERT INTO RFIDS (nummer,snelheid) values (" + r.Nummer
                                 + "', " + r.Snelheid + ")";

                Database.OpenConnection();


                try
                {
                    // ExecuteNonQuery wordt gebruikt als we geen gegevens verwachten van de query
                    Database.Command.ExecuteNonQuery();
                    success = true;
                }
                catch (SQLiteException e)
                {
                    // Code 19 geeft aan dat een veld wat uniek moet zijn in de database, dit door
                    // deze insert niet meer zou zijn. Het is dus niet toegevoegd. Aangezien in deze
                    // applicatie deze constraint alleen op het Kunstnummer staat, kunnen we de
                    // foutmelding heel specifiek weergeven.
                    if (e.ErrorCode == 19)
                    {
                        return success;
                    }
                }
            }
            Database.CloseConnection();
            return success;
        }
    }
}
