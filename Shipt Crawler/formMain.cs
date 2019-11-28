using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Shipt_Crawler
{
	public partial class formMain : Form
	{
		#region Variables
		/// <summary>
		/// The login page to Shipt.
		/// </summary>
		readonly String URL_Login = "https://shop.shipt.com/login";
		/// <summary>
		/// The home page to Shipt (Should be checked for equivalency, not if the URL contains this string)
		/// </summary>
		readonly String URL_MainHomePage = "https://shop.shipt.com/";
		/// <summary>
		/// Check equivalency for browsing unfiltered, check if URL contains this for any type of (un)filtered browsing.
		/// </summary>
		readonly String URL_MainBrowse = "https://shop.shipt.com/search";
		/// <summary>
		/// The prefix for a product page
		/// </summary>
		readonly String URL_Prefix_Product = "https://shop.shipt.com/products/";
		/// <summary>
		/// The prefix for a featured promotion page.
		/// </summary>
		readonly String URL_Prefix_Featured_Promotion = "https://shop.shipt.com/featured-promotions/";

		/// <summary>
		/// The thread that the browser and everything relevant to it is ran on.
		/// </summary>
		Thread threadBrowser;
		/// <summary>
		/// Only to be written by the crawling thread, and read from the main (or any other) thread.
		/// </summary>
		private bool browserRunning = false;
		/// <summary>
		/// Set true by main (or any other) thread, read by the crawling thread, and set false at end of thread.
		/// </summary>
		private bool requestBrowserAbort = false;
		/// <summary>
		/// We need this so we don't attempt to modify any of the form's controls.
		/// </summary>
		private bool mainFormIsClosing = false;
		/// <summary>
		/// If the user is on a product page, this variable stores the information about the product.
		/// </summary>
		private Shipt_Product currentProduct;

		/// <summary>
		/// Whenever the URL of the browser matches the <paramref name="URL_Prefix_Product"/>, user should be on a product page.
		/// </summary>
		public event EventHandler<Shipt_Product> LandedOnProductPageEvent;
		/// <summary>
		/// Whenever the previous URL matched <paramref name="URL_Prefix_Product"/>, but is now on any page other than <paramref name="URL_Prefix_Product"/>.
		/// </summary>
		public event EventHandler<EventArgs> LeftProductPageEvent;
		/// <summary>
		/// Whenever the user is first logged in, the browser will check the delivery addresses and stores available.
		/// </summary>
		public event EventHandler<Shipt_Available_Stores> ReportAvailableStoresEvent;
		#endregion

		#region Form Methods

		/// <summary>
		/// Constructor for formMain, first form of program.
		/// </summary>
		public formMain()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Either starts the threadBrowser, or ends it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnCrawl_Click(object sender, EventArgs e)
		{
			if (browserRunning)
			{
				btnCrawl.Text = "Start Browser";
				requestBrowserAbort = true;

				LandedOnProductPageEvent -= FormMain_LandedOnProductPageEvent;
				ReportAvailableStoresEvent -= FormMain_ReportAvailableStoresEvent;
				LeftProductPageEvent -= FormMain_LeftProductPageEvent;

				ClearAllProductFields();
			}

			else
			{
				threadBrowser = new Thread(t_Crawl);
				threadBrowser.Start();
				btnCrawl.Text = "Stop Browser";

				LandedOnProductPageEvent += FormMain_LandedOnProductPageEvent;
				ReportAvailableStoresEvent += FormMain_ReportAvailableStoresEvent;
				LeftProductPageEvent += FormMain_LeftProductPageEvent;
			}
		}

		/// <summary>
		/// The user is closing the form. Check if we need to close <paramref name="threadBrowser"/> before exiting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (threadBrowser != null)
			{
				mainFormIsClosing = true;
				requestBrowserAbort = true;

				while (threadBrowser.ThreadState != ThreadState.Stopped)
				{
					Thread.Sleep(50);
				}
			}
		}

		/// <summary>
		/// If <paramref name="currentProduct"/> is not null or whitespace, attempt to insert that product to DB.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnTrackItem_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(currentProduct.Product_ID.ToString()))
			{
				DB_Manager.InsertProduct(currentProduct);
			}
		}

		private void TrackedProductsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			formTrackedProducts TrackedProducts = new formTrackedProducts();
			TrackedProducts.ShowDialog();
		}

		private void CrawlPricesToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		#endregion

		#region Custom Event Methods

		/// <summary>
		/// Whenever the browser lands on a product page, load the info onto the form and into currentProduct variable
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e">The object containing the product information</param>
		private void FormMain_LandedOnProductPageEvent(object sender, Shipt_Product e)
		{
			UpdateTextBox(txtbxStore, e.Store);
			UpdateTextBox(txtbxItemNum, e.Product_ID.ToString());
			UpdateTextBox(txtbxBrandName, e.Brand_Name);
			UpdateTextBox(txtbxProductName, e.Product_Name);
			UpdateTextBox(txtbxRegularPrice, e.Regular_Price.ToString());
			UpdateTextBox(txtbxSalePrice, e.Sale_Price.ToString());
			UpdateTextBox(txtbxUnitSize, e.PricePerUnitFormatted());

			// Clear lstbxPromotions and insert any promos if applicable
			{
				if (lstbxPromotions.InvokeRequired)
				{
					lstbxPromotions.Invoke(new MethodInvoker(() => lstbxPromotions.Items.Clear()));
				}

				else
				{
					lstbxPromotions.Items.Clear();
				}

				if (e.Promotions.Count > 0)
				{
					if (lstbxPromotions.InvokeRequired)
					{
						lstbxPromotions.Invoke(new MethodInvoker(() =>
						{
							foreach (var promo in e.Promotions)
							{
								lstbxPromotions.Items.Add(promo);
							}
						}));
					}

					else
					{
						foreach (var promo in e.Promotions)
						{
							lstbxPromotions.Items.Add(promo);
						}
					}
				}
			}

			if (btnTrackItem.InvokeRequired)
			{
				btnTrackItem.Invoke(new MethodInvoker(() => btnTrackItem.Enabled = true));
			}

			else
			{
				btnTrackItem.Enabled = true;
			}

			currentProduct = e;
		}

		/// <summary>
		/// Whenever we browse the addresses and the stores available at that address, insert the address and the stores in the DB
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e">The object containing the address and available stores</param>
		private void FormMain_ReportAvailableStoresEvent(object sender, Shipt_Available_Stores e)
		{
			if (e.StoreNames.Count > 0)
			{
				DB_Manager.InsertAddress(e.Delivery_Address);
				DB_Manager.InsertStores(e);
			}
		}

		/// <summary>
		/// Called when the user is no longer on a product page, we need to empty the TextBoxes and the currentProduct variable
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormMain_LeftProductPageEvent(object sender, EventArgs e)
		{
			ClearAllProductFields();

			if (btnTrackItem.InvokeRequired)
			{
				btnTrackItem.Invoke(new MethodInvoker(() => btnTrackItem.Enabled = false));
			}

			else
			{
				btnTrackItem.Enabled = false;
			}
		}

		#endregion

		#region Methods
		/// <summary>
		/// Checks if invoke is required, then updates the value of a text box accordingly.
		/// </summary>
		/// <param name="textBox">The TextBox class to be updated</param>
		/// <param name="value">The value to set the Text field of the TextBox</param>
		private void UpdateTextBox(TextBox textBox, string value)
		{
			if (textBox.InvokeRequired)
			{
				textBox.Invoke(new MethodInvoker(() => textBox.Text = value));
			}

			else
			{
				textBox.Text = value;
			}
		}

		/// <summary>
		/// Empties all product field TextBoxes on the main form, and clears the currentProduct variable
		/// </summary>
		private void ClearAllProductFields()
		{
			UpdateTextBox(txtbxStore, string.Empty);
			UpdateTextBox(txtbxItemNum, string.Empty);
			UpdateTextBox(txtbxBrandName, string.Empty);
			UpdateTextBox(txtbxProductName, string.Empty);
			UpdateTextBox(txtbxRegularPrice, string.Empty);
			UpdateTextBox(txtbxSalePrice, string.Empty);
			UpdateTextBox(txtbxUnitSize, string.Empty);

			// Clear lstbxPromotions
			{
				if (lstbxPromotions.InvokeRequired)
				{
					lstbxPromotions.Invoke(new MethodInvoker(() => lstbxPromotions.Items.Clear()));
				}

				else
				{
					lstbxPromotions.Items.Clear();
				}
			}

			currentProduct = new Shipt_Product();
		}

		/// <summary>
		/// The thread that controls the browser, and everything relevant to it
		/// </summary>
		private void t_Crawl()
		{
			try
			{
				browserRunning = true;

				ChromeOptions chromeOptions = new ChromeOptions();
				ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
				HtmlAgilityPack.HtmlDocument FullWebPage;

				//chromeOptions.AddArguments("headless");
				chromeOptions.PageLoadStrategy = PageLoadStrategy.Normal;
				driverService.HideCommandPromptWindow = true;

				// Open Browser
				using (var browser = new ChromeDriver(driverService, chromeOptions))
				{
					browser.Navigate().GoToUrl(URL_Login);

					// Wait for Url to change from URL_Login to URL_MainHomePage
					while (browser.Url != URL_MainHomePage)
					{
						if(!requestBrowserAbort)
						{
							Thread.Sleep(100);
						}

						else
						{
							return;
						}
					}

					// Let's check if we're logged in by looking for an address in the header of the page
					bool isLoggedIn = false;
					string hasAddressInHeader = browser.FindElements(By.XPath("//a[@class=\"pointer link darkness\"][@href=\"/account/addresses\"]//span")).First().Text;
					if(!string.IsNullOrEmpty(hasAddressInHeader))
					{
						isLoggedIn = true;
					}

					if (isLoggedIn)
					{
						WebDriverWait wait = new WebDriverWait(browser, TimeSpan.FromSeconds(5));
						wait.PollingInterval = TimeSpan.FromMilliseconds(50);

						// Get the addresses on the account, and the available stores at each address
						{
							wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[@data-test=\"ShoppingStoreSelect-storeView\"]"))).Click(); // Trigger event to bring up the Select Store screen

							wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[@id=\"SelectAddress-select\"][@data-test=\"Select-button\"]"))).Click(); // Trigger event to drop down list of addresses

							wait.Until(ExpectedConditions.ElementExists(By.XPath("//li[@data-test=\"Dropdown-option\"]")));
							int numOfAddresses = browser.FindElements(By.XPath("//li[@data-test=\"Dropdown-option\"]")).Count; // Get count of addresses

							for (int i = 0; i < numOfAddresses; i++)
							{
								browser.FindElement(By.XPath("//button[@id=\"SelectAddress-select\"][@data-test=\"Select-button\"]")).Click();

								if (i > 0)
								{
									wait.Until(ExpectedConditions.ElementExists(By.XPath("//li[@data-test=\"Dropdown-option\"]")));
									browser.FindElements(By.XPath("//li[@data-test=\"Dropdown-option\"]"))[i].Click();
								}

								Shipt_Available_Stores avail_stores = new Shipt_Available_Stores();
								avail_stores.Delivery_Address = browser.FindElement(By.XPath("//div[@id=\"SelectAddress-select-selected-option-label\"]")).Text;

								wait.Until(ExpectedConditions.ElementExists(By.XPath("//div[@class=\"cf\"]")));
								var Stores = browser.FindElements(By.XPath("//div[@class=\"cf\"]//div//div//p"));

								foreach (var store in Stores)
								{
									avail_stores.AddStore(store.Text);
								}

								// TODO: This event might be causing a huge delay in the program
								ReportAvailableStoresEvent?.Invoke(this, avail_stores);
							}

							browser.FindElement(By.XPath("//button[@id=\"SelectAddress-select\"][@data-test=\"Select-button\"]")).Click();
							browser.FindElements(By.XPath("//li[@data-test=\"Dropdown-option\"]"))[0].Click();
							browser.FindElement(By.XPath("//button[@data-test=\"Choose Store-modal-close\"]")).Click();
						}

						string currURL = browser.Url;
						while(!requestBrowserAbort)
						{
							if (currURL != browser.Url) // Check if the URL has changed
							{
								// If the user navigated to a product page
								if (browser.Url.Contains(@"shop.shipt.com/products/"))
								{
									Shipt_Product product = new Shipt_Product();

									/*
									// I thought this was a good idea in case I change the type of Product_ID, I knew I'd forget to update this
									// but it just creates more work than it's saving.
									Type type = product.Product_ID.GetType();
									type.GetMethod("Parse").Invoke(null, new object[] { browser.Url.Substring(browser.Url.LastIndexOf("/")) } );
									*/

									product.Address = browser.FindElements(By.XPath("//a[@class=\"pointer link darkness\"][@href=\"/account/addresses\"]//span")).First().Text;

									wait.Until(ExpectedConditions.ElementExists(By.XPath("//div[@data-test=\"ProductDetail-product-name\"]")));

									// Product_ID
									product.Product_ID = uint.Parse(browser.Url.Substring(browser.Url.LastIndexOf("/") + 1));

									// Store
									if(browser.FindElements(By.XPath("//span[@data-test=\"ShoppingStoreSelect-storeView-storeName\"]")).Count > 0)
									{
										product.Store = browser.FindElement(By.XPath("//span[@data-test=\"ShoppingStoreSelect-storeView-storeName\"]")).Text;
									}

									// Brand_Name if it exists
									if (browser.FindElements(By.XPath("//span[@data-test=\"ProductDetail-brand-name\"]")).Count > 0)
									{
										product.Brand_Name = browser.FindElement(By.XPath("//span[@data-test=\"ProductDetail-brand-name\"]")).Text;
									}

									// Product_Name
									if (browser.FindElements(By.XPath("//div[@data-test=\"ProductDetail-product-name\"]")).Count > 0)
									{
										product.Product_Name = browser.FindElement(By.XPath("//div[@data-test=\"ProductDetail-product-name\"]")).Text;
									}

									// Sale_Price and Regular_Price if product is on sale
									if(browser.FindElements(By.XPath("//div[@data-test=\"ProductDetail-product-sale-price\"]")).Count > 0)
									{
										/*
										 * TIL the two FindElements inline are redundant
										 * 
										string temp_Sale_Price = browser.FindElement(By.XPath("//div[@data-test=\"ProductDetail-product-sale-price\"]")).FindElement(By.XPath("//span[@class=\"mr1 title-2 tomato\"]"))
																		.Text
																		.Substring(1);

										string temp_Regular_Price = browser.FindElement(By.XPath("//div[@data-test=\"ProductDetail-product-sale-price\"]")).FindElement(By.XPath("//span[@class=\"mr1 body-2 gray strike\"]"))
																		.Text
																		.Substring(1);
										*/
										ReadOnlyCollection<IWebElement> Prices = browser.FindElements(By.XPath("//div[@data-test=\"ProductDetail-product-sale-price\"]//span"));

										if(Prices.Count >= 2)
										{
											string temp_Sale_Price = Prices[0].Text.Substring(1);
											string temp_Regular_Price = Prices[1].Text.Substring(1);

											try
											{
												product.Sale_Price = Convert.ToDecimal(temp_Sale_Price);
												product.Regular_Price = Convert.ToDecimal(temp_Regular_Price);
											}

											catch (Exception)
											{
												throw;
											}
										}

										else
										{
											//TODO: What happens if/when we don't have two elements for discount and regular price?
										}
									}

									// If product not on sale, it should have a regular price
									else if (browser.FindElements(By.XPath("//div[@data-test=\"ProductDetail-product-price\"]")).Count > 0)
									{
										string temp_Regular_Price = browser.FindElement(By.XPath("//div[@data-test=\"ProductDetail-product-price\"]")).Text.Substring(1);

										try
										{
											product.Regular_Price = Convert.ToDecimal(temp_Regular_Price);
										}

										catch (Exception)
										{
											throw;
										}
									}

									// This shouldn't be reached, but this should be handled
									else
									{
										//TODO: If product doesn't have a sale price or a regular price, what do we do?
									}

									// Subtext for Units and Unit_Type
									if (browser.FindElements(By.XPath("//div[@data-test=\"ProductDetail-subtext\"]")).Count > 0)
									{
										//TODO: Sometimes, subtext looks like "12 ct; 12 fl oz". Do we want to handle this a little differently?
										//TODO: "1/2 gal" won't parse to decimal.
										string temp_Subtext = browser.FindElement(By.XPath("//div[@data-test=\"ProductDetail-subtext\"]")).Text;

										if (char.IsDigit(temp_Subtext[0]))// TODO: Probably not a good idea to hard-code this
										{
											string temp_Units = temp_Subtext.Substring(0, (temp_Subtext.IndexOf(' '))).Replace("$", "");
											string temp_UnitType = temp_Subtext.Substring(temp_Subtext.IndexOf(' ') + 1);

											try
											{
												product.Units = Convert.ToDecimal(temp_Units);
											}

											catch (Exception)
											{
												throw;
											}

											product.Unit_Type = temp_UnitType;
										}

										else
										{
											product.Units = 1;
											product.Unit_Type = temp_Subtext;
										}
									}

									// If product is BOGO
									if (browser.FindElements(By.XPath("//div[@data-test=\"ProductDetail-bogo-text\"]")).Count > 0)
									{
										product.AddPromotion("Buy 1, Get 1 Free");
									}

									// If item has a promotion
									if (browser.FindElements(By.XPath("//button[@data-test=\"ProductDetail-feature-promotion\"]")).Count > 0)
									{
										string temp_Promotion = browser.FindElement(By.XPath("//button[@data-test=\"ProductDetail-feature-promotion\"]//div//div")).Text;
										product.AddPromotion(temp_Promotion);
									}

									LandedOnProductPageEvent?.Invoke(this, product);
								}

								// Check if the user left a product page for anything other than a product page
								else if (currURL.Contains(URL_Prefix_Product)
											&& !browser.Url.Contains(URL_Prefix_Product))
								{
									LeftProductPageEvent?.Invoke(this, new EventArgs());
								}

								currURL = browser.Url;
							}
						}
					}
				}

				requestBrowserAbort = false;
				browserRunning = false;

				if (!mainFormIsClosing)
				{
					//Set btnCrawl text to "Start"
					if (btnCrawl.InvokeRequired)
					{
						btnCrawl.Invoke(new MethodInvoker(() => btnCrawl.Text = "Start Browser"));
					}

					else
					{
						btnCrawl.Text = "Start Browser";
					}
				}
			}

			catch (ThreadAbortException)
			{

				requestBrowserAbort = false;
				browserRunning = false;

				if (!mainFormIsClosing)
				{
					//Set btnCrawl text to "Start"
					if (btnCrawl.InvokeRequired)
					{
						// This seems to lock the application and keep it alive, even though btnCrawl.IsDisposed and btnCrawl.Disposing are false 
						btnCrawl.Invoke(new MethodInvoker(() => btnCrawl.Text = "Start Browser"));
					}

					else
					{
						btnCrawl.Text = "Start Browser";
					}
				}
			}
		}

		#endregion

	}
}
