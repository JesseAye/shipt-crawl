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
		public List<string> Promotions;
		public string Store;

		/// <summary>
		/// DO NOT "Shipt_Product.Promotions.Add()", Promotions needs to be checked for null.
		/// </summary>
		/// <param name="promotion">The promotion string to be added</param>
		/// <returns>Returns true if sucessfully added</returns>
		public bool AddPromotion(string promotion)
		{
			try
			{
				if(Promotions == null)
				{
					Promotions = new List<string>();
				}

				Promotions.Add(promotion);
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
				&& (Unit_Type != null || Unit_Type != ""))
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
	}
}
