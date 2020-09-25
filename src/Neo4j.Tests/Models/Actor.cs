namespace Squadron.Neo4j.Models
{
    public class Actor
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Actor(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}
