using System.Text.Json;
using Abjjad.Images.Core;
using Microsoft.Extensions.Hosting;

namespace Abjjad.Images.Storage;

public class JsonFileStorage<TEntity, TId> : IDisposable, IAsyncDisposable, IHostedService
    where TEntity : class, IEntity<TId>, new() where TId : notnull
{
    private readonly string _filePath;
    private readonly Dictionary<TId, TEntity> _data = new();
    private bool _disposed;

    public JsonFileStorage(string filePath)
    {
        _filePath = filePath;
        EnsureFileExists();
        LoadData();
    }

    private void EnsureFileExists()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]"); // Initialize with an empty array
        }
    }

    private void LoadData()
    {
        string json = File.ReadAllText(_filePath);
        var items = JsonSerializer.Deserialize<List<TEntity>>(json) ?? new List<TEntity>();
        _data.Clear();
        foreach (var item in items)
        {
            _data[item.Id] = item;
        }
    }

    private void SaveData()
    {
        string json = JsonSerializer.Serialize(_data.Values, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
    
    private async Task SaveDataAsync(CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(_data.Values, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json, cancellationToken);
    }

    public IEnumerable<TEntity> GetAll() => _data.Values;

    public TEntity? GetById(TId id) => _data.GetValueOrDefault(id);

    public void Add(TEntity entity)
    {
        _data.TryAdd(entity.Id, entity);
    }

    public void Update(TEntity entity)
    {
        if (_data.ContainsKey(entity.Id))
        {
            _data[entity.Id] = entity;
        }
    }

    public void Delete(TId id)
    {
        _data.Remove(id);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            SaveData();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await FinalizeAsync();
    }

    ~JsonFileStorage()
    {
        Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await FinalizeAsync(cancellationToken);
    }
    
    private async Task FinalizeAsync(CancellationToken cancellationToken = default)
    {
        if (!_disposed)
        {
            await SaveDataAsync(cancellationToken);
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}