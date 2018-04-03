namespace Camaleao.Core.Entities
{
    public class Variable
    {
        public string Name { get; set; }
        public string Initialize { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

        public string GetValue()
        {
            if ("text".Equals(this.Type.ToLower()))
                return $"'{Value}'";
            return Value;
        }


    }
}
