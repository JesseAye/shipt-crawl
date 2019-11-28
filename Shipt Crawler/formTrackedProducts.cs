using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shipt_Crawler
{
	public partial class formTrackedProducts : Form
	{
		public formTrackedProducts()
		{
			InitializeComponent();
		}

		private void FormTrackedProducts_Shown(object sender, EventArgs e)
		{
			string testString = "";
			DB_Manager.ReadProducts(testString);
		}
	}
}
