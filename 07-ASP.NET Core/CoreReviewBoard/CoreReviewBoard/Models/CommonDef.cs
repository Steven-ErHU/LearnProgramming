using System.Collections.Generic;

namespace CoreReviewBoard.Models
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
        public uint Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public Group Group { get; set; }

        public AuthorityLevel Level { get; set; }
    }

    public class Group
    {
        public uint Id { get; set; }

        public string Name { get; set; }

        public List<User> Members { get; set; }
    }

}