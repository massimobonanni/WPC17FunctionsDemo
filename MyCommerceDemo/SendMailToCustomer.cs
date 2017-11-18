using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SendGrid.Helpers.Mail;

namespace MyCommerceDemo
{
    public static class SendMailToCustomer
    {
        [FunctionName("SendMailToCustomer")]
        public static void Run([BlobTrigger("invoices/{filename}.txt", Connection = "StorageAccount")]string myBlob, string filename,
            [Table("ordersTable", "Order", "{filename}", Connection = "StorageAccount", Filter = null, Take = 50)] OrderRow orderRow,
            TraceWriter log, [SendGrid(ApiKey ="SendGridApiKey")] out Mail message)
        {
            log.Info($"File Processed : {filename}");
            log.Info($"Order: {orderRow}");
            log.Info($"Customer mail: {orderRow.custEmail}");

            message = new Mail()
            {
                Subject = "Azure Functions Invoice",
                From = new Email("azureinvoice@invoiceplatform.com")
            };
           
            var personalization = new Personalization();
            personalization.AddTo(new Email(orderRow.custEmail));
            message.AddPersonalization(personalization);

            Content content = new Content
            {
                Type = "text/plain",
                Value = myBlob
            };
            message.AddContent(content);

            Attachment attachment = new Attachment();
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(myBlob);
            attachment.Content = System.Convert.ToBase64String(plainTextBytes);
            attachment.Type = "text/plain";
            attachment.Filename = "invoice.txt";
            attachment.Disposition = "attachment";
            attachment.ContentId = "Invoice File";
            message.AddAttachment(attachment);
        }
    }
}
