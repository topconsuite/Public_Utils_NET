using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace Telluria.Utils.Crud.Tests;

public class RepositoryTests
{
  private readonly CustomRepository _sut;
  private readonly Mock<DbContext> _dbContextMock = new();

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

  public RepositoryTests()
  {
    _dbContextMock.Setup(x => x.Set<CustomEntity>()).ReturnsDbSet(_entityMockList);
    _sut = new(_dbContextMock.Object);
  }

  private static IEnumerable<object[]> FindAsyncSuccessTestData()
  {
    var first = _entityMockList.First();
    Expression<Func<CustomEntity, bool>> firstFilter = (t) => t.CreatedAt >= first.CreatedAt && t.Id != first.Id;
    var last = _entityMockList.Last();
    Expression<Func<CustomEntity, bool>> lastFilter = (t) => t.CreatedAt <= last.CreatedAt;

    yield return new object[] { false, firstFilter, null! };
    yield return new object[] { true, firstFilter, null! };
    yield return new object[] { false, lastFilter, Array.Empty<string>() };
    yield return new object[] { true, lastFilter, Array.Empty<string>() };
    yield return new object[] { false, firstFilter, new[] { "child1", "child2.nestedChild" } };
    yield return new object[] { true, lastFilter, new[] { "child1", "child2.nestedChild" } };
  }

  [Theory]
  [MemberData(nameof(FindAsyncSuccessTestData))]
  public async Task FindAsync_ShouldReturnEntity_WhenFilterHasAMatch(
    bool tracking, Expression<Func<CustomEntity, bool>> filter, IEnumerable<string> includeProperties)
  {
    // Arrange
    var entity = _entityMockList.AsQueryable().FirstOrDefault(filter);

    // Act
    var result = await _sut.FindAsync(tracking, filter, includeProperties, default);

    // Assert
    result.Should().NotBeNull();
    result?.Id.Should().Be(entity!.Id);
  }

  private static IEnumerable<object[]> FindAsyncNotFoundTestData()
  {
    Expression<Func<CustomEntity, bool>> firstFilter = (t) => t.CreatedAt < DateTime.UtcNow.AddDays(-100);
    Expression<Func<CustomEntity, bool>> lastFilter = (t) => t.CreatedAt > DateTime.UtcNow.AddDays(100);

    yield return new object[] { false, firstFilter, null! };
    yield return new object[] { true, firstFilter, null! };
    yield return new object[] { false, lastFilter, Array.Empty<string>() };
    yield return new object[] { true, lastFilter, Array.Empty<string>() };
    yield return new object[] { false, firstFilter, new[] { "child1", "child2.nestedChild" } };
    yield return new object[] { true, lastFilter, new[] { "child1", "child2.nestedChild" } };
  }

  [Theory]
  [MemberData(nameof(FindAsyncNotFoundTestData))]
  public async Task FindAsync_ShouldReturnNull_WhenFilterDoesNotHaveAMatch(
    bool tracking, Expression<Func<CustomEntity, bool>> filter, IEnumerable<string> includeProperties)
  {
    // Act
    var result = await _sut.FindAsync(tracking, filter, includeProperties, default);

    // Assert
    result.Should().BeNull();
  }

  private static IEnumerable<object[]> GetAsyncSuccessTestData()
  {
    var first = _entityMockList.First();
    var last = _entityMockList.Last();

    yield return new object[] { first.Id, false, null! };
    yield return new object[] { first.Id, true, null! };
    yield return new object[] { last.Id, false, Array.Empty<string>() };
    yield return new object[] { last.Id, true, Array.Empty<string>() };
    yield return new object[] { first.Id, false, new[] { "child1", "child2.nestedChild" } };
    yield return new object[] { last.Id, true, new[] { "child1", "child2.nestedChild" } };
  }

  [Theory]
  [MemberData(nameof(GetAsyncSuccessTestData))]
  public async Task GetAsync_ShouldReturnEntity_WhenIdIsFound(
    Guid entityId, bool tracking, IEnumerable<string> includeProperties)
  {
    // Act
    var result = await _sut.GetAsync(entityId, tracking, includeProperties, default);

    // Assert
    result.Should().NotBeNull();
    result?.Id.Should().Be(entityId);
  }

  private static IEnumerable<object[]> GetAsyncNotFoundTestData()
  {
    var entityId = Guid.NewGuid();

    yield return new object[] { entityId, false, null! };
    yield return new object[] { entityId, true, null! };
    yield return new object[] { entityId, false, Array.Empty<string>() };
    yield return new object[] { entityId, true, Array.Empty<string>() };
    yield return new object[] { entityId, false, new[] { "child1", "child2.nestedChild" } };
    yield return new object[] { entityId, true, new[] { "child1", "child2.nestedChild" } };
  }

  [Theory]
  [MemberData(nameof(GetAsyncNotFoundTestData))]
  public async Task GetAsync_ShouldReturnNull_WhenIdIsNotFound(
    Guid entityId, bool tracking, IEnumerable<string> includeProperties)
  {
    // Act
    var result = await _sut.GetAsync(entityId, tracking, includeProperties, default);

    // Assert
    result.Should().BeNull();
  }

  [Theory]
  [MemberData(nameof(FindAsyncSuccessTestData))]
  public async Task ListAsync_ShouldReturnEntityList_WhenFilterHasAMatch(
    bool tracking, Expression<Func<CustomEntity, bool>> filter, IEnumerable<string> includeProperties)
  {
    // Arrange
    var expected = _entityMockList.AsQueryable().Where(filter).ToList();

    // Act
    var result = await _sut.ListAsync(tracking, filter, includeProperties, default);

    // Assert
    result.Should().NotBeNull();
    result.Count().Should().Be(expected.Count);
  }

  [Theory]
  [MemberData(nameof(FindAsyncSuccessTestData))]
  public async Task ListAllAsync_ShouldReturnEntityList_WhenFilterHasAMatch(
    bool tracking, Expression<Func<CustomEntity, bool>> filter, IEnumerable<string> includeProperties)
  {
    // Arrange
    var expected = _entityMockList.AsQueryable().Where(filter).ToList();

    // Act
    var result = await _sut.ListAllAsync(tracking, filter, includeProperties, default);

    // Assert
    result.Should().NotBeNull();
    result.Count().Should().Be(expected.Count);
  }

  private static IEnumerable<object[]> ListAsyncSuccessTestData()
  {
    var first = _entityMockList.First();
    Expression<Func<CustomEntity, bool>> firstFilter = (t) => t.CreatedAt >= first.CreatedAt && t.Id != first.Id;
    var last = _entityMockList.Last();
    Expression<Func<CustomEntity, bool>> lastFilter = (t) => t.CreatedAt <= last.CreatedAt;

    yield return new object[] { 0, 30, false, firstFilter, null! };
    yield return new object[] { 1, 30, true, firstFilter, null! };
    yield return new object[] { 1, 0, false, lastFilter, Array.Empty<string>() };
    yield return new object[] { 0, 0, true, lastFilter, Array.Empty<string>() };
    yield return new object[] { 30, 2, false, firstFilter, new[] { "child1", "child2.nestedChild" } };
    yield return new object[] { 2, 2, true, lastFilter, new[] { "child1", "child2.nestedChild" } };
  }

  [Theory]
  [MemberData(nameof(ListAsyncSuccessTestData))]
  public async Task ListAsync_ShouldReturnPagedEntityList_WhenFilterHasAMatch(
    uint page, uint perPage, bool tracking, Expression<Func<CustomEntity, bool>> filter, IEnumerable<string> includeProperties)
  {
    // Arrange
    var expected = _entityMockList.AsQueryable().Where(filter).ToList();

    // Act
    var result = await _sut.ListAsync(page, perPage, tracking, filter, includeProperties, default);

    // Assert
    result.Should().NotBeNull();
    result.Page.Should().BeGreaterThan(0);
    result.PerPage.Should().BeGreaterThan(0);
    result.Records.Count().Should().BeLessThanOrEqualTo((int)result.PerPage);
    result.TotalCount.Should().Be((ulong)expected.Count);
  }

  [Theory]
  [MemberData(nameof(ListAsyncSuccessTestData))]
  public async Task ListAllAsync_ShouldReturnPagedEntityList_WhenFilterHasAMatch(
    uint page, uint perPage, bool tracking, Expression<Func<CustomEntity, bool>> filter, IEnumerable<string> includeProperties)
  {
    // Arrange
    var expected = _entityMockList.AsQueryable().Where(filter).ToList();

    // Act
    var result = await _sut.ListAllAsync(page, perPage, tracking, filter, includeProperties, default);

    // Assert
    result.Should().NotBeNull();
    result.Page.Should().BeGreaterThan(0);
    result.PerPage.Should().BeGreaterThan(0);
    result.Records.Count().Should().BeLessThanOrEqualTo((int)result.PerPage);
    result.TotalCount.Should().Be((ulong)expected.Count);
  }

  [Fact]
  public async Task AddAsync_ShouldNotThrows_WhenSuccessfullyAdded()
  {
    // Arrange
    var entity = new CustomEntity();

    // Act
    var getResult = async () => await _sut.AddAsync(entity, default);

    // Assert
    await getResult.Should().NotThrowAsync();
  }

  [Fact]
  public async Task UpdateAsync_ShouldNotThrows_WhenSuccessfullyUpdated()
  {
    // Arrange
    var entity = _entityMockList.First();

    // Act
    var getResult = async () => await _sut.UpdateAsync(entity, default);

    // Assert
    await getResult.Should().NotThrowAsync();
  }

  [Fact]
  public async Task UpdateAsync_ShouldThrows_WhenNotFound()
  {
    // Arrange
    var entity = new CustomEntity();

    // Act
    var getResult = async () => await _sut.UpdateAsync(entity, default);

    // Assert
    await getResult.Should().ThrowAsync<Exception>().WithMessage($"*{entity.Id}*not found");
  }

  [Fact]
  public async Task SoftDeleteAsync_ShouldNotThrows_WhenSuccessfullyDeleted()
  {
    // Arrange
    var entity = _entityMockList.First();

    // Act
    var getResult = async () => await _sut.SoftDeleteAsync(entity, default);

    // Assert
    await getResult.Should().NotThrowAsync();
  }

  [Fact]
  public async Task SoftDeleteAsync_ShouldThrows_WhenNotFound()
  {
    // Arrange
    var entity = new CustomEntity();

    // Act
    var getResult = async () => await _sut.SoftDeleteAsync(entity, default);

    // Assert
    await getResult.Should().ThrowAsync<Exception>().WithMessage($"*{entity.Id}*not found");
  }

  [Fact]
  public async Task SoftDeleteAsync_ShouldThrows_WhenIdIsEmpty()
  {
    // Arrange
    var entity = new CustomEntity { Id = Guid.Empty };

    // Act
    var getResult = async () => await _sut.SoftDeleteAsync(entity, default);

    // Assert
    await getResult.Should().ThrowAsync<Exception>().WithMessage($"*{Guid.Empty}*not found");
  }

  [Fact]
  public async Task RemoveAsync_ShouldNotThrows_WhenSuccessfullyRemoved()
  {
    // Arrange
    var entity = _entityMockList.First();

    // Act
    var getResult = async () => await _sut.RemoveAsync(entity, default);

    // Assert
    await getResult.Should().NotThrowAsync();
  }

  [Fact]
  public async Task RemoveAsync_ShouldThrows_WhenNotFound()
  {
    // Arrange
    var entity = new CustomEntity();

    // Act
    var getResult = async () => await _sut.RemoveAsync(entity, default);

    // Assert
    await getResult.Should().ThrowAsync<Exception>().WithMessage($"*{entity.Id}*not found");
  }

  [Fact]
  public async Task RemoveAsync_ShouldThrows_WhenIdIsEmpty()
  {
    // Arrange
    var entity = new CustomEntity { Id = Guid.Empty };

    // Act
    var getResult = async () => await _sut.RemoveAsync(entity, default);

    // Assert
    await getResult.Should().ThrowAsync<Exception>().WithMessage($"*{Guid.Empty}*not found");
  }

  [Fact]
  public async Task Commit_ShouldNotThrows_WhenSuccessfullySaveChanges()
  {
    // Act
    var getResult = async () => await _sut.Commit(default);

    // Assert
    await getResult.Should().NotThrowAsync();
  }

  [Fact]
  public async Task Commit_ShouldThrows_WhenUnableToSaveChanges()
  {
    // Arrange
    var errorMessage = "something is wrong";
    _dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new Exception(errorMessage));

    // Act
    var getResult = async () => await _sut.Commit(default);

    // Assert
    await getResult.Should().ThrowAsync<Exception>().WithMessage(errorMessage);
  }
}
