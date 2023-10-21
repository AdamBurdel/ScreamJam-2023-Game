namespace CandiceAIforGames.Data
{

    public class CandiceColumnInfo
    {
        string name;
        string type;
        string defaultValue;
        bool notNull;
        bool pk;
        bool ai;

        public CandiceColumnInfo(string name, string type, string defaultValue, bool notNull, bool pk, bool ai)
        {
            this.name = name;
            this.type = type;
            this.defaultValue = defaultValue;
            this.notNull = notNull;
            this.pk = pk;
            this.ai = ai;
        }

        public string Name { get => name; set => name = value; }
        public string Type { get => type; set => type = value; }
        public string DefaultValue { get => defaultValue; set => defaultValue = value; }
        public bool NotNull { get => notNull; set => notNull = value; }
        public bool Pk { get => pk; set => pk = value; }
        public bool Ai { get => ai; set => ai = value; }
    }

}
