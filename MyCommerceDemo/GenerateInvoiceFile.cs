using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.IO;

namespace MyCommerceDemo
{
    public static class GenerateInvoiceFile
    {
        [FunctionName("GenerateInvoiceFile")]
        [StorageAccount("StorageAccount")]
        public static void Run([QueueTrigger("orderreceivedqueue")]Order myQueueItem,
            IBinder binder, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");

            using (var outputBlob = binder.Bind<TextWriter>(new BlobAttribute($"invoices/{myQueueItem.orderId}.txt")))
            {
                outputBlob.WriteLine($"Fattura generata il {DateTime.Now} per l'ordine {myQueueItem.orderId} del {myQueueItem.date}");
                outputBlob.WriteLine($"");
                outputBlob.WriteLine($"Cliente : {myQueueItem.custName}");
                outputBlob.WriteLine($"Domicilio: {myQueueItem.custAddress}");
                outputBlob.WriteLine($"Email: {myQueueItem.custEmail}");
                outputBlob.WriteLine($"");
                outputBlob.WriteLine($"Prezzo : {myQueueItem.price}€");
            }
        }
    }
}
