using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using TrollChat.BusinessLogic.Actions.UserToken.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.UserToken
{
    [Collection("mapper")]
    public class GetUserTokenByUserIdTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var userTokenFromDb = new DataAccess.Models.UserToken()
            {
                Id = 1,
                SecretToken = "123",
                SecretTokenTimeStamp = DateTime.MaxValue,
                CreatedOn = DateTime.MinValue,
                ModifiedOn = DateTime.MinValue,
                DeletedOn = null,
                User = new DataAccess.Models.User() { Name = "bob" },
            };

            var findByResult = new List<DataAccess.Models.UserToken>() { userTokenFromDb };

            // prepare
            var userTokenRepository = new Mock<IUserTokenRepository>();
            userTokenRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()))
                .Returns(findByResult.AsQueryable());
            var action = new GetUserTokenByUserId(userTokenRepository.Object);

            // action
            var userToken = action.Invoke(1);

            // check
            Assert.Equal(1, userToken.Id);
            Assert.Equal("bob", userToken.User.Name);
            Assert.Equal("123", userToken.SecretToken);
            Assert.Equal(DateTime.MaxValue, userToken.SecretTokenTimeStamp);
            Assert.Equal(DateTime.MinValue, userToken.CreatedOn);
            Assert.Equal(DateTime.MinValue, userToken.ModifiedOn);
            Assert.Null(userToken.DeletedOn);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var action = new GetUserTokenByUserId(mockedUserTokenRepository.Object);

            // action
            var user = action.Invoke(1337);

            // check
            Assert.Null(user);
        }
    }
}