using Microsoft.Extensions.Logging.Abstractions; // Додайте цей using для імпорту ILogger
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using EmailBlobTrigger;

namespace EmailBlobTrigger.Tests
{
    [TestClass]
    public class MyEmailBlobTriggerTests
    {
        [TestMethod]
        public void TestFunctionWithValidParameters()
        {
            // Arrange
            var function = new MyEmailBlobTrigger();
            var blobStream = new MemoryStream(); // Поставте дані потоку, які вам потрібні для тестування
            var name = "test.txt";
            var metadata = new Dictionary<string, string>
            {
                { "email", "test@example.com" }
            };
            var log = new NullLogger<MyEmailBlobTrigger>(); // Використовуємо NullLogger для ILogger

            // Act
            function.Run(blobStream, name, metadata, log);

            // Assert
            // В даному тесті ми перевіряємо, чи функція виконується без помилок.
            // Ви можете перевірити логіку журналювання або інші аспекти своєї функції за потреби.
        }
    }
}
