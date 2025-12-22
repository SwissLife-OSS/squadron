namespace Squadron.Neo4j.Models;

public class Actor(string name, int age)
{
    public string Name { get; set; } = name;
    public int Age { get; set; } = age;
}