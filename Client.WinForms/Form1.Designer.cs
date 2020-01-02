namespace Client.WinForms
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.cyanTitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.closeBtn = new System.Windows.Forms.PictureBox();
            this.bunifuDatepicker1 = new Bunifu.Framework.UI.BunifuDatepicker();
            this.statusLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Plot = new ScottPlot.FormsPlot();
            this.bunifuDatepicker2 = new Bunifu.Framework.UI.BunifuDatepicker();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.closeBtn)).BeginInit();
            this.SuspendLayout();
            // 
            // cyanTitle
            // 
            this.cyanTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(219)))), ((int)(((byte)(222)))));
            this.cyanTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 1.25F);
            this.cyanTitle.Location = new System.Drawing.Point(2, 21);
            this.cyanTitle.Name = "cyanTitle";
            this.cyanTitle.Size = new System.Drawing.Size(800, 1);
            this.cyanTitle.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(770, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Client Log Explorer";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // closeBtn
            // 
            this.closeBtn.Image = ((System.Drawing.Image)(resources.GetObject("closeBtn.Image")));
            this.closeBtn.Location = new System.Drawing.Point(778, 0);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(21, 20);
            this.closeBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.closeBtn.TabIndex = 2;
            this.closeBtn.TabStop = false;
            this.closeBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            this.closeBtn.MouseEnter += new System.EventHandler(this.CloseBtn_MouseEnter);
            this.closeBtn.MouseLeave += new System.EventHandler(this.CloseBtn_MouseLeave);
            // 
            // bunifuDatepicker1
            // 
            this.bunifuDatepicker1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(219)))), ((int)(((byte)(222)))));
            this.bunifuDatepicker1.BorderRadius = 0;
            this.bunifuDatepicker1.ForeColor = System.Drawing.Color.White;
            this.bunifuDatepicker1.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.bunifuDatepicker1.FormatCustom = null;
            this.bunifuDatepicker1.Location = new System.Drawing.Point(572, 37);
            this.bunifuDatepicker1.Name = "bunifuDatepicker1";
            this.bunifuDatepicker1.Size = new System.Drawing.Size(218, 36);
            this.bunifuDatepicker1.TabIndex = 3;
            this.bunifuDatepicker1.Value = new System.DateTime(2020, 1, 2, 0, 58, 48, 304);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Tai Le", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ForeColor = System.Drawing.Color.White;
            this.statusLabel.Location = new System.Drawing.Point(12, 40);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(251, 45);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.Text = "Last 24 hours...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "label2";
            // 
            // Plot
            // 
            this.Plot.Location = new System.Drawing.Point(336, 79);
            this.Plot.Name = "Plot";
            this.Plot.Size = new System.Drawing.Size(454, 367);
            this.Plot.TabIndex = 7;
            // 
            // bunifuDatepicker2
            // 
            this.bunifuDatepicker2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(219)))), ((int)(((byte)(222)))));
            this.bunifuDatepicker2.BorderRadius = 0;
            this.bunifuDatepicker2.ForeColor = System.Drawing.Color.White;
            this.bunifuDatepicker2.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.bunifuDatepicker2.FormatCustom = null;
            this.bunifuDatepicker2.Location = new System.Drawing.Point(336, 37);
            this.bunifuDatepicker2.Name = "bunifuDatepicker2";
            this.bunifuDatepicker2.Size = new System.Drawing.Size(218, 36);
            this.bunifuDatepicker2.TabIndex = 8;
            this.bunifuDatepicker2.Value = new System.DateTime(2020, 1, 2, 0, 58, 48, 304);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Tai Le", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(12, 212);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 45);
            this.label3.TabIndex = 9;
            this.label3.Text = "All time...";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(802, 458);
            this.ControlBox = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bunifuDatepicker2);
            this.Controls.Add(this.Plot);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.bunifuDatepicker1);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cyanTitle);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.closeBtn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label cyanTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox closeBtn;
        private Bunifu.Framework.UI.BunifuDatepicker bunifuDatepicker1;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label label2;
        private ScottPlot.FormsPlot Plot;
        private Bunifu.Framework.UI.BunifuDatepicker bunifuDatepicker2;
        private System.Windows.Forms.Label label3;
    }
}

