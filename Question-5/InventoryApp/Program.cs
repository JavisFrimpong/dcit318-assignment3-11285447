using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryManagementSystem
{
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(_log);
        }

        public void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
                Console.WriteLine($"\u2705 Inventory saved to {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\u274C Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath)) return;

                var json = File.ReadAllText(_filePath);
                var items = JsonSerializer.Deserialize<List<T>>(json);
                if (items != null)
                    _log = items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\u274C Error loading from file: {ex.Message}");
            }
        }
    }

    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp()
        {
            _logger = new InventoryLogger<InventoryItem>("inventory.json");
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Keyboard", 25, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Mouse", 40, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Monitor", 15, DateTime.Now));
            _logger.Add(new InventoryItem(5, "USB Cable", 100, DateTime.Now));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Added: {item.DateAdded}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var app = new InventoryApp();

            Console.WriteLine("\n--- Seeding Inventory ---");
            app.SeedSampleData();
            app.SaveData();

            Console.WriteLine("\n--- Simulating Restart ---");
            var restartedApp = new InventoryApp();
            restartedApp.LoadData();

            Console.WriteLine("\n--- Loaded Inventory ---");
            restartedApp.PrintAllItems();
        }
    }
}
