using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ishTime
{
    internal class Models
    {
        private string connectionString;

        public Models()
        {

            connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\Desktop\global\C#\ishTime\ishTime\timesDB.mdf;Integrated Security=True";
        }

        void InsertData(DateTime data, string column)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = $"INSERT INTO activity ({column}) VALUES (@data)";
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@data", data);
                    command.ExecuteNonQuery();
                }
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT start_date FROM activity WHERE id = (SELECT MAX(id) FROM activity)";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(updateSql, connection))
                {
                    command.Parameters.AddWithValue("@Value1", end_date);

                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }

        public double GetTodayStat()
        {
            double result = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT * FROM activity WHERE CONVERT(date, start_date) = CONVERT(date, GETDATE());";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
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
