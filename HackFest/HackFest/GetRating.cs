
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HackFest
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "rating/{ratingId}")]HttpRequest req, [CosmosDB("Ratings", "Ratings", ConnectionStringSetting = "hackfestapi_DOCUMENTDB", Id = "{ratingId}")] RatingData rating, ILogger log, string ratingId)
        {
            if (rating != null)
            {
                return new OkObjectResult(rating);
            }
            return new NotFoundObjectResult($"No rating exists with id {ratingId}");
        }

        public class RatingData
        {
            public string Id { get; set; }
            public string UserId { get; set; }
            public string ProductId { get; set; }
            public string Timestamp { get; set; }
            public int Rating { get; set; }
            public string UserNotes { get; set; }
        }
    }
}
