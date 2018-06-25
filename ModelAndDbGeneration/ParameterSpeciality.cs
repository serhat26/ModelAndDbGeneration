using System.Data;

namespace ModelAndDbGeneration
{
    public class ParameterSpeciality
    {
        public DbType dbType { get; set; }
        public string ColumnName { get; set; }
        public bool PrimaryKey { get; set; } = false;
        
    }
}
