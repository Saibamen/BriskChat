using System;
using BriskChat.BusinessLogic.Models;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Models
{
    public class BaseModelTests
    {
        [Fact]
        public void ValidData_ModelAreCorrect()
        {
            // prepare
            var guid = new Guid();
            var createdOn = DateTime.Today;
            var modifiedOn = DateTime.Today.AddDays(4);
            var deletedOn = DateTime.Today.AddDays(5);

            // action
            var action = new BaseModel
            {
                Id = guid,
                CreatedOn = createdOn,
                ModifiedOn = modifiedOn,
                DeletedOn = deletedOn
            };

            // check
            Assert.Equal(guid, action.Id);
            Assert.Equal(DateTime.Today, action.CreatedOn);
            Assert.Equal(DateTime.Today.AddDays(4), action.ModifiedOn);
            Assert.Equal(DateTime.Today.AddDays(5), action.DeletedOn);
            Assert.True(action.IsValid());
        }
    }
}