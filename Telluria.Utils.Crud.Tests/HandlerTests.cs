using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Commands.BaseCommands;
using Xunit;

namespace Telluria.Utils.Crud.Tests
{
  public class HandlerTests
  {
    private readonly CustomHandler _sut;
    private readonly Mock<ICustomRepository> _repoMock = new();

    private static readonly List<CustomEntity> _entityMockList = new()
    {
      new() { CreatedAt = DateTime.UtcNow.AddDays(-5), UpdatedAt = DateTime.UtcNow },
      new() { CreatedAt = DateTime.UtcNow.AddDays(-5) },
      new() { CreatedAt = DateTime.UtcNow.AddDays(-3), UpdatedAt = DateTime.UtcNow },
      new() { CreatedAt = DateTime.UtcNow.AddDays(-2) },
      new() { CreatedAt = DateTime.UtcNow.AddDays(-2), UpdatedAt = DateTime.UtcNow },
      new() { CreatedAt = DateTime.UtcNow.AddDays(-1) },
      new() { CreatedAt = DateTime.UtcNow }
    };

    public HandlerTests()
    {
      _sut = new(_repoMock.Object);
    }

    [Fact]
    public async Task BaseFindCommand_ShouldReturnEntityCommandResult_WhenFilterHasAMatch()
    {
      // Arrange
      var entity = new CustomEntity();
      var command = new BaseFindCommand<CustomEntity>(x => x.Id == entity.Id, null, default);
      _repoMock.Setup(x => x.FindAsync(false, command.Where, null, default)).ReturnsAsync(entity);

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.SUCCESS);
      result.Result.Should().NotBeNull();
      result.Result?.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task BaseFindCommand_ShouldReturnAlertCommandResult_WhenFilterDoesNotHaveAMatch()
    {
      // Arrange
      var command = new BaseFindCommand<CustomEntity>(x => x.Id == Guid.NewGuid(), null, default);
      _repoMock.Setup(x => x.FindAsync(false, command.Where, null, default))
        .ReturnsAsync(default(CustomEntity)!);

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.ALERT);
      result.Result.Should().BeNull();
    }

    [Fact]
    public async Task BaseFindCommand_ShouldReturnErrorCommandResult_WhenAnyErrorOccurs()
    {
      // Arrange
      var command = new BaseFindCommand<CustomEntity>(x => x.Id == Guid.NewGuid(), null, default);
      var errorMessage = "something is wrong";
      _repoMock.Setup(x => x.FindAsync(false, command.Where, null, default))
        .ThrowsAsync(new Exception(errorMessage));

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.ERROR);
      result.Result.Should().BeNull();
      result.Message.Should().Contain(errorMessage);
    }

    [Fact]
    public async Task BaseGetCommand_ShouldReturnEntityCommandResult_WhenIdIsFound()
    {
      // Arrange
      var entity = new CustomEntity();
      var command = new BaseGetCommand(entity.Id, null, default);
      _repoMock.Setup(x => x.GetAsync(entity.Id, false, null, default)).ReturnsAsync(entity);

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.SUCCESS);
      result.Result.Should().NotBeNull();
      result.Result?.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task BaseGetCommand_ShouldReturnAlertCommandResult_WhenIdIsNotFound()
    {
      // Arrange
      var command = new BaseGetCommand(Guid.NewGuid(), null, default);
      _repoMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), false, null, default))
        .ReturnsAsync(default(CustomEntity)!);

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.ALERT);
      result.Result.Should().BeNull();
    }

    [Fact]
    public async Task BaseGetCommand_ShouldReturnErrorCommandResult_WhenAnyErrorOccurs()
    {
      // Arrange
      var command = new BaseGetCommand(Guid.NewGuid(), null, default);
      var errorMessage = "something is wrong";
      _repoMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), false, null, default))
        .ThrowsAsync(new Exception(errorMessage));

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.ERROR);
      result.Result.Should().BeNull();
      result.Message.Should().Contain(errorMessage);
    }

    [Fact]
    public async Task BaseCreateCommand_ShouldReturnEntityCommandResult_WhenSuccessfullyCreated()
    {
      // Arrange
      var entity = new CustomEntity();
      var command = new BaseCreateCommand<CustomEntity>(entity, null, default);
      _repoMock.Setup(x => x.AddAsync(entity, default)).Returns(Task.CompletedTask);
      _repoMock.Setup(x => x.GetAsync(entity.Id, false, null, default)).ReturnsAsync(entity);

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.SUCCESS);
      result.Result.Should().NotBeNull();
      result.Result?.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task BaseCreateCommand_ShouldReturnAlertCommandResult_WhenValidationFail()
    {
      // Arrange
      var entity = new CustomEntity { Id = Guid.Empty };
      var command = new BaseCreateCommand<CustomEntity>(entity, null, default);

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.ALERT);
      result.Result.Should().BeNull();
    }

    [Fact]
    public async Task BaseCreateCommand_ShouldReturnErrorCommandResult_WhenAnyErrorOccurs()
    {
      // Arrange
      var entity = new CustomEntity();
      var command = new BaseCreateCommand<CustomEntity>(entity, null, default);
      var errorMessage = "something is wrong";
      _repoMock.Setup(x => x.AddAsync(entity, default)).ThrowsAsync(new Exception(errorMessage));

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.ERROR);
      result.Result.Should().BeNull();
      result.Message.Should().Contain(errorMessage);
    }

    [Fact]
    public async Task BaseCreateManyCommand_ShouldReturnEntityCommandResult_WhenSuccessfullyCreated()
    {
      // Arrange
      var entities = new CustomEntity[] { new(), new() };
      var command = new BaseCreateManyCommand<CustomEntity>(entities, null, default);
      var ids = entities.Select(x => x.Id);
      _repoMock.Setup(x => x.AddAsync(entities, default)).Returns(Task.CompletedTask);
      _repoMock
        .Setup(x => x.ListAsync(false, It.IsAny<Expression<Func<CustomEntity, bool>>>(), null, default))
        .ReturnsAsync(entities);

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.SUCCESS);
      result.Result.Should().NotBeNull();
      result.Result?.Count().Should().Be(entities.Length);
      result.Result?.Select(x => x.Id).Should().BeEquivalentTo(ids);
    }

    [Fact]
    public async Task BaseCreateManyCommand_ShouldReturnAlertCommandResult_WhenAnyValidationFail()
    {
      // Arrange
      var entities = new CustomEntity[] { new(), new() { Id = Guid.Empty } };
      var command = new BaseCreateManyCommand<CustomEntity>(entities, null, default);

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.ALERT);
      result.Result.Should().BeNull();
    }

    [Fact]
    public async Task BaseCreateManyCommand_ShouldReturnErrorCommandResult_WhenAnyErrorOccurs()
    {
      // Arrange
      var entities = new CustomEntity[] { new(), new() };
      var command = new BaseCreateManyCommand<CustomEntity>(entities, null, default);
      var errorMessage = "something is wrong";
      _repoMock.Setup(x => x.AddAsync(entities, default)).ThrowsAsync(new Exception(errorMessage));

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.ERROR);
      result.Result.Should().BeNull();
      result.Message.Should().Contain(errorMessage);
    }

    [Fact]
    public async Task BaseUpdateCommand_ShouldReturnEntityCommandResult_WhenSuccessfullyUpdated()
    {
      // Arrange
      var entity = new CustomEntity();
      var command = new BaseUpdateCommand<CustomEntity>(entity, null, default);
      _repoMock.Setup(x => x.UpdateAsync(entity, default)).Returns(Task.CompletedTask);
      _repoMock.Setup(x => x.GetAsync(entity.Id, false, null, default)).ReturnsAsync(entity);

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.SUCCESS);
      result.Result.Should().NotBeNull();
      result.Result?.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task BaseUpdateCommand_ShouldReturnAlertCommandResult_WhenValidationFail()
    {
      // Arrange
      var entity = new CustomEntity { Id = Guid.Empty };
      var command = new BaseUpdateCommand<CustomEntity>(entity, null, default);

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.ALERT);
      result.Result.Should().BeNull();
    }

    [Fact]
    public async Task BaseUpdateCommand_ShouldReturnErrorCommandResult_WhenAnyErrorOccurs()
    {
      // Arrange
      var entity = new CustomEntity();
      var command = new BaseUpdateCommand<CustomEntity>(entity, null, default);
      var errorMessage = "something is wrong";
      _repoMock.Setup(x => x.UpdateAsync(entity, default)).ThrowsAsync(new Exception(errorMessage));

      // Act
      var result = await _sut.HandleAsync(command);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(ECommandResultStatus.ERROR);
      result.Result.Should().BeNull();
      result.Message.Should().Contain(errorMessage);
    }
  }
}
