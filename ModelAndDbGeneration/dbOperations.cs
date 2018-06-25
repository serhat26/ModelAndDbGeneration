using DbConnection;
using System.Data;

namespace ModelAndDbGeneration
{
    class dbOperations
    {
        public DataTable GetAllTables()
        {
            string sql = "";
            if (SqlUtils.DbTypes == Enums.DbTypes.ORACLE)
            {
                sql = @"SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME";
            }
            else
            {
                sql = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' ORDER BY TABLE_NAME";
            }
            using (Database db = new Database())
            {
                return db.ExecuteDataTable(sql);
            }
        }
        public DataTable GetColumns(string tabloName)
        {
            string sql = "";
            if (SqlUtils.DbTypes == Enums.DbTypes.ORACLE)
            {
                sql = @"SELECT COLUMN_NAME FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + tabloName + "'";
            }
            else
            {
                sql = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tabloName + "'";
            }
            using (Database db = new Database())
            {
                return db.ExecuteDataTable(sql);
            }
        }
        public string GetPrimaryColumnName(string tableName)
        {
            string sql = "";
            if (SqlUtils.DbTypes == Enums.DbTypes.ORACLE)
            {
                sql = string.Format(@"SELECT UCC.COLUMN_NAME
                        FROM USER_CONSTRAINTS UC
                        JOIN USER_CONS_COLUMNS UCC ON UCC.CONSTRAINT_NAME = UC.CONSTRAINT_NAME
                        WHERE UC.TABLE_NAME = '{0}'
                            AND UC.constraint_type = 'P'
                            and UC.status = 'ENABLED'", tableName);
            }
            else
            {
                sql = string.Format(@"SELECT KU.COLUMN_NAME as PRIMARYKEYCOLUMN
                        FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC
                        INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME
                        WHERE 1=1
	                        AND TC.CONSTRAINT_TYPE = 'PRIMARY KEY' 
	                        AND KU.TABLE_NAME='{0}'", tableName);
            }
            using (Database db = new Database())
            {
                string COLUMNName = "";
                try
                {
                    COLUMNName = db.ExecuteScalar(sql).ToString();
                }
                catch 
                {
                }
                return COLUMNName;
            }
        }
        public DataTable TableInformation(string tableName)
        {
            using (Database db = new Database())
            {
                string sql = "";
                if (SqlUtils.DbTypes == Enums.DbTypes.ORACLE)
                {
                    sql = string.Format(@"SELECT UTC.COLUMN_NAME AS COLUMNNAME, UTC.DATA_TYPE AS DATATYPE
                            ,UTC.DATA_LENGTH AS COLUMNLENGTHSTRING
                            ,UTC.DATA_PRECISION AS COLUMNLENGTHINT1, UTC.DATA_SCALE AS COLUMNLENGTHINT2, UTC.NULLABLE AS COLUMNNULLABLE
                            ,MAX(UC.CONSTRAINT_TYPE) AS KEYTYPE, MAX(UCCR.TABLE_NAME) AS FOREIGNTABLENAME 
                            ,MAX(UCCR.COLUMN_NAME) AS FOREIGNCOLUMNNAME, MAX(UC.CONSTRAINT_NAME) AS FOREIGNKEYNAME
                        FROM USER_TAB_COLUMNS UTC
                        LEFT JOIN USER_CONS_COLUMNS UCC ON UCC.TABLE_NAME = UTC.TABLE_NAME AND UTC.COLUMN_NAME = UCC.column_name
                        LEFT JOIN USER_CONSTRAINTS UC ON UC.constraint_name = UCC.constraint_name 
                            AND UC.constraint_type IN ('P', 'R') and UC.status = 'ENABLED'
                        LEFT JOIN USER_CONS_COLUMNS UCCR ON UCCR.CONSTRAINT_NAME = UC.R_CONSTRAINT_NAME
                        WHERE UTC.TABLE_NAME = '{0}'
                        GROUP BY UTC.COLUMN_NAME, UTC.DATA_TYPE, UTC.DATA_LENGTH, UTC.DATA_PRECISION, UTC.DATA_SCALE, UTC.NULLABLE   
                        ORDER BY UTC.COLUMN_NAME", tableName);
                }
                else
                {
                    sql = string.Format(@"SELECT C.COLUMN_NAME  AS COLUMNNAME, C.DATA_TYPE AS DATATYPE, 
	                        C.CHARACTER_MAXIMUM_LENGTH AS COLUMNLENGTHSTRING, 
	                        C.NUMERIC_PRECISION AS COLUMNLENGTHINT1, C.NUMERIC_SCALE AS COLUMNLENGTHINT2, 
	                        CASE WHEN C.IS_NULLABLE = 'YES' THEN 'Y' ELSE 'N' END AS COLUMNNULLABLE
	                        ,CASE WHEN MAX(KCU1.CONSTRAINT_NAME) IS NOT NULL AND MAX(KCU2.TABLE_NAME) IS NULL THEN 'P' 
		                        WHEN MAX(KCU2.TABLE_NAME) IS NOT NULL THEN 'R' END AS KEYTYPE
	                        ,MAX(KCU2.TABLE_NAME) AS FOREIGNTABLENAME, MAX(KCU2.COLUMN_NAME) AS FOREIGNCOLUMNNAME
	                        ,MAX(KCU1.CONSTRAINT_NAME) AS FOREIGNKEYNAME
                        FROM INFORMATION_SCHEMA.COLUMNS C
                        LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU1 ON KCU1.TABLE_NAME = C.TABLE_NAME AND KCU1.COLUMN_NAME = C.COLUMN_NAME
                        LEFT JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS RC ON KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME 
                        LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU2 ON KCU2.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME 
                            AND KCU2.ORDINAL_POSITION = KCU1.ORDINAL_POSITION 
                        WHERE C.TABLE_NAME='{0}'
                        GROUP BY C.COLUMN_NAME, C.DATA_TYPE, C.CHARACTER_MAXIMUM_LENGTH, C.NUMERIC_PRECISION, C.NUMERIC_SCALE, C.IS_NULLABLE 
                        ORDER BY C.COLUMN_NAME", tableName);
                }

                DataTable dtList = new DataTable();
                dtList.Columns.Add("COLUMNNAME", typeof(string));
                dtList.Columns.Add("DATATYPE", typeof(string));
                dtList.Columns.Add("COLUMNLENGTHSTRING", typeof(int));
                dtList.Columns.Add("COLUMNLENGTHINT1", typeof(int));
                dtList.Columns.Add("COLUMNLENGTHINT2", typeof(int));
                dtList.Columns.Add("COLUMNNULLABLE", typeof(bool));
                dtList.Columns.Add("PRIMARYKEY", typeof(bool));
                dtList.Columns.Add("FOREIGNKEYNAME", typeof(string));
                dtList.Columns.Add("FOREIGNKEYTABLECOLUMN", typeof(string));
                dtList.Columns.Add("WHEREFORUPDATE", typeof(bool));

                if (!string.IsNullOrEmpty(tableName))
                {
                    DataTable dt = db.ExecuteDataTable(sql);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        string columnName = CommonHelper.Nvl(dr["COLUMNNAME"]);
                        string dataType = CommonHelper.Nvl(dr["DATATYPE"]);
                        int columnLengthString = CommonHelper.ToInt32(dr["COLUMNLENGTHSTRING"]);
                        int columnLengthInt1 = CommonHelper.ToInt32(dr["COLUMNLENGTHINT1"]);
                        if (columnLengthInt1 > 0)
                        {
                            columnLengthString = 0;
                        }
                        int columnLengthInt2 = CommonHelper.ToInt32(dr["COLUMNLENGTHINT2"]);
                        bool columnNullable = false;
                        if (string.Equals(CommonHelper.Nvl(dr["COLUMNNULLABLE"]), "Y"))
                            columnNullable = true;
                        bool primaryKey = false;
                        if (string.Equals(CommonHelper.Nvl(dr["KEYTYPE"]), "P"))
                            primaryKey = true;

                        string ForeignKeyName = CommonHelper.Nvl(dr["FOREIGNKEYNAME"]);
                        string ForeignKeyTableColumn = "";
                        if (primaryKey == false && !string.IsNullOrEmpty(CommonHelper.Nvl(dr["FOREIGNTABLENAME"])))
                        {
                            ForeignKeyTableColumn = CommonHelper.Nvl(dr["FOREIGNTABLENAME"]) + "(" + CommonHelper.Nvl(dr["FOREIGNCOLUMNNAME"]) + ")";
                        }

                        bool whereOfrUpd = false;
                        if (primaryKey || !string.IsNullOrEmpty(CommonHelper.Nvl(dr["FOREIGNTABLENAME"])))
                        {
                            whereOfrUpd = true;
                        }

                        dtList.Rows.Add(new object[] { columnName, dataType, columnLengthString, columnLengthInt1, columnLengthInt2,
                            columnNullable, primaryKey, ForeignKeyName, ForeignKeyTableColumn, whereOfrUpd });
                    }
                }
                return dtList;
            }
        }

        public DataTable IndexInformation(string tableName)
        {
            using (Database db = new Database())
            {
                string sql = "";
                if (SqlUtils.DbTypes == Enums.DbTypes.ORACLE)
                {
                    sql = string.Format(@"SELECT INDEX_NAME AS INDEXNAME, 
                                LISTAGG(COLUMN_NAME, ',') WITHIN GROUP (ORDER BY COLUMN_NAME) AS INDEXCOLUMNS 
                            FROM USER_IND_COLUMNS
                            WHERE TABLE_NAME = '{0}'
                            GROUP BY INDEX_NAME", tableName);
                }
                else
                {
                    sql = string.Format(@"SELECT DISTINCT ind2.name AS INDEXNAME,
                                STUFF(
                                        (SELECT ', ' + col.name
                                        FROM sys.indexes ind 
			                            INNER JOIN sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
			                            INNER JOIN sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
			                            INNER JOIN sys.tables t ON ind.object_id = t.object_id 
			                            WHERE 1=1
				                                AND ind.is_unique_constraint = 0 
				                                AND t.is_ms_shipped = 0 
				                                AND t.name = '{0}'
				                                AND ind.name = ind2.name
                                        FOR XML PATH (''))
                                        , 1, 1, '')  AS INDEXCOLUMNS
                            FROM sys.indexes ind2 
                            INNER JOIN sys.index_columns ic ON  ind2.object_id = ic.object_id and ind2.index_id = ic.index_id 
                            INNER JOIN sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
                            INNER JOIN sys.tables t ON ind2.object_id = t.object_id 
                            WHERE 1=1
		                            AND ind2.is_unique_constraint = 0 
		                            AND t.is_ms_shipped = 0 
		                            AND t.name = '{1}'", tableName, tableName);
                }

                DataTable dtList = new DataTable();
                dtList.Columns.Add("INDEXNAME", typeof(string));
                dtList.Columns.Add("INDEXCOLUMNS", typeof(string));
                if (!string.IsNullOrEmpty(tableName))
                {
                    DataTable dt = db.ExecuteDataTable(sql);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        string indexName = CommonHelper.Nvl(dr["INDEXNAME"]);
                        string indexColumns = CommonHelper.Nvl(dr["INDEXCOLUMNS"]);
                        dtList.Rows.Add(new object[] { indexName, indexColumns });
                    }
                }
                return dtList;
            }
        }
    }
}
