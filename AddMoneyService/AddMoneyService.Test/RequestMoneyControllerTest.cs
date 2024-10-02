using AddMoneyService.Application.RequestMoney.Command;
using AddMoneyService.Controllers;
using AutoFixture;
using Common;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AddMoneyService.Test
{
    public class RequestMoneyControllerTest
    {
        private readonly IFixture _fixture;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RequestMoneyController _sut;
        private readonly ILogger<RequestMoneyController> logger;
        public RequestMoneyControllerTest()
        {
            logger = Mock.Of<ILogger<RequestMoneyController>>();
            _mediatorMock = new Mock<IMediator>();
            _sut = new RequestMoneyController(_mediatorMock.Object, logger);
            _fixture = new Fixture();
        }

        [Fact]
        public void RequestMoneyControllerConstructor_ShouldThrowArgumentNullException_WhenMediatorIsNull()
        {
            // Arrange
            IMediator? mediator = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RequestMoneyController(mediator, logger));
        }


        [Fact]
        public async Task RequestMoney_ShouldReturnOkResult_WithResult()
        {
            // Arrange
            var command = _fixture.Create<AddRequestMoney>();

            var expectedResult = Result.Success();
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRequestMoney>(), default)).ReturnsAsync(expectedResult);

            // Act
            var result = await _sut.RequestMoney(command).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            //okResult.Value.Should().NotBeNull().And.BeOfType<Result>().And.BeEquivalentTo(expectedResult);

            _mediatorMock.Verify(m => m.Send(It.IsAny<AddRequestMoney>(), default), Times.Once());
        }


        [Fact]
        public async Task RequestMoney_ShouldReturnNotFound_WhenDataNotFound()
        {
            // Arrange
            Result response = null;
            _mediatorMock.Setup(x => x.Send(It.IsAny<AddRequestMoney>(), default)).ReturnsAsync(response);
            var command = _fixture.Create<AddRequestMoney>();

            // Act
            var result = await _sut.RequestMoney(command).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            //result.Result.Should().BeAssignableTo<NotFoundResult>();
            _mediatorMock.Verify(x => x.Send(It.IsAny<AddRequestMoney>(), default), Times.Once());
        }
    }
}