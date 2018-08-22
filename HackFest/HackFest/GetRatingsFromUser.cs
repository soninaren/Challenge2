
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace HackFest
{
    public static class GetRatingsFromUser
    {
        [FunctionName("GetRatingsFromUser")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{userId}/ratings")]HttpRequest req, [CosmosDB("Ratings", "Ratings", ConnectionStringSetting = "hackfestapi_DOCUMENTDB", SqlQuery = "SELECT * FROM c WHERE c.userId = {userId}")] IEnumerable<RatingData> ratings, ILogger log)
        {
            if (ratings.Any())
            {
                return new OkObjectResult(ratings);
            }
            return new NotFoundObjectResult("The user does not have any ratings");
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
