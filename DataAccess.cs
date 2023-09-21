using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Data.SQLite;
using System;

namespace Valkryie
{
    public static class DataAccess
    {
        public static void InitializeDatabase()
        {
            string dbPath = "Valkryie.db";

            // Create a connection to the SQLite database
            using (var dbConnection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                dbConnection.Open();

                // Create the BaseLineExecutables table
                string BaseLineTableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS BaseLineExecutables (id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "fileName TEXT, " +
                    "filePath TEXT, " +
                    "fileHash TEXT)";

                using (var createBaseLineTable = new SQLiteCommand(BaseLineTableCommand, dbConnection))
                {
                    createBaseLineTable.ExecuteNonQuery();
                }

                // Create the CurrentExecutables table
                string CurrentTableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS CurrentExecutables (id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "fileName TEXT, " +
                    "filePath TEXT, " +
                    "fileHash TEXT)";

                using (var createCurrentTable = new SQLiteCommand(CurrentTableCommand, dbConnection))
                {
                    createCurrentTable.ExecuteNonQuery();
                }
            }
        }
    }
}