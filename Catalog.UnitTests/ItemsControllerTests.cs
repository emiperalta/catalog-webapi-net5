using System;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.Dtos;
using Catalog.Api.Models;
using Catalog.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Catalog.UnitTests
{
    public class ItemsControllerTests
    {
        private readonly Mock<IItemsRepository> repositoryStub = new();
        private readonly Random rand = new();

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

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            repositoryStub
                .Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);
            var controller = new ItemsController(repositoryStub.Object);

            var result = await controller.GetItemAsync(Guid.NewGuid()); 

            result.Result.Should().BeOfType<NotFoundResult>();
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

            result.Value.Should().BeEquivalentTo(expectedItem, opt => opt.ComparingByMembers<Item>());
        }

        [Fact]
        public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
        {
            var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
            repositoryStub
                .Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(expectedItems);
            var controller = new ItemsController(repositoryStub.Object);

            var result = await controller.GetItemsAsync();

            result.Should().BeEquivalentTo(expectedItems, opt => opt.ComparingByMembers<Item>());
        }

        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            var itemToCreate = new CreateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), rand.Next(1000));
            var controller = new ItemsController(repositoryStub.Object);

            var result = await controller.CreateItemAsync(itemToCreate);

            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
            itemToCreate.Should().BeEquivalentTo(
                createdItem, 
                opt => opt.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );
            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
        }

        [Fact]
        public async Task UpdateItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            repositoryStub
                .Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);
            var controller = new ItemsController(repositoryStub.Object);

            var result = await controller.GetItemAsync(Guid.NewGuid()); 

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        {
            var existingItem = CreateRandomItem();
            repositoryStub
                .Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);
            var controller = new ItemsController(repositoryStub.Object);
            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDto(Guid.NewGuid().ToString(),Guid.NewGuid().ToString(),existingItem.Price + 3);

            var result = await controller.UpdateItemAsync(itemId, itemToUpdate); 

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            repositoryStub
                .Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);
            var controller = new ItemsController(repositoryStub.Object);

            var result = await controller.GetItemAsync(Guid.NewGuid()); 

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteItemAsync_WithExistingItem_ReturnsNotFound()
        {
            var existingItem = CreateRandomItem();
            repositoryStub
                .Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);
            var controller = new ItemsController(repositoryStub.Object);
            var itemId = existingItem.Id;

            var result = await controller.DeleteItemAsync(itemId); 

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
