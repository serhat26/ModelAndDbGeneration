using DbConnection;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ModelAndDbGeneration
{
    public partial class frmModelAndDbClassGeneration : Form
    {
        dbOperations _dbOperations = new dbOperations();

        #region RepositoryItemButtonEdit properties
        RepositoryItemButtonEdit foreignKeyButton = new RepositoryItemButtonEdit();
        RepositoryItemButtonEdit foreignKeyButtonSil = new RepositoryItemButtonEdit();
        #endregion
        public frmModelAndDbClassGeneration()
        {
            InitializeComponent();

            #region foreignKeyButton
            foreignKeyButton.Buttons[0].Assign(buttonEdit1.Properties.Buttons[0]);
            foreignKeyButton.ButtonsStyle = BorderStyles.NoBorder;
            foreignKeyButton.ButtonClick += new ButtonPressedEventHandler(repositoryItemForeignKey_ButtonClick);
            foreignKeyButtonSil.Buttons[0].Assign(buttonEdit1.Properties.Buttons[1]);
            foreignKeyButtonSil.ButtonsStyle = BorderStyles.NoBorder;
            foreignKeyButtonSil.ButtonClick += new ButtonPressedEventHandler(repositoryItemForeignKeyDelete_ButtonClick);
            #endregion
            gv.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
        }

        private void frm_ModelSinifDbSinifOlusturma_Load(object sender, EventArgs e)
        {
            gcTableInformation.Enabled = false;
            gcIndexInformation.Enabled = false;
            pcButonlar.Enabled = false;
            string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            txtFilePath.Text = Path.GetFullPath(Path.Combine(filePath, @"..\..\"));
            grd.DataSource = _dbOperations.TableInformation(null);
            grdIndex.DataSource = _dbOperations.IndexInformation(null);
        }
        private void txtFilePath_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = txtFilePath.Text;
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txtFilePath.Text = fbd.SelectedPath;
                }
            }
        }
        private void slueTable_QueryPopUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            slueTable.Properties.DataSource = _dbOperations.GetAllTables();
        }
        private void slueTable_EditValueChanged(object sender, EventArgs e)
        {
            grd.DataSource = _dbOperations.TableInformation(null);
            grdIndex.DataSource = _dbOperations.IndexInformation(null);
            txtTableName.Text = "";

            if (slueTable.EditValue != null)
            {
                grd.DataSource = _dbOperations.TableInformation(slueTable.EditValue.ToString());
                grdIndex.DataSource = _dbOperations.IndexInformation(slueTable.EditValue.ToString());
                txtTableName.Text = slueTable.EditValue.ToString();
            }
        }
        private void txtTableName_EditValueChanged(object sender, EventArgs e)
        {
            txtTableName.Properties.Appearance.ForeColor = System.Drawing.Color.DarkRed;
            lblTableNameAlert.Visible = true;
            gcTableInformation.Enabled = false;
            gcIndexInformation.Enabled = false;
            pcButonlar.Enabled = false;
            gcTableInformation.Text = "";
            gcIndexInformation.Text = "";

            if (!string.IsNullOrEmpty(txtTableName.Text))
            {
                gcTableInformation.Enabled = true;
                gcIndexInformation.Enabled = true;
                gcTableInformation.Text = txtTableName.Text;
                gcIndexInformation.Text = txtTableName.Text;
                if (CommonHelper.isAllUpper(txtTableName.Text) == false 
                    && txtTableName.Text.Length > 1 && char.IsUpper(txtTableName.Text[0]))
                {
                    pcButonlar.Enabled = true;
                    txtTableName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
                    lblTableNameAlert.Visible = false;

                    DataTable dtTable = (DataTable)grd.DataSource;
                    if(dtTable != null)
                    {
                        for (int i = 0; i < dtTable.Rows.Count; i++)
                        {
                            string foreignKeyTableColumn = CommonHelper.Nvl(dtTable.Rows[i]["FOREIGNKEYTABLECOLUMN"]);
                            if (!string.IsNullOrEmpty(foreignKeyTableColumn))
                            {
                                string upperLettersTableName = string.Concat(txtTableName.Text.Where(c => c >= 'A' && c <= 'Z'));
                                string foreignTableName = foreignKeyTableColumn.Substring(0, foreignKeyTableColumn.IndexOf("("));
                                if (foreignTableName.Length > 3)
                                {
                                    foreignTableName = foreignTableName.Substring(0, 3);
                                }
                                string foreignKeyColumn = foreignKeyTableColumn.Split('(', ')')[1];
                                string foreignKeyName = "FK_" + upperLettersTableName + "_" + foreignTableName + "_" + foreignKeyColumn;

                                gv.SetRowCellValue(i, "FOREIGNKEYNAME", foreignKeyName);
                            }
                        }
                    }
                    

                    DataTable dtIndex = (DataTable)grdIndex.DataSource;
                    if (dtIndex != null)
                    {
                        for (int i = 0; i < dtIndex.Rows.Count; i++)
                        {
                            string indexColumns = CommonHelper.Nvl(dtIndex.Rows[i]["INDEXCOLUMNS"]);
                            string upperLettersTableName = string.Concat(txtTableName.Text.Where(c => c >= 'A' && c <= 'Z'));

                            string indexColumnsStr = "";
                            string[] indexColumnsArr = indexColumns.Split(',');
                            indexColumnsStr = string.Join("_", indexColumnsArr);

                            string indexName = "EX_" + upperLettersTableName + "_" + indexColumnsStr.ToUpperInvariant();
                            gvIndex.SetRowCellValue(i, "INDEXNAME", indexName);
                        }
                    }
                }
            }
        }
        private void gcTableInformation_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTableName.Text))
            {
                MessageBox.Show("Table name have to be filled!");
                return;
            }
            gv.CloseEditor();
            gv.UpdateCurrentRow();
            gv.AddNewRow();
            gv.CloseEditor();
            gv.UpdateCurrentRow();
        }
        private void gcIndexInformation_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTableName.Text))
            {
                MessageBox.Show("Table name have to be filled!");
                return;
            }
            gvIndex.CloseEditor();
            gvIndex.UpdateCurrentRow();
            gvIndex.AddNewRow();
            gvIndex.CloseEditor();
            gvIndex.UpdateCurrentRow();
        }
        private void gv_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            gv.SetRowCellValue(e.RowHandle, "COLUMNNAME", null);
            gv.SetRowCellValue(e.RowHandle, "DATATYPE", string.Empty);
            gv.SetRowCellValue(e.RowHandle, "COLUMNLENGTHSTRING", 0);
            gv.SetRowCellValue(e.RowHandle, "COLUMNLENGTHINT1", 0);
            gv.SetRowCellValue(e.RowHandle, "COLUMNLENGTHINT2", 0);
            gv.SetRowCellValue(e.RowHandle, "COLUMNNULLABLE", false);
            gv.SetRowCellValue(e.RowHandle, "PRIMARYKEY", false);
            gv.SetRowCellValue(e.RowHandle, "FOREIGNKEYNAME", string.Empty);
            gv.SetRowCellValue(e.RowHandle, "FOREIGNKEYTABLECOLUMN", string.Empty);
            gv.SetRowCellValue(e.RowHandle, "WHEREFORUPDATE", false);
        }
        private void gvIndex_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            gvIndex.SetRowCellValue(e.RowHandle, "INDEXNAME", string.Empty);
            gvIndex.SetRowCellValue(e.RowHandle, "INDEXCOLUMNS", string.Empty);
        }

        void repositoryItemForeignKey_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTableName.Text))
            {
                MessageBox.Show("TTable name have to be filled!");
                return;
            }
            frmForeignTableSelection _frmForeignTableSelection = new frmForeignTableSelection();
            _frmForeignTableSelection.tableName = txtTableName.Text;
            _frmForeignTableSelection.ShowDialog(this);
            string foreignKeyName = _frmForeignTableSelection.foreignKeyName;
            string foreignTableNameColumnName = _frmForeignTableSelection.foreignTableNameColumnName;
            gv.SetRowCellValue(gv.FocusedRowHandle, gv.Columns["FOREIGNKEYNAME"], foreignKeyName);
            gv.SetRowCellValue(gv.FocusedRowHandle, gv.Columns["FOREIGNKEYTABLECOLUMN"], foreignTableNameColumnName);

            _frmForeignTableSelection.Dispose();
        }
        void repositoryItemForeignKeyDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            gv.SetRowCellValue(gv.FocusedRowHandle, gv.Columns["FOREIGNKEYNAME"], string.Empty);
            gv.SetRowCellValue(gv.FocusedRowHandle, gv.Columns["FOREIGNKEYTABLECOLUMN"], string.Empty);
        }
        private void RIBEDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            int value = ((grd.FocusedView).DataController).CurrentControllerRow;
            if (value > -1)
            {
                var rV = ((DataRowView)(grd.MainView.GetRow(value)));
                gv.DeleteRow(gv.FocusedRowHandle);
            }
        }

        private void RIBEIndexDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            int value = ((grdIndex.FocusedView).DataController).CurrentControllerRow;
            if (value > -1)
            {
                var rV = ((DataRowView)(grdIndex.MainView.GetRow(value)));
                gvIndex.DeleteRow(gvIndex.FocusedRowHandle);
            }
        }

        private void gv_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
        }

        private void gvIndex_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
        }

        private void gv_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.Column.FieldName == "COLUMNNULLABLE")
            {
                string columnNullable = e.Value.ToString();

                if (columnNullable.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    view.SetRowCellValue(e.RowHandle, "PRIMARYKEY", false);
                }
            }
            if (e.Column.FieldName == "PRIMARYKEY")
            {
                string prımaryKey = e.Value.ToString();

                if (prımaryKey.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    view.SetRowCellValue(e.RowHandle, "COLUMNNULLABLE", false);
                    gv.SetRowCellValue(gv.FocusedRowHandle, gv.Columns["FOREIGNKEYNAME"], string.Empty);
                    gv.SetRowCellValue(gv.FocusedRowHandle, gv.Columns["FOREIGNKEYTABLECOLUMN"], string.Empty);
                }
            }
        }

        private void gv_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "FOREIGNKEYNAME")
            {
                if (string.IsNullOrEmpty(e.CellValue.ToString()))
                {
                    e.RepositoryItem = foreignKeyButton;
                }
                else
                {
                    e.RepositoryItem = foreignKeyButtonSil;
                }
            }
        }

        private void gvIndex_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.Column.FieldName == "INDEXCOLUMNS")
            {
                string indexColumns = e.Value.ToString();

                string upperLettersTableName = string.Concat(txtTableName.Text.Where(c => c >= 'A' && c <= 'Z'));

                string indexColumnsStr = "";
                string[] indexColumnsArr = indexColumns.Split(',');
                indexColumnsStr = string.Join("_", indexColumnsArr);

                string indexName = "EX_" + upperLettersTableName + "_" + indexColumnsStr.ToUpperInvariant();
                view.SetRowCellValue(e.RowHandle, "INDEXNAME", indexName);

            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            if (e.SelectedControl != grd) return;

            ToolTipControlInfo info = null;
            //Get the view at the current mouse position
            GridView view = grd.GetViewAt(e.ControlMousePosition) as GridView;
            if (view == null) return;
            //Get the view's element information that resides at the current position
            GridHitInfo hi = view.CalcHitInfo(e.ControlMousePosition);
            //Display a hint for row indicator cells
            if (hi.InRowCell && hi.Column.FieldName == "COLUMNNAME")
            {
                //An object that uniquely identifies a row indicator cell
                object o = hi.Column.FieldName + hi.RowHandle.ToString();
                info = new ToolTipControlInfo(o, "column name has to be camelCase sample: nNo, nProtokolNo");
            }
            //Supply tooltip information if applicable, otherwise preserve default tooltip (if any)
            if (info != null)
                e.Info = info;
        }

        private void btnOracleCreateTable_Click(object sender, EventArgs e)
        {
            DataTable dtTablo = (DataTable)grd.DataSource;
            List<string> columnInformation = new List<string>();
            List<string> constraintInformation = new List<string>();
            foreach (DataRow dr in dtTablo.Rows)
            {
                string columnName = CommonHelper.Nvl(dr["COLUMNNAME"]);
                string dataType = CommonHelper.Nvl(dr["DATATYPE"]);
                int columnLengthStrıng = CommonHelper.ToInt32(dr["COLUMNLENGTHSTRING"]);
                int columnLengthInt1 = CommonHelper.ToInt32(dr["COLUMNLENGTHINT1"]);
                int columnLengthInt2 = CommonHelper.ToInt32(dr["COLUMNLENGTHINT2"]);
                bool columnNullable = Convert.ToBoolean(dr["COLUMNNULLABLE"]);
                bool primaryKey = Convert.ToBoolean(dr["PRIMARYKEY"]);
                string foreignKeyName = CommonHelper.Nvl(dr["FOREIGNKEYNAME"]);
                string foreignKeyColumn = CommonHelper.Nvl(dr["FOREIGNKEYTABLECOLUMN"]);

                if (string.IsNullOrEmpty(columnName) || string.IsNullOrEmpty(dataType))
                {
                    MessageBox.Show("Column Name and DataType can not be empty!", Text);
                    return;
                }

                string columnConcat = columnName + " ";
                if (columnLengthStrıng > 0 && dataType.Contains("CHAR"))
                {
                    if (dataType.Contains("VAR"))
                        columnConcat += "VARCHAR2(" + columnLengthStrıng + ") ";
                    else
                        columnConcat += dataType + "(" + columnLengthStrıng + ") ";
                }
                else if (columnLengthInt1 > 0)
                {
                    columnConcat += "NUMERIC(" + columnLengthInt1 + "," + columnLengthInt2 + ") ";
                }
                else
                {
                    if (dataType.Contains("DATE"))
                        columnConcat += "DATE ";
                    else if (dataType.Contains("BLOB") || dataType.Contains("BINARY"))
                        columnConcat += "BLOB ";
                    else
                        columnConcat += dataType + " ";
                }
                if (columnNullable)
                {
                    columnConcat += "NULL";
                }
                else
                {
                    columnConcat += "NOT NULL";
                }

                columnInformation.Add(columnConcat);

                if (primaryKey)
                {
                    constraintInformation.Add("ALTER TABLE " + txtTableName.Text.ToUpperInvariant() + " ADD CONSTRAINT " +
                        txtTableName.Text.ToUpperInvariant() + "_PK PRIMARY KEY (" + columnName + ");");
                }
                else if (!string.IsNullOrEmpty(foreignKeyName))
                {
                    constraintInformation.Add("ALTER TABLE " + txtTableName.Text.ToUpperInvariant() + " ADD CONSTRAINT " + foreignKeyName +
                         " FOREIGN KEY(" + columnName + ") REFERENCES " + foreignKeyColumn + ";");
                }
            }

            DataTable dtIndex = (DataTable)grdIndex.DataSource;
            List<string> indexInformation = new List<string>();
            foreach (DataRow dr in dtIndex.Rows)
            {
                string indexName = CommonHelper.Nvl(dr["INDEXNAME"]);
                string indexColumns = CommonHelper.Nvl(dr["INDEXCOLUMNS"]);
                if (!string.IsNullOrEmpty(indexName) && !string.IsNullOrEmpty(indexColumns))
                {
                    indexInformation.Add("CREATE INDEX " + indexName + " ON " + txtTableName.Text.ToUpperInvariant() + "(" + indexColumns + ");");
                }
            }

            string createSql = "CREATE TABLE " + txtTableName.Text.ToUpperInvariant() + "(" + Environment.NewLine + "\t" +
                string.Join("," + Environment.NewLine + "\t", columnInformation.ToArray()) + Environment.NewLine + ");";
            string constraintSql = string.Join(Environment.NewLine, constraintInformation.ToArray());
            string indexSql = string.Join(Environment.NewLine, indexInformation.ToArray());

            string sql = createSql + Environment.NewLine + constraintSql + Environment.NewLine + indexSql;
            using (Form form = new Form())
            {
                form.Text = btnOracleCreateTable.Text;
                form.ClientSize = new Size(500, 400);
                form.StartPosition = FormStartPosition.CenterScreen;
                DevExpress.XtraEditors.MemoEdit memoEdit = new DevExpress.XtraEditors.MemoEdit();
                memoEdit.Text = sql;
                memoEdit.Dock = DockStyle.Fill;
                form.Controls.Add(memoEdit);
                form.ShowDialog();
            }
        }

        private void btnSqlserverCreate_Click(object sender, EventArgs e)
        {
            DataTable dtTablo = (DataTable)grd.DataSource;
            List<string> columnInformation = new List<string>();
            List<string> constraintInformation = new List<string>();
            foreach (DataRow dr in dtTablo.Rows)
            {
                string columnName = CommonHelper.Nvl(dr["COLUMNNAME"]);
                string dataType = CommonHelper.Nvl(dr["DATATYPE"]);
                int columnLengthStrıng = CommonHelper.ToInt32(dr["COLUMNLENGTHSTRING"]);
                int columnLengthInt1 = CommonHelper.ToInt32(dr["COLUMNLENGTHINT1"]);
                int columnLengthInt2 = CommonHelper.ToInt32(dr["COLUMNLENGTHINT2"]);
                bool columnNullable = Convert.ToBoolean(dr["COLUMNNULLABLE"]);
                bool primaryKey = Convert.ToBoolean(dr["PRIMARYKEY"]);
                string foreignKeyName = CommonHelper.Nvl(dr["FOREIGNKEYNAME"]);
                string foreignKeyColumn = CommonHelper.Nvl(dr["FOREIGNKEYTABLECOLUMN"]);

                if (string.IsNullOrEmpty(columnName) || string.IsNullOrEmpty(dataType))
                {
                    MessageBox.Show("Column Name and DataType can not be empty!", Text);
                    return;
                }

                string columnConcat = columnName + " ";
                if (columnLengthStrıng > 0 && dataType.Contains("CHAR"))
                {
                    if(dataType.Contains("VAR"))
                        columnConcat += "VARCHAR(" + columnLengthStrıng + ") ";
                    else
                        columnConcat += dataType + "(" + columnLengthStrıng + ") ";
                }
                else if (columnLengthInt1 > 0)
                {
                    columnConcat += "NUMERIC(" + columnLengthInt1 + "," + columnLengthInt2 + ") ";
                }
                else
                {
                    if (dataType.Contains("DATE"))
                        columnConcat += "DATETIME ";
                    else if (dataType.Contains("BLOB") || dataType.Contains("BINARY"))
                        columnConcat += "VARBINARY(MAX) ";
                    else
                        columnConcat += dataType + " ";
                }
                if (columnNullable)
                {
                    columnConcat += "NULL";
                }
                else
                {
                    columnConcat += "NOT NULL";
                }

                columnInformation.Add(columnConcat);

                if (primaryKey)
                {
                    constraintInformation.Add("ALTER TABLE " + txtTableName.Text.ToUpperInvariant() + " ADD CONSTRAINT " +
                        txtTableName.Text.ToUpperInvariant() + "_PK PRIMARY KEY (" + columnName + ");");
                }
                else if (!string.IsNullOrEmpty(foreignKeyName))
                {
                    constraintInformation.Add("ALTER TABLE " + txtTableName.Text.ToUpperInvariant() + " ADD CONSTRAINT " + foreignKeyName +
                         " FOREIGN KEY(" + columnName + ") REFERENCES " + foreignKeyColumn + ";");
                }
            }

            DataTable dtIndex = (DataTable)grdIndex.DataSource;
            List<string> indexInformation = new List<string>();
            foreach (DataRow dr in dtIndex.Rows)
            {
                string indexName = CommonHelper.Nvl(dr["INDEXNAME"]);
                string indexColumns = CommonHelper.Nvl(dr["INDEXCOLUMNS"]);
                if (!string.IsNullOrEmpty(indexName) && !string.IsNullOrEmpty(indexColumns))
                {
                    indexInformation.Add("CREATE INDEX " + indexName + " ON " + txtTableName.Text.ToUpperInvariant() + "(" + indexColumns + ");");
                }
            }

            string createSql = "CREATE TABLE " + txtTableName.Text.ToUpperInvariant() + "(" + Environment.NewLine + "\t" +
                string.Join("," + Environment.NewLine + "\t", columnInformation.ToArray()) + Environment.NewLine + ");";
            string constraintSql = string.Join(Environment.NewLine, constraintInformation.ToArray());
            string indexSql = string.Join(Environment.NewLine, indexInformation.ToArray());

            string sql = createSql + Environment.NewLine + constraintSql + Environment.NewLine + indexSql;
            using (Form form = new Form())
            {
                form.Text = btnOracleCreateTable.Text;
                form.ClientSize = new Size(500, 400);
                form.StartPosition = FormStartPosition.CenterScreen;
                DevExpress.XtraEditors.MemoEdit memoEdit = new DevExpress.XtraEditors.MemoEdit();
                memoEdit.Text = sql;
                memoEdit.Dock = DockStyle.Fill;
                form.Controls.Add(memoEdit);
                form.ShowDialog();
            }
        }

        private void btnModelAndDbClass_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = txtFilePath.Text;
                string tableName = txtTableName.Text;
                string namespacePath = "";
                if (filePath.Substring(filePath.Length - 1).Equals("\\"))
                {
                    string sonSlashAtilmisStr = filePath.Remove(filePath.Length - 1);
                    namespacePath = sonSlashAtilmisStr.Split('\\').Last();
                }
                else
                {
                    namespacePath = filePath.Split('\\').Last();
                }

                List<ParameterSpeciality> parameters = new List<ParameterSpeciality>();
                List<ParameterSpeciality> whereList = new List<ParameterSpeciality>();

                DataTable dtTable = (DataTable)grd.DataSource;
                List<string> columnInfo = new List<string>();
                List<string> constraintInfo = new List<string>();
                foreach (DataRow dr in dtTable.Rows)
                {
                    string columnName = CommonHelper.Nvl(dr["COLUMNNAME"]);
                    string dataType = CommonHelper.Nvl(dr["DATATYPE"]);
                    int columnLengthStrıng = CommonHelper.ToInt32(dr["COLUMNLENGTHSTRING"]);
                    int columnLengthInt1 = CommonHelper.ToInt32(dr["COLUMNLENGTHINT1"]);
                    int columnLengthInt2 = CommonHelper.ToInt32(dr["COLUMNLENGTHINT2"]);
                    bool whereBool = Convert.ToBoolean(dr["WHEREFORUPDATE"]);
                    bool primaryKey = Convert.ToBoolean(dr["PRIMARYKEY"]);
                    string foreignKeyName = CommonHelper.Nvl(dr["FOREIGNKEYNAME"]);

                    ParameterSpeciality ParameterSpeciality;
                    if (columnLengthStrıng > 0 && dataType.Contains("CHAR"))
                    {
                        ParameterSpeciality = new ParameterSpeciality { dbType = DbType.String, ColumnName = columnName };
                    }
                    else if (columnLengthInt1 > 0 && columnLengthInt2 > 0)
                    {
                        ParameterSpeciality = new ParameterSpeciality { dbType = DbType.Decimal, ColumnName = columnName };
                    }
                    else if (columnLengthInt1 > 8)
                    {
                        ParameterSpeciality = new ParameterSpeciality { dbType = DbType.Int64, ColumnName = columnName };
                    }
                    else if (columnLengthInt1 > 1)
                    {
                        ParameterSpeciality = new ParameterSpeciality { dbType = DbType.Int32, ColumnName = columnName };
                    }
                    else if (columnLengthInt1 == 1)
                    {
                        ParameterSpeciality = new ParameterSpeciality { dbType = DbType.Int16, ColumnName = columnName };
                    }
                    else
                    {
                        if (dataType.Contains("DATE"))
                            ParameterSpeciality = new ParameterSpeciality { dbType = DbType.DateTime, ColumnName = columnName };
                        else
                            ParameterSpeciality = new ParameterSpeciality { dbType = DbType.Binary, ColumnName = columnName };
                    }

                    if (primaryKey)
                        ParameterSpeciality.PrimaryKey = true;

                    parameters.Add(ParameterSpeciality);

                    if (whereBool)
                    {
                        whereList.Add(ParameterSpeciality);
                    }
                }

                string modelClass = ModelAndDbTemplate.modeltemplate(namespacePath, tableName, parameters);
                Directory.CreateDirectory(filePath + "\\" + tableName); 
                string modelClassName = tableName;
                File.WriteAllText(filePath + "\\" + tableName + "\\" + modelClassName + ".cs", modelClass);

                string dbClass = ModelAndDbTemplate.dbtemplate(namespacePath, tableName, parameters, whereList);
                string dbClassName = "dbOperations" + tableName;
                File.WriteAllText(filePath + "\\" + tableName + "\\" + dbClassName + ".cs", dbClass);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Text);
            }

        }

        private void txtTableName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (System.Text.Encoding.UTF8.GetByteCount(new char[] { e.KeyChar }) > 1)
            {
                e.Handled = true;
            }
        }
    }
}
