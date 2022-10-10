using Moq;
using Xunit;
using BL.Services;
using BL.Models;
using BL.IRepositories;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using Allure.Xunit.Attributes;
using BL.Tests.ArrangeHelpers;

// классический подход -- изоляция тестов
// код не изолируется от зависимостей внутри unit-а (то есть В отличие от других тестов, НЕ использутеся mock: ICheckPermissionService)
// код изолируется от shared-зависимостей. Для этого используются stub-ы: ILiftsRepository, ISlopesRepository, ILiftsSlopesRepository, IUsersRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные)
// используется Fabric (Object Mother) для генерации объектов для тестов
// используется fixture: AutoMoqData (по сути Dummy)

namespace BL.Tests
{
    [AllureSuite("LiftsSlopesServiceSuite")]
    public class LiftsSlopesServiceTests
    {
        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetLiftsSlopesInfoAsync(
            List<LiftSlope> liftsSlopes,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryStub,
            [Frozen] Mock<ILiftsRepository> liftsRepositoryStub,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryStub
            )
        {
            // Arrange
            liftsSlopesRepositoryStub.Setup(m => m.GetLiftsSlopesAsync(It.IsAny<uint>(), It.IsAny<uint>())).ReturnsAsync(liftsSlopes);

            var user = UsersObjectMother.AdminUser();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            usersRepositoryStub.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            var checkPermissionsService = new CheckPermissionsService(usersRepositoryStub.Object);

            LiftsSlopesService sut = new(checkPermissionsService, liftsSlopesRepositoryStub.Object, liftsRepositoryStub.Object, slopesRepositoryStub.Object);

            // act
            var liftsSlopesFromService = await sut.GetLiftsSlopesInfoAsync(user.UserID);

            //assert
            liftsSlopesRepositoryStub.Verify(m => m.GetLiftsSlopesAsync(It.IsAny<uint>(), It.IsAny<uint>()), Times.Once);
            Assert.Equal(liftsSlopes, liftsSlopesFromService);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminDeleteLiftSlopeAsync(
            Lift lift, 
            Slope slope,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryStub,
            [Frozen] Mock<ILiftsRepository> liftsRepositoryStub,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryStub
            )
        {
            // Arrange
            liftsRepositoryStub.Setup(m => m.GetLiftByNameAsync(lift.LiftName)).ReturnsAsync(lift);
            slopesRepositoryStub.Setup(m => m.GetSlopeByNameAsync(slope.SlopeName)).ReturnsAsync(slope);

            var user = UsersObjectMother.AdminUser();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            usersRepositoryStub.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            var checkPermissionsService = new CheckPermissionsService(usersRepositoryStub.Object);
           
            LiftsSlopesService sut = new(checkPermissionsService, liftsSlopesRepositoryStub.Object, liftsRepositoryStub.Object, slopesRepositoryStub.Object);

            // act
            await sut.AdminDeleteLiftSlopeAsync(user.UserID, lift.LiftName, slope.SlopeName);

            //assert
            liftsSlopesRepositoryStub.Verify(m => m.DeleteLiftSlopesByIDsAsync(lift.LiftID, slope.SlopeID), Times.Once);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminAddAutoIncrementLiftSlopeAsync(
            Lift lift,
            Slope slope,
            [Frozen] Mock<ISlopesRepository> slopesRepositoryStub,
            [Frozen] Mock<ILiftsRepository> liftsRepositoryStub,
            [Frozen] Mock<ILiftsSlopesRepository> liftsSlopesRepositoryStub
            )
        {
            // Arrange
            liftsRepositoryStub.Setup(m => m.GetLiftByNameAsync(lift.LiftName)).ReturnsAsync(lift);
            slopesRepositoryStub.Setup(m => m.GetSlopeByNameAsync(slope.SlopeName)).ReturnsAsync(slope);

            var user = UsersObjectMother.AdminUser();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            usersRepositoryStub.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            var checkPermissionsService = new CheckPermissionsService(usersRepositoryStub.Object);

            LiftsSlopesService sut = new(checkPermissionsService, liftsSlopesRepositoryStub.Object, liftsRepositoryStub.Object, slopesRepositoryStub.Object);

            // act
            await sut.AdminAddAutoIncrementLiftSlopeAsync(user.UserID, lift.LiftName, slope.SlopeName);

            //assert
            liftsSlopesRepositoryStub.Verify(m => m.AddLiftSlopeAutoIncrementAsync(lift.LiftID, slope.SlopeID), Times.Once);
        }
    }
}
