namespace CandiceAIforGames.Data
{

    public class CandiceTypeAttribute
    {
        public string name;
        public string type;
        public string modifier;
        public bool hasGetter;
        public bool hasSetter;

        public CandiceTypeAttribute(string name, string type, string modifier, bool hasGetter, bool hasSetter)
        {
            this.name = name;
            this.type = type;
            this.modifier = modifier;
            this.hasGetter = hasGetter;
            this.hasSetter = hasSetter;
        }
    }

}
