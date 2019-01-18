namespace WinFormsTicks
{
    partial class Form1
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
            this.LastPrice = new System.Windows.Forms.Label();
            this.BidPrice = new System.Windows.Forms.Label();
            this.AskPrice = new System.Windows.Forms.Label();
            this.AskLabel = new System.Windows.Forms.Label();
            this.BidLabel = new System.Windows.Forms.Label();
            this.LastLabel = new System.Windows.Forms.Label();
            this.AskSize = new System.Windows.Forms.Label();
            this.BidSize = new System.Windows.Forms.Label();
            this.LastSize = new System.Windows.Forms.Label();
            this.Volume = new System.Windows.Forms.Label();
            this.ChangeLabel = new System.Windows.Forms.Label();
            this.Change = new System.Windows.Forms.Label();
            this.Symbol = new System.Windows.Forms.TextBox();
            this.SymbolLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LastPrice
            // 
            this.LastPrice.AutoSize = true;
            this.LastPrice.BackColor = System.Drawing.Color.White;
            this.LastPrice.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LastPrice.ForeColor = System.Drawing.Color.Black;
            this.LastPrice.Location = new System.Drawing.Point(166, 71);
            this.LastPrice.MinimumSize = new System.Drawing.Size(80, 0);
            this.LastPrice.Name = "LastPrice";
            this.LastPrice.Size = new System.Drawing.Size(80, 17);
            this.LastPrice.TabIndex = 1;
            this.LastPrice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BidPrice
            // 
            this.BidPrice.AutoSize = true;
            this.BidPrice.BackColor = System.Drawing.Color.White;
            this.BidPrice.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BidPrice.ForeColor = System.Drawing.Color.Black;
            this.BidPrice.Location = new System.Drawing.Point(2, 71);
            this.BidPrice.MinimumSize = new System.Drawing.Size(80, 0);
            this.BidPrice.Name = "BidPrice";
            this.BidPrice.Size = new System.Drawing.Size(80, 17);
            this.BidPrice.TabIndex = 2;
            this.BidPrice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AskPrice
            // 
            this.AskPrice.AutoSize = true;
            this.AskPrice.BackColor = System.Drawing.Color.White;
            this.AskPrice.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AskPrice.ForeColor = System.Drawing.Color.Black;
            this.AskPrice.Location = new System.Drawing.Point(84, 71);
            this.AskPrice.MinimumSize = new System.Drawing.Size(80, 0);
            this.AskPrice.Name = "AskPrice";
            this.AskPrice.Size = new System.Drawing.Size(80, 17);
            this.AskPrice.TabIndex = 3;
            this.AskPrice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AskLabel
            // 
            this.AskLabel.AutoSize = true;
            this.AskLabel.BackColor = System.Drawing.Color.White;
            this.AskLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AskLabel.ForeColor = System.Drawing.Color.Black;
            this.AskLabel.Location = new System.Drawing.Point(84, 55);
            this.AskLabel.MinimumSize = new System.Drawing.Size(80, 0);
            this.AskLabel.Name = "AskLabel";
            this.AskLabel.Size = new System.Drawing.Size(80, 17);
            this.AskLabel.TabIndex = 6;
            this.AskLabel.Text = "Ask";
            this.AskLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BidLabel
            // 
            this.BidLabel.AutoSize = true;
            this.BidLabel.BackColor = System.Drawing.Color.White;
            this.BidLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BidLabel.ForeColor = System.Drawing.Color.Black;
            this.BidLabel.Location = new System.Drawing.Point(2, 55);
            this.BidLabel.MinimumSize = new System.Drawing.Size(80, 0);
            this.BidLabel.Name = "BidLabel";
            this.BidLabel.Size = new System.Drawing.Size(80, 17);
            this.BidLabel.TabIndex = 5;
            this.BidLabel.Text = "Bid";
            this.BidLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LastLabel
            // 
            this.LastLabel.AutoSize = true;
            this.LastLabel.BackColor = System.Drawing.Color.White;
            this.LastLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LastLabel.ForeColor = System.Drawing.Color.Black;
            this.LastLabel.Location = new System.Drawing.Point(166, 55);
            this.LastLabel.MinimumSize = new System.Drawing.Size(80, 0);
            this.LastLabel.Name = "LastLabel";
            this.LastLabel.Size = new System.Drawing.Size(80, 17);
            this.LastLabel.TabIndex = 4;
            this.LastLabel.Text = "Last";
            this.LastLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AskSize
            // 
            this.AskSize.AutoSize = true;
            this.AskSize.BackColor = System.Drawing.Color.White;
            this.AskSize.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AskSize.ForeColor = System.Drawing.Color.Black;
            this.AskSize.Location = new System.Drawing.Point(84, 87);
            this.AskSize.MinimumSize = new System.Drawing.Size(80, 0);
            this.AskSize.Name = "AskSize";
            this.AskSize.Size = new System.Drawing.Size(80, 17);
            this.AskSize.TabIndex = 9;
            this.AskSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BidSize
            // 
            this.BidSize.AutoSize = true;
            this.BidSize.BackColor = System.Drawing.Color.White;
            this.BidSize.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BidSize.ForeColor = System.Drawing.Color.Black;
            this.BidSize.Location = new System.Drawing.Point(2, 87);
            this.BidSize.MinimumSize = new System.Drawing.Size(80, 0);
            this.BidSize.Name = "BidSize";
            this.BidSize.Size = new System.Drawing.Size(80, 17);
            this.BidSize.TabIndex = 8;
            this.BidSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LastSize
            // 
            this.LastSize.AutoSize = true;
            this.LastSize.BackColor = System.Drawing.Color.White;
            this.LastSize.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LastSize.ForeColor = System.Drawing.Color.Black;
            this.LastSize.Location = new System.Drawing.Point(166, 87);
            this.LastSize.MinimumSize = new System.Drawing.Size(80, 0);
            this.LastSize.Name = "LastSize";
            this.LastSize.Size = new System.Drawing.Size(80, 17);
            this.LastSize.TabIndex = 7;
            this.LastSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Volume
            // 
            this.Volume.AutoSize = true;
            this.Volume.BackColor = System.Drawing.Color.White;
            this.Volume.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Volume.ForeColor = System.Drawing.Color.Black;
            this.Volume.Location = new System.Drawing.Point(248, 87);
            this.Volume.MinimumSize = new System.Drawing.Size(110, 0);
            this.Volume.Name = "Volume";
            this.Volume.Size = new System.Drawing.Size(110, 17);
            this.Volume.TabIndex = 12;
            this.Volume.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ChangeLabel
            // 
            this.ChangeLabel.AutoSize = true;
            this.ChangeLabel.BackColor = System.Drawing.Color.White;
            this.ChangeLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChangeLabel.ForeColor = System.Drawing.Color.Black;
            this.ChangeLabel.Location = new System.Drawing.Point(248, 55);
            this.ChangeLabel.MinimumSize = new System.Drawing.Size(110, 0);
            this.ChangeLabel.Name = "ChangeLabel";
            this.ChangeLabel.Size = new System.Drawing.Size(110, 17);
            this.ChangeLabel.TabIndex = 11;
            this.ChangeLabel.Text = "Change/Volume";
            this.ChangeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Change
            // 
            this.Change.AutoSize = true;
            this.Change.BackColor = System.Drawing.Color.White;
            this.Change.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Change.ForeColor = System.Drawing.Color.Black;
            this.Change.Location = new System.Drawing.Point(248, 71);
            this.Change.MinimumSize = new System.Drawing.Size(110, 0);
            this.Change.Name = "Change";
            this.Change.Size = new System.Drawing.Size(110, 17);
            this.Change.TabIndex = 10;
            this.Change.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Symbol
            // 
            this.Symbol.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
            this.Symbol.BackColor = System.Drawing.Color.White;
            this.Symbol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Symbol.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.Symbol.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Symbol.ForeColor = System.Drawing.Color.Black;
            this.Symbol.Location = new System.Drawing.Point(171, 18);
            this.Symbol.Name = "Symbol";
            this.Symbol.Size = new System.Drawing.Size(80, 25);
            this.Symbol.TabIndex = 15;
            this.Symbol.Visible = false;
            this.Symbol.WordWrap = false;
            // 
            // SymbolLabel
            // 
            this.SymbolLabel.AutoSize = true;
            this.SymbolLabel.BackColor = System.Drawing.Color.White;
            this.SymbolLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SymbolLabel.ForeColor = System.Drawing.Color.Black;
            this.SymbolLabel.Location = new System.Drawing.Point(107, 21);
            this.SymbolLabel.Name = "SymbolLabel";
            this.SymbolLabel.Size = new System.Drawing.Size(54, 17);
            this.SymbolLabel.TabIndex = 16;
            this.SymbolLabel.Text = "Symbol:";
            this.SymbolLabel.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(384, 131);
            this.Controls.Add(this.SymbolLabel);
            this.Controls.Add(this.Symbol);
            this.Controls.Add(this.Volume);
            this.Controls.Add(this.ChangeLabel);
            this.Controls.Add(this.Change);
            this.Controls.Add(this.AskSize);
            this.Controls.Add(this.BidSize);
            this.Controls.Add(this.LastSize);
            this.Controls.Add(this.AskLabel);
            this.Controls.Add(this.BidLabel);
            this.Controls.Add(this.LastLabel);
            this.Controls.Add(this.AskPrice);
            this.Controls.Add(this.BidPrice);
            this.Controls.Add(this.LastPrice);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "InterReact";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LastPrice;
        private System.Windows.Forms.Label BidPrice;
        private System.Windows.Forms.Label AskPrice;
        private System.Windows.Forms.Label AskLabel;
        private System.Windows.Forms.Label BidLabel;
        private System.Windows.Forms.Label LastLabel;
        private System.Windows.Forms.Label AskSize;
        private System.Windows.Forms.Label BidSize;
        private System.Windows.Forms.Label LastSize;
        private System.Windows.Forms.Label Volume;
        private System.Windows.Forms.Label ChangeLabel;
        private System.Windows.Forms.Label Change;
        private System.Windows.Forms.TextBox Symbol;
        private System.Windows.Forms.Label SymbolLabel;
    }
}

