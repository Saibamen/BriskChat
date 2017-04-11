using System;
using TrollChat.BusinessLogic.Models;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Models
{
    public class BaseModelTests
    {
        [Fact]
        public void ValidData_ModelAreCorrect()
        {
            // prepare
            var id = 5;
            var createdOn = DateTime.Today;
            var modifiedOn = DateTime.Today.AddDays(4);
            var deletedOn = DateTime.Today.AddDays(5);

            // action
            var action = new BaseModel
            {
                Id = id,
                CreatedOn = createdOn,
                ModifiedOn = modifiedOn,
                DeletedOn = deletedOn
            };

            // check
            Assert.Equal(5, action.Id);
            Assert.Equal(DateTime.Today, action.CreatedOn);
            Assert.Equal(DateTime.Today.AddDays(4), action.ModifiedOn);
            Assert.Equal(DateTime.Today.AddDays(5), action.DeletedOn);
            Assert.True(action.IsValid());
        }
    }
}