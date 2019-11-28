using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace Shipt_Crawler
{
	class DB_Manager
	{
		#region Variables
		/// <summary>
		/// The default file path for the .db file.
		/// </summary>
		private const string DefaultDatabaseFile = @".\Database.db";
		#endregion

		#region Methods

		#region Create
		/// <summary>
		/// Add a new delivery address to the Addresses table.
		/// </summary>
		/// <param name="address">The address to be added.</param>
		/// <returns>True if successfully added.</returns>
		public static bool InsertAddress(string address)
		{
			if (!DatabaseExists())
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

					// If Address already exists in Database
					if (FindAddress_id(connection, address) > 0)
					{
						return false;
					}

					else
					{
						command.ExecuteNonQuery();
						return true;
					}
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
		/// Add new stores to the Stores table.<para/>
		/// Query's the Addresses table for a match for use at Stores.Address_id.<para/>
		/// Query's the Stores table for existing Stores.id and Stores.Address_id.
		/// </summary>
		/// <param name="stores">The address and stores to be added.</param>
		/// <returns>True if successfully added.</returns>
		public static bool InsertStores(Shipt_Available_Stores stores)
		{
			if (!DatabaseExists())
			{
				CreateDatabase();
			}

			using (SQLiteConnection connection = new SQLiteConnection(LoadConnectionString()))
			{
				int Address_ID;

				SQLiteCommand QueryStores = new SQLiteCommand("SELECT Stores.Store FROM Stores WHERE (Address_id = ?)", connection);

				SQLiteCommand command = new SQLiteCommand("INSERT INTO Stores (Address_id, Store, Active) VALUES (?, ?, ?)", connection);
				command.Parameters.Add("Address_id", DbType.Int32);
				command.Parameters.Add("Store", DbType.String);
				command.Parameters.Add("Active", DbType.String);

				try
				{
					connection.Open();

					Address_ID = FindAddress_id(connection, stores.Delivery_Address);

					if (Address_ID > 0)
					{
						command.Parameters["Address_id"].Value = Address_ID;

						List<string> modifiedStores = stores.StoreNames;

						QueryStores.Parameters.AddWithValue("Address_id", Address_ID);
						SQLiteDataReader reader = QueryStores.ExecuteReader();

						// Take out any existing stores from modifiedStores so we don't spend time adding rows that already exist
						while (reader.Read())
						{
							if (modifiedStores.Contains(reader[0].ToString()))
							{
								modifiedStores.Remove(reader[0].ToString());
							}
						}

						foreach (string store in modifiedStores)
						{
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
					command.Dispose();
				}
			}
		}

		/// <summary>
		/// Add new product to the Products table.<para/>
		/// Query's the Addresses table for a match for use at Stores.Address_id and Products.Address_id.<para/>
		/// Query's the Stores table for a match for use at Products.Store_id.<para/>
		/// Query's the Products table for existing Products.Address_id, Products.Store_id, and Products.Product_id.
		/// </summary>
		/// <param name="product"></param>
		/// <returns></returns>
		public static bool InsertProduct(Shipt_Product product)
		{
			if (!DatabaseExists())
			{
				CreateDatabase();
			}

			using (SQLiteConnection connection = new SQLiteConnection(LoadConnectionString()))
			{
				int Address_ID = 0;
				SQLiteCommand QueryAddressID = new SQLiteCommand("SELECT Addresses.id FROM Addresses WHERE (Address LIKE ?)", connection);
				QueryAddressID.Parameters.AddWithValue("Address", product.Address + "%");

				int Store_ID = 0;
				SQLiteCommand QueryStoreID = new SQLiteCommand("SELECT Stores.id FROM Stores WHERE (Address_id = ?) AND (Store = ?)", connection);

				SQLiteCommand QueryProductID = new SQLiteCommand("SELECT * FROM Products WHERE (Address_id = ?) AND (Store_id = ?) AND (Product_id = ?)", connection);
				QueryProductID.Parameters.AddWithValue("Product_id", product.Product_ID);

				SQLiteCommand InsertProduct = new SQLiteCommand("INSERT INTO Products (Address_id, Store_id, Product_id, Brand_Name, Product_Name, Units, Unit_Type, Active) VALUES (?, ?, ?, ?, ?, ?, ?, ?)", connection);

				try
				{
					connection.Open();
					SQLiteDataReader AddressReader = QueryAddressID.ExecuteReader();

					// If AddressReader returned at least 1 row
					if (AddressReader.HasRows)
					{
						while (AddressReader.Read())
						{
							Address_ID = Convert.ToInt32(AddressReader["id"]);

							if (AddressReader.Read())
							{
								MessageBox.Show("While reading the Addresses table, it somehow returned multiple rows for one address. You should have this checked out by a professional!", "Addresses returned multiple rows", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							}

							AddressReader.Close();
							break;
						}

						QueryStoreID.Parameters.AddWithValue("Address_id", Address_ID);
						QueryStoreID.Parameters.AddWithValue("Store", product.Store);

						SQLiteDataReader StoreReader = QueryStoreID.ExecuteReader();

						// If StoreReader returned at least 1 row
						if (StoreReader.HasRows)
						{
							while (StoreReader.Read())
							{
								Store_ID = Convert.ToInt32(StoreReader["id"]);

								if (StoreReader.Read())
								{
									MessageBox.Show("While reading the Stores table, it somehow returned multiple rows for one address and store. You should have this checked out by a professional!", "Stores returned multiple rows", MessageBoxButtons.OK, MessageBoxIcon.Warning);
								}

								StoreReader.Close();
								break;
							}

							QueryProductID.Parameters.AddWithValue("Address_id", Address_ID);
							QueryProductID.Parameters.AddWithValue("Store_id", Store_ID);

							SQLiteDataReader ProductReader = QueryProductID.ExecuteReader();

							// If ProductReader returned at least 1 row, then the product is already being tracked
							if (ProductReader.HasRows)
							{
								// TODO: Handle the case when a product already exists in the Products table
								ProductReader.Close();
							}

							// The product does not yet exist
							else
							{
								InsertProduct.Parameters.AddWithValue("Address_id", Address_ID);
								InsertProduct.Parameters.AddWithValue("Store_id", Store_ID);
								InsertProduct.Parameters.AddWithValue("Product_id", product.Product_ID);
								InsertProduct.Parameters.AddWithValue("Brand_Name", product.Brand_Name);
								InsertProduct.Parameters.AddWithValue("Product_Name", product.Product_Name);
								InsertProduct.Parameters.AddWithValue("Units", product.Units);
								InsertProduct.Parameters.AddWithValue("Unit_Type", product.Unit_Type);
								InsertProduct.Parameters.AddWithValue("Active", "True");
								InsertProduct.ExecuteNonQuery();
							}
						}
					}


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
		#endregion

		#region Read

		/// <summary>
		/// (Will) Returns a list of Shipt_Products from the Products table.
		/// </summary>
		/// <param name="fromAddress">Filter by Address_id</param>
		/// <param name="fromStore">Filter by Store_id</param>
		public static void ReadProducts(string fromAddress = null, string fromStore = null)
		{
			SQLiteCommand QueryProducts;

			using (SQLiteConnection connection = new SQLiteConnection(LoadConnectionString()))
			{

				try
				{
					connection.Open();

					// If a certain address is specified
					if (!string.IsNullOrWhiteSpace(fromAddress))
					{
						int Address_id = FindAddress_id(connection, fromAddress);

						// If a certain address AND store is specified
						if (!string.IsNullOrWhiteSpace(fromStore))
						{
							int Store_id = FindStore_id(connection, fromStore, Address_id);
							QueryProducts = new SQLiteCommand("SELECT * FROM Products WHERE (Address_id = ?) AND (Store_id = ?)", connection);
							QueryProducts.Parameters.AddWithValue("Address_id", Address_id);
							QueryProducts.Parameters.AddWithValue("Store_id", Store_id);
						}

						// If only a certain address is specified
						else
						{
							QueryProducts = new SQLiteCommand("SELECT * FROM Products WHERE (Address_id = ?)", connection);
							QueryProducts.Parameters.AddWithValue("Address_id", Address_id);
						}
					}

					// If no address and store is specified
					else
					{
						QueryProducts = new SQLiteCommand("SELECT * FROM Products", connection);
					}

					connection.Close();
				}

				catch (Exception)
				{

					throw;
				}
			}
		}

		/// <summary>
		/// Uses the <paramref name="connection"/> to query the Addresses table for a specific Address_id.
		/// </summary>
		/// <param name="connection">The OPENED connection to use.</param>
		/// <param name="Address">The Addresses.Address to query with.</param>
		/// <returns></returns>
		private static int FindAddress_id(SQLiteConnection connection, string Address)
		{
			if (connection.State == ConnectionState.Open)
			{
				if (!string.IsNullOrWhiteSpace(Address))
				{
					SQLiteCommand QueryAddressID = new SQLiteCommand("SELECT Addresses.id FROM Addresses WHERE (Address LIKE ?)", connection);
					QueryAddressID.Parameters.AddWithValue("Address", Address + "%");

					SQLiteDataReader reader = QueryAddressID.ExecuteReader();
					if (reader.HasRows)
					{
						while (reader.Read())
						{
							int Address_ID = Convert.ToInt32(reader["id"]);

							if (reader.Read())
							{
								MessageBox.Show("While reading the Addresses table, it somehow returned multiple rows for one address. You should have this checked out by a professional!", "Addresses returned multiple rows", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							}

							reader.Close();
							return Address_ID;
						}
					}
				}
			}

			return 0;
		}

		/// <summary>
		/// Uses the connection to query the Stores table for a specific Stores.id.
		/// </summary>
		/// <param name="connection">he OPENED connection to use.</param>
		/// <param name="Store">The Stores.Store to query with.</param>
		/// <param name="Address_id">The Addresses.id to query with.</param>
		/// <returns></returns>
		private static int FindStore_id(SQLiteConnection connection, string Store, int Address_id)
		{
			if (connection.State == ConnectionState.Open)
			{
				if ((!string.IsNullOrWhiteSpace(Store))
					&& (Address_id > 0))
				{
					SQLiteCommand QueryStoreID = new SQLiteCommand("SELECT Stores.id FROM Stores WHERE (Address_id = ?) AND (Store = ?)", connection);
					QueryStoreID.Parameters.AddWithValue("Address_id", Address_id);
					QueryStoreID.Parameters.AddWithValue("Store", Store);

					SQLiteDataReader reader = QueryStoreID.ExecuteReader();
					if (reader.HasRows)
					{
						while (reader.Read())
						{
							int Store_id = Convert.ToInt32(reader["id"]);

							if (reader.Read())
							{
								MessageBox.Show("While reading the Stores table, it somehow returned multiple rows for one address and Store. You should have this checked out by a professional!", "Stores returned multiple rows", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							}

							reader.Close();
							return Store_id;
						}
					}
				}
			}

			return 0;
		}

		#endregion

		/// <summary>
		/// Creates a new database to store data
		/// </summary>
		/// <param name="FilePath">The filepath where the Database file is located</param>
		/// <returns></returns>
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
		#endregion
	}
}
