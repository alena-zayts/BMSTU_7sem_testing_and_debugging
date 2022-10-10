using Moq;
using Xunit;
using BL.Services;
using BL.Models;
using BL.IRepositories;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using Allure.Xunit.Attributes;

// лондонский вариант -- изоляция кода от зависимостей
// используется mock для: ICheckPermissionsService, (вызовы и взаимодействия, которые исполняются SUT к зависимым объектам)
// используется stub для: ITurnstilesRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные)
// используется fixture: AutoMoqData (по сути Dummy)

namespace BL.Tests
{
    [AllureSuite("TurnstilesServiceSuite")]
    public class TurnstilesServiceTest
    {
        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminGetTurnstileAsync(
            uint userID,
            Turnstile turnstile,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ITurnstilesRepository> turnstilesRepositoryStub,
            TurnstilesService sut)
        {
            // Arrange
            turnstilesRepositoryStub.Setup(m => m.GetTurnstileByIdAsync(turnstile.TurnstileID)).ReturnsAsync(turnstile);

            // act
            Turnstile turnstileFromService = await sut.AdminGetTurnstileAsync(userID, turnstile.TurnstileID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            turnstilesRepositoryStub.Verify(m => m.GetTurnstileByIdAsync(turnstile.TurnstileID), Times.Once);
            Assert.Equal(turnstile, turnstileFromService);
        }

        [AllureXunitTheory]
        [AutoMoqData] 
        public async void TestAdminGetTurnstilesAsync(
            uint userID,
            [Frozen] List<Turnstile> initialTurnstiles,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ITurnstilesRepository> turnstilesRepositoryStub,
            TurnstilesService sut)
        {
            // arrange
            turnstilesRepositoryStub.Setup(m => m.GetTurnstilesAsync(It.IsAny<uint>(), It.IsAny<uint>())).ReturnsAsync(initialTurnstiles);

            // act
            List<Turnstile> turnstilesFromService = await sut.AdminGetTurnstilesAsync(userID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            turnstilesRepositoryStub.Verify(m => m.GetTurnstilesAsync(It.IsAny<uint>(), It.IsAny<uint>()), Times.Once);
            Assert.Equal(initialTurnstiles.Count, turnstilesFromService.Count);
        }



        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminUpdateTurnstileAsync(
            uint userID,
            Turnstile turnstile,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ITurnstilesRepository> turnstilesRepositoryStub,
            TurnstilesService sut)
        {
            // arrange
            turnstilesRepositoryStub.Setup(m => m.GetTurnstileByIdAsync(turnstile.TurnstileID)).ReturnsAsync(turnstile);

            // act
            await sut.AdminUpdateTurnstileAsync(userID, turnstile.TurnstileID, turnstile.LiftID, turnstile.IsOpen);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            turnstilesRepositoryStub.Verify(m => m.UpdateTurnstileByIDAsync(turnstile.TurnstileID, turnstile.LiftID, turnstile.IsOpen), Times.Once);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminDeleteTurnstileAsync(
            uint userID,
            Turnstile turnstile,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ITurnstilesRepository> turnstilesRepositoryStub,
            TurnstilesService sut)
        {
            // arrange
            turnstilesRepositoryStub.Setup(m => m.GetTurnstileByIdAsync(turnstile.TurnstileID)).ReturnsAsync(turnstile);

            // act
            await sut.AdminDeleteTurnstileAsync(userID, turnstile.TurnstileID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            turnstilesRepositoryStub.Verify(m => m.DeleteTurnstileByIDAsync(turnstile.TurnstileID), Times.Once);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminAddAutoIncrementTurnstileAsync(
            uint userID,
            Turnstile turnstile,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ITurnstilesRepository> turnstilesRepositoryStub,
            TurnstilesService sut)
        {
            // arrange
            {
                turnstilesRepositoryStub.Setup(m => m.AddTurnstileAutoIncrementAsync(turnstile.LiftID, turnstile.IsOpen)).ReturnsAsync(turnstile.TurnstileID);
            }

            // act
            uint turnstileIDFromService = await sut.AdminAddAutoIncrementTurnstileAsync(userID, turnstile.LiftID, turnstile.IsOpen);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            turnstilesRepositoryStub.Verify(m => m.AddTurnstileAutoIncrementAsync(turnstile.LiftID, turnstile.IsOpen), Times.Once);
            Assert.Equal(turnstile.TurnstileID, turnstileIDFromService);
        }
    }
}