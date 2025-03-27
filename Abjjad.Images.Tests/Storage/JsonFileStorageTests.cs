using Abjjad.Images.Storage;
using Abjjad.Images.Core;

namespace Abjjad.Images.Tests.Storage;

public class JsonFileStorageTests : IDisposable
{
    private readonly string _testFilePath;
    private readonly JsonFileStorage<TestEntity, Guid> _storage;

    public JsonFileStorageTests()
    {
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test_storage_{Guid.NewGuid()}.json");
        _storage = new JsonFileStorage<TestEntity, Guid>(_testFilePath);
    }

    public void Dispose()
    {
        _storage.Dispose();
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [Fact]
    public void Constructor_CreatesFileIfNotExists()
    {
        Assert.True(File.Exists(_testFilePath));
        var content = File.ReadAllText(_testFilePath);
        Assert.Equal("[]", content);
    }

    [Fact]
    public void Add_NewEntity_AddsToStorage()
    {
        // Arrange
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };

        // Act
        _storage.Add(entity);

        // Assert
        var retrieved = _storage.GetById(entity.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(entity.Name, retrieved.Name);
    }

    [Fact]
    public void Update_ExistingEntity_UpdatesInStorage()
    {
        // Arrange
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Original" };
        _storage.Add(entity);

        // Act
        entity.Name = "Updated";
        _storage.Update(entity);

        // Assert
        var retrieved = _storage.GetById(entity.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("Updated", retrieved.Name);
    }

    [Fact]
    public void Delete_ExistingEntity_RemovesFromStorage()
    {
        // Arrange
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };
        _storage.Add(entity);

        // Act
        _storage.Delete(entity.Id);

        // Assert
        var retrieved = _storage.GetById(entity.Id);
        Assert.Null(retrieved);
    }

    [Fact]
    public void GetAll_ReturnsAllEntities()
    {
        // Arrange
        var entities = new[]
        {
            new TestEntity { Id = Guid.NewGuid(), Name = "First" },
            new TestEntity { Id = Guid.NewGuid(), Name = "Second" }
        };
        foreach (var entity in entities)
        {
            _storage.Add(entity);
        }

        // Act
        var retrieved = _storage.GetAll();

        // Assert
        Assert.Equal(2, retrieved.Count());
        Assert.Contains(retrieved, e => e.Name == "First");
        Assert.Contains(retrieved, e => e.Name == "Second");
    }

    [Fact]
    public void Dispose_SavesDataToFile()
    {
        // Arrange
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };
        _storage.Add(entity);

        // Act
        _storage.Dispose();

        // Assert
        var newStorage = new JsonFileStorage<TestEntity, Guid>(_testFilePath);
        var retrieved = newStorage.GetById(entity.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(entity.Name, retrieved.Name);
        newStorage.Dispose();
    }

    private class TestEntity : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
} 