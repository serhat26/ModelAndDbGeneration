using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DbConnection
{
    public partial class frmDBAyarlar : Form
    {
        ConnectionProvider cp = new ConnectionProvider();
        ConnectionSettings settings = new ConnectionSettings();
        public frmDBAyarlar()
        {
            InitializeComponent();
        }

        private void btnTamam_Click(object sender, EventArgs e)
        {
            if (cmbVeritabaniTuru.Text == "")
            {
                MessageBox.Show("DbType has to be selected !", "Db Type", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            settings.dbType = cmbVeritabaniTuru.Text;
            settings.dsn = txtDSN.Text;
            settings.database = txtDb.Text;
            settings.server = txtServer.Text;
            settings.uid = txtKullanici.Text;
            settings.pwd = txtSifre.Text;

            cp.SaveDbConnectionSettings(settings);

            Close();
        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmDBAyarlar_Load(object sender, EventArgs e)
        {
            settings = ConnectionProvider.LoadDbConnectionSettings();

            txtDSN.Text = settings.dsn;
            txtDb.Text = settings.database;
            txtKullanici.Text = settings.uid;
            txtSifre.Text = settings.pwd;
            txtServer.Text = settings.server;
            cmbVeritabaniTuru.Text = settings.dbType;
            changelabel();
        }

        private void cmbVeritabaniTuru_SelectedIndexChanged(object sender, EventArgs e)
        {
            changelabel();
        }

        private void changelabel()
        {
            if (cmbVeritabaniTuru.Text == "ORACLE")
            {
                lbldatabase.Text = "Service Name";
            }
            else if (cmbVeritabaniTuru.Text == "MSSQL")
            {
                lbldatabase.Text = "Database";
            }
        }
    }
}
