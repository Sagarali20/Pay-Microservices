using AutoFixture;
using Castle.Core.Logging;
using Common;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RemittanceService.Application.Request.Ramittance.Command;
using RemittanceService.Controllers;

namespace RemittanceService.Test
{
    public class RamittanceControllerTest
    {
        private readonly IFixture _fixture;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RamittanceController _sut;

        private readonly ILogger<RamittanceController> logger;
        public RamittanceControllerTest()
        {
            logger = Mock.Of<ILogger<RamittanceController>>();
            _fixture = new Fixture();
            _mediatorMock = new Mock<IMediator>();
            _sut = new RamittanceController(_mediatorMock.Object, logger);
        }

        [Fact]
        public void RamittanceControllerConstructor_ShouldThrowArgumentNullException_WhenMediatorIsNull()
        {
            // Arrange
            IMediator? mediator = null;
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RamittanceController(mediator, logger));
        }

        [Fact]
        public async void Remittance_ShouldReturnOkResult_WithResult()
        {

            // Arrange
            //var logger = new Mock<ILogger<RamittanceController>>();

            var command = _fixture.Create<RamittanceInformation>();
            var expectedResult = Result.Success();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RamittanceInformation>(), default)).ReturnsAsync(expectedResult);


            // Act
            var result = await _sut.Ramittance(command).ConfigureAwait(false);


            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            //okResult.Value.Should().NotBeNull().And.BeOfType<Result>().And.BeEquivalentTo(expectedResult);
            _mediatorMock.Verify(m => m.Send(It.IsAny<RamittanceInformation>(), default), Times.Once());
        }


        [Fact]
        public async Task Send_ShouldReturnNotFound_WhenDataNotFound()
        {
            // Arrange
            Result response = null;
            _mediatorMock.Setup(x => x.Send(It.IsAny<RamittanceInformation>(), default)).ReturnsAsync(response);
            var command = _fixture.Create<RamittanceInformation>();
            // Act
            var result = await _sut.Ramittance(command).ConfigureAwait(false);
            // Assert
            result.Should().NotBeNull();
            //result.Result.Should().BeAssignableTo<NotFoundResult>();
            _mediatorMock.Verify(x => x.Send(It.IsAny<RamittanceInformation>(), default), Times.Once());
        }
    }
}