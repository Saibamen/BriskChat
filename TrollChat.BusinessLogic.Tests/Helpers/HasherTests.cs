using TrollChat.BusinessLogic.Helpers.Implementations;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Helpers
{
    public class HasherTests
    {
        [Fact]
        public void CreateHash_ForValidInput_ReturnsCorrectHash()
        {
            const string password = "Other test data";
            var hasher = new Hasher();
            var salt = hasher.GenerateRandomSalt();
            var hashedPassowrd = hasher.CreatePasswordHash(password, salt);
            Assert.NotNull(hashedPassowrd);
        }

        [Theory]
        [InlineData("Some test data", "oEFSqItixaXfePO4H7jck9DWFcuDfc/HXpZA1NGE7UQ+IDqRqiDqBNwh5w4yYY3kNPpPY8d2q4+vtpbuQLwo3lUJfnFvL5ruEXBE3BzomYevCwpVdrvE4nFhGxli4U7b", "8YqMq4gcEpSXcY7kB9axZMiThQvKpqdSPxDHjEUJXXTPeICIXEBEnNrF5NxOzRpkXBrV3UTi28eZaXl1dZX43xPJjaiMcvoUpdb5roE5eDM1t6mMlvqHuHxKSGZ/B2NPZ5y3WOrDc3TGm6kpVH8AP1iA5ueIMfvX+4qyPxEOsQSR+g6X3/EUdqFRgWVRC6TxHeHsZ1d7MjFtE/V09a4gBV5UnlGDW7iayaRn+a2KIPpK4woD/XYNA9dxiUERLH5z")]
        [InlineData("Other test data", "RxkXpwEOSyXp3H/2ZejyGD5CBxu2F43+CSn3aJmM3w9x6bEc49qZymH3ztysDJJz6gTKmPaUaz1CfjrSSM2lx425IqPf7KKuD/11yLC6UphASe3gRxqPlDwCTDZqmhEf", "QGt7UIL8FdJT/4VSl9Lyiu6Kr+VyheUT6O5NtzA3BOvTfhnETC4s0uZNRJrxV2jiE8PHEBjIFAFwQrGJNY4o01Nj403/vK+9SRVm33U4olTMJp0NBM/mzJN53kOV66z4S8bkWGFhX+bDr7nxZnwuIA0oz5nB1Wf54tub+b2eLf3pp/r+xj9bWtYcagyLImHWEMIV4LbQm72P4m7qDwepZkE9zAYQUdtNrr9fbUscGFnxMd/9OvQzCtyRb/P145P+")]
        public void ValidateHash_ForValidInput_ReturnsTrue(string password, string salt, string expectedHashedPassword)
        {
            var hasher = new Hasher();
            var hashedPassowrd = hasher.CreatePasswordHash(password, salt);
            Assert.Equal(hashedPassowrd, expectedHashedPassword);
        }

        [Fact]
        public void ValidateHash_ForInvalidInput_ReturnsNull()
        {
            const string password = "";
            var hasher = new Hasher();
            var salt = hasher.GenerateRandomSalt();
            var hashedPassowrd = hasher.CreatePasswordHash(password, salt);
            Assert.Null(hashedPassowrd);
        }
    }
}