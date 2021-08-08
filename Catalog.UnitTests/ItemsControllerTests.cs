using System;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.Dtos;
using Catalog.Api.Models;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Catalog.UnitTests
{
    public class ItemsControllerTests
    {
        private readonly Mock<IItemsRepository> repositoryStub = new();
        private readonly Random rand = new();

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            repositoryStub
                .Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);
            var controller = new ItemsController(repositoryStub.Object);

            var result = await controller.GetItemAsync(Guid.NewGuid()); 

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {
            var expectedItem = CreateRandomItem();
            repositoryStub
                .Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);
            var controller = new ItemsController(repositoryStub.Object);

            var result = await controller.GetItemAsync(Guid.NewGuid());

            Assert.IsType<ItemDto>(result.Value);    
            var dto = (result as ActionResult<ItemDto>).Value;
            Assert.Equal(expectedItem.Id, dto.Id);
            Assert.Equal(expectedItem.Name, dto.Name);
            Assert.Equal(expectedItem.Price, dto.Price);
            Assert.Equal(expectedItem.CreatedDate, dto.CreatedDate);
        }

        private Item CreateRandomItem()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
