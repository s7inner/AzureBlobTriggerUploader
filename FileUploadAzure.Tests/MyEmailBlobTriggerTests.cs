using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using FileUploadAzure.Tests;

namespace EmailBlobTrigger.Tests
{
    [TestClass]
    public class MyEmailBlobTriggerTests: MyEmailBlobTrigger
    {
        private MyEmailBlobTrigger _emailBlobTrigger;
        private Mock<ILogger> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            _emailBlobTrigger = new MyEmailBlobTrigger();
            _loggerMock = new Mock<ILogger>();
        }

        [TestMethod]
        public void Run_ValidMetadata_SendsEmail()
        {
            // Arrange
            var metadata = new Dictionary<string, string>
        {
            { "email", "skillfull357@gmail.com" }
        };
            var memoryStream = new MemoryStream();
            var logger = new TestLogger();

            // Act
            var emailBlobTrigger = new MyEmailBlobTrigger();
            emailBlobTrigger.Run(memoryStream, "test.docx", metadata, logger);

            // Assert
            Assert.IsTrue(logger.LogMessages.Any(message => message.Contains("Електронний лист відправлено успішно")));
        }

        [TestMethod]
        public void Run_NoEmailMetadata_DoesNotSendEmail()
        {
            // Arrange
            var metadata = new Dictionary<string, string>(); // Пусті метадані
            var memoryStream = new MemoryStream();
            var logger = new TestLogger();
            var emailBlobTrigger = new MyEmailBlobTrigger();

            // Act
            emailBlobTrigger.Run(memoryStream, "test.docx", metadata, logger);

            // Assert
            Assert.IsFalse(logger.LogMessages.Any(message => message.Contains("Електронний лист відправлено успішно")));

        }

            [TestMethod]
        public void GenerateSasTokenForBlob_ValidBlobName_GeneratesToken()
        {
            // Arrange
            var blobName = "test.docx";

            // Act
            var sasToken = _emailBlobTrigger.GenerateSasTokenForBlob(blobName);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(sasToken));
        }

        [TestMethod]
        public void GetBlobSasUri_ValidBlobName_GeneratesUriWithToken()
        {
            // Arrange
            var blobName = "test.docx";
            var sasToken = "sampleToken";

            // Act
            var sasUri = _emailBlobTrigger.GetBlobSasUri(blobName, sasToken);

            // Assert
            Assert.IsTrue(sasUri.Contains(blobName) && sasUri.Contains(sasToken));
        }

        [TestMethod]
        public void SendEmail_ValidParameters_SendsEmail()
        {
            // Arrange
            var fileName = "test.docx";
            var toEmail = "recipient@example.com";
            var blobSasUri = "https://example.com/sasToken"; 
            var logger = new TestLogger();

            // Act
            _emailBlobTrigger.SendEmail(fileName, toEmail, blobSasUri, logger);

            // Assert
            Assert.IsTrue(logger.LogMessages.Any(message => message.Contains("Електронний лист відправлено успішно.")));
        }
    }
}
