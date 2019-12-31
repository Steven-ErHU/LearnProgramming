namespace CoreReviewBoard.Models
{
    using System.Collections.Generic;
    public interface IReviewRequest
    {
         IEnumerable<ReviewItem> GetAllReviewItems();
    }
}