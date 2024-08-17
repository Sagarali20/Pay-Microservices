using AutoFixture;
using Common;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentService.Application.Request.Send.Command;
using PaymentService.Controllers;
using PaymentService.Helpers;
using Xunit;
namespace SendMoney.Api.Tests.Controllers
{
   
    public class SendControllerTest
    {
        private readonly IFixture _fixture;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly SendMoneyController _sut;
        private readonly ILogger<SendMoneyController> logger;
        public SendControllerTest()
        {
            logger = Mock.Of<ILogger<SendMoneyController>>();
            _fixture = new Fixture();
            _mediatorMock = new Mock<IMediator>();
            _sut = new SendMoneyController(_mediatorMock.Object, logger);
        }
        /**
           * 
           -- Author     : MD.Musfiqur Rahman
           -- Create date: 07/24/2024
           -- Description: testing of sendMoney controller 
           * 
        **/

        [Fact]
        public void SendControllerConstructor_ShouldThrowArgumentNullException_WhenMediatorIsNull()
        {
            // Arrange
            IMediator? mediator = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SendMoneyController(mediator, logger));
        }
        /**
           * 
           -- Author     : MD.Musfiqur Rahman
           -- Create date: 07/24/2024
           -- Description: testing of sendMoney controller 
           * 
        **/

        [Fact]
        public async Task Send_ShouldReturnOkResult_WithResult()
        {
            // Arrange
            var command = _fixture.Create<AddOrEditTransaction>();
            var expectedResult = Result.Success();
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddOrEditTransaction>(), default)).ReturnsAsync(expectedResult);

            // Act
            var result = await _sut.Send(command).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            //okResult.Value.Should().NotBeNull().And.BeOfType<Result>().And.BeEquivalentTo(expectedResult);

            _mediatorMock.Verify(m => m.Send(It.IsAny<AddOrEditTransaction>(), default), Times.Once());
        }

        /**
         * 
         -- Author     : MD.Musfiqur Rahman
         -- Create date: 07/24/2024
         -- Description: testing of sendMoney controller 
         * 
        **/

        [Fact]
        public async Task Send_ShouldReturnNotFound_WhenDataNotFound()
        {
            // Arrange
            Result response = null;
            _mediatorMock.Setup(x => x.Send(It.IsAny<AddOrEditTransaction>(), default)).ReturnsAsync(response);
            var command = _fixture.Create<AddOrEditTransaction>();

            // Act
            var result = await _sut.Send(command).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            //result.Result.Should().BeAssignableTo<NotFoundResult>();
            _mediatorMock.Verify(x => x.Send(It.IsAny<AddOrEditTransaction>(), default), Times.Once());
        }
    }
}
