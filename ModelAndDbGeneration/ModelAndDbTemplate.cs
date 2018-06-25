using System;
using System.Collections.Generic;
using System.Data;

namespace ModelAndDbGeneration
{
    public static class ModelAndDbTemplate
    {
        public static string modeltemplate(string namespacePath, string tableName, List<ParameterSpeciality> parameters)
        {
            List<string> modelParams = new List<string>();
            List<string> equalsParams = new List<string>();
            List<string> hashCodeParams = new List<string>();

            foreach (var prm in parameters)
            {
                if (prm.dbType.Equals(DbType.String))
                {
                    modelParams.Add("public string " + prm.ColumnName + " { get; set; }");
                    equalsParams.Add("string.Equals(" + prm.ColumnName + ", other." + prm.ColumnName + ")");
                    hashCodeParams.Add("hashCode = (hashCode * 397) ^ (" + prm.ColumnName + " != null ? " + prm.ColumnName + ".GetHashCode() : 0);");
                }
                if (prm.dbType.Equals(DbType.Decimal))
                {
                    modelParams.Add("public decimal " + prm.ColumnName + " { get; set; }");
                    equalsParams.Add(prm.ColumnName + " == other." + prm.ColumnName);
                    hashCodeParams.Add("hashCode = (hashCode * 397) ^ " + prm.ColumnName + ".GetHashCode();");
                }
                if (prm.dbType.Equals(DbType.Int64))
                {
                    modelParams.Add("public long " + prm.ColumnName + " { get; set; }");
                    equalsParams.Add(prm.ColumnName + " == other." + prm.ColumnName);
                    hashCodeParams.Add("hashCode = (hashCode * 397) ^ " + prm.ColumnName + ".GetHashCode();");
                }
                if (prm.dbType.Equals(DbType.Int32))
                {
                    modelParams.Add("public int " + prm.ColumnName + " { get; set; }");
                    equalsParams.Add(prm.ColumnName + " == other." + prm.ColumnName);
                    hashCodeParams.Add("hashCode = (hashCode * 397) ^ " + prm.ColumnName + ".GetHashCode();");
                }
                if (prm.dbType.Equals(DbType.Int16))
                {
                    modelParams.Add("public short " + prm.ColumnName + " { get; set; }");
                    equalsParams.Add(prm.ColumnName + " == other." + prm.ColumnName);
                    hashCodeParams.Add("hashCode = (hashCode * 397) ^ " + prm.ColumnName + ".GetHashCode();");
                }
                if (prm.dbType.Equals(DbType.DateTime))
                {
                    modelParams.Add("public DateTime " + prm.ColumnName + " { get; set; }");
                    equalsParams.Add(prm.ColumnName + ".Equals(other." + prm.ColumnName + ")");
                    hashCodeParams.Add("hashCode = (hashCode * 397) ^ " + prm.ColumnName + ".GetHashCode();");
                }
                if (prm.dbType.Equals(DbType.Binary))
                {
                    modelParams.Add("public byte[] " + prm.ColumnName + " { get; set; }");
                    equalsParams.Add(prm.ColumnName + ".SequenceEqual(other." + prm.ColumnName + ")");
                    hashCodeParams.Add("hashCode = (hashCode * 397) ^ " + prm.ColumnName + ".GetHashCode();");
                }
            }

            string modelParamsStr = string.Join("\n", modelParams);
            string equalsParamsStr = "return " + string.Join(" && ", equalsParams) + ";";
            string hashCodeParamsStr = string.Join("\n", hashCodeParams);

            string template = @"using DbConnection;
                            using System.Linq;
                            using System;

                            namespace " + namespacePath + "." + tableName + @"
                            {
                                public class " + tableName + @" : ICloneable
                                {
                                    " + modelParamsStr + @"
        
                                    public object Clone()
                                    {
                                        return this.MemberwiseClone();
                                    }

                                    protected bool Equals(" + tableName + @" other)
                                    {
                                        " + equalsParamsStr + @"
                                    }

                                    public override bool Equals(object obj)
                                    {
                                        if (ReferenceEquals(null, obj)) return false;
                                        if (ReferenceEquals(this, obj)) return true;
                                        if (obj.GetType() != this.GetType()) return false;
                                        return Equals((" + tableName + @") obj);
                                    }

                                    public override int GetHashCode()
                                    {
                                        unchecked
                                        {
                                            int hashCode = 0;
                                            " + hashCodeParamsStr + @"
                
                                            return hashCode;
                                        }
                                    }
                                }
                            }";
            return template;
        }
        public static string dbtemplate(string namespacePath, string tableName, List<ParameterSpeciality> parameters, 
            List<ParameterSpeciality> whereList)
        {
            string newLine = Environment.NewLine;
            string tableObj = char.ToLowerInvariant(tableName[0]) + tableName.Substring(1);
            
            List<string> selectList = new List<string>();
            List<string> selectWithAtList = new List<string>();
            List<string> updateStrList = new List<string>();
            List<string> dtStartList = new List<string>();
            List<string> dbReturnList = new List<string>();
            List<string> dbModelList = new List<string>();
            List<string> insUpdPrmList = new List<string>();
            ParameterSpeciality primaryColumn = new ParameterSpeciality();

            foreach (var prm in parameters)
            {
                if (prm.PrimaryKey)
                {
                    primaryColumn = prm;
                }

                selectList.Add(prm.ColumnName);
                selectWithAtList.Add("@" + prm.ColumnName);
                updateStrList.Add(prm.ColumnName + " = @" + prm.ColumnName);

                if (prm.dbType.Equals(DbType.String))
                {
                    dtStartList.Add("dtList.Columns.Add(\"" + prm.ColumnName + "\", typeof(string));");
                    dbReturnList.Add("string " + prm.ColumnName + " = CommonHelper.Nvl(dt.Rows[i][\"" + prm.ColumnName + "\"]);");
                    insUpdPrmList.Add("parameters.Add(new DbParameter(\"" + prm.ColumnName + "\", DbType.String, " + tableObj + "." + prm.ColumnName + "));");
                    dbModelList.Add(tableObj + "." + prm.ColumnName + " = CommonHelper.Nvl(dr[\"" + prm.ColumnName + "\"]);");
                }
                if (prm.dbType.Equals(DbType.Decimal))
                {
                    dtStartList.Add("dtList.Columns.Add(\"" + prm.ColumnName + "\", typeof(decimal));");
                    dbReturnList.Add("decimal " + prm.ColumnName + " = CommonHelper.ToDecimal(dt.Rows[i][\"" + prm.ColumnName + "\"]);");
                    insUpdPrmList.Add("parameters.Add(new DbParameter(\"" + prm.ColumnName + "\", DbType.Decimal, " + tableObj + "." + prm.ColumnName + "));");
                    dbModelList.Add(tableObj + "." + prm.ColumnName + " = CommonHelper.ToDecimal(dr[\"" + prm.ColumnName + "\"]);");
                }   
                if (prm.dbType.Equals(DbType.Int64))
                {
                    dtStartList.Add("dtList.Columns.Add(\"" + prm.ColumnName + "\", typeof(long));");
                    dbReturnList.Add("long " + prm.ColumnName + " = CommonHelper.ToInt64(dt.Rows[i][\"" + prm.ColumnName + "\"]);");
                    insUpdPrmList.Add("parameters.Add(new DbParameter(\"" + prm.ColumnName + "\", DbType.Int64, " + tableObj + "." + prm.ColumnName + "));");
                    dbModelList.Add(tableObj + "." + prm.ColumnName + " = CommonHelper.ToInt64(dr[\"" + prm.ColumnName + "\"]);");
                }
                if (prm.dbType.Equals(DbType.Int32))
                {
                    dtStartList.Add("dtList.Columns.Add(\"" + prm.ColumnName + "\", typeof(int));");
                    dbReturnList.Add("int " + prm.ColumnName + " = CommonHelper.ToInt32(dt.Rows[i][\"" + prm.ColumnName + "\"]);");
                    insUpdPrmList.Add("parameters.Add(new DbParameter(\"" + prm.ColumnName + "\", DbType.Int32, " + tableObj + "." + prm.ColumnName + "));");
                    dbModelList.Add(tableObj + "." + prm.ColumnName + " = CommonHelper.ToInt32(dr[\"" + prm.ColumnName + "\"]);");
                }
                if (prm.dbType.Equals(DbType.Int16))
                {
                    dtStartList.Add("dtList.Columns.Add(\"" + prm.ColumnName + "\", typeof(bool));");
                    dbReturnList.Add("bool " + prm.ColumnName + " = Convert.ToBoolean(dt.Rows[i][\"" + prm.ColumnName + "\"]);");
                    insUpdPrmList.Add("parameters.Add(new DbParameter(\"" + prm.ColumnName + "\", DbType.Int16, " + tableObj + "." + prm.ColumnName + "));");
                    dbModelList.Add(tableObj + "." + prm.ColumnName + " = CommonHelper.ToInt16(dr[\"" + prm.ColumnName + "\"]);");
                }
                if (prm.dbType.Equals(DbType.DateTime))
                {
                    dtStartList.Add("dtList.Columns.Add(\"" + prm.ColumnName + "\", typeof(DateTime));");
                    dbReturnList.Add("DateTime " + prm.ColumnName + " = CommonHelper.ToDateTime(dt.Rows[i][\"" + prm.ColumnName + "\"]);");
                    insUpdPrmList.Add("parameters.Add(new DbParameter(\"" + prm.ColumnName + "\", DbType.DateTime, " + tableObj + "." + prm.ColumnName + "));");
                    dbModelList.Add(tableObj + "." + prm.ColumnName + " = CommonHelper.ToDateTime(dr[\"" + prm.ColumnName + "\"]);");
                }
                if (prm.dbType.Equals(DbType.Binary))
                {
                    dtStartList.Add("dtList.Columns.Add(\"" + prm.ColumnName + "\", typeof(byte[]));");
                    dbReturnList.Add("byte[] " + prm.ColumnName + " = CommonHelper.ToByteArray(dt.Rows[i][\"" + prm.ColumnName + "\"]);");
                    insUpdPrmList.Add("parameters.Add(new DbParameter(\"" + prm.ColumnName + "\", DbType.Binary, " + tableObj + "." + prm.ColumnName + "));");
                    dbModelList.Add(tableObj + "." + prm.ColumnName + " = CommonHelper.ToByteArray(dr[\"" + prm.ColumnName + "\"]);");
                }
            }

            string template = @"using DbConnection;
                            using System;
                            using System.Data;
                            using System.Collections.Generic;

                            namespace " + namespacePath + "." + tableName + @"
                            {
                                public class dbOperations" + tableName + @"
                                { 
                                    public DataTable get" + tableName + @"List()
                                    {
                                        DataTable dtList = new DataTable();
                                    ";

                                        template = template + "string sql = @\"SELECT " + string.Join(", ", selectList) + @"
                                                 FROM " + tableName.ToUpperInvariant() + "\"" + @";
                                        
                                        using (Database db = new Database())
                                        {
                                            DataTable dt = db.ExecuteDataTable(sql);
                                            " + string.Join(newLine, dtStartList) + @"

                                            for (int i = 0; i < dt.Rows.Count; i++)
                                            {
                                                " + string.Join(newLine, dbReturnList) + @"

                                                dtList.Rows.Add(new object[] { " + string.Join(", ", selectList) + @" });
                                            }
                                            return dtList;
                                        }
                                    }
                                    ";

                                    foreach (var whereObj in whereList)
                                    {
                                        if (whereObj.PrimaryKey)
                                        {
                                            template = template + @"
                                            public " + tableName + " get" + tableName + "By" + whereObj.ColumnName + "(" +
                                            whereObj.dbType.ToString() + " " + whereObj.ColumnName + @")
                                            {
                                                " + "string sql = @\"SELECT " + string.Join(", ", selectList) + @"
                                                            FROM " + tableName.ToUpperInvariant() + @" 
                                                            WHERE  " + whereObj.ColumnName + " = @" + whereObj.ColumnName + "\";" + @"

                                                List<DbParameter> parameters = new List<DbParameter>();
                                                ";
                                                string dbWhereParemeterPrimaryKey = "";
                                                if (whereObj.dbType.Equals(DbType.String))
                                                    dbWhereParemeterPrimaryKey = dbWhereParemeterPrimaryKey + "parameters.Add(new DbParameter(\"" + 
                                                        whereObj.ColumnName + "\", DbType.String, " + whereObj.ColumnName + "));" + newLine;
                                                if (whereObj.dbType.Equals(DbType.Decimal))
                                                    dbWhereParemeterPrimaryKey = dbWhereParemeterPrimaryKey + "parameters.Add(new DbParameter(\"" +
                                                        whereObj.ColumnName + "\", DbType.Decimal, " + whereObj.ColumnName + "));" + newLine;
                                                if (whereObj.dbType.Equals(DbType.Int64))
                                                    dbWhereParemeterPrimaryKey = dbWhereParemeterPrimaryKey + "parameters.Add(new DbParameter(\"" +
                                                        whereObj.ColumnName + "\", DbType.Int64, " + whereObj.ColumnName + "));" + newLine;
                                                if (whereObj.dbType.Equals(DbType.Int32))
                                                    dbWhereParemeterPrimaryKey = dbWhereParemeterPrimaryKey + "parameters.Add(new DbParameter(\"" +
                                                        whereObj.ColumnName + "\", DbType.Int32, " + whereObj.ColumnName + "));" + newLine;
                                                if (whereObj.dbType.Equals(DbType.Int16))
                                                    dbWhereParemeterPrimaryKey = dbWhereParemeterPrimaryKey + "parameters.Add(new DbParameter(\"" +
                                                        whereObj.ColumnName + "\", DbType.Int16, " + whereObj.ColumnName + "));" + newLine;
                                                if (whereObj.dbType.Equals(DbType.DateTime))
                                                    dbWhereParemeterPrimaryKey = dbWhereParemeterPrimaryKey + "parameters.Add(new DbParameter(\"" +
                                                        whereObj.ColumnName + "\", DbType.DateTime, " + whereObj.ColumnName + "));" + newLine;
                                                if (whereObj.dbType.Equals(DbType.Binary))
                                                    dbWhereParemeterPrimaryKey = dbWhereParemeterPrimaryKey + "parameters.Add(new DbParameter(\"" +
                                                        whereObj.ColumnName + "\", DbType.Binary, " + whereObj.ColumnName + "));" + newLine;

                                                template = template + dbWhereParemeterPrimaryKey + @"
                                        
                                                using (Database db = new Database())
                                                {
                                                    DataTable dt = db.ExecuteDataTable(sql, parameters.ToArray());
                                                    if (dt.Rows.Count > 0)
                                                    {
                                                        DataRow dr = dt.Rows[0];
                                                        " + tableName + " " + tableObj + " = new " + tableName + @"();
                                                        " + string.Join(newLine, dbModelList) + @"

                                                        return " + tableObj + @"; 
                                                    }
                                                }
                                                return null;
                                            }" + newLine;
                                        }

                                        template = template + @"
                                        public DataTable get" + tableName + "ListBy" + whereObj.ColumnName + "(" + 
                                            whereObj.dbType.ToString() + " " + whereObj.ColumnName + @")
                                        {
                                            " + "string sql = @\"SELECT " + string.Join(", ", selectList) + @"
                                                        FROM " + tableName.ToUpperInvariant() + @" 
                                                        WHERE  " + whereObj.ColumnName + " = @" + whereObj.ColumnName + "\";" + @"

                                            List<DbParameter> parameters = new List<DbParameter>();
                                            ";
                                        string dbWhereParemeter = "";
                                        if (whereObj.dbType.Equals(DbType.String))
                                            dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName + "\", DbType.String, " + whereObj.ColumnName + "));" + newLine;
                                        if (whereObj.dbType.Equals(DbType.Decimal))
                                            dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName + "\", DbType.Decimal, " + whereObj.ColumnName + "));" + newLine;
                                        if (whereObj.dbType.Equals(DbType.Int64))
                                            dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName + "\", DbType.Int64, " + whereObj.ColumnName + "));" + newLine;
                                        if (whereObj.dbType.Equals(DbType.Int32))
                                            dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName + "\", DbType.Int32, " + whereObj.ColumnName + "));" + newLine;
                                        if (whereObj.dbType.Equals(DbType.Int16))
                                            dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName + "\", DbType.Int16, " + whereObj.ColumnName + "));" + newLine;
                                        if (whereObj.dbType.Equals(DbType.DateTime))
                                            dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName + "\", DbType.DateTime, " + whereObj.ColumnName + "));" + newLine;
                                        if (whereObj.dbType.Equals(DbType.Binary))
                                            dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" +
                                                whereObj.ColumnName + "\", DbType.Binary, " + whereObj.ColumnName + "));" + newLine;

                                        template = template + dbWhereParemeter + @"
                                        
                                            using (Database db = new Database())
                                            {
                                                return db.ExecuteDataTable(sql, parameters.ToArray());
                                            }
                                        }" + newLine;
                                    }
                                          
                                    template = template + @"
                                    public bool " + tableName + "Insert(" + tableName + " " + tableObj + @") 
                                    {
                                        " + "string sql = @\"INSERT INTO " + tableName.ToUpperInvariant() + @"
                                            (" + string.Join(", ", selectList) + @") 
                                            VALUES (" + string.Join(", ", selectWithAtList) + ")\";" + @"

                                        List<DbParameter> parameters = new List<DbParameter>();
                                        ";

                                        template = template + string.Join(newLine, insUpdPrmList) + newLine + @"
                                        using (Database db = new Database())
                                        {
                                            return db.ExecuteNonQuery(sql, parameters.ToArray()) > 0;
                                        }
                                            
                                    }" + newLine; 
                                    
                                    foreach (var whereObj in whereList)
                                    {
                                        template = template + @"
                                        public bool " + tableName + "UpdateBy" + whereObj.ColumnName + "(" + tableName + " " + tableObj + "," +
                                            whereObj.dbType.ToString() + " " + whereObj.ColumnName + @")
                                        {
                                            ";
                                            List<string> updateStrListOzel = new List<string>();
                                            foreach (var updateStr in updateStrList)
                                            {
                                                if (!updateStr.Substring(0, updateStr.IndexOf(" =")).Equals(whereObj.ColumnName))
                                                {
                                                    updateStrListOzel.Add(updateStr);
                                                }
                                            }

                                            List<string> insUpdPrmListOzel = new List<string>();
                                            foreach (var insUpdPrm in insUpdPrmList)
                                            {
                                                if (!insUpdPrm.Contains("\"" + whereObj.ColumnName + "\""))
                                                {
                                                    insUpdPrmListOzel.Add(insUpdPrm);
                                                }
                                            }

                                            template = template + "string sql = @\"UPDATE " + tableName.ToUpperInvariant() + 
                                                        " SET " + string.Join(", ", updateStrListOzel) + @" 
                                                        WHERE  " + whereObj.ColumnName + " = @" + whereObj.ColumnName + "\";" + @"

                                            List<DbParameter> parameters = new List<DbParameter>();
                                            ";

                                            string dbWhereParemeter = "";
                                            if (whereObj.dbType.Equals(DbType.String))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName + 
                                                    "\", DbType.String, " + whereObj.ColumnName + "));" + newLine;
                                            if (whereObj.dbType.Equals(DbType.Decimal))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName + 
                                                    "\", DbType.Decimal, " + whereObj.ColumnName + "));" + newLine;
                                            if (whereObj.dbType.Equals(DbType.Int64))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName + 
                                                    "\", DbType.Int64, " + whereObj.ColumnName + "));" + newLine;
                                            if (whereObj.dbType.Equals(DbType.Int32))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName + 
                                                    "\", DbType.Int32, " + whereObj.ColumnName + "));" + newLine;
                                            if (whereObj.dbType.Equals(DbType.Int16))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName + 
                                                    "\", DbType.Int16, " + whereObj.ColumnName + "));" + newLine;
                                            if (whereObj.dbType.Equals(DbType.DateTime))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName + 
                                                    "\", DbType.DateTime, " + whereObj.ColumnName + "));" + newLine;
                                            if (whereObj.dbType.Equals(DbType.Binary))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" +
                                                    whereObj.ColumnName + "\", DbType.Binary, " + whereObj.ColumnName + "));" + newLine;

                                            template = template + string.Join(newLine, insUpdPrmListOzel) + newLine;
                                            template = template + dbWhereParemeter + @"

                                            using (Database db = new Database())
                                            {
                                                return db.ExecuteNonQuery(sql, parameters.ToArray()) > 0;
                                            }
                                        }" + newLine;
                                    }

                                    foreach (var whereObj in whereList)
                                    {
                                        template = template + @"
                                        public bool " + tableName + "DeleteBy" + whereObj.ColumnName + "(" +
                                            whereObj.dbType.ToString() + " " + whereObj.ColumnName + @")
                                        {
                                            ";

                                            template = template + "string sql = @\"DELETE FROM " + tableName.ToUpperInvariant() +
                                                     " WHERE  " + whereObj.ColumnName + " = @" + whereObj.ColumnName + "\";" + @"

                                            List<DbParameter> parameters = new List<DbParameter>();
                                            ";

                                            string dbWhereParemeter = "";
                                            if (whereObj.dbType.Equals(DbType.String))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName +
                                                    "\", DbType.String, " + whereObj.ColumnName + "));" + newLine;
                                            if (whereObj.dbType.Equals(DbType.Decimal))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName +
                                                    "\", DbType.Decimal, " + whereObj.ColumnName + "));" + newLine;
                                            if (whereObj.dbType.Equals(DbType.Int64))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName +
                                                    "\", DbType.Int64, " + whereObj.ColumnName + "));" + newLine;
                                            if (whereObj.dbType.Equals(DbType.Int32))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName +
                                                    "\", DbType.Int32, " + whereObj.ColumnName + "));" + newLine;
                                            if (whereObj.dbType.Equals(DbType.Int16))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName +
                                                    "\", DbType.Int16, " + whereObj.ColumnName + "));" + newLine;
                                            if (whereObj.dbType.Equals(DbType.DateTime))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" + whereObj.ColumnName +
                                                    "\", DbType.DateTime, " + whereObj.ColumnName + "));" + newLine;
                                            if (whereObj.dbType.Equals(DbType.Binary))
                                                dbWhereParemeter = dbWhereParemeter + "parameters.Add(new DbParameter(\"" +
                                                    whereObj.ColumnName + "\", DbType.Binary, " + whereObj.ColumnName + "));" + newLine;

                                            template = template + dbWhereParemeter + @"

                                            using (Database db = new Database())
                                            {
                                                return db.ExecuteNonQuery(sql, parameters.ToArray()) > 0;
                                            }
                                        }" + newLine;
                                    }

                                    template = template + @"
                                }
                            }";

            return template;
        }
    }
}
