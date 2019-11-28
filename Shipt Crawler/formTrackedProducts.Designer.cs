namespace Shipt_Crawler
{
	partial class formTrackedProducts
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
			this.dgvProducts = new System.Windows.Forms.DataGridView();
			this.lblAddress = new System.Windows.Forms.Label();
			this.lblStore = new System.Windows.Forms.Label();
			this.cbAddress = new System.Windows.Forms.ComboBox();
			this.cbStore = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvProducts
			// 
			this.dgvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvProducts.Location = new System.Drawing.Point(0, 49);
			this.dgvProducts.Name = "dgvProducts";
			this.dgvProducts.Size = new System.Drawing.Size(800, 401);
			this.dgvProducts.TabIndex = 0;
			// 
			// lblAddress
			// 
			this.lblAddress.AutoSize = true;
			this.lblAddress.Location = new System.Drawing.Point(12, 15);
			this.lblAddress.Name = "lblAddress";
			this.lblAddress.Size = new System.Drawing.Size(45, 13);
			this.lblAddress.TabIndex = 1;
			this.lblAddress.Text = "Address";
			// 
			// lblStore
			// 
			this.lblStore.AutoSize = true;
			this.lblStore.Location = new System.Drawing.Point(307, 15);
			this.lblStore.Name = "lblStore";
			this.lblStore.Size = new System.Drawing.Size(32, 13);
			this.lblStore.TabIndex = 2;
			this.lblStore.Text = "Store";
			// 
			// cbAddress
			// 
			this.cbAddress.FormattingEnabled = true;
			this.cbAddress.Location = new System.Drawing.Point(63, 12);
			this.cbAddress.Name = "cbAddress";
			this.cbAddress.Size = new System.Drawing.Size(238, 21);
			this.cbAddress.TabIndex = 3;
			// 
			// cbStore
			// 
			this.cbStore.Enabled = false;
			this.cbStore.FormattingEnabled = true;
			this.cbStore.Location = new System.Drawing.Point(345, 12);
			this.cbStore.Name = "cbStore";
			this.cbStore.Size = new System.Drawing.Size(194, 21);
			this.cbStore.TabIndex = 4;
			// 
			// formTrackedProducts
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.cbStore);
			this.Controls.Add(this.cbAddress);
			this.Controls.Add(this.lblStore);
			this.Controls.Add(this.lblAddress);
			this.Controls.Add(this.dgvProducts);
			this.Name = "formTrackedProducts";
			this.Text = "formTrackedProducts";
			this.Shown += new System.EventHandler(this.FormTrackedProducts_Shown);
			((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvProducts;
		private System.Windows.Forms.Label lblAddress;
		private System.Windows.Forms.Label lblStore;
		private System.Windows.Forms.ComboBox cbAddress;
		private System.Windows.Forms.ComboBox cbStore;
	}
}