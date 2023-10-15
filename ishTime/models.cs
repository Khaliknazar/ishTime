using System;
using System.IO;
using System.Configuration;
using System.Windows;
using System.Data.SQLite;
using System.Runtime.Remoting.Contexts;

namespace ishTime
{
    internal class Models
    {
        private string connectionString;

        public Models()
        {
            connectionString = @"Data Source=C:\ishTime.db; Version=3;";

            if (!Properties.Settings.Default.isFirstRun) //CHNAGE BOOLEAN!!!!!!!!!!
            {
                if (!File.Exists(@"C:\ishTime.db"))
                {
                    SQLiteConnection.CreateFile(@"C:\ishTime.db");

                    using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=C:\ishTime.db; Version=3;"))
                    {
                        string createTableSql = @"
                        CREATE TABLE activity (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        start_date DATETIME NULL,
                        end_date DATETIME NULL DEFAULT 0
                        );";
                        SQLiteCommand Command = new SQLiteCommand(createTableSql, Connect);
                        Connect.Open();
                        Command.ExecuteNonQuery();
                        Connect.Close();
                    }

                }             
            }

        }

        void InsertData(DateTime data, string column)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                
                string insertQuery = $"INSERT INTO activity ({column}) VALUES (@data)";
                SQLiteCommand Command = new SQLiteCommand(insertQuery, connection);
                Command.Parameters.AddWithValue("@data", data);
                connection.Open();
                Command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void StartTimer()
        {
            DateTime dateTime = DateTime.Now;
            this.InsertData(dateTime, "start_date");
        }

        public void StopTimer()
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = GetDateTime();
            if (startDate.Day != endDate.Day)
            {

                this.UpdateLastRow(new DateTime(year: startDate.Year, day: startDate.Day, month: startDate.Month, hour: 23, minute: 59, second: 59));
                this.InsertData(new DateTime(year: startDate.Year, day: endDate.Day, month: startDate.Month), "start_date");
            }
            this.UpdateLastRow(endDate);

        }

        public TimeSpan GetActiveTime()
        {

            return DateTime.Now - GetDateTime();
        }


        public DateTime GetDateTime()
        {
            DateTime result = new DateTime();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT start_date FROM activity WHERE id = (SELECT MAX(id) FROM activity)";
                using (SQLiteCommand command = new SQLiteCommand(sqlQuery, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime startDate = (DateTime)reader["start_date"];
                            result = startDate;
                           
                        }
                    }
                }
            }
            return result;
        }

        public void UpdateLastRow(DateTime end_date)
        {
            string updateSql = "UPDATE activity SET end_date = @Value1 WHERE ID = (SELECT MAX(id) FROM activity)";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(updateSql, connection))
                {
                    command.Parameters.AddWithValue("@Value1", end_date);

                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }

        public double GetTodayStat()
        {
            double result = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT * FROM activity WHERE DATE(start_date) = DATE('now', 'localtime');";
                using (SQLiteCommand command = new SQLiteCommand(sqlQuery, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime startDate = (DateTime)reader["start_date"];
                            result +=  ((DateTime)reader["end_date"] - (DateTime)reader["start_date"]).TotalSeconds;

                        }
                    }
                }
            }

            return result;
        }
    }
}
