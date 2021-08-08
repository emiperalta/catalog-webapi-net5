using System;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.Models;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Catalog.UnitTests
{
    public class ItemsControllerTests
    {
        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            var repositoryStub = new Mock<IItemsRepository>();
            repositoryStub
                .Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);
            var controller = new ItemsController(repositoryStub.Object);

            var result = await controller.GetItemAsync(Guid.NewGuid()); 

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
