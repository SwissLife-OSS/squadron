using System;

namespace Squadron.Samples.Shared
{
    public class UserAudit
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Action { get; set; }

        public static UserAudit CreateSample(string userId)
        {
            return new UserAudit
            {
                Id = Guid.NewGuid().ToString(),
                Action = "add",
                UserId = userId
            };
        }
    }
}
