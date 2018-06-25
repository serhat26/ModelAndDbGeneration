using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ModelAndDbGeneration
{
    public partial class frmForeignTableSelection : Form
    {
        dbOperations _dbOperations = new dbOperations();
        public string tableName;
        public string foreignKeyName;
        public string foreignTableNameColumnName;
        
        public frmForeignTableSelection()
        {
            InitializeComponent();
            CenterToParent();
        }

        private void frm_ForeignTableSelection_Load(object sender, EventArgs e)
        {
            slueTable.Properties.DataSource = _dbOperations.GetAllTables();
        }

        private void slueTable_EditValueChanged(object sender, EventArgs e)
        {
            if (slueTable.EditValue != null)
            {
                slueColumn.Properties.DataSource = _dbOperations.GetColumns(slueTable.EditValue.ToString());
                string upperLettersTableName = string.Concat(tableName.Where(c => c >= 'A' && c <= 'Z'));
                string foreignTableName = slueTable.EditValue.ToString();
                if (foreignTableName.Length > 3)
                {
                    foreignTableName = foreignTableName.Substring(0, 3);
                }
                txtForeingKey.Text = "FK_" + upperLettersTableName + "_" + foreignTableName;
                string primaryColumnName = _dbOperations.GetPrimaryColumnName(slueTable.EditValue.ToString());
                if (!string.IsNullOrEmpty(primaryColumnName))
                {
                    slueColumn.EditValue = primaryColumnName;
                }
            }
        }
        private void slueColumn_EditValueChanged(object sender, EventArgs e)
        {
            if (slueColumn.EditValue != null)
            {
                string upperLettersTableName = string.Concat(tableName.Where(c => c >= 'A' && c <= 'Z'));
                string foreignTableName = slueTable.EditValue.ToString();
                if (foreignTableName.Length > 3)
                {
                    foreignTableName = foreignTableName.Substring(0, 3);
                }
                txtForeingKey.Text = "FK_" + upperLettersTableName + "_" + foreignTableName + "_" + slueColumn.EditValue.ToString();
            }
        }
        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtForeingKey.Text))
            {
                MessageBox.Show("Foreign Name has to be entered.");
                return;
            }
            if (slueTable.EditValue == null)
            {
                MessageBox.Show("Table has to be selected.");
                return;
            }
            if (slueColumn.EditValue == null)
            {
                MessageBox.Show("Column has to be selected.");
                return;
            }
            foreignKeyName = txtForeingKey.Text;
            foreignTableNameColumnName = slueTable.EditValue.ToString() + "(" + slueColumn.EditValue.ToString() + ")";
            this.Close();
        }
    }
}
