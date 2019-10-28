using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shipt_Crawler
{
	public partial class formMain : Form
	{
		Thread threadCrawl;
		private bool crawlRunning = false; // Only to be written by the crawling thread, and read from the main (or any other) thread.
		private bool crawlRequestAbort = false; // Set true by main (or any other) thread, read by the crawling thread, and set false at end of thread.

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
			}

			else
			{
				threadCrawl = new Thread(t_Crawl);
				threadCrawl.Start();
				btnCrawl.Text = "Stop";
			}
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

				//chromeOptions.AddArguments("headless");
				driverService.HideCommandPromptWindow = true;

				using (var browser = new ChromeDriver(driverService, chromeOptions))
				{
					browser.Navigate().GoToUrl(URL_Login);

					while (browser.Url != URL_MainPageLoggedIn)
					{
						Thread.Sleep(100);
					}

					FullWebPage = new HtmlAgilityPack.HtmlDocument();
					FullWebPage.LoadHtml(browser.PageSource);

					bool isLoggedIn = false;
					try
					{
						// This is horrible. Something tells me this can go bad really quickly. Please, Shipt, don't change https://shop.shipt.com/ one bit.
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
						var cookies = browser.Manage().Cookies.AllCookies.Where(cookie => cookie.Name.Contains("shiptAuthData.production")).First().Value;
						cookies = cookies.Replace("%22", "").Replace(@"{access_token:", "").Substring(0, cookies.IndexOf(@"%2C"));

						HttpWebRequest test = (HttpWebRequest)WebRequest.Create(@"https://api.shipt.com/search/v3/search/?bucket_number=99&white_label_key=shipt");
						test.Method = "POST";
						test.Host = "api.shipt.com";
						test.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0";
						test.Accept = "*/*";
						test.Referer = "https://shop.shipt.com/";
						test.ContentType = "application/json";
						test.Headers.Add(HttpRequestHeader.Authorization, cookies);
						test.Headers.Add("Origin", "https://shop.shipt.com");
						test.Connection = "keep-alive";
						//TODO: check hgow to apply a body to test, and see if we can fetch json

					}

					browser.Navigate().GoToUrl(URL_MainBrowse);
				}

				crawlRequestAbort = false;
				crawlRunning = false;

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

			catch (ThreadAbortException ex)
			{

				crawlRequestAbort = false;
				crawlRunning = false;

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

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{

		}
	}
}
