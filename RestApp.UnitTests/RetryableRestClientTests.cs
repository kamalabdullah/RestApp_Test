using Moq;
using RestApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestApp.UnitTests
{
    public class RetryableRestClientTests
    {
        [Fact]
        public async Task Get_SuccessfulRequest_ReturnsData_Test()
        {
            // Arrange
            var mockInnerRestClient = new Mock<IRestClient>();
            mockInnerRestClient.Setup(client => client.Get<string>("test-url")).ReturnsAsync("Test data");

            var logger = new Mock<ILogger>();

            var retryableRestClient = new RetryableRestClient(mockInnerRestClient.Object, logger.Object,ConfigurationFactory.CreateDEVConf());

            // Act
            var result = await retryableRestClient.Get<string>("test-url");

            // Assert
            Assert.Equal("Test data", result);
            mockInnerRestClient.Verify(client => client.Get<string>("test-url"), Times.Once);
            logger.Verify(l => l.Error(It.IsAny<WebException>()), Times.Never);
        }


        [Fact]
        public async Task Get_AllRetriesFail_ReturnsDefault_Test()
        {
            // Arrange
            var mockInnerRestClient = new Mock<IRestClient>();
            mockInnerRestClient.Setup(client => client.Get<string>("test-url"))
                                .ThrowsAsync(new WebException("Test exception", WebExceptionStatus.ConnectionClosed));

            var logger = new Mock<ILogger>();

            var retryableRestClient = new RetryableRestClient(mockInnerRestClient.Object, logger.Object, ConfigurationFactory.CreateDEVConf());

            // Act
            var result = await retryableRestClient.Get<string>("test-url");

            // Assert
            Assert.Equal(default(string), result);
        }


        //New
        [Fact]
        public async Task Retry_ThreeTimesBeforeGetSucceedData_Test()
        {
            // Arrange
            var mockInnerRestClient = new Mock<IRestClient>();
            mockInnerRestClient.SetupSequence(client => client.Get<string>("test-url"))
                               .ThrowsAsync(new WebException("Test exception", WebExceptionStatus.ConnectionClosed))
                               .ThrowsAsync(new WebException("Test exception", WebExceptionStatus.ConnectionClosed))
                               .ThrowsAsync(new WebException("Test exception", WebExceptionStatus.ConnectionClosed))
                               .ReturnsAsync("Test data");

            var logger = new Mock<ILogger>();

            var retryableRestClient = new RetryableRestClient(mockInnerRestClient.Object, logger.Object, ConfigurationFactory.CreateDEVConf());

            // Act
            var result = await retryableRestClient.Get<string>("test-url");

            // Assert
            Assert.Equal("Test data", result);
            mockInnerRestClient.Verify(client => client.Get<string>("test-url"), Times.Exactly(4));
        }

        [Fact]
        public async Task Retry_ConfigurableTimeoutBetweenRetries_Test()
        {
            // Arrange
            var mockInnerRestClient = new Mock<IRestClient>();
            mockInnerRestClient.SetupSequence(client => client.Get<string>("test-url"))
                               .ThrowsAsync(new WebException("Test exception", WebExceptionStatus.ConnectionClosed))
                               .ReturnsAsync("Test data");

            var logger = new Mock<ILogger>();

            var retryableRestClient = new RetryableRestClient(mockInnerRestClient.Object, logger.Object, ConfigurationFactory.CreateDEVConf());

            // Act
            var startTime = DateTime.Now;
            var result = await retryableRestClient.Get<string>("test-url");
            var endTime = DateTime.Now;

            // Assert
            Assert.True((endTime - startTime).TotalSeconds >= 1);
        }

        [Fact]
        public async Task Retry_AllAttemptsFail_LogsAndReturnsNull_Test()
        {
            // Arrange
            var mockInnerRestClient = new Mock<IRestClient>();
            mockInnerRestClient.Setup(client => client.Get<string>("test-url"))
                               .ThrowsAsync(new WebException("Test exception", WebExceptionStatus.ConnectionClosed));

            var logger = new Mock<ILogger>();

            var retryableRestClient = new RetryableRestClient(mockInnerRestClient.Object, logger.Object, ConfigurationFactory.CreateDEVConf());

            // Act
            var result = await retryableRestClient.Get<string>("test-url");

            // Assert
            Assert.Null(result);
            logger.Verify(l => l.Error(It.IsAny<WebException>()), Times.Once);
        }

        [Fact]
        public async Task Log_ExceptionThrownOnceDuringRetries_Test()
        {
            // Arrange
            var mockInnerRestClient = new Mock<IRestClient>();
            mockInnerRestClient.SetupSequence(client => client.Get<string>("test-url"))
                               .ThrowsAsync(new WebException("Test exception", WebExceptionStatus.ConnectionClosed))
                               .ThrowsAsync(new WebException("Test exception", WebExceptionStatus.ConnectionClosed))
                               .ThrowsAsync(new WebException("Test exception", WebExceptionStatus.ConnectionClosed))
                               .ReturnsAsync("Test data");

            var logger = new Mock<ILogger>();

            var retryableRestClient = new RetryableRestClient(mockInnerRestClient.Object, logger.Object, ConfigurationFactory.CreateDEVConf());

            // Act
            await retryableRestClient.Get<string>("test-url");

            // Assert
            logger.Verify(l => l.Error(It.IsAny<WebException>()), Times.Once);
        }

        [Fact]
        public async Task FailFast_WhenNonWebExceptionThrown_Test()
        {
            // Arrange
            var mockInnerRestClient = new Mock<IRestClient>();
            mockInnerRestClient.Setup(client => client.Get<string>("test-url"))
                               .ThrowsAsync(new InvalidOperationException("Test exception"));

            var logger = new Mock<ILogger>();

            var retryableRestClient = new RetryableRestClient(mockInnerRestClient.Object, logger.Object, ConfigurationFactory.CreateDEVConf());

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await retryableRestClient.Get<string>("test-url"));

            // Assert
            mockInnerRestClient.Verify(client => client.Get<string>("test-url"), Times.Once); 
            logger.Verify(l => l.Error(It.IsAny<InvalidOperationException>()), Times.Once); 
        }
    }
}
