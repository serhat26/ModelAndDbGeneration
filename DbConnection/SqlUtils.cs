namespace DbConnection
{
    public class SqlUtils
    {
        public static Enums.DbTypes DbTypes
        {
            get
            {
                try
                {
                    Database db = new Database();
                    return db.DbTypes;
                }
                catch
                {
                    return Enums.DbTypes.MSSQL;
                }
            }
        }
    }
}
