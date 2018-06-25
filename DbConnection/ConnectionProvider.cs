using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace DbConnection
{
    public class ConnectionProvider
    {
        static string filename = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "") + "\\dbconfig.dll";
        static string tempfilename = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "") + "\\dbconfigtemp.dll";

        public static bool isConfigFileExists()
        {
            return System.IO.File.Exists(filename);
        }
        public void SaveDbConnectionSettings(ConnectionSettings settings)
        {
            try
            {
                string ayar = "{\"dbType\":\"" + settings.dbType + "\",\"server\":\"" + settings.server + "\",\"database\":\"" + settings.database + "\",\"dsn\":\"" + settings.dsn + "\",\"uid\":\"" + settings.uid + "\",\"pwd\":\"" + settings.pwd + "\"}";

                string connectionString = Security.Encrypt(ayar);

                System.IO.File.Delete(filename);
                System.IO.File.WriteAllText(filename, connectionString, Encoding.UTF8);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public static ConnectionSettings LoadDbConnectionSettings()
        {
            try
            {
                ConnectionSettings settings = new ConnectionSettings();

                if (!isConfigFileExists()) return settings;
                FileStream fsCrypt = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                System.IO.StreamReader reader = new System.IO.StreamReader(fsCrypt);
                string ayar = reader.ReadToEnd();
                fsCrypt.Close();
                if (ayar == "")
                {
                    throw new ApplicationException("Connection settings error!");
                }
                string connStr = Security.Decrypt(ayar);
                try
                {
                    settings = JsonParser<ConnectionSettings>.getClass(connStr);
                    return settings;
                }
                catch (Exception)
                {
                    throw new ApplicationException("Connection settings error!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SqlConnection getSqlConnection()
        {
            SqlConnection sqlConnection = null;
            ConnectionSettings settings = LoadDbConnectionSettings();

            if (settings.uid == "")
            {
                throw new ApplicationException("Connection settings error !");
            }
            string connstr =
                "Data Source= " + settings.server +
                "; Initial Catalog= " + settings.database +
                "; User Id = " + settings.uid +
                "; Password = " + settings.pwd +
                ";MultipleActiveResultSets=true" +
                ";pooling=true" + ";connection lifetime=120" + ";max pool size=250;";
            sqlConnection.Open();
            return sqlConnection;
        }

        public OracleConnection getOracleConnection()
        {
            OracleConnection oraConn = null;
            ConnectionSettings settings = LoadDbConnectionSettings();

            if (settings.uid == "")
            {
                throw new ApplicationException("Connection settings error !");
            }
            string constr = "User Id=" + settings.uid + ";Password=" + settings.pwd + ";data source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + settings.server + ")(PORT = 1521))(CONNECT_DATA =(SID = " + settings.database + ")))";

            oraConn = new OracleConnection(constr);

            oraConn.Open();

            return oraConn;
        }

        public static OdbcConnection getODBCConnection()
        {
            OdbcConnection connec = null;
            try
            {
                ConnectionSettings settings = LoadDbConnectionSettings();
                if (String.IsNullOrEmpty(settings.dsn))
                {
                    frmDBAyarlar frm = new frmDBAyarlar();
                    frm.ShowDialog();
                    frm.Dispose();
                    getODBCConnection();
                }
                string constr = "DSN=" + settings.dsn + ";UID=" + settings.uid + ";PWD=" + settings.pwd + ";";
                connec = new OdbcConnection(constr);
                connec.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Error: " + ex.Message);
            }
            return connec;
        }

        public ConnectionSettings getConnectionParameters()
        {
            ConnectionSettings settings = LoadDbConnectionSettings();

            if (settings.uid == "")
            {
                throw new ApplicationException("Connection settings error !");
            }

            return settings;
        }

        public static string getSqlConnectionString()
        {
            ConnectionSettings settings = LoadDbConnectionSettings();

            if (settings.uid == "")
            {
                throw new ApplicationException("Connection settings error !");
            }

            string constr =
                "Data Source= " + settings.server +
                "; Initial Catalog= " + settings.database +
                "; User Id = " + settings.uid +
                "; Password = " + settings.pwd +
                ";MultipleActiveResultSets=true" +
                ";pooling=true" + ";connection lifetime=120" + ";max pool size=250;";

            return constr;
        }

        public static string getOracleConnectionString()
        {
            ConnectionSettings settings = LoadDbConnectionSettings();

            if (settings.uid == "")
            {
                throw new ApplicationException("Connection settings error !");
            }

            string constr = "User Id=" + settings.uid + ";Password=" + settings.pwd + ";data source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + settings.server + ")(PORT = 1521))(CONNECT_DATA =(SID = " + settings.database + ")))";

            return constr;
        }
    }
}
