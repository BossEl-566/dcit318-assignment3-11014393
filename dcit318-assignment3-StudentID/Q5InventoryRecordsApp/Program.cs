
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Q5InventoryRecordsApp
{
    // a) Immutable record
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // b) Marker interface
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // c) Generic InventoryLogger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item) => _log.Add(item);
        public List<T> GetAll() => new(_log);

        public void SaveToFile()
        {
            try
            {
                using var stream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                var options = new JsonSerializerOptions { WriteIndented = true };
                JsonSerializer.Serialize(stream, _log, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("No saved file found. Skipping load.");
                    return;
                }
                using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var data = JsonSerializer.Deserialize<List<T>>(stream) ?? new List<T>();
                _log.Clear();
                _log.AddRange(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
    }

    // f) Integration layer
    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger = new InventoryLogger<InventoryItem>("inventory.json");

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Pens", 50, DateTime.Today));
            _logger.Add(new InventoryItem(2, "Notebooks", 120, DateTime.Today));
            _logger.Add(new InventoryItem(3, "Markers", 30, DateTime.Today));
            _logger.Add(new InventoryItem(4, "Staplers", 10, DateTime.Today));
            _logger.Add(new InventoryItem(5, "Folders", 75, DateTime.Today));
        }

        public void SaveData() => _logger.SaveToFile();

        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"{item.Id}: {item.Name} | Qty={item.Quantity} | Added={item.DateAdded:d}");
            }
        }

        public static void Main()
        {
            var app = new InventoryApp();
            app.SeedSampleData();
            app.SaveData();

            // simulate new session
            app = new InventoryApp();
            app.LoadData();
            app.PrintAllItems();
        }
    }
}
