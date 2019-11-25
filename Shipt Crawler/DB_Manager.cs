using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Shipt_Crawler
{
	class DB_Manager
	{
		private const string DefaultDatabaseFile = @".\Database.db";

		/// <summary>
		/// Add a new delivery address to the database
		/// </summary>
		/// <param name="address">The address to be added</param>
		/// <returns>True if successfully added</returns>
		public static bool InsertAddress(string address)
		{
			if(!DatabaseExists())
			{
				CreateDatabase();
			}

			using (SQLiteConnection connection = new SQLiteConnection(LoadConnectionString()))
			{
				SQLiteCommand QueryAddressID = new SQLiteCommand("SELECT * FROM Addresses WHERE (Address = ?)", connection);
				QueryAddressID.Parameters.AddWithValue("Address", address);

				SQLiteCommand command = new SQLiteCommand("INSERT INTO Addresses (Address, Active) VALUES (?, ?)", connection);
				command.Parameters.AddWithValue("Address", address);
				command.Parameters.AddWithValue("Active", "True");

				try
				{
					connection.Open();
					SQLiteDataReader reader = QueryAddressID.ExecuteReader();

					while(reader.Read()) // If QueryAddressID has any rows, then the address is already in the DB
					{
						return false;
					}

					command.ExecuteNonQuery();
					return true;
				}

				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					return false;
				}

				finally
				{
					connection.Close();
				}
			}
		}

		/// <summary>
		/// Add new stores to the database
		/// </summary>
		/// <param name="stores"></param>
		/// <returns></returns>
		public static bool InsertStores(Available_Stores stores)
		{
			if (!DatabaseExists())
			{
				CreateDatabase();
			}

			using (SQLiteConnection connection = new SQLiteConnection(LoadConnectionString()))
			{
				int Address_ID;
				SQLiteCommand QueryAddressID = new SQLiteCommand("SELECT Addresses.id FROM Addresses WHERE (Address = ?)", connection);
				QueryAddressID.Parameters.AddWithValue("Address", stores.Delivery_Address);

				SQLiteCommand QueryStores = new SQLiteCommand("SELECT Stores.Store FROM Stores WHERE (Address_id = ?)", connection);

				SQLiteCommand command = new SQLiteCommand("INSERT INTO Stores (Address_id, Store, Active) VALUES (?, ?, ?)", connection);
				command.Parameters.Add("Address_id", DbType.Int32);
				command.Parameters.Add("Store", DbType.String);
				command.Parameters.Add("Active", DbType.String);

				try
				{
					connection.Open();
					SQLiteDataReader reader = QueryAddressID.ExecuteReader();
					reader.Read();
					bool ValidStoreID = int.TryParse(reader["id"].ToString(), out Address_ID);
					reader.Close();

					if(ValidStoreID)
					{
						List<string> modifiedStores = stores.StoreNames;

						QueryStores.Parameters.AddWithValue("Address_id", Address_ID);
						reader = QueryStores.ExecuteReader();

						// Take out any existing stores from modifiedStores so we don't spend time adding rows that already exist
						while (reader.Read())
						{
							if(modifiedStores.Contains(reader[0].ToString()))
							{
								modifiedStores.Remove(reader[0].ToString());
							}
						}

						foreach (string store in modifiedStores)
						{
							command.Parameters["Address_id"].Value = Address_ID;
							command.Parameters["Store"].Value = store;
							command.Parameters["Active"].Value = "True";
							command.ExecuteNonQuery();
						}
					}

					return true;
				}

				catch (Exception ex)
				{
					return false;
				}

				finally
				{
					connection.Close();
					QueryAddressID.Dispose();
					command.Dispose();
				}
			}
		}

		private static bool CreateDatabase(string FilePath = DefaultDatabaseFile)
		{
			using (SQLiteConnection connection = new SQLiteConnection(LoadConnectionString()))
			{
				string sql;
				sql = @"
CREATE TABLE IF NOT EXISTS Stores (
	id	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	Address_id	INTEGER NOT NULL,
	Store	TEXT NOT NULL,
	Active	TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS Price_History (
	Address_id	INTEGER NOT NULL,
	Store_id	INTEGER NOT NULL,
	Product_id	INTEGER NOT NULL,
	Date	TEXT NOT NULL,
	Time	TEXT,
	Regular_Price	TEXT,
	Sale_Price	TEXT,
	Promotions	TEXT,
	PRIMARY KEY(Address_id,Store_id,Product_id,Date)
);
CREATE TABLE IF NOT EXISTS Addresses (
	id	INTEGER NOT NULL UNIQUE,
	Address	TEXT NOT NULL UNIQUE,
	Active	INTEGER NOT NULL,
	PRIMARY KEY(id)
);
CREATE TABLE IF NOT EXISTS Products (
	Address_id	INTEGER NOT NULL,
	Store_id	INTEGER NOT NULL,
	Product_id	INTEGER NOT NULL,
	Brand_Name	TEXT,
	Product_Name	TEXT,
	Units	TEXT,
	Unit_Type	TEXT,
	Active	TEXT,
	PRIMARY KEY(Address_id,Store_id,Product_id),
	FOREIGN KEY(Address_id) REFERENCES Addresses(id) ON UPDATE CASCADE,
	FOREIGN KEY(Store_id) REFERENCES Stores(id) ON UPDATE CASCADE
);";// The full SQL string. What a monstrosity.

				SQLiteCommand command = new SQLiteCommand(sql, connection);

				try
				{
					connection.Open();
					command.ExecuteNonQuery();
				}

				catch (Exception)
				{

					throw;
				}

				finally
				{
					if(connection.State != ConnectionState.Closed)
					{
						connection.Close();
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Determines if a Database already exists
		/// </summary>
		/// <param name="FilePath">The file path and name of the .db file starting in the application's working directory</param>
		/// <returns>True or false depending if it exists</returns>
		private static bool DatabaseExists(string FilePath = DefaultDatabaseFile)
		{
			return File.Exists(FilePath);
		}

		/// <summary>
		/// Provides a connection string
		/// </summary>
		/// <param name="FilePath">The file path and name of the .db file</param>
		/// <returns>The connection string</returns>
		private static string LoadConnectionString(string FilePath = DefaultDatabaseFile)
		{
			return @"Data Source=" + @FilePath + @";Version=3;";
		}
	}
}
