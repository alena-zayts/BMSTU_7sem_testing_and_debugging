using Moq;
using Xunit;
using BL.Services;
using BL.Models;
using BL.IRepositories;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using Allure.Xunit.Attributes;

// ���������� ������� -- �������� ���� �� ������������
//*fixture � ������� AutoMoqData (����������� ������� AutoMoqDataAttribute, ����� � ����� ArrangeHelpers) (�� ���� Dummy)--��� ��������� �������� ��� ������
//* mock ���: ICheckPermissionsService, (������ � ��������������, ������� ����������� SUT � ��������� ��������)
//*stub ���: ISlopesRepository, ILiftsSlopesRepository(������ � ��������������, ������� ����������� SUT � ��������� ��������, ����� ��������� � ��������  ������)


namespace BL.Tests
{
    [AllureSuite("SlopesServiceSuite")]
    public class SlopesServiceTest
    {
        // with fixture
        // ���� ���������� � �����������. ������ -- ������������� ����������������
        [AllureXunitTheory]
        [AutoMoqData] 
        public async void TestGetSlopeInfoAsync(
            uint userID, 
            Slope slope, 
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock, 
            [Frozen] Mock<ISlopesRepository> slopesRepositoryStub,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryStub,
            SlopesService sut)
        {
            // Arrange
            slopesRepositoryStub.Setup(m => m.GetSlopeByNameAsync(slope.SlopeName)).ReturnsAsync(slope);
            liftsSlopesRepositoryStub.Setup(m => m.GetLiftsBySlopeIdAsync(slope.SlopeID)).ReturnsAsync(slope.ConnectedLifts);

            // act
            Slope slopeFromService = await sut.GetSlopeInfoAsync(userID, slope.SlopeName);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            slopesRepositoryStub.Verify(m => m.GetSlopeByNameAsync(slope.SlopeName), Times.Once);
            liftsSlopesRepositoryStub.Verify(m => m.GetLiftsBySlopeIdAsync(slope.SlopeID), Times.Once);
            Assert.Equal(slope, slopeFromService);
        }
        [AllureXunitTheory]
        [AutoMoqData] // ���� ���������� � �����������. ������ -- ������������� ����������������
        public async void TestGetSlopesInfoAsync(
            uint userID,
            [Frozen] List<Slope> initialSlopes,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryStub,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryStub,
            SlopesService sut)
        {
            // arrange
            slopesRepositoryStub.Setup(m => m.GetSlopesAsync(It.IsAny<uint>(), It.IsAny<uint>())).ReturnsAsync(initialSlopes);
            List<Slope> slopes = new List<Slope> { };
            foreach (Slope slope in initialSlopes)
            {
                slopes.Add(new Slope(slope, new List<Lift> { }));
            }

            // act
            List<Slope> slopesFromService = await sut.GetSlopesInfoAsync(userID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            slopesRepositoryStub.Verify(m => m.GetSlopesAsync(It.IsAny<uint>(), It.IsAny<uint>()), Times.Once);
            liftsSlopesRepositoryStub.Verify(m => m.GetLiftsBySlopeIdAsync(It.IsAny<uint>()), Times.Exactly(slopes.Count));
            Assert.Equal(slopes.Count, slopesFromService.Count);
        }


        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestUpdateSlopeInfoAsync(
            uint userID,
            Slope slope,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryStub,
            SlopesService sut)
        {
            // arrange
            slopesRepositoryStub.Setup(m => m.GetSlopeByNameAsync(slope.SlopeName)).ReturnsAsync(slope);

            // act
            await sut.UpdateSlopeInfoAsync(userID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            //slopesRepositoryStub.Verify(m => m.GetSlopeByNameAsync(slope.SlopeName), Times.Once); �� ����� ������ ����������
            slopesRepositoryStub.Verify(m => m.UpdateSlopeByIDAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel), Times.Once);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminDeleteSlopeAsync(
            uint userID,
            Slope slope,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryStub,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryStub,
            SlopesService sut)
        {
            // arrange
            slopesRepositoryStub.Setup(m => m.GetSlopeByNameAsync(slope.SlopeName)).ReturnsAsync(slope);
            liftsSlopesRepositoryStub.Setup(m => m.GetLiftsSlopesAsync(It.IsAny<uint>(), It.IsAny<uint>())).ReturnsAsync(new List<LiftSlope> { });

            // act
            await sut.AdminDeleteSlopeAsync(userID, slope.SlopeName);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            //slopesRepositoryStub.Verify(m => m.GetSlopeByNameAsync(slope.SlopeName), Times.Once); �� ����� ������ ����������
            //liftsSlopesRepositoryStub.Verify(m => m.GetLiftsSlopesAsync(It.IsAny<uint>(), It.IsAny<uint>()), Times.Once);
            //liftsSlopesRepositoryStub.Verify(m => m.DeleteLiftSlopesByIDAsync(It.IsAny<uint>()), Times.Never);
            slopesRepositoryStub.Verify(m => m.DeleteSlopeByIDAsync(slope.SlopeID), Times.Once);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminAddAutoIncrementSlopeAsync(
            uint userID,
            Slope slope,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryStub,
            SlopesService sut)
        {
            // arrange
            {
                slopesRepositoryStub.Setup(m => m.AddSlopeAutoIncrementAsync(slope.SlopeName, slope.IsOpen, slope.DifficultyLevel)).ReturnsAsync(slope.SlopeID);
            }

            // act
            uint slopeIDFromService = await sut.AdminAddAutoIncrementSlopeAsync(userID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            slopesRepositoryStub.Verify(m => m.AddSlopeAutoIncrementAsync(slope.SlopeName, slope.IsOpen, slope.DifficultyLevel), Times.Once);
            Assert.Equal(slope.SlopeID, slopeIDFromService);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminAddSlopeAsync(
            uint userID,
            Slope slope,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryStub,
            SlopesService sut)
        {
            // arrange

            // act
            await sut.AdminAddSlopeAsync(userID, slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            slopesRepositoryStub.Verify(m => m.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel), Times.Once);
        }

        // ���� ������ frozen, �� ��� verify ��������

        //    // without Fixture
        //    [AllureXunit]
        //    public async void TestGetSlopeInfoAsyncWithoutFixture()
        //    {
        //        // arrange
        //        uint userID = 0;
        //        Slope slope = new(slopeID: 0, slopeName: "A0", isOpen: true, difficultyLevel: 1);
        //        var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
        //        var slopesRepositoryStub = new Mock<ISlopesRepository>();
        //        var liftsSlopesRepositoryStub = new Mock<ILiftsSlopesRepository>();
        //        {
        //            slopesRepositoryStub.Setup(m => m.GetSlopeByNameAsync(slope.SlopeName)).ReturnsAsync(slope);
        //            liftsSlopesRepositoryStub.Setup(m => m.GetLiftsBySlopeIdAsync(slope.SlopeID)).ReturnsAsync(new List<Lift> { });
        //        }

        //        var sut = new SlopesService(checkPermissionServiceMock.Object, slopesRepositoryStub.Object, liftsSlopesRepositoryStub.Object);

        //        // act
        //        Slope slopeFromService = await sut.GetSlopeInfoAsync(userID, slope.SlopeName);

        //        //assert
        //        checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
        //        slopesRepositoryStub.Verify(m => m.GetSlopeByNameAsync(slope.SlopeName), Times.Once);
        //        liftsSlopesRepositoryStub.Verify(m => m.GetLiftsBySlopeIdAsync(slope.SlopeID), Times.Once);
        //        Assert.Equal(slope, slopeFromService);
        //    }
    }
}