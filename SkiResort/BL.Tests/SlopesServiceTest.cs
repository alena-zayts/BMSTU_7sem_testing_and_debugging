using Moq;
using Xunit;
using BL.Services;
using BL.Models;
using BL.IRepositories;
using System.Collections.Generic;
using AutoFixture.Xunit2;

// лондонский вариант -- изоляция кода от зависимостей
// используется mock для: ICheckPermissionsService, ISlopesRepository, ILiftsSlopesRepository
// используется fixture: AutoMoqData (есть вариант и без него)

namespace BL.Tests
{
    public class SlopesServiceTest
    {
        // with fixture
        // тест вызывается с параметрами. какими -- автоматически сгенерированными
        [Theory, AutoMoqData] 
        public async void TestGetSlopeInfoAsync(
            uint userID, 
            Slope slope, 
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock, 
            [Frozen] Mock<ISlopesRepository> slopesRepositoryMock,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryMock,
            SlopesService sut)
        {
            // Arrange
            slopesRepositoryMock.Setup(m => m.GetSlopeByNameAsync(slope.SlopeName)).ReturnsAsync(slope);
            liftsSlopesRepositoryMock.Setup(m => m.GetLiftsBySlopeIdAsync(slope.SlopeID)).ReturnsAsync(slope.ConnectedLifts);

            // act
            Slope slopeFromService = await sut.GetSlopeInfoAsync(userID, slope.SlopeName);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            slopesRepositoryMock.Verify(m => m.GetSlopeByNameAsync(slope.SlopeName), Times.Once);
            liftsSlopesRepositoryMock.Verify(m => m.GetLiftsBySlopeIdAsync(slope.SlopeID), Times.Once);
            Assert.Equal(slope, slopeFromService);
        }

        [Theory, AutoMoqData] // тест вызывается с параметрами. какими -- автоматически сгенерированными
        public async void TestGetSlopesInfoAsync(
            uint userID,
            [Frozen] List<Slope> initialSlopes,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryMock,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryMock,
            SlopesService sut)
        {
            // arrange
            slopesRepositoryMock.Setup(m => m.GetSlopesAsync(It.IsAny<uint>(), It.IsAny<uint>())).ReturnsAsync(initialSlopes);
            List<Slope> slopes = new List<Slope> { };
            foreach (Slope slope in initialSlopes)
            {
                slopes.Add(new Slope(slope, new List<Lift> { }));
            }

            // act
            List<Slope> slopesFromService = await sut.GetSlopesInfoAsync(userID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            slopesRepositoryMock.Verify(m => m.GetSlopesAsync(It.IsAny<uint>(), It.IsAny<uint>()), Times.Once);
            liftsSlopesRepositoryMock.Verify(m => m.GetLiftsBySlopeIdAsync(It.IsAny<uint>()), Times.Exactly(slopes.Count));
            Assert.Equal(slopes.Count, slopesFromService.Count);
        }

        

        [Theory, AutoMoqData]
        public async void TestUpdateSlopeInfoAsync(
            uint userID,
            Slope slope,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryMock,
            SlopesService sut)
        {
            // arrange
            slopesRepositoryMock.Setup(m => m.GetSlopeByNameAsync(slope.SlopeName)).ReturnsAsync(slope);

            // act
            await sut.UpdateSlopeInfoAsync(userID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            //slopesRepositoryMock.Verify(m => m.GetSlopeByNameAsync(slope.SlopeName), Times.Once); не нужны детали реализации
            slopesRepositoryMock.Verify(m => m.UpdateSlopeByIDAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel), Times.Once);
        }

        [Theory, AutoMoqData]
        public async void TestAdminDeleteSlopeAsync(
            uint userID,
            Slope slope,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryMock,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryMock,
            SlopesService sut)
        {
            // arrange
            slopesRepositoryMock.Setup(m => m.GetSlopeByNameAsync(slope.SlopeName)).ReturnsAsync(slope);
            liftsSlopesRepositoryMock.Setup(m => m.GetLiftsSlopesAsync(It.IsAny<uint>(), It.IsAny<uint>())).ReturnsAsync(new List<LiftSlope> { });

            // act
            await sut.AdminDeleteSlopeAsync(userID, slope.SlopeName);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            //slopesRepositoryMock.Verify(m => m.GetSlopeByNameAsync(slope.SlopeName), Times.Once); не нужны детали реализации
            //liftsSlopesRepositoryMock.Verify(m => m.GetLiftsSlopesAsync(It.IsAny<uint>(), It.IsAny<uint>()), Times.Once);
            //liftsSlopesRepositoryMock.Verify(m => m.DeleteLiftSlopesByIDAsync(It.IsAny<uint>()), Times.Never);
            slopesRepositoryMock.Verify(m => m.DeleteSlopeByIDAsync(slope.SlopeID), Times.Once);
        }

        [Theory, AutoMoqData]
        public async void TestAdminAddAutoIncrementSlopeAsync(
            uint userID,
            Slope slope,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryMock,
            SlopesService sut)
        {
            // arrange
            {
                slopesRepositoryMock.Setup(m => m.AddSlopeAutoIncrementAsync(slope.SlopeName, slope.IsOpen, slope.DifficultyLevel)).ReturnsAsync(slope.SlopeID);
            }

            // act
            uint slopeIDFromService = await sut.AdminAddAutoIncrementSlopeAsync(userID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            slopesRepositoryMock.Verify(m => m.AddSlopeAutoIncrementAsync(slope.SlopeName, slope.IsOpen, slope.DifficultyLevel), Times.Once);
            Assert.Equal(slope.SlopeID, slopeIDFromService);
        }

        [Theory, AutoMoqData]
        public async void TestAdminAddSlopeAsync(
            uint userID,
            Slope slope,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryMock,
            SlopesService sut)
        {
            // arrange

            // act
            await sut.AdminAddSlopeAsync(userID, slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            slopesRepositoryMock.Verify(m => m.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel), Times.Once);
        }

        // если убрать frozen, то при verify проблемы

        //    // without Fixture
        //    [Fact]
        //    public async void TestGetSlopeInfoAsyncWithoutFixture()
        //    {
        //        // arrange
        //        uint userID = 0;
        //        Slope slope = new(slopeID: 0, slopeName: "A0", isOpen: true, difficultyLevel: 1);
        //        var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
        //        var slopesRepositoryMock = new Mock<ISlopesRepository>();
        //        var liftsSlopesRepositoryMock = new Mock<ILiftsSlopesRepository>();
        //        {
        //            slopesRepositoryMock.Setup(m => m.GetSlopeByNameAsync(slope.SlopeName)).ReturnsAsync(slope);
        //            liftsSlopesRepositoryMock.Setup(m => m.GetLiftsBySlopeIdAsync(slope.SlopeID)).ReturnsAsync(new List<Lift> { });
        //        }

        //        var sut = new SlopesService(checkPermissionServiceMock.Object, slopesRepositoryMock.Object, liftsSlopesRepositoryMock.Object);

        //        // act
        //        Slope slopeFromService = await sut.GetSlopeInfoAsync(userID, slope.SlopeName);

        //        //assert
        //        checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
        //        slopesRepositoryMock.Verify(m => m.GetSlopeByNameAsync(slope.SlopeName), Times.Once);
        //        liftsSlopesRepositoryMock.Verify(m => m.GetLiftsBySlopeIdAsync(slope.SlopeID), Times.Once);
        //        Assert.Equal(slope, slopeFromService);
        //    }
    }
}