using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Products.Controllers;
using Products.DbContexts;
using Products.Entities;
using Products.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Tests
{
    public class AccountControllerTests
    {
        private readonly ProductContext _dbContext;
        private readonly AccountController _accountController;

        public AccountControllerTests()
        {
            var options = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _dbContext = new ProductContext(options);

            var mockConfig = new Mock<IConfiguration>();
            _accountController = new AccountController(_dbContext, mockConfig.Object);
        }

        [Fact]
        public async Task Register_ShouldCreateUser()
        {
            var userDto = new UserDto
            {
                Username = "testuser",
                Firstname = "Test",
                Email = "test@test.com",
                Password = "Test1234"
            };

            var result = await _accountController.Register(userDto);

            Assert.IsType<OkObjectResult>(result);
            Assert.Single(_dbContext.Users);
        }

        [Fact]
        public void Login_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            var loginPayload = new LoginPayload { Email = "notfound@test.com", Password = "wrong" };

            var result = _accountController.Login(loginPayload);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
