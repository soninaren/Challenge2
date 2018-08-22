
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using System;

namespace HackFest
{
    public static class Function1
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log,
            [CosmosDB("Ratings", "Ratings", ConnectionStringSetting = "hackfestapi_DOCUMENTDB")] IAsyncCollector<RatingData> ratingData)
        {
            log.LogInformation("Received request");
            HttpResponseMessage response = null;
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string userId = data?.userId;
            string productId = data?.productId;
            if (int.TryParse(data?.rating.ToString(), out int rating))
            {
                if (rating > 5 && rating < 0)
                {
                    return new BadRequestObjectResult("Rating must be between 0 and 5");
                }
            }
            else
            {
                return new BadRequestObjectResult("Invalid Rating");
            }

            if (productId != null)
            {
                string productApiUrl = $"http://serverlessohproduct.trafficmanager.net/api/GetProduct?productId={productId}";
                response = await client.GetAsync(productApiUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return new BadRequestObjectResult("Invalid Product id");
                }

                string productsAPIResonse = await response.Content.ReadAsStringAsync();
                dynamic products = JsonConvert.DeserializeObject(productsAPIResonse);
                if (products?.productId == null)
                {
                    return new BadRequestObjectResult("Invalid Product id");
                }
            }
            else
            {
                return new BadRequestObjectResult("Product id is required");
            }

            if (userId != null)
            {
                string userApiUrl = $"http://serverlessohuser.trafficmanager.net/api/Getuser?userId={userId}";
                response = await client.GetAsync(userApiUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return new BadRequestObjectResult("Invalid Product id");
                }
                string usersAPIResonse = await response.Content.ReadAsStringAsync();
                dynamic users = JsonConvert.DeserializeObject(usersAPIResonse);
                if (users?.userId == null)
                {
                    return new BadRequestObjectResult("Invalid user id");
                }
            }
            else
            {
                return new BadRequestObjectResult("user id is required");
            }

            var ratingDocument = new RatingData
            {
                id = Guid.NewGuid().ToString(),
                userId = data.userId,
                productId = data.productId,
                timestamp = DateTime.UtcNow,
                rating = rating,
                userNotes = data.userNotes
            };
            await ratingData.AddAsync(ratingDocument);

            return new OkObjectResult(ratingDocument);
        }
    }
    public class RatingData
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string productId { get; set; }
        public DateTime timestamp { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
    }
}



//{
//    "userId": "cc20a6fb-a91f-4192-874d-132493685376",
//    "productId": "4c25613a-a3c2-4ef3-8e02-9c335eb23204",
//    "locationName": "Sample ice cream shop",
//    "rating": 5,
//    "userNotes": "I love the subtle notes of orange in this ice cream!"
//}