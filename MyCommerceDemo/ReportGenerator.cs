using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using System.IO;

namespace MyCommerceDemo
{
    public static class ReportGenerator
    {
        [FunctionName("ReportGenerator")]
        [StorageAccount("StorageAccount")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            [Table("ordersTable", "Order")] IQueryable<ReportOrderRow> inputTable,
            IBinder reportBinder,
            [Table("ordersTable", "Order")] ICollector<ReportOrderRow> outputTable,
            TraceWriter log)
        {
            log.Info($"ReportGenerator executed at: {DateTime.Now}");
            var ordersToReport = inputTable.Where(o => !o.isReported).ToList();

            if (ordersToReport.Count() > 0)
            {
                log.Info($"Orders to report {ordersToReport.Count()}");
                var reportDate = DateTime.Now;

                using (var reportBlob = reportBinder.Bind<TextWriter>(new BlobAttribute($"reports/{reportDate:yyyyMMddHHmmss}.rep")))
                {
                    reportBlob.WriteLine($"OrderId;Customer;OrderDate;Price");
                    double total = 0.0;
                    foreach (var order in ordersToReport)
                    {
                        log.Info($"Order: {order}");
                        reportBlob.WriteLine($"{order.orderId};{order.custName};{order.date};{order.price}");
                        total += order.price;
                        order.isReported = true;
                        outputTable.Add(order);
                    }
                    reportBlob.WriteLine($";Total;;{total}");
                }
            }
            else
            {
                log.Info($"Nothing to report!");
            }
        }
    }


    public class ReportOrderRow : TableEntity
    {
        public string orderId { get; set; }
        public string custName { get; set; }
        public string custAddress { get; set; }
        public string custEmail { get; set; }
        public string cartId { get; set; }
        public DateTime date { get; set; }
        public double price { get; set; }
        public bool isReported { get; set; }
        public string fileName { get; set; }

        public override String ToString()
        {
            return $"orderId={orderId}, custName={custName}, custAddress={custAddress}, custEmail={custEmail}, cartId={cartId}, date={date}, price={price}";
        }
    }
}
