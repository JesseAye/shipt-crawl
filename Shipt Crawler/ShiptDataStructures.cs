using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipt_Crawler
{
	public struct Shipt_Product
	{
		public UInt32 Product_ID;
		public string Brand_Name;
		public string Product_Name;
		public decimal Regular_Price;
		public decimal Sale_Price;
		public decimal Units;
		public string Unit_Type;
		private List<string> PromotionsPrivate;
		public string Store;
		public string Address;

		public List<string> Promotions { get { return PromotionsPrivate ?? new List<string>(); } }

		/// <summary>
		/// Add a promotion to the product
		/// </summary>
		/// <param name="promotion">The promotion string to be added</param>
		/// <returns>Returns true if successfully added</returns>
		public bool AddPromotion(string promotion)
		{
			try
			{
				if(PromotionsPrivate == null)
				{
					PromotionsPrivate = new List<string>();
				}

				PromotionsPrivate.Add(promotion);
				return true;
			}

			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Provides the unit price of an item
		/// e.g. 0.25
		/// </summary>
		/// <returns>Either the unit price WITHOUT descriptive formatting, or 0 for invalid value</returns>
		public decimal PricePerUnit()
		{
			if(Units <= 0)
			{
				if(Sale_Price <= 0)
				{
					return (Sale_Price / Units);
				}

				else if (Regular_Price <= 0)
				{
					return (Regular_Price / Units);
				}

				else
				{
					return 0;
				}
			}

			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Provides a descriptive unit price of an item
		/// e.g. $0.25 per fl oz
		/// </summary>
		/// <returns>Either the unit price formatted to be descriptive of unit type, or 0 for invalid value</returns>
		public string PricePerUnitFormatted()
		{
			if (Units <= 0
				&& (Unit_Type != null && Unit_Type != ""))
			{
				if (Sale_Price <= 0)
				{
					return ("$" + Sale_Price / Units  + "per " + Unit_Type);
				}

				else if (Regular_Price <= 0)
				{
					return ("$" + Regular_Price / Units + "per " + Unit_Type);
				}

				else
				{
					return "0";
				}
			}

			else
			{
				return "0";
			}
		}

		public string UnitWithUnitType()
		{
			if ((Units > 0)
				&& ((Unit_Type != null)
					&& Unit_Type != ""))
			{
				return Units + " " + Unit_Type;
			}

			else
			{
				return "";
			}
		}
	}

	public struct Available_Stores
	{
		public string Delivery_Address;
		private List<string> StoreNamesPrivate;

		public List<string> StoreNames { get { return StoreNamesPrivate ?? new List<string>(); } }

		/// <summary>
		/// Add a store to the delivery address
		/// </summary>
		/// <param name="store">The store name to be added</param>
		/// <returns>Returns true if successfully added</returns>
		public bool AddStore(string store)
		{
			try
			{
				if (StoreNamesPrivate == null)
				{
					StoreNamesPrivate = new List<string>();
				}

				StoreNamesPrivate.Add(store);
				return true;
			}

			catch (Exception)
			{
				throw;
			}
		}
	}
}
