namespace Squadron.Samples.Shared
{
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public static User CreateSample()
        {
            return new User
            {
                Id = "A1",
                Name = "John",
                Email = "john.sample@squadron.io"
            };
        }
    }
}
