using Bazirano.Controllers;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Bazirano.Tests.Controllers
{
    public class AuthorInterfaceControllerTests
    {
        //[Fact]
        //public void Index_NotAuthor_RedirectsToError()
        //{
        //    var repoMock = new Mock<IColumnRepository>();
        //    repoMock.Setup(x => x.Authors).Returns(new Author[] { new Author { Name = "Test"} }.AsQueryable);
        //    var controllerMock = new Mock<AuthorInterfaceController>(null, repoMock.Object);
        //    controllerMock.Setup(x => x.User.Identity.Name).Returns("Test");

        //    var result = (RedirectToActionResult)controllerMock.Object.Index();

        //    Assert.Equal("NotAuthor", result.ActionName);
        //    Assert.Equal("Error", result.ControllerName);
        //}
    }
}
