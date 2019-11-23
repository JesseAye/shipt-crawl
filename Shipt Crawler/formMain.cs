using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Shipt_Crawler
{
	public partial class formMain : Form
	{
		Thread threadCrawl;
		private bool crawlRunning = false; // Only to be written by the crawling thread, and read from the main (or any other) thread.
		private bool crawlRequestAbort = false; // Set true by main (or any other) thread, read by the crawling thread, and set false at end of thread.
		private bool mainFormIsClosing = false;

		public event EventHandler<Shipt_Product> LandedOnProductPageEvent;

		/// <summary>
		/// Constructor for formMain, first form of program.
		/// </summary>
		public formMain()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Ran whenever btnCrawl is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnCrawl_Click(object sender, EventArgs e)
		{
			if (crawlRunning)
			{
				btnCrawl.Text = "Start";
				crawlRequestAbort = true;

				LandedOnProductPageEvent -= FormMain_LandedOnProductPageEvent;
			}

			else
			{
				threadCrawl = new Thread(t_Crawl);
				threadCrawl.Start();
				btnCrawl.Text = "Stop";

				LandedOnProductPageEvent += FormMain_LandedOnProductPageEvent;
			}
		}

		private void FormMain_LandedOnProductPageEvent(object sender, Shipt_Product e)
		{
			Console.WriteLine(e);
		}

		private void t_Crawl()
		{
			try
			{
				crawlRunning = true;

				ChromeOptions chromeOptions = new ChromeOptions();
				ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
				HtmlAgilityPack.HtmlDocument FullWebPage;
				String URL_Login = "https://shop.shipt.com/login";
				String URL_MainPageLoggedIn = "https://shop.shipt.com/";
				String URL_MainBrowse = "https://shop.shipt.com/search";
				String URL_Prefix_Product = "https://shop.shipt.com/products/";
				String URL_Prefix_Featured_Promotion = "https://shop.shipt.com/featured-promotions/";

				//chromeOptions.AddArguments("headless");
				chromeOptions.PageLoadStrategy = PageLoadStrategy.Normal;
				driverService.HideCommandPromptWindow = true;

				using (var browser = new ChromeDriver(driverService, chromeOptions))
				{
					browser.Navigate().GoToUrl(URL_Login);

					while (browser.Url != URL_MainPageLoggedIn)
					{
						if(!crawlRequestAbort)
						{
							Thread.Sleep(100);
						}

						else
						{
							return;
						}
					}

					FullWebPage = new HtmlAgilityPack.HtmlDocument();
					FullWebPage.LoadHtml(browser.PageSource);

					bool isLoggedIn = false;
					try
					{
						// This is horrible. Something tells me this can go bad really quickly. Please, Shipt, don't change https://shop.shipt.com/ one bit.
						// It's broken up into single line assignments so when this eventually throws an exception, I can figure out which line is causing it.
						var node = FullWebPage.DocumentNode.SelectSingleNode("html");
						node = node.SelectSingleNode("body");
						node = node.SelectNodes("div").Where(div => div.Attributes["id"].Value == "root").First();
						node = node.SelectSingleNode("div");
						node = node.SelectNodes("div").Where(div => div.Attributes["class"].Value.Contains("fixed")).First();
						node = node.SelectNodes("div").Where(div => div.Attributes["class"].Value.Contains("bg-white-gray")).First();
						node = node.SelectSingleNode("header");
						node = node.SelectSingleNode("div");
						node = node.SelectNodes("a").Where(a => a.Attributes["href"].Value.Contains("addresses")).First();
						node = node.SelectSingleNode("span");

						if ((node.InnerText != "")
							&& (node.InnerText != null))
						{
							isLoggedIn = true;
						}
					}

					catch (NoSuchElementException ex)
					{
						MessageBox.Show(ex.Message, "Error verifying logged in status", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}

					if (isLoggedIn)
					{
						string currURL = browser.Url;
						while(!crawlRequestAbort)
						{
							if (currURL != browser.Url)
							{
								// Using hard-coded Thread.Sleep in this case isn't great practice. Check out https://stackoverflow.com/questions/43203243 on having this thread wait until a certain element(s) are visible.

								if (browser.Url.Contains(@"shop.shipt.com/products/"))
								{
									Thread.Sleep(750);
									Shipt_Product product = new Shipt_Product();

									/*
									// I thought this was a good idea in case I change the type of Product_ID, I knew I'd forget to update this
									// but it just creates more work than it's saving.
									Type type = product.Product_ID.GetType();
									type.GetMethod("Parse").Invoke(null, new object[] { browser.Url.Substring(browser.Url.LastIndexOf("/")) } );
									*/

									// Product_ID
									product.Product_ID = uint.Parse(browser.Url.Substring(browser.Url.LastIndexOf("/") + 1));

									// Brand_Name if it exists
									if (browser.FindElement(By.XPath("//span[@data-test=\"ProductDetail-brand-name\"]")) != null)
									{
										product.Brand_Name = browser.FindElement(By.XPath("//span[@data-test=\"ProductDetail-brand-name\"]")).Text;
									}

									// Product_Name
									if (browser.FindElement(By.XPath("//div[@data-test=\"ProductDetail-product-name\"]")) != null)
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
										string temp_Subtext = browser.FindElement(By.XPath("//div[@data-test=\"ProductDetail-subtext\"]")).Text;
										string temp_Units = temp_Subtext.Substring(0, (temp_Subtext.IndexOf(' ')));
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

									// If product is BOGO
									if (browser.FindElements(By.XPath("//div[@data-test=\"ProductDetail-bogo-text\"]")).Count > 0)
									{
										product.AddPromotion("Buy 1, Get 1 Free");
									}

									// If item is within a promotion
									if (browser.FindElements(By.XPath("//button[@data-test=\"ProductDetail-feature-promotion\"]")).Count > 0)
									{
										string temp_Promotion = browser.FindElement(By.XPath("//button[@data-test=\"ProductDetail-feature-promotion\"]//div//div")).Text;
										product.AddPromotion(temp_Promotion);
									}

									LandedOnProductPageEvent?.Invoke(this, product);
								}

								currURL = browser.Url;
								Thread.Sleep(250);
							}
						}

						{
						/*
						var cookies = browser.Manage().Cookies.AllCookies.Where(cookie => cookie.Name.Contains("shiptAuthData.production")).First().Value;
						cookies = cookies.Replace("%22", "").Replace(@"{access_token:", "").Substring(0, cookies.IndexOf(@"%2C"));

						HttpWebRequest test = (HttpWebRequest)WebRequest.Create(@"https://api.shipt.com/search/v3/search/?bucket_number=99&white_label_key=shipt");
						test.Method = "POST";
						test.Host = "api.shipt.com";
						test.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0";
						test.Accept = "*//*";
						test.Referer = "https://shop.shipt.com/";
						test.ContentType = "application/json";
						test.Headers.Add(HttpRequestHeader.Authorization, cookies);
						test.Headers.Add("Origin", "https://shop.shipt.com");
						test.Connection = "keep-alive";
						test.
						//TODO: check how to apply a body to test, and see if we can fetch json

						test.MaximumAutomaticRedirections = 10;
						test.MaximumResponseHeadersLength = 10;

						StringBuilder responseString = new StringBuilder();

						try
						{
							WebResponse receiveStream = test.GetResponse();

							if (receiveStream != null)
							{
								StreamReader readStream = new StreamReader(receiveStream.GetResponseStream(), Encoding.UTF8);

								string line = "";

								while((line = readStream.ReadLine()) != null)
								{
									responseString.Append(line);
								}

								readStream.Close();
							}
						}

						catch (Exception)
						{

							throw;
						}
						*/
						}
					}

					//browser.Navigate().GoToUrl(URL_MainBrowse);
				}

				crawlRequestAbort = false;
				crawlRunning = false;

				if (!mainFormIsClosing)
				{
					//Set btnCrawl text to "Start"
					if (btnCrawl.InvokeRequired)
					{
						btnCrawl.Invoke(new MethodInvoker(() => btnCrawl.Text = "Start"));
					}

					else
					{
						btnCrawl.Text = "Start";
					}
				}
			}

			catch (ThreadAbortException)
			{

				crawlRequestAbort = false;
				crawlRunning = false;

				if (!mainFormIsClosing)
				{
					//Set btnCrawl text to "Start"
					if (btnCrawl.InvokeRequired)
					{
						// This seems to lock the application and keep it alive, even though btnCrawl.IsDisposed and btnCrawl.Disposing are false 
						btnCrawl.Invoke(new MethodInvoker(() => btnCrawl.Text = "Start"));
					}

					else
					{
						btnCrawl.Text = "Start";
					}
				}
			}
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			// TODO: Check to make sure this works as intended
			mainFormIsClosing = true;
			crawlRequestAbort = true;
			//threadCrawl.Abort(); This was bad news bears

			while(threadCrawl.ThreadState != ThreadState.Stopped)
			{
				Thread.Sleep(50);
			}
		}
	}
}
