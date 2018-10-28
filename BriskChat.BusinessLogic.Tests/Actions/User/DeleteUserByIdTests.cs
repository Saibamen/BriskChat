using System;
using BriskChat.BusinessLogic.Actions.User.Implementations;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.User
{
    public class DeleteUserByIdTests
    {
        [Fact]
        public void Invoke_ValidData_DeleteAndSaveAreCalled()
        {
            // prepare
            var guid = Guid.NewGuid();
            var userFromDb = new DataAccess.Models.User
            {
                Id = guid
            };

            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetById(guid)).Returns(userFromDb);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteUserById(mockedUserRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(guid);

            // assert
            Assert.True(result);
            mockedUserRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.User>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_ValidData_DeleteNorSaveAreCalled()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteUserById(mockedUserRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(Guid.NewGuid());

            // assert
            Assert.False(result);
            mockedUserRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.User>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_DeleteNorSaveAreCalled()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteUserById(mockedUserRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(new Guid());

            // assert
            Assert.False(result);
            mockedUserRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.User>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
        }
    }
}