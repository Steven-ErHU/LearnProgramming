using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreReviewBoard.Core;
using CoreReviewBoard.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreReviewBoard.Pages
{
    public class ReviewBoardModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IReviewData _reviewData;
        private readonly ILogger<ReviewBoardModel> _logger;

        public IEnumerable<ReviewItem> Items { get; set; }

        public ReviewBoardModel(ILogger<ReviewBoardModel> logger, IConfiguration config, IReviewData reviewData)
        {
            _logger = logger;

            _config = config;
            _reviewData = reviewData;
        }

        public void OnGet()
        {
            Items = _reviewData.GetAllReviewItems();
        }
    }
}
