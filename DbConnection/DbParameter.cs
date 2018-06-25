using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace DbConnection
{
    public class DbParameter
    {
        public string ParamName { get; set; }
        public DbType ParamType { get; set; }
        public OracleDbType? oraParamType { get; set; }
        public object ParamValue { get; set; }
        public ParameterDirection ParamDirection { get; set; }
        public DbParameter()
        {
        }
        public DbParameter(string name, DbType type, OracleDbType? oraParamType, object value, ParameterDirection paramDirection)
        {
            this.ParamName = name;
            this.ParamType = type;
            this.ParamValue = value;
            this.ParamDirection = paramDirection;
        }
        public DbParameter(string name, DbType type, object value)
        {
            this.ParamName = name;
            this.ParamType = type;
            this.ParamValue = value;
        }
        public DbParameter(string name, DbType type, OracleDbType oraParamType, object value)
        {
            this.ParamName = name;
            this.oraParamType = oraParamType;
            this.ParamValue = value;
        }
        public DbParameter(string name, object value)
        {
            this.ParamName = name;
            this.ParamValue = value;
        }
    }
}
