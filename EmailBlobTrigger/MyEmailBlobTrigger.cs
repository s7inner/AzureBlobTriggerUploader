using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;

namespace EmailBlobTrigger
{
    public class MyEmailBlobTrigger
    {
        private readonly string storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=defaultstoragesa1;AccountKey=VG3sGDDpKdG+76tbGCDfya/5klUkKGprU52bGSiIZF1pd5oWu7mNHda8lDquXaZUjWOJSWbU6NAw+ASt623EBQ==;EndpointSuffix=core.windows.net";
        private readonly string azureBlobContainerName = "defaultblobcontainer";

        [FunctionName("MyEmailBlobTrigger")]
        public void Run([BlobTrigger("defaultblobcontainer/{name}", Connection = "AzureWebJobsStorage")] Stream blobStream, string name, IDictionary<string, string> metadata, ILogger log)
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
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(azureBlobContainerName);
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
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(azureBlobContainerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            string blobSasUri = blob.Uri + sasToken;
            return blobSasUri;
        }

        public void SendEmail(string fileName, string toEmail, string blobSasUri, ILogger log)
        {
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "ss7inner@gmail.com";
            string smtpGoogleAccountAppPassword = "csidcihqubpkrjmr";

            string fromEmail = smtpUsername;

            // Object for sending e-mails
            SmtpClient smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpGoogleAccountAppPassword),
                EnableSsl = true
            };

            // Create a message
            MailMessage mail = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = "Test Task, new file added successfully!",
                Body = $"The name of the file: {fileName}<br><a href=\"{blobSasUri}\">{blobSasUri}</a>",
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            smtpClient.Send(mail);
            log.LogInformation("Email sent successfully!");
        }
    }
}
