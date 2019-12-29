namespace Client.WinForms
{
    partial class GameWindow
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.webBrowser = new CefSharp.WinForms.ChromiumWebBrowser();
            this.button3 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "SelectGame";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(0, 25);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "StartGame";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.ActivateBrowserOnCreation = false;
            this.webBrowser.Location = new System.Drawing.Point(81, 0);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(518, 401);
            this.webBrowser.TabIndex = 3;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(0, 54);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1700;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(605, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(518, 401);
            this.pictureBox.TabIndex = 5;
            this.pictureBox.TabStop = false;
            // 
            // GameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1127, 401);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.webBrowser);
            this.Name = "GameWindow";
            this.Text = "GameWindow";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private CefSharp.WinForms.ChromiumWebBrowser webBrowser;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox pictureBox;
    }
}