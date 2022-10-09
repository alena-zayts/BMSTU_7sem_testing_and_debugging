using Moq;
using Xunit;
using BL.Services;
using BL.Models;
using BL.IRepositories;
using System.Collections.Generic;
using AutoFixture.Xunit2;

// лондонский вариант -- изоляция кода от зависимостей
// используется mock для: ICheckPermissionsService, ILiftsRepository, ILiftsSlopesRepository
// используется fixture: AutoMoqData (есть вариант и без него)

namespace BL.Tests
{
    public class LiftsServiceTest
    {
        [Theory, AutoMoqData] 
        public async void TestGetLiftInfoAsync(
            uint userID, 
            Lift lift, 
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock, 
            [Frozen] Mock<ILiftsRepository> liftsRepositoryMock,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryMock,
            LiftsService sut)
        {
            // Arrange
            liftsRepositoryMock.Setup(m => m.GetLiftByNameAsync(lift.LiftName)).ReturnsAsync(lift);
            liftsSlopesRepositoryMock.Setup(m => m.GetSlopesByLiftIdAsync(lift.LiftID)).ReturnsAsync(lift.ConnectedSlopes);

            // act
            Lift liftFromService = await sut.GetLiftInfoAsync(userID, lift.LiftName);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            liftsRepositoryMock.Verify(m => m.GetLiftByNameAsync(lift.LiftName), Times.Once);
            liftsSlopesRepositoryMock.Verify(m => m.GetSlopesByLiftIdAsync(lift.LiftID), Times.Once);
            Assert.Equal(lift, liftFromService);
        }

        [Theory, AutoMoqData] // тест вызывается с параметрами. какими -- автоматически сгенерированными
        public async void TestGetLiftsInfoAsync(
            uint userID,
            [Frozen] List<Lift> initialLifts,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ILiftsRepository> liftsRepositoryMock,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryMock,
            LiftsService sut)
        {
            // arrange
            liftsRepositoryMock.Setup(m => m.GetLiftsAsync(It.IsAny<uint>(), It.IsAny<uint>())).ReturnsAsync(initialLifts);
            List<Lift> lifts = new List<Lift> { };
            foreach (Lift lift in initialLifts)
            {
                lifts.Add(new Lift(lift, new List<Slope> { }));
            }

            // act
            List<Lift> liftsFromService = await sut.GetLiftsInfoAsync(userID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            liftsRepositoryMock.Verify(m => m.GetLiftsAsync(It.IsAny<uint>(), It.IsAny<uint>()), Times.Once);
            liftsSlopesRepositoryMock.Verify(m => m.GetSlopesByLiftIdAsync(It.IsAny<uint>()), Times.Exactly(lifts.Count));
            Assert.Equal(lifts.Count, liftsFromService.Count);
        }

        

        [Theory, AutoMoqData]
        public async void TestUpdateLiftInfoAsync(
            uint userID,
            Lift lift,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ILiftsRepository> liftsRepositoryMock,
            LiftsService sut)
        {
            // arrange
            liftsRepositoryMock.Setup(m => m.GetLiftByNameAsync(lift.LiftName)).ReturnsAsync(lift);

            // act
            await sut.UpdateLiftInfoAsync(userID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            liftsRepositoryMock.Verify(m => m.UpdateLiftByIDAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime), Times.Once);
        }

        [Theory, AutoMoqData]
        public async void TestAdminDeleteLiftAsync(
            uint userID,
            Lift lift,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ILiftsRepository> liftsRepositoryMock,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryMock,
            LiftsService sut)
        {
            // arrange
            liftsRepositoryMock.Setup(m => m.GetLiftByNameAsync(lift.LiftName)).ReturnsAsync(lift);
            liftsSlopesRepositoryMock.Setup(m => m.GetLiftsSlopesAsync(It.IsAny<uint>(), It.IsAny<uint>())).ReturnsAsync(new List<LiftSlope> { });

            // act
            await sut.AdminDeleteLiftAsync(userID, lift.LiftName);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            liftsRepositoryMock.Verify(m => m.DeleteLiftByIDAsync(lift.LiftID), Times.Once);
        }

        [Theory, AutoMoqData]
        public async void TestAdminAddAutoIncrementLiftAsync(
            uint userID,
            Lift lift,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ILiftsRepository> liftsRepositoryMock,
            LiftsService sut)
        {
            // arrange
            {
                liftsRepositoryMock.Setup(m => m.AddLiftAutoIncrementAsync(lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime)).ReturnsAsync(lift.LiftID);
            }

            // act
            uint liftIDFromService = await sut.AdminAddAutoIncrementLiftAsync(userID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            liftsRepositoryMock.Verify(m => m.AddLiftAutoIncrementAsync(lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime), Times.Once);
            Assert.Equal(lift.LiftID, liftIDFromService);
        }
    }
}