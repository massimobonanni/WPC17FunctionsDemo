using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyCommerceDemo
{
    public static class OnOrderReceived
    {
        [FunctionName("OnOrderReceived")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ReceiveOrder")]HttpRequestMessage req,
            [Queue("orderreceivedqueue", Connection = "StorageAccount")]IAsyncCollector<Order> outputQueueItem,
            [Table("ordersTable", Connection = "StorageAccount")] IAsyncCollector<OrderRow> outputTable,
            TraceWriter log)
        {
            log.Info($"Order received!");

            string jsonContent = await req.Content.ReadAsStringAsync();
            Order order = JsonConvert.DeserializeObject<Order>(jsonContent);

            await outputQueueItem.AddAsync(order);

            OrderRow orderRow = new OrderRow(order);
            await outputTable.AddAsync(orderRow);

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                result = $"Order {order} received."
            });
        }
    }
}
