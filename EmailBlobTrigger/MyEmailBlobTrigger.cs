using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace EmailBlobTrigger
{
    public class MyEmailBlobTrigger
    {
        [FunctionName("MyEmailBlobTrigger")]
        public void Run([BlobTrigger("file-upload/{name}", Connection = "AzureWebJobsStorage")] Stream blobStream, string name, IDictionary<string, string> metadata, ILogger log)
        {
           
            if (metadata != null && metadata.ContainsKey("email"))
            {
                string email = metadata["email"];

                log.LogInformation($"Email address from metadata: {email}");

                // Generate SAS token for the blob
                string sasToken = GenerateSasTokenForBlob(name);

                // Create a URL with the SAS token
                string blobSasUri = GetBlobSasUri(name, sasToken);

                // Send the email with the blobSasUri
                SendEmail(name, email, blobSasUri, log);
            }
        }

        public string GenerateSasTokenForBlob(string blobName)
        {
            string storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=ss7inner;AccountKey=ZoSmdSV69WsBtsvQ28U0/xdowqWYYOmUfDjEeIIGtZbSvCwL3gzZuOotUPRpLXw7PLXkDjAU/lkn+AStxRuUgg==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("file-upload");
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            SharedAccessBlobPolicy sasPolicy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read, // Read permission
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1) // Token expires in 1 hour
            };

            string sasToken = blob.GetSharedAccessSignature(sasPolicy, null, null, null, null);
            return sasToken;
        }

        public string GetBlobSasUri(string blobName, string sasToken)
        {
            string storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=ss7inner;AccountKey=ZoSmdSV69WsBtsvQ28U0/xdowqWYYOmUfDjEeIIGtZbSvCwL3gzZuOotUPRpLXw7PLXkDjAU/lkn+AStxRuUgg==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("file-upload");
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            string blobSasUri = blob.Uri + sasToken;
            return blobSasUri;
        }

        public void SendEmail(string fileName, string toEmail, string blobSasUri, ILogger log)
        {
            // Settings for SMTP server
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "ss7inner@gmail.com";
            string smtpPassword = "mzyzwbxazpypejaf";

            string fromEmail = smtpUsername;

            // Object for sending e-mails
            SmtpClient smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            // Create a message
            MailMessage mail = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = "Новий файл доданий",
                Body = $"Назва файлу: {fileName}\nЕлектронна пошта: {toEmail}\nПосилання: {blobSasUri}",
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            smtpClient.Send(mail);
            log.LogInformation("Електронний лист відправлено успішно.");
        }

    }
}
