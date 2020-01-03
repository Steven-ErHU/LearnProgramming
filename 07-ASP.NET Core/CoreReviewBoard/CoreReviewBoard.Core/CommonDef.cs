using System.Collections.Generic;

namespace CoreReviewBoard.Core
{
    public enum ProjectType
    {
        Freelance,
        WinCS
    }

    public enum ReviewStatus
    {
        ToBeReview,
        Reviewing,
        Reviewed,
        Rejected
    }

    public enum AuthorityLevel
    {
        Admin,
        Standard,
    }

    public class User
    {
        public User(uint id, string name, string password, Group group, AuthorityLevel level)
        {
            this.Id = id;
            this.Name = name;
            this.Password = password;
            this.Group = group;
            this.Level = level;

        }

        public uint Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public Group Group { get; set; }

        public AuthorityLevel Level { get; set; }
    }

    public class Group
    {
        public Group(uint id, string name)
        {
            this.Id = id;
            this.Name = name;

        }

        public uint Id { get; set; }

        public string Name { get; set; }

        public List<User> Members { get; set; }
    }

}