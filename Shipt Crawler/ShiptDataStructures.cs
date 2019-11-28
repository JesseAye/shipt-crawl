using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipt_Crawler
{
	/// <summary>
	/// Stores the information regarding a product.
	/// </summary>
	public struct Shipt_Product
	{
		#region Variables
		/// <summary>
		/// The ID of the product, usually in reference to the number of the product in the URL.
		/// </summary>
		public UInt32 Product_ID;
		/// <summary>
		/// The brand name of the product if it has a brand.
		/// </summary>
		public string Brand_Name;
		/// <summary>
		/// The product name.
		/// </summary>
		public string Product_Name;
		/// <summary>
		/// The regular price of the product.
		/// </summary>
		public decimal Regular_Price;
		/// <summary>
		/// The sale price of the product if applicable.
		/// </summary>
		public decimal Sale_Price;
		/// <summary>
		/// Attempts to reference the numerical weight/size/quantity of the product.
		/// </summary>
		public decimal Units;
		/// <summary>
		/// Attempts to reference the type of weight/size/quantity of the product (oz, per lb, cans, etc).
		/// </summary>
		public string Unit_Type;
		/// <summary>
		/// The list of promotions currently applicable to the product.
		/// </summary>
		private List<string> PromotionsPrivate;
		/// <summary>
		/// The store name the product exists in.
		/// </summary>
		public string Store;
		/// <summary>
		/// The address the store delivers to.
		/// </summary>
		public string Address;
		#endregion

		#region Methods
		/// <summary>
		/// Returns the list of promotions currently applicable to the product.
		/// </summary>
		public List<string> Promotions { get { return PromotionsPrivate ?? new List<string>(); } }

		/// <summary>
		/// Add a promotion to the product.
		/// </summary>
		/// <param name="promotion">The promotion string to be added.</param>
		/// <returns>Returns true if successfully added.</returns>
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

		/// <summary>
		/// Combines <paramref name="Units"/> and <paramref name="Unit_Type"/>.
		/// </summary>
		/// <returns>If <paramref name="Units"/> and <paramref name="Unit_Type"/> are valid strings, returns concatenated. Else returns empty string.</returns>
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
				return string.Empty;
			}
		}
		#endregion
	}

	/// <summary>
	/// Stores the delivery address and the applicable stores available for that address.
	/// </summary>
	public struct Shipt_Available_Stores
	{
		#region Variables
		/// <summary>
		/// The full delivery address as gathered from the ChooseStore-form.
		/// </summary>
		public string Delivery_Address;
		/// <summary>
		/// The list of store names available at the <paramref name="Delivery_Address"/>.
		/// </summary>
		private List<string> StoreNamesPrivate;
		#endregion

		#region Methods
		/// <summary>
		/// Returns the list of store names currently applicable to the <paramref name="Delivery_Address"/>.
		/// </summary>
		public List<string> StoreNames { get { return StoreNamesPrivate ?? new List<string>(); } }

		/// <summary>
		/// Add a store for the <paramref name="Delivery_Address"/>.
		/// </summary>
		/// <param name="store">The store name to be added<./param>
		/// <returns>Returns true if successfully added.</returns>
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
		#endregion
	}
}
