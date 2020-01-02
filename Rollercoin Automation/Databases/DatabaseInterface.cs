using MySql.Data.MySqlClient;
using Rollercoin.API.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollercoin.API.Databases
{
    public class DatabaseInterface
    {
        public string Server;
        public string Username;
        public string Password;
        public string Database;
        MySqlConnection connection;

        public DatabaseInterface(string server, string username, string password, string database)
        {
            Server = server;
            Username = username;
            Password = password;
            Database = database;
        }

        public bool Write(BotGameLog log)
        {
            if (connection == null)
                if(!OpenConn())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[DatabaseInterface] Failed to open the database connection.");
                    Console.ResetColor();
                    return false;
                }

            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO `logs` (`ID`, `Date`, `GameResult`, `GameType`, `GainPowerResult`) VALUES (NULL, '{log.Date.ToBinary()}', '{(int)log.GameResult}', '{log.GameType}', '{(int)log.GainPowerResult}');";
            cmd.ExecuteNonQuery();
            return true;
        }

        public Dictionary<int, BotGameLog> ReadAllRows()
        {
            if (connection == null)
                if (!OpenConn())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[DatabaseInterface] Failed to open the database connection.");
                    Console.ResetColor();
                    return null;
                }

            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM `logs`;";
            MySqlDataReader reader = cmd.ExecuteReader();
            Dictionary<int, BotGameLog> entries = new Dictionary<int, BotGameLog>();
            while(reader.Read())
            {
                BotGameLog log = new BotGameLog(DateTime.FromBinary((long)reader["Date"]), (GameResult)(int)reader["GameResult"], (string)reader["GameType"], (GainPowerResult)(int)reader["GainPowerResult"]);
                entries.Add((int)reader["ID"], log);
            }

            return entries;
        }

        public bool OpenConn()
        {
            connection = new MySqlConnection($"Server={Server};Database={Database};UID={Username};Pwd={Password};");
            connection.Open();
            return (connection.State == System.Data.ConnectionState.Open);
        }

        public bool IsConnOpen()
        {
            if (connection == null) return false;
            return(connection.State == System.Data.ConnectionState.Open);
        }
    }
}
