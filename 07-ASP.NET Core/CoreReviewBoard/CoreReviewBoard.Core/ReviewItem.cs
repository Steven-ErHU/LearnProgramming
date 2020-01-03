using System;

namespace CoreReviewBoard.Core
{
    public class ReviewItem
    {
        public ReviewItem(uint id, string summary, string description, ReviewStatus status, string comments, ProjectType project, string module, User reviewer, User owner, string trackerCase, DateTime postTime, DateTime updateTime)
        {
            this.Id = id;
            this.Summary = summary;
            this.Description = description;
            this.Status = status;
            this.Comments = comments;
            this.Project = project;
            this.Module = module;
            this.Reviewer = reviewer;
            this.Owner = owner;
            this.TrackerCase = trackerCase;
            this.PostTime = postTime;
            this.UpdateTime = updateTime;
        }

        public ReviewItem()
        {
        }

        public uint Id { get; set; }

        public string Summary { get; set; }
        public string Description { get; set; }
        public ReviewStatus Status { get; set; }
        public string Comments { get; set; }
        public ProjectType Project { get; set; }

        public string Module { get; set; }

        public User Reviewer { get; set; }

        public User Owner { get; set; }

        public string TrackerCase { get; set; }

        public DateTime PostTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}