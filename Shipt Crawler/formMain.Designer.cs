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
			this.SuspendLayout();
			// 
			// btnCrawl
			// 
			this.btnCrawl.Location = new System.Drawing.Point(13, 415);
			this.btnCrawl.Name = "btnCrawl";
			this.btnCrawl.Size = new System.Drawing.Size(75, 23);
			this.btnCrawl.TabIndex = 0;
			this.btnCrawl.Text = "Start";
			this.btnCrawl.UseVisualStyleBackColor = true;
			this.btnCrawl.Click += new System.EventHandler(this.BtnCrawl_Click);
			// 
			// formMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.btnCrawl);
			this.Name = "formMain";
			this.Text = "Shipt Crawl";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnCrawl;
	}
}

