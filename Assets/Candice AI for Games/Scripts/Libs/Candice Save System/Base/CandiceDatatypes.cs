using System.Collections.Generic;

namespace CandiceAIforGames.Data
{
    public class CandiceDatatypes
    {
        public static List<string> dataTypes = GetDataTypes();
        public const string TYPE_BOOLEAN = "BOOLEAN";
        public const string TYPE_CHAR = "CHAR";
        public const string TYPE_DATE = "DATE";
        public const string TYPE_DATETIME = "DATETIME";
        public const string TYPE_DECIMAL = "DECIMAL";
        public const string TYPE_DOUBLE = "DOUBLE";
        public const string TYPE_INTEGER = "INTEGER";
        public const string TYPE_NUMERIC = "NUMERIC";
        public const string TYPE_REAL = "REAL";
        public const string TYPE_STRING = "STRING";
        public const string TYPE_TEXT = "TEXT";
        public const string TYPE_TIME = "TIME";
        public const string TYPE_VARCHAR = "VARCHAR";
        private static List<string> GetDataTypes()
        {

            List<string> dataTypes = new List<string>();
            dataTypes.Add(TYPE_BOOLEAN);
            dataTypes.Add(TYPE_CHAR);
            dataTypes.Add(TYPE_DATE);
            dataTypes.Add(TYPE_DATETIME);
            dataTypes.Add(TYPE_DECIMAL);
            dataTypes.Add(TYPE_DOUBLE);
            dataTypes.Add(TYPE_INTEGER);
            dataTypes.Add(TYPE_NUMERIC);
            dataTypes.Add(TYPE_REAL);
            dataTypes.Add(TYPE_STRING);
            dataTypes.Add(TYPE_TEXT);
            dataTypes.Add(TYPE_TIME);
            dataTypes.Add(TYPE_VARCHAR);

            return dataTypes;
        }
    }

}
