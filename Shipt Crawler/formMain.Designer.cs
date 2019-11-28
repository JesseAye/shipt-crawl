namespace Shipt_Crawler
{
	partial class formMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnCrawl = new System.Windows.Forms.Button();
			this.lblStore = new System.Windows.Forms.Label();
			this.lblBrandName = new System.Windows.Forms.Label();
			this.lblProductName = new System.Windows.Forms.Label();
			this.lblRegularPrice = new System.Windows.Forms.Label();
			this.lblSalePrice = new System.Windows.Forms.Label();
			this.lblUnitSize = new System.Windows.Forms.Label();
			this.txtbxRegularPrice = new System.Windows.Forms.TextBox();
			this.txtbxProductName = new System.Windows.Forms.TextBox();
			this.txtbxSalePrice = new System.Windows.Forms.TextBox();
			this.txtbxBrandName = new System.Windows.Forms.TextBox();
			this.txtbxStore = new System.Windows.Forms.TextBox();
			this.lstbxPromotions = new System.Windows.Forms.ListBox();
			this.lblPromotions = new System.Windows.Forms.Label();
			this.txtbxUnitSize = new System.Windows.Forms.TextBox();
			this.btnTrackItem = new System.Windows.Forms.Button();
			this.txtbxItemNum = new System.Windows.Forms.TextBox();
			this.lblItemNum = new System.Windows.Forms.Label();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.trackedProductsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.crawlPricesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCrawl
			// 
			this.btnCrawl.Location = new System.Drawing.Point(12, 245);
			this.btnCrawl.Name = "btnCrawl";
			this.btnCrawl.Size = new System.Drawing.Size(84, 23);
			this.btnCrawl.TabIndex = 0;
			this.btnCrawl.Text = "Start Browser";
			this.btnCrawl.UseVisualStyleBackColor = true;
			this.btnCrawl.Click += new System.EventHandler(this.BtnCrawl_Click);
			// 
			// lblStore
			// 
			this.lblStore.AutoSize = true;
			this.lblStore.Location = new System.Drawing.Point(12, 30);
			this.lblStore.Name = "lblStore";
			this.lblStore.Size = new System.Drawing.Size(32, 13);
			this.lblStore.TabIndex = 1;
			this.lblStore.Text = "Store";
			// 
			// lblBrandName
			// 
			this.lblBrandName.AutoSize = true;
			this.lblBrandName.Location = new System.Drawing.Point(12, 56);
			this.lblBrandName.Name = "lblBrandName";
			this.lblBrandName.Size = new System.Drawing.Size(66, 13);
			this.lblBrandName.TabIndex = 2;
			this.lblBrandName.Text = "Brand Name";
			// 
			// lblProductName
			// 
			this.lblProductName.AutoSize = true;
			this.lblProductName.Location = new System.Drawing.Point(12, 82);
			this.lblProductName.Name = "lblProductName";
			this.lblProductName.Size = new System.Drawing.Size(75, 13);
			this.lblProductName.TabIndex = 3;
			this.lblProductName.Text = "Product Name";
			// 
			// lblRegularPrice
			// 
			this.lblRegularPrice.AutoSize = true;
			this.lblRegularPrice.Location = new System.Drawing.Point(12, 108);
			this.lblRegularPrice.Name = "lblRegularPrice";
			this.lblRegularPrice.Size = new System.Drawing.Size(71, 13);
			this.lblRegularPrice.TabIndex = 4;
			this.lblRegularPrice.Text = "Regular Price";
			// 
			// lblSalePrice
			// 
			this.lblSalePrice.AutoSize = true;
			this.lblSalePrice.Location = new System.Drawing.Point(12, 134);
			this.lblSalePrice.Name = "lblSalePrice";
			this.lblSalePrice.Size = new System.Drawing.Size(55, 13);
			this.lblSalePrice.TabIndex = 5;
			this.lblSalePrice.Text = "Sale Price";
			// 
			// lblUnitSize
			// 
			this.lblUnitSize.AutoSize = true;
			this.lblUnitSize.Location = new System.Drawing.Point(12, 160);
			this.lblUnitSize.Name = "lblUnitSize";
			this.lblUnitSize.Size = new System.Drawing.Size(49, 13);
			this.lblUnitSize.TabIndex = 6;
			this.lblUnitSize.Text = "Unit Size";
			// 
			// txtbxRegularPrice
			// 
			this.txtbxRegularPrice.Enabled = false;
			this.txtbxRegularPrice.Location = new System.Drawing.Point(93, 105);
			this.txtbxRegularPrice.Name = "txtbxRegularPrice";
			this.txtbxRegularPrice.Size = new System.Drawing.Size(272, 20);
			this.txtbxRegularPrice.TabIndex = 8;
			// 
			// txtbxProductName
			// 
			this.txtbxProductName.Enabled = false;
			this.txtbxProductName.Location = new System.Drawing.Point(93, 79);
			this.txtbxProductName.Name = "txtbxProductName";
			this.txtbxProductName.Size = new System.Drawing.Size(272, 20);
			this.txtbxProductName.TabIndex = 9;
			// 
			// txtbxSalePrice
			// 
			this.txtbxSalePrice.Enabled = false;
			this.txtbxSalePrice.Location = new System.Drawing.Point(93, 131);
			this.txtbxSalePrice.Name = "txtbxSalePrice";
			this.txtbxSalePrice.Size = new System.Drawing.Size(272, 20);
			this.txtbxSalePrice.TabIndex = 10;
			// 
			// txtbxBrandName
			// 
			this.txtbxBrandName.Enabled = false;
			this.txtbxBrandName.Location = new System.Drawing.Point(93, 53);
			this.txtbxBrandName.Name = "txtbxBrandName";
			this.txtbxBrandName.Size = new System.Drawing.Size(272, 20);
			this.txtbxBrandName.TabIndex = 11;
			// 
			// txtbxStore
			// 
			this.txtbxStore.Enabled = false;
			this.txtbxStore.Location = new System.Drawing.Point(93, 27);
			this.txtbxStore.Name = "txtbxStore";
			this.txtbxStore.Size = new System.Drawing.Size(133, 20);
			this.txtbxStore.TabIndex = 12;
			// 
			// lstbxPromotions
			// 
			this.lstbxPromotions.Enabled = false;
			this.lstbxPromotions.FormattingEnabled = true;
			this.lstbxPromotions.Location = new System.Drawing.Point(93, 183);
			this.lstbxPromotions.Name = "lstbxPromotions";
			this.lstbxPromotions.Size = new System.Drawing.Size(272, 56);
			this.lstbxPromotions.TabIndex = 13;
			// 
			// lblPromotions
			// 
			this.lblPromotions.AutoSize = true;
			this.lblPromotions.Location = new System.Drawing.Point(12, 186);
			this.lblPromotions.Name = "lblPromotions";
			this.lblPromotions.Size = new System.Drawing.Size(59, 13);
			this.lblPromotions.TabIndex = 14;
			this.lblPromotions.Text = "Promotions";
			// 
			// txtbxUnitSize
			// 
			this.txtbxUnitSize.Enabled = false;
			this.txtbxUnitSize.Location = new System.Drawing.Point(93, 157);
			this.txtbxUnitSize.Name = "txtbxUnitSize";
			this.txtbxUnitSize.Size = new System.Drawing.Size(272, 20);
			this.txtbxUnitSize.TabIndex = 15;
			// 
			// btnTrackItem
			// 
			this.btnTrackItem.Enabled = false;
			this.btnTrackItem.Location = new System.Drawing.Point(274, 245);
			this.btnTrackItem.Name = "btnTrackItem";
			this.btnTrackItem.Size = new System.Drawing.Size(91, 23);
			this.btnTrackItem.TabIndex = 17;
			this.btnTrackItem.Text = "Track Item";
			this.btnTrackItem.UseVisualStyleBackColor = true;
			this.btnTrackItem.Click += new System.EventHandler(this.BtnTrackItem_Click);
			// 
			// txtbxItemNum
			// 
			this.txtbxItemNum.Enabled = false;
			this.txtbxItemNum.Location = new System.Drawing.Point(275, 27);
			this.txtbxItemNum.Name = "txtbxItemNum";
			this.txtbxItemNum.Size = new System.Drawing.Size(90, 20);
			this.txtbxItemNum.TabIndex = 18;
			// 
			// lblItemNum
			// 
			this.lblItemNum.AutoSize = true;
			this.lblItemNum.Location = new System.Drawing.Point(232, 30);
			this.lblItemNum.Name = "lblItemNum";
			this.lblItemNum.Size = new System.Drawing.Size(37, 13);
			this.lblItemNum.TabIndex = 19;
			this.lblItemNum.Text = "Item #";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trackedProductsToolStripMenuItem,
            this.crawlPricesToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(377, 24);
			this.menuStrip1.TabIndex = 20;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// trackedProductsToolStripMenuItem
			// 
			this.trackedProductsToolStripMenuItem.Name = "trackedProductsToolStripMenuItem";
			this.trackedProductsToolStripMenuItem.Size = new System.Drawing.Size(110, 20);
			this.trackedProductsToolStripMenuItem.Text = "Tracked Products";
			this.trackedProductsToolStripMenuItem.Click += new System.EventHandler(this.TrackedProductsToolStripMenuItem_Click);
			// 
			// crawlPricesToolStripMenuItem
			// 
			this.crawlPricesToolStripMenuItem.Name = "crawlPricesToolStripMenuItem";
			this.crawlPricesToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
			this.crawlPricesToolStripMenuItem.Text = "Crawl Prices";
			this.crawlPricesToolStripMenuItem.Click += new System.EventHandler(this.CrawlPricesToolStripMenuItem_Click);
			// 
			// formMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(377, 280);
			this.Controls.Add(this.lblItemNum);
			this.Controls.Add(this.txtbxItemNum);
			this.Controls.Add(this.btnTrackItem);
			this.Controls.Add(this.txtbxUnitSize);
			this.Controls.Add(this.lblPromotions);
			this.Controls.Add(this.lstbxPromotions);
			this.Controls.Add(this.txtbxStore);
			this.Controls.Add(this.txtbxBrandName);
			this.Controls.Add(this.txtbxSalePrice);
			this.Controls.Add(this.txtbxProductName);
			this.Controls.Add(this.txtbxRegularPrice);
			this.Controls.Add(this.lblUnitSize);
			this.Controls.Add(this.lblSalePrice);
			this.Controls.Add(this.lblRegularPrice);
			this.Controls.Add(this.lblProductName);
			this.Controls.Add(this.lblBrandName);
			this.Controls.Add(this.lblStore);
			this.Controls.Add(this.btnCrawl);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "formMain";
			this.Text = "Shipt Crawl";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCrawl;
		private System.Windows.Forms.Label lblStore;
		private System.Windows.Forms.Label lblBrandName;
		private System.Windows.Forms.Label lblProductName;
		private System.Windows.Forms.Label lblRegularPrice;
		private System.Windows.Forms.Label lblSalePrice;
		private System.Windows.Forms.Label lblUnitSize;
		private System.Windows.Forms.TextBox txtbxRegularPrice;
		private System.Windows.Forms.TextBox txtbxProductName;
		private System.Windows.Forms.TextBox txtbxSalePrice;
		private System.Windows.Forms.TextBox txtbxBrandName;
		private System.Windows.Forms.TextBox txtbxStore;
		private System.Windows.Forms.ListBox lstbxPromotions;
		private System.Windows.Forms.Label lblPromotions;
		private System.Windows.Forms.TextBox txtbxUnitSize;
		private System.Windows.Forms.Button btnTrackItem;
		private System.Windows.Forms.TextBox txtbxItemNum;
		private System.Windows.Forms.Label lblItemNum;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem trackedProductsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem crawlPricesToolStripMenuItem;
	}
}

