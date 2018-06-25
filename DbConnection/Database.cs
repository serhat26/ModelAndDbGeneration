using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Oracle.ManagedDataAccess.Client;
using System.Threading;
using System.Threading.Tasks;

namespace DbConnection
{
    public class Database : IDisposable
    {
        #region LeftCtrl,LeftShift ve NumLock,CapsLock,ScrollLock durumu
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);
        bool VK_LCONTROL = (((ushort)GetKeyState(0xA2)) & 0xffff) != 0; //Left Control
        bool VK_LSHIFT = (((ushort)GetKeyState(0xA0)) & 0xffff) != 0; //Left Shift
        bool VK_CAPITAL = (((ushort)GetKeyState(0x14)) & 0xffff) != 0; //Caps Lock
        bool VK_NUMLOCK = (((ushort)GetKeyState(0x90)) & 0xffff) != 0; //Num Lock
        bool VK_SCROLL = (((ushort)GetKeyState(0x91)) & 0xffff) != 0; //Scroll Lock
        private void RefreshKeys()
        {
            VK_LCONTROL = (((ushort)GetKeyState(0xA2)) & 0xffff) != 0; //Left Control
            VK_LSHIFT = (((ushort)GetKeyState(0xA0)) & 0xffff) != 0; //Left Shift
            VK_CAPITAL = (((ushort)GetKeyState(0x14)) & 0xffff) != 0; //Caps Lock
            VK_NUMLOCK = (((ushort)GetKeyState(0x90)) & 0xffff) != 0; //Num Lock
            VK_SCROLL = (((ushort)GetKeyState(0x91)) & 0xffff) != 0; //Scroll Lock
        }
        //RefreshKeys();
        //if (VK_SCROLL && (VK_LCONTROL && VK_LSHIFT)) MessageBox.Show(_sql, "Debug Mod: Scroll(Ctrl+Shift)");
        #endregion


        ConnectionProvider connectionProvider = new ConnectionProvider();
        ConnectionSettings conectionSettings;
        Connection connection;

        OracleConnection connectionOracle = null;
        SqlConnection connectionSqlServer = null;
        OracleCommand cmdOracle = null;
        SqlCommand cmdSqlServer = null;
        OracleTransaction transactionOracle = null;
        SqlTransaction transactionSqlServer = null;
        //int MAX_RETRY_COUNT = 5;

        public Database()
        {
            connection = new Connection();

            try
            {
                conectionSettings = connectionProvider.getConnectionParameters();
            }
            catch (Exception)
            {
                frmDBAyarlar frm = new frmDBAyarlar();
                frm.ShowDialog();
                frm.Dispose();
                conectionSettings = connectionProvider.getConnectionParameters();
            }

            if (conectionSettings.dbType == "ORACLE")
            {
                connection.DatabaseType = Enums.DbTypes.ORACLE;
            }
            else
            {
                connection.DatabaseType = Enums.DbTypes.MSSQL;
            }
        }


        public Enums.DbTypes DbTypes
        {
            get
            {
                return connection.DatabaseType;
            }
        }

        public string ConnectionString
        {
            get
            {
                return connection.ConnectionString;
            }
        }

        #region Execute İşlemleri

        public object ExecuteScalar(string _sql)
        {
            _sql = sqlBeautifier(_sql);

            try
            {
                if (connection.DatabaseType == Enums.DbTypes.ORACLE)
                {
                    using (OracleConnection conn = new OracleConnection(ConnectionProvider.getOracleConnectionString()))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand(_sql, conn))
                        {
                            cmd.CommandTimeout = 0;
                            return cmd.ExecuteScalar();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionProvider.getSqlConnectionString()))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(_sql, conn))
                        {
                            cmd.CommandTimeout = 0;
                            return cmd.ExecuteScalar();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException("SQL Yürütme hatası : " + Environment.NewLine + ex.Message + Environment.NewLine + _sql);
            }
        }

        public object ExecuteScalar(string _sql, DbParameter[] dbParameters)
        {
            System.Data.Common.DbParameter[] retParameters = PrepareParameters(dbParameters);
            _sql = sqlBeautifier(_sql);
            try
            {
                if (connection.DatabaseType == Enums.DbTypes.ORACLE)
                {
                    using (OracleConnection conn = new OracleConnection(ConnectionProvider.getOracleConnectionString()))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand(_sql, conn))
                        {
                            if (retParameters != null && retParameters.Length > 0)
                            {
                                cmd.Parameters.AddRange(retParameters);
                            }

                            cmd.CommandTimeout = 0;
                            return cmd.ExecuteScalar();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionProvider.getSqlConnectionString()))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(_sql, conn))
                        {
                            if (retParameters != null && retParameters.Length > 0)
                            {
                                cmd.Parameters.AddRange(retParameters);
                            }

                            cmd.CommandTimeout = 0;
                            return cmd.ExecuteScalar();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("SQL Yürütme hatası : " + Environment.NewLine + ex.Message + Environment.NewLine + _sql);
            }
        }

        public int ExecuteNonQuery(string _sql)
        {
            _sql = sqlBeautifier(_sql);

            try
            {
                if (connection.DatabaseType == Enums.DbTypes.ORACLE)
                {
                    using (OracleConnection conn = new OracleConnection(ConnectionProvider.getOracleConnectionString()))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand(_sql, conn))
                        {
                            cmd.CommandTimeout = 0;
                            return cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionProvider.getSqlConnectionString()))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(_sql, conn))
                        {
                            cmd.CommandTimeout = 0;
                            return cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("SQL Yürütme hatası : " + Environment.NewLine + ex.Message + Environment.NewLine + _sql);
            }
        }

        public int ExecuteNonQuery(string _sql, DbParameter[] dbParameters)
        {
            System.Data.Common.DbParameter[] retParameters = PrepareParameters(dbParameters);
            _sql = sqlBeautifier(_sql);
            try
            {
                if (connection.DatabaseType == Enums.DbTypes.ORACLE)
                {
                    using (OracleConnection conn = new OracleConnection(ConnectionProvider.getOracleConnectionString()))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand(_sql, conn))
                        {
                            if (retParameters != null && retParameters.Length > 0)
                            {
                                cmd.Parameters.AddRange(retParameters);
                            }

                            cmd.CommandTimeout = 0;
                            return cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionProvider.getSqlConnectionString()))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(_sql, conn))
                        {
                            if (retParameters != null && retParameters.Length > 0)
                            {
                                cmd.Parameters.AddRange(retParameters);
                            }

                            cmd.CommandTimeout = 0;
                            return cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("SQL Yürütme hatası : " + Environment.NewLine + ex.Message + Environment.NewLine + _sql);
            }
        }

        public DataTable ExecuteDataTable(string _sql)
        {
            _sql = sqlBeautifier(_sql);

            try
            {
                if (connection.DatabaseType == Enums.DbTypes.ORACLE)
                {
                    DataTable dt = new DataTable();
                    using (OracleConnection conn = new OracleConnection(ConnectionProvider.getOracleConnectionString()))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand(_sql, conn))
                        {
                            cmd.CommandTimeout = 0;

                            dt.Load(cmd.ExecuteReader());

                        }
                    }
                    return dt;
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionProvider.getSqlConnectionString()))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(_sql, conn))
                        {
                            cmd.CommandTimeout = 0;
                            DataTable dt = new DataTable();
                            dt.Load(cmd.ExecuteReader());
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("SQL Yürütme hatası : " + Environment.NewLine + ex.Message + Environment.NewLine + _sql);
            }
        }

        public async Task<DataTable> ExecuteDataTable(string _sql, CancellationToken ct)
        {
            _sql = sqlBeautifier(_sql);

            try
            {
                DataTable dt = new DataTable();
                if (connection.DatabaseType == Enums.DbTypes.ORACLE)
                {
                    using (OracleConnection conn = new OracleConnection(ConnectionProvider.getOracleConnectionString()))
                    {
                        await conn.OpenAsync();
                        using (OracleCommand cmd = new OracleCommand(_sql, conn))
                        {
                            DbDataReader readers = await cmd.ExecuteReaderAsync();
                            DataTable schemaTable = readers.GetSchemaTable();
                            foreach (DataRow dataRow in schemaTable.Rows)
                            {
                                DataColumn dataColumn = new DataColumn();
                                dataColumn.ColumnName = dataRow[0].ToString();
                                dataColumn.DataType = Type.GetType(dataRow["DataType"].ToString());
                                dt.Columns.Add(dataColumn);
                            }
                            readers.Close();
                            cmd.CommandTimeout = 0;
                            using (DbDataReader reader = await cmd.ExecuteReaderAsync(ct))
                            {
                                while (await reader.ReadAsync(ct))
                                {
                                    DataRow dataRow = dt.NewRow();
                                    for (int i = 0; i < dt.Columns.Count; i++)
                                    {
                                        dataRow[i] = reader[i];
                                    }
                                    dt.Rows.Add(dataRow);
                                }
                            }
                            return dt;
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionProvider.getSqlConnectionString()))
                    {
                        await conn.OpenAsync();
                        using (SqlCommand cmd = new SqlCommand(_sql, conn))
                        {
                            DbDataReader readers = cmd.ExecuteReader();
                            DataTable schemaTable = readers.GetSchemaTable();
                            foreach (DataRow dataRow in schemaTable.Rows)
                            {
                                DataColumn dataColumn = new DataColumn();
                                dataColumn.ColumnName = dataRow[0].ToString();
                                dataColumn.DataType = Type.GetType(dataRow["DataType"].ToString());
                                dt.Columns.Add(dataColumn);
                            }
                            readers.Close();
                            cmd.CommandTimeout = 0;
                            using (DbDataReader reader = await cmd.ExecuteReaderAsync(ct))
                            {
                                while (await reader.ReadAsync(ct))
                                {
                                    DataRow dataRow = dt.NewRow();
                                    for (int i = 0; i < dt.Columns.Count; i++)
                                    {
                                        dataRow[i] = reader[i];
                                    }
                                    dt.Rows.Add(dataRow);
                                }
                            }
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("SQL Yürütme hatası : " + Environment.NewLine + ex.Message + Environment.NewLine + _sql);
            }
        }

        public DataTable ExecuteDataTable(string _sql, DbParameter[] dbParameters)
        {
            System.Data.Common.DbParameter[] retParameters = PrepareParameters(dbParameters);
            _sql = sqlBeautifier(_sql);
            try
            {
                if (connection.DatabaseType == Enums.DbTypes.ORACLE)
                {
                    using (OracleConnection conn = new OracleConnection(ConnectionProvider.getOracleConnectionString()))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand(_sql, conn))
                        {
                            if (retParameters != null && retParameters.Length > 0)
                            {
                                cmd.Parameters.AddRange(retParameters);
                            }

                            cmd.CommandTimeout = 0;
                            DataTable dt = new DataTable();
                            dt.Load(cmd.ExecuteReader());
                            return dt;
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionProvider.getSqlConnectionString()))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(_sql, conn))
                        {
                            if (retParameters != null && retParameters.Length > 0)
                            {
                                cmd.Parameters.AddRange(retParameters);
                            }

                            cmd.CommandTimeout = 0;
                            DataTable dt = new DataTable();
                            dt.Load(cmd.ExecuteReader());
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("SQL Yürütme hatası : " + Environment.NewLine + ex.Message + Environment.NewLine + _sql);
            }
        }

        public object ExecuteTransactionalOperations(string _sql, DbParameter[] dbParameters, string scalarORnonQuery, bool lastOperation)
        {
            System.Data.Common.DbParameter[] retParameters = PrepareParameters(dbParameters);
            _sql = sqlBeautifier(_sql);

            object result = null;
            try
            {
                if (connection.DatabaseType == Enums.DbTypes.ORACLE)
                {
                    if (_sql == null && dbParameters == null && scalarORnonQuery == null && lastOperation)
                    {
                        transactionOracle.Commit();
                        transactionOracle.Dispose();
                        cmdOracle.Connection.Close();
                        connectionOracle = null;
                        cmdOracle = null;
                        transactionOracle = null;
                    }
                    else
                    {
                        if (cmdOracle == null)
                        {
                            connectionOracle = connectionProvider.getOracleConnection();
                            cmdOracle = connectionOracle.CreateCommand();
                            transactionOracle = connectionOracle.BeginTransaction();
                            cmdOracle.Transaction = transactionOracle;
                        }
                        cmdOracle.CommandText = _sql;
                        if (dbParameters != null && dbParameters.Length > 0)
                        {
                            cmdOracle.Parameters.AddRange(retParameters);
                        }
                        if (scalarORnonQuery.Equals("scalar"))
                            result = cmdOracle.ExecuteScalar();
                        else if (scalarORnonQuery.Equals("nonQuery"))
                            result = cmdOracle.ExecuteNonQuery();
                        cmdOracle.CommandText = "";
                        cmdOracle.Parameters.Clear();
                        if (lastOperation)
                        {
                            transactionOracle.Commit();
                            transactionOracle.Dispose();
                            cmdOracle.Connection.Close();
                            connectionOracle = null;
                            cmdOracle = null;
                            transactionOracle = null;
                        }
                    }
                }
                else
                {
                    if (_sql == null && dbParameters == null && scalarORnonQuery == null && lastOperation)
                    {
                        transactionSqlServer.Commit();
                        transactionSqlServer.Dispose();
                        cmdSqlServer.Connection.Close();
                        connectionSqlServer = null;
                        cmdSqlServer = null;
                        transactionSqlServer = null;
                    }
                    else
                    {
                        if (cmdSqlServer == null)
                        {
                            connectionSqlServer = connectionProvider.getSqlConnection();
                            cmdSqlServer = connectionSqlServer.CreateCommand();
                            transactionSqlServer = connectionSqlServer.BeginTransaction();
                            cmdSqlServer.Transaction = transactionSqlServer;
                        }
                        cmdSqlServer.CommandText = _sql;
                        if (dbParameters != null && dbParameters.Length > 0)
                        {
                            cmdSqlServer.Parameters.AddRange(retParameters);
                        }
                        if (scalarORnonQuery.Equals("scalar"))
                            result = cmdSqlServer.ExecuteScalar();
                        else if (scalarORnonQuery.Equals("nonQuery"))
                            result = cmdSqlServer.ExecuteNonQuery();
                        cmdSqlServer.CommandText = "";
                        cmdSqlServer.Parameters.Clear();
                        if (lastOperation)
                        {
                            transactionSqlServer.Commit();
                            transactionSqlServer.Dispose();
                            cmdSqlServer.Connection.Close();
                            connectionSqlServer = null;
                            cmdSqlServer = null;
                            transactionSqlServer = null;
                        }
                    }
                }
                return result;
            }
            catch (SqlException ex)
            {
                transactionSqlServer.Rollback();
                transactionSqlServer.Dispose();
                cmdSqlServer.Connection.Close();
                connectionSqlServer = null;
                cmdSqlServer = null;
                transactionSqlServer = null;
                return new ApplicationException("SQL Yürütme hatası : " + Environment.NewLine + ex.Message + Environment.NewLine + _sql);
            }
            catch (OracleException ex)
            {
                transactionOracle.Rollback();
                transactionOracle.Dispose();
                cmdOracle.Connection.Close();
                connectionOracle = null;
                cmdOracle = null;
                transactionOracle = null;
                return new ApplicationException("SQL Yürütme hatası : " + Environment.NewLine + ex.Message + Environment.NewLine + _sql);
            }
            catch (Exception ex)
            {
                if (connection.DatabaseType == Enums.DbTypes.ORACLE)
                {
                    connectionOracle = null;
                    cmdOracle = null;
                    transactionOracle = null;
                }
                else
                {
                    connectionSqlServer = null;
                    cmdSqlServer = null;
                    transactionSqlServer = null;
                }
                return new ApplicationException("SQL Yürütme hatası : " + Environment.NewLine + ex.Message + Environment.NewLine + _sql);
            }
        }

        #endregion Execute İşlemleri

        private string sqlBeautifier(string _sql)
        {
            if (string.IsNullOrEmpty(_sql))
            {
                return null;
            }

            if (connection.DatabaseType == Enums.DbTypes.ORACLE)
            {
                _sql = _sql.Replace("ISNULL", "NVL").Replace("SUBSTRING", "SUBSTR").Replace("+'", "||'").Replace("'+", "'||").Replace("GETDATE()", "SYSDATE").Replace("SYSDATETIME()", "SYSDATE");

                _sql = _sql.Replace("@", ":");
            }
            else
            {
                _sql = Regex.Replace(Regex.Replace(Regex.Replace(_sql, @"\bNVL\b", "ISNULL"), @"\bSUBSTR\b", "SUBSTRING"), @"\bTRIM\b", "RTRIM").Replace("||'", "+'").Replace("'||", "'+").Replace("SYSDATE", "GETDATE()");

            }

            return _sql;
        }

        private System.Data.Common.DbParameter[] PrepareParameters(DbParameter[] parameterArray)
        {
            if (parameterArray == null)
                return null;

            System.Data.Common.DbParameter[] retParameters = null;

            List<DbParameter> parameters = parameterArray.ToList();

            if (connection.DatabaseType == Enums.DbTypes.ORACLE)
            {
                retParameters = new OracleParameter[parameters.Count];
            }
            else
            {
                retParameters = new SqlParameter[parameters.Count];
            }

            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].ParamValue == null)
                {
                    parameters[i].ParamValue = DBNull.Value;
                }

                if (connection.DatabaseType == Enums.DbTypes.ORACLE)
                {
                    OracleParameter prm = new OracleParameter();
                    if (parameters[i].oraParamType.HasValue)
                    {
                        prm = new OracleParameter()
                        {
                            ParameterName = parameters[i].ParamName,
                            Value = parameters[i].ParamValue,
                            OracleDbType = parameters[i].oraParamType.Value
                        };
                    }
                    else
                    {
                        prm = new OracleParameter()
                        {
                            ParameterName = parameters[i].ParamName,
                            Value = parameters[i].ParamValue,
                            DbType = parameters[i].ParamType
                        };
                    }
                    retParameters[i] = prm;
                }
                else
                {
                    SqlParameter prm = new SqlParameter()
                    {
                        ParameterName = parameters[i].ParamName,
                        Value = parameters[i].ParamValue ?? (object)DBNull.Value,
                        DbType = parameters[i].ParamType
                    };
                    retParameters[i] = prm;
                }
            }

            return retParameters;
        }

        public void Dispose()
        {
            connectionProvider = null;
            conectionSettings = null;
            connection = null;

            connectionOracle = null;
            connectionSqlServer = null;
            cmdOracle = null;
            cmdSqlServer = null;
            transactionOracle = null;
            transactionSqlServer = null;
        }
    }

}
