namespace DbConnection
{
    partial class frmDBAyarlar
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
            this.lbldatabase = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbldbserver = new System.Windows.Forms.Label();
            this.cmbVeritabaniTuru = new System.Windows.Forms.ComboBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.txtDb = new System.Windows.Forms.TextBox();
            this.txtKullanici = new System.Windows.Forms.TextBox();
            this.txtSifre = new System.Windows.Forms.TextBox();
            this.txtDSN = new System.Windows.Forms.TextBox();
            this.btnKapat = new System.Windows.Forms.Button();
            this.btnTamam = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbldatabase
            // 
            this.lbldatabase.AutoSize = true;
            this.lbldatabase.Location = new System.Drawing.Point(12, 57);
            this.lbldatabase.Name = "lbldatabase";
            this.lbldatabase.Size = new System.Drawing.Size(53, 13);
            this.lbldatabase.TabIndex = 17;
            this.lbldatabase.Text = "Database";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "DSN";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Pass";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "User";
            // 
            // lbldbserver
            // 
            this.lbldbserver.AutoSize = true;
            this.lbldbserver.Location = new System.Drawing.Point(12, 35);
            this.lbldbserver.Name = "lbldbserver";
            this.lbldbserver.Size = new System.Drawing.Size(51, 13);
            this.lbldbserver.TabIndex = 12;
            this.lbldbserver.Text = "Server IP";
            // 
            // cmbVeritabaniTuru
            // 
            this.cmbVeritabaniTuru.FormattingEnabled = true;
            this.cmbVeritabaniTuru.Items.AddRange(new object[] {
            "ORACLE",
            "MSSQL"});
            this.cmbVeritabaniTuru.Location = new System.Drawing.Point(90, 9);
            this.cmbVeritabaniTuru.Name = "cmbVeritabaniTuru";
            this.cmbVeritabaniTuru.Size = new System.Drawing.Size(199, 21);
            this.cmbVeritabaniTuru.TabIndex = 18;
            this.cmbVeritabaniTuru.SelectedIndexChanged += new System.EventHandler(this.cmbVeritabaniTuru_SelectedIndexChanged);
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(90, 32);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(199, 20);
            this.txtServer.TabIndex = 19;
            // 
            // txtDb
            // 
            this.txtDb.Location = new System.Drawing.Point(90, 54);
            this.txtDb.Name = "txtDb";
            this.txtDb.Size = new System.Drawing.Size(199, 20);
            this.txtDb.TabIndex = 20;
            // 
            // txtKullanici
            // 
            this.txtKullanici.Location = new System.Drawing.Point(90, 76);
            this.txtKullanici.Name = "txtKullanici";
            this.txtKullanici.Size = new System.Drawing.Size(199, 20);
            this.txtKullanici.TabIndex = 21;
            // 
            // txtSifre
            // 
            this.txtSifre.Location = new System.Drawing.Point(90, 98);
            this.txtSifre.Name = "txtSifre";
            this.txtSifre.Size = new System.Drawing.Size(199, 20);
            this.txtSifre.TabIndex = 22;
            // 
            // txtDSN
            // 
            this.txtDSN.Location = new System.Drawing.Point(90, 120);
            this.txtDSN.Name = "txtDSN";
            this.txtDSN.Size = new System.Drawing.Size(199, 20);
            this.txtDSN.TabIndex = 23;
            // 
            // btnKapat
            // 
            this.btnKapat.Location = new System.Drawing.Point(137, 169);
            this.btnKapat.Name = "btnKapat";
            this.btnKapat.Size = new System.Drawing.Size(75, 23);
            this.btnKapat.TabIndex = 24;
            this.btnKapat.Text = "Close";
            this.btnKapat.UseVisualStyleBackColor = true;
            this.btnKapat.Click += new System.EventHandler(this.btnKapat_Click);
            // 
            // btnTamam
            // 
            this.btnTamam.Location = new System.Drawing.Point(214, 169);
            this.btnTamam.Name = "btnTamam";
            this.btnTamam.Size = new System.Drawing.Size(75, 23);
            this.btnTamam.TabIndex = 25;
            this.btnTamam.Text = "Ok";
            this.btnTamam.UseVisualStyleBackColor = true;
            this.btnTamam.Click += new System.EventHandler(this.btnTamam_Click);
            // 
            // frmDBAyarlar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 204);
            this.Controls.Add(this.btnTamam);
            this.Controls.Add(this.btnKapat);
            this.Controls.Add(this.txtDSN);
            this.Controls.Add(this.txtSifre);
            this.Controls.Add(this.txtKullanici);
            this.Controls.Add(this.txtDb);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.cmbVeritabaniTuru);
            this.Controls.Add(this.lbldatabase);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbldbserver);
            this.Name = "frmDBAyarlar";
            this.Text = "DB Configuration";
            this.Load += new System.EventHandler(this.frmDBAyarlar_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbldatabase;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbldbserver;
        private System.Windows.Forms.ComboBox cmbVeritabaniTuru;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtDb;
        private System.Windows.Forms.TextBox txtKullanici;
        private System.Windows.Forms.TextBox txtSifre;
        private System.Windows.Forms.TextBox txtDSN;
        private System.Windows.Forms.Button btnKapat;
        private System.Windows.Forms.Button btnTamam;
    }
}