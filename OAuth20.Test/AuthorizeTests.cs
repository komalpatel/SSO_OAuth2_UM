
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OAuth20.Server.Controllers;
using System.Security.Claims;
using System.Text.Json;

namespace OAuth20.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WhenGetIsCalled_ThenAListOfCustomersIsReturned()
        {
            //var returnedCustomers = new string[] { "John Doe", "Jane Doe" };

            //var claims = new List<Claim>()
            //{
            //    new Claim(ClaimTypes.Name, "johndoe"),
            //    new Claim(ClaimTypes.NameIdentifier, "1122"),
            //    new Claim(ClaimTypes.Role, "Admin"),
            //    new Claim("firstname", "John"),
            //    new Claim("lastname", "Doe")
            //};
            //var identity = new ClaimsIdentity(claims, "Test");
            //var claimsPrincipal = new ClaimsPrincipal(identity);

            //var mockHttpContext = new Mock<HttpContext>();
            //mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);

            //var httpContextAccessor = new Mock<IHttpContextAccessor>();
            //httpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

            //var service = new Mock<ICustomerService>();
            //service.Setup(m => m.GetCustomers()).Returns(returnedCustomers);

            //var controller = new AccountsController(httpContextAccessor.Object, service.Object);
            //controller.ControllerContext = new ControllerContext()
            //{
            //    HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            //};

            //var result = controller.Get();

            //var resultJson = JsonSerializer.Serialize(result);
            //var initialJson = JsonSerializer.Serialize(returnedCustomers);
            //Assert.That(initialJson, Is.EqualTo(resultJson));
        }

    }
}