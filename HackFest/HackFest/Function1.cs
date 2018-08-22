
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

namespace HackFest
{
    public static class Function1
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("Received request");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string userId = data?.name;
            string productId = data?.productId;
            if (int.TryParse(data?.rating.ToString(), out int rating))
            {
                if (rating > 5)
                {
                    new BadRequestObjectResult("Rating higher that 5");
                }
            }
            else
            {
                new BadRequestObjectResult("Invalid Rating");
            }

            if (productId != null)
            {
                string productApiUrl = $"http://serverlessohproduct.trafficmanager.net/api/GetProduct?productId={productId}";
                var response = await client.GetAsync(productApiUrl);
                string productsAPIResonse = await response.Content.ReadAsStringAsync();
                dynamic products = JsonConvert.DeserializeObject(productsAPIResonse);
                if (products?.productId == null)
                {
                    new BadRequestObjectResult("Invalid Product id");
                }
            }
            else
            {
                new BadRequestObjectResult("Product id is required");
            }


            return new OkObjectResult("Created");
        }
    }
}



//{
//    "userId": "cc20a6fb-a91f-4192-874d-132493685376",
//    "productId": "4c25613a-a3c2-4ef3-8e02-9c335eb23204",
//    "locationName": "Sample ice cream shop",
//    "rating": 5,
//    "userNotes": "I love the subtle notes of orange in this ice cream!"
//}