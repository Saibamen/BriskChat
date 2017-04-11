using TrollChat.BusinessLogic.Models;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Models
{
    public class RoleModelTests
    {
        [Fact]
        public void ValidData_ModelAreCorrect()
        {
            // prepare
            var name = "name";
            var description = "description";

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