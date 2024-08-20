using Moq;
using MailService.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using MailService.Interfaces;
using MailService.Models;
using Microsoft.Extensions.Configuration;
using MailService.Services;
using MailService.Config;
using System.Text;

namespace MailService.Tests
{
    public class MailControllerTest
    {
        private readonly IConfiguration configuration;
        private readonly IIDbConnection iDbConnection;
        private readonly Mock<ILogger<MailServices>> mockMailLogger;
        private readonly Mock<ILogger<MailLogService>> mockMailLogLogger;
        private readonly IMailLogService mailLogService;
        private readonly IMailServices mailServices;

        public MailControllerTest()
        {
            configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  // Set base path to current directory
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)  // Load the JSON file
            .Build();
            iDbConnection = new DbConnection(configuration);
            mockMailLogger = new Mock<ILogger<MailServices>>();
            mockMailLogLogger = new Mock<ILogger<MailLogService>>();
            mailLogService = new MailLogService(iDbConnection, mockMailLogLogger.Object);
            mailServices = new MailServices(configuration, mockMailLogger.Object, mailLogService);
        }
        [Fact]
        public void FnSendMailTest()
        {
            // Arrange 
            var mockLogger = new Mock<ILogger<MailController>>();
            MailBody body = new()
            {
                IsWithAttachment = false,
                IsWithCc = false,
                IsWithBcc = false,
                ToMail = new Dictionary<string, string>
                {
                  {"masud", "masud.ahmed.necmoney@gmail.com" }
                },
                Subject = "Test",
                Text = "Test"
            };

            ObjectResult expectedResult = new ObjectResult(new
            {
                ResponseCode = "SUCCESS",
                message = "Success! The email was delivered."
            })
            {
                StatusCode = 250
            };

            // Create controller instance
            var controller = new MailController(mockLogger.Object, mailServices);

            // Act

            IActionResult result = controller.SendMail(body);


            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result); // Check the type
            Assert.Equal(expectedResult.StatusCode, objectResult.StatusCode); // Compare status code

            Assert.Equal(expectedResult.Value.ToString(), objectResult.Value.ToString()); // Compare result content


        }

        [Fact]
        public void FnSendMailWithFileTest()
        {
            // Arrange 
            var mockLogger = new Mock<ILogger<MailController>>();
            string readContents;
            using (StreamReader streamReader = new StreamReader("F:\\DotNet\\A-GITHUB\\necpaybd-core\\backend\\MailService\\MailService.Tests\\attachment\\base64.txt", Encoding.UTF8))
            {
                readContents = streamReader.ReadToEnd();
            }


            MailBody body = new()
            {
                IsWithAttachment = true,
                IsWithCc = false,
                IsWithBcc = false,
                ToMail = new Dictionary<string, string>
                {
                  {"masud", "masud.ahmed.necmoney@gmail.com" }
                },
                Files = new List<Models.FileInfo> { new(FileName:"monkey.PNG", Base64: readContents, Extension:"png") },
                Subject = "Test",
                Text = "Test"
            };

            ObjectResult expectedResult = new ObjectResult(new
            {
                ResponseCode = "SUCCESS",
                message = "Success! The email was delivered."
            })
            {
                StatusCode = 250
            };

            // Create controller instance
            var controller = new MailController(mockLogger.Object, mailServices);

            // Act

            IActionResult result = controller.SendMail(body);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result); // Check the type
            Assert.Equal(expectedResult.StatusCode, objectResult.StatusCode); // Compare status code

            Assert.Equal(expectedResult.Value.ToString(), objectResult.Value.ToString()); // Compare result content


        }
        
        [Fact]
        public void FnSendMailWithCcTest()
        {
            // Arrange 
            var mockLogger = new Mock<ILogger<MailController>>();

            MailBody body = new()
            {
                IsWithAttachment = false,
                IsWithCc = true,
                IsWithBcc = false,
                ToMail = new Dictionary<string, string>
                {
                  {"masud", "masud.ahmed.necmoney@gmail.com" }
                },
                CcEmail = new Dictionary<string, string>
                {
                  {"musfiqur", "musfiqur@primotechltd.com" }
                },
                Subject = "Test",
                Text = "Test"
            };

            ObjectResult expectedResult = new ObjectResult(new
            {
                ResponseCode = "SUCCESS",
                message = "Success! The email was delivered."
            })
            {
                StatusCode = 250
            };

            // Create controller instance
            var controller = new MailController(mockLogger.Object, mailServices);

            // Act

            IActionResult result = controller.SendMail(body);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result); // Check the type
            Assert.Equal(expectedResult.StatusCode, objectResult.StatusCode); // Compare status code

            Assert.Equal(expectedResult.Value.ToString(), objectResult.Value.ToString()); // Compare result content
        }

        [Fact]
        public void FnSendMailWithBccTest()
        {
            // Arrange 
            var mockLogger = new Mock<ILogger<MailController>>();

            MailBody body = new()
            {
                IsWithAttachment = false,
                IsWithCc = false,
                IsWithBcc = true,
                ToMail = new Dictionary<string, string>
                {
                  {"masud", "masud.ahmed.necmoney@gmail.com" }
                },
                BccEmail = new Dictionary<string, string>
                {
                  {"musfiqur", "musfiqur@primotechltd.com" }
                },
                Subject = "Test",
                Text = "Test"
            };

            ObjectResult expectedResult = new ObjectResult(new
            {
                ResponseCode = "SUCCESS",
                message = "Success! The email was delivered."
            })
            {
                StatusCode = 250
            };

            // Create controller instance
            var controller = new MailController(mockLogger.Object, mailServices);

            // Act

            IActionResult result = controller.SendMail(body);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result); // Check the type
            Assert.Equal(expectedResult.StatusCode, objectResult.StatusCode); // Compare status code

            Assert.Equal(expectedResult.Value.ToString(), objectResult.Value.ToString()); // Compare result content
        }

        [Fact]
        public void FnSendCcWithFileTest()
        {
            // Arrange 
            var mockLogger = new Mock<ILogger<MailController>>();
            string readContents;
            using (StreamReader streamReader = new StreamReader("F:\\DotNet\\A-GITHUB\\necpaybd-core\\backend\\MailService\\MailService.Tests\\attachment\\base64.txt", Encoding.UTF8))
            {
                readContents = streamReader.ReadToEnd();
            }


            MailBody body = new()
            {
                IsWithAttachment = true,
                IsWithCc = true,
                IsWithBcc = false,
                ToMail = new Dictionary<string, string>
                {
                  {"masud", "masud.ahmed.necmoney@gmail.com" }
                },
                CcEmail = new Dictionary<string, string>
                {
                  {"musfiqur", "musfiqur@primotechltd.com" }
                },
                Files = new List<Models.FileInfo> { new(FileName: "monkey.PNG", Base64: readContents, Extension: "png") },
                Subject = "Test",
                Text = "Test"
            };

            ObjectResult expectedResult = new ObjectResult(new
            {
                ResponseCode = "SUCCESS",
                message = "Success! The email was delivered."
            })
            {
                StatusCode = 250
            };

            // Create controller instance
            var controller = new MailController(mockLogger.Object, mailServices);

            // Act

            IActionResult result = controller.SendMail(body);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result); // Check the type
            Assert.Equal(expectedResult.StatusCode, objectResult.StatusCode); // Compare status code

            Assert.Equal(expectedResult.Value.ToString(), objectResult.Value.ToString()); // Compare result content


        }

        [Fact]
        public void FnSendBccWithFileTest()
        {
            // Arrange 
            var mockLogger = new Mock<ILogger<MailController>>();
            string readContents;
            using (StreamReader streamReader = new StreamReader("F:\\DotNet\\A-GITHUB\\necpaybd-core\\backend\\MailService\\MailService.Tests\\attachment\\base64.txt", Encoding.UTF8))
            {
                readContents = streamReader.ReadToEnd();
            }


            MailBody body = new()
            {
                IsWithAttachment = true,
                IsWithCc = false,
                IsWithBcc = true,
                ToMail = new Dictionary<string, string>
                {
                  {"masud", "masud.ahmed.necmoney@gmail.com" }
                },
                BccEmail = new Dictionary<string, string>
                {
                  {"musfiqur", "musfiqur@primotechltd.com" }
                },
                Files = new List<Models.FileInfo> { new(FileName: "monkey.PNG", Base64: readContents, Extension: "png") },
                Subject = "Test",
                Text = "Test"
            };

            ObjectResult expectedResult = new ObjectResult(new
            {
                ResponseCode = "SUCCESS",
                message = "Success! The email was delivered."
            })
            {
                StatusCode = 250
            };

            // Create controller instance
            var controller = new MailController(mockLogger.Object, mailServices);

            // Act

            IActionResult result = controller.SendMail(body);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result); // Check the type
            Assert.Equal(expectedResult.StatusCode, objectResult.StatusCode); // Compare status code

            Assert.Equal(expectedResult.Value.ToString(), objectResult.Value.ToString()); // Compare result content


        }
    }
}
