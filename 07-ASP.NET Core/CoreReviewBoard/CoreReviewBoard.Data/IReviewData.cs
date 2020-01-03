namespace CoreReviewBoard.Data
{
    using CoreReviewBoard.Core;
    using System;
    using System.Collections.Generic;
    
    public interface IReviewData
    {
        IEnumerable<ReviewItem> GetAllReviewItems();
    }

    public class InMemoryReviewItems : IReviewData
    {
        private readonly List<ReviewItem> _items;

        public InMemoryReviewItems()
        {
            _items = new List<ReviewItem>(){
                new ReviewItem(00001, "test review item 1", "description-weafewfwaefw", ReviewStatus.ToBeReview, "", ProjectType.Freelance, "", new User(001, "Steven", "12345678", new Group(001, "TestGroup"), AuthorityLevel.Admin), new User(002, "Aaron", "12345678", new Group(001, "TestGroup"), AuthorityLevel.Admin), "88888", DateTime.Now, DateTime.Now),
                new ReviewItem(00002, "test review item 2", "description-weafewfwaefw", ReviewStatus.ToBeReview, "", ProjectType.Freelance, "", new User(003, "abc", "12345678", new Group(001, "TestGroup"), AuthorityLevel.Admin), new User(002, "Aaron", "12345678", new Group(001, "TestGroup"), AuthorityLevel.Admin), "77777", DateTime.Now, DateTime.Now),
                new ReviewItem(00003, "test review item 3", "description-weafewfwaefw", ReviewStatus.ToBeReview, "", ProjectType.Freelance, "", new User(004, "efewfwfw", "12345678", new Group(002, "TestGroup2"), AuthorityLevel.Admin), new User(002, "Aaron", "12345678", new Group(001, "TestGroup"), AuthorityLevel.Admin), "99999", DateTime.Now, DateTime.Now),
            };
        }
        public IEnumerable<ReviewItem> GetAllReviewItems()
        {
            return _items;
        }
    }
}