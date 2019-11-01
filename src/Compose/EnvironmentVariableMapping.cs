namespace Squadron
{
    public class EnvironmentVariableMapping
    {
        public EnvironmentVariableMapping(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Name}={Value}";
        }
    }

}
