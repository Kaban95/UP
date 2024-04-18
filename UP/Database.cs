﻿using System;
using System.Data.SqlClient;

namespace UP
{
    internal class Database
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source = DESKTOP-RR23KG5\SQLEXPRESS;Initial Catalog = Practice;Integrated Security = True");

        public void OpenConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }
        public void CloseConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }
        public SqlConnection GetConnection()
        {
            return sqlConnection;
        }
    }
}
