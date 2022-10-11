using Moq;
using Xunit;
using BL.Services;
using BL.Models;
using BL.IRepositories;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using Allure.Xunit.Attributes;

// лондонский вариант -- изол€ци€ кода от зависимостей
//*fixture с помощью AutoMoqData (собственный атрибут AutoMoqDataAttribute, лежит в папке ArrangeHelpers) (по сути Dummy)--дл€ генерации объектов дл€ тестов
//* mock дл€: ICheckPermissionsService, (вызовы и взаимодействи€, которые исполн€ютс€ SUT к зависимым объектам)
//*stub дл€: ILiftsRepository, ILiftsSlopesRepository(вызовы и взаимодействи€, которые исполн€ютс€ SUT к зависимым объектам, чтобы запросить и получить  данные)


namespace BL.Tests
{
    [AllureSuite("LiftsServiceSuite")]
    public class LiftsServiceTest
    {
        [AllureXunitTheory] 
        [AutoMoqData]
        public async void TestGetLiftInfoAsync(
            uint userID,
            Lift lift,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ILiftsRepository> liftsRepositoryStub,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryStub,
            LiftsService sut)
        {
            // Arrange
            liftsRepositoryStub.Setup(m => m.GetLiftByNameAsync(lift.LiftName)).ReturnsAsync(lift);
            liftsSlopesRepositoryStub.Setup(m => m.GetSlopesByLiftIdAsync(lift.LiftID)).ReturnsAsync(lift.ConnectedSlopes);

            // act
            Lift liftFromService = await sut.GetLiftInfoAsync(userID, lift.LiftName);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            liftsRepositoryStub.Verify(m => m.GetLiftByNameAsync(lift.LiftName), Times.Once);
            liftsSlopesRepositoryStub.Verify(m => m.GetSlopesByLiftIdAsync(lift.LiftID), Times.Once);
            Assert.Equal(lift, liftFromService);
        }

        [AllureXunitTheory]
        [AutoMoqData] // тест вызываетс€ с параметрами. какими -- автоматически сгенерированными
        public async void TestGetLiftsInfoAsync(
            uint userID,
            [Frozen] List<Lift> initialLifts,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ILiftsRepository> liftsRepositoryStub,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryStub,
            LiftsService sut)
        {
            // arrange
            liftsRepositoryStub.Setup(m => m.GetLiftsAsync(It.IsAny<uint>(), It.IsAny<uint>())).ReturnsAsync(initialLifts);
            List<Lift> lifts = new List<Lift> { };
            foreach (Lift lift in initialLifts)
            {
                lifts.Add(new Lift(lift, new List<Slope> { }));
            }

            // act
            List<Lift> liftsFromService = await sut.GetLiftsInfoAsync(userID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            liftsRepositoryStub.Verify(m => m.GetLiftsAsync(It.IsAny<uint>(), It.IsAny<uint>()), Times.Once);
            liftsSlopesRepositoryStub.Verify(m => m.GetSlopesByLiftIdAsync(It.IsAny<uint>()), Times.Exactly(lifts.Count));
            Assert.Equal(lifts.Count, liftsFromService.Count);
        }



        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestUpdateLiftInfoAsync(
            uint userID,
            Lift lift,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ILiftsRepository> liftsRepositoryStub,
            LiftsService sut)
        {
            // arrange
            liftsRepositoryStub.Setup(m => m.GetLiftByNameAsync(lift.LiftName)).ReturnsAsync(lift);

            // act
            await sut.UpdateLiftInfoAsync(userID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            liftsRepositoryStub.Verify(m => m.UpdateLiftByIDAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime), Times.Once);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminDeleteLiftAsync(
            uint userID,
            Lift lift,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ILiftsRepository> liftsRepositoryStub,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryStub,
            LiftsService sut)
        {
            // arrange
            liftsRepositoryStub.Setup(m => m.GetLiftByNameAsync(lift.LiftName)).ReturnsAsync(lift);
            liftsSlopesRepositoryStub.Setup(m => m.GetLiftsSlopesAsync(It.IsAny<uint>(), It.IsAny<uint>())).ReturnsAsync(new List<LiftSlope> { });

            // act
            await sut.AdminDeleteLiftAsync(userID, lift.LiftName);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            liftsRepositoryStub.Verify(m => m.DeleteLiftByIDAsync(lift.LiftID), Times.Once);
        }

        [AllureXunitTheory] 
        [AutoMoqData]
        public async void TestAdminAddAutoIncrementLiftAsync(
            uint userID,
            Lift lift,
            [Frozen] Mock<ICheckPermissionService> checkPermissionServiceMock,
            [Frozen] Mock<ILiftsRepository> liftsRepositoryStub,
            LiftsService sut)
        {
            // arrange
            {
                liftsRepositoryStub.Setup(m => m.AddLiftAutoIncrementAsync(lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime)).ReturnsAsync(lift.LiftID);
            }

            // act
            uint liftIDFromService = await sut.AdminAddAutoIncrementLiftAsync(userID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            liftsRepositoryStub.Verify(m => m.AddLiftAutoIncrementAsync(lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime), Times.Once);
            Assert.Equal(lift.LiftID, liftIDFromService);
        }
    }
}