namespace CRMPro
{
    partial class FormGiris
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelSS = new System.Windows.Forms.Panel();
            this.btnSifreKoduGonder = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SifreSeposta = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtSifre = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.MusteriOlLink = new System.Windows.Forms.LinkLabel();
            this.CikisButonu = new System.Windows.Forms.Label();
            this.Sifremiunuttum = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            this.panelSS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.OldLace;
            this.panel1.Controls.Add(this.panelSS);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(335, 550);
            this.panel1.TabIndex = 0;
            // 
            // panelSS
            // 
            this.panelSS.Controls.Add(this.btnSifreKoduGonder);
            this.panelSS.Controls.Add(this.label3);
            this.panelSS.Controls.Add(this.SifreSeposta);
            this.panelSS.Location = new System.Drawing.Point(26, 238);
            this.panelSS.Name = "panelSS";
            this.panelSS.Size = new System.Drawing.Size(257, 239);
            this.panelSS.TabIndex = 1;
            this.panelSS.Visible = false;
            // 
            // btnSifreKoduGonder
            // 
            this.btnSifreKoduGonder.BackColor = System.Drawing.Color.Tan;
            this.btnSifreKoduGonder.Location = new System.Drawing.Point(76, 161);
            this.btnSifreKoduGonder.Name = "btnSifreKoduGonder";
            this.btnSifreKoduGonder.Size = new System.Drawing.Size(101, 37);
            this.btnSifreKoduGonder.TabIndex = 5;
            this.btnSifreKoduGonder.Text = "Gönder";
            this.btnSifreKoduGonder.UseVisualStyleBackColor = false;
            this.btnSifreKoduGonder.Click += new System.EventHandler(this.btnSifreKoduGonder_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
            this.label3.Location = new System.Drawing.Point(14, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(223, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Sifre Sıfırlamak icin e-posta Giriniz";
            // 
            // SifreSeposta
            // 
            this.SifreSeposta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SifreSeposta.Location = new System.Drawing.Point(17, 100);
            this.SifreSeposta.Name = "SifreSeposta";
            this.SifreSeposta.Size = new System.Drawing.Size(220, 20);
            this.SifreSeposta.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CRMPro.Properties.Resources.Ekran_görüntüsü_2026_03_14_195454;
            this.pictureBox1.Location = new System.Drawing.Point(43, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(127, 117);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(545, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "E-Posta Adresi";
            // 
            // txtEmail
            // 
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.Location = new System.Drawing.Point(452, 145);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(300, 20);
            this.txtEmail.TabIndex = 2;
            // 
            // txtSifre
            // 
            this.txtSifre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSifre.Location = new System.Drawing.Point(452, 238);
            this.txtSifre.Name = "txtSifre";
            this.txtSifre.PasswordChar = '*';
            this.txtSifre.Size = new System.Drawing.Size(300, 20);
            this.txtSifre.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(581, 192);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 21);
            this.label2.TabIndex = 3;
            this.label2.Text = "Sifre";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.OldLace;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button1.ForeColor = System.Drawing.Color.DimGray;
            this.button1.Location = new System.Drawing.Point(452, 315);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(300, 32);
            this.button1.TabIndex = 5;
            this.button1.Text = "Sisteme Giris";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // MusteriOlLink
            // 
            this.MusteriOlLink.AutoSize = true;
            this.MusteriOlLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.MusteriOlLink.LinkColor = System.Drawing.Color.SkyBlue;
            this.MusteriOlLink.Location = new System.Drawing.Point(467, 371);
            this.MusteriOlLink.Name = "MusteriOlLink";
            this.MusteriOlLink.Size = new System.Drawing.Size(276, 15);
            this.MusteriOlLink.TabIndex = 6;
            this.MusteriOlLink.TabStop = true;
            this.MusteriOlLink.Text = "Hesabın yok mu? Hemen Müşteri Olarak Kayıt Ol.";
            this.MusteriOlLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.MusteriOlLink_LinkClicked);
            // 
            // CikisButonu
            // 
            this.CikisButonu.AutoSize = true;
            this.CikisButonu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CikisButonu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.CikisButonu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.CikisButonu.Location = new System.Drawing.Point(885, 0);
            this.CikisButonu.Name = "CikisButonu";
            this.CikisButonu.Size = new System.Drawing.Size(15, 16);
            this.CikisButonu.TabIndex = 7;
            this.CikisButonu.Text = "X";
            this.CikisButonu.Click += new System.EventHandler(this.CikisButonu_Click);
            // 
            // Sifremiunuttum
            // 
            this.Sifremiunuttum.AutoSize = true;
            this.Sifremiunuttum.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.Sifremiunuttum.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Sifremiunuttum.Location = new System.Drawing.Point(546, 407);
            this.Sifremiunuttum.Name = "Sifremiunuttum";
            this.Sifremiunuttum.Size = new System.Drawing.Size(118, 18);
            this.Sifremiunuttum.TabIndex = 8;
            this.Sifremiunuttum.TabStop = true;
            this.Sifremiunuttum.Text = "Şifre mi Unuttum";
            this.Sifremiunuttum.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Sifremiunuttum_LinkClicked);
            // 
            // FormGiris
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(900, 550);
            this.Controls.Add(this.Sifremiunuttum);
            this.Controls.Add(this.CikisButonu);
            this.Controls.Add(this.MusteriOlLink);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtSifre);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormGiris";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Giriş Ekranı";
            this.panel1.ResumeLayout(false);
            this.panelSS.ResumeLayout(false);
            this.panelSS.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtSifre;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.LinkLabel MusteriOlLink;
        private System.Windows.Forms.Label CikisButonu;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panelSS;
        private System.Windows.Forms.Button btnSifreKoduGonder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox SifreSeposta;
        private System.Windows.Forms.LinkLabel Sifremiunuttum;
    }
}