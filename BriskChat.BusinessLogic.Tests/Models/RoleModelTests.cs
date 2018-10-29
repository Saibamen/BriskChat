using BriskChat.BusinessLogic.Models;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Models
{
    public class RoleModelTests
    {
        [Fact]
        public void ValidData_ModelAreCorrect()
        {
            // prepare
            const string name = "name";
            const string description = "description";

            // action
            var action = new RoleModel
            {
                Name = name,
                Description = description
            };

            // check
            Assert.Equal("name", action.Name);
            Assert.Equal("description", action.Description);
        }
    }
}