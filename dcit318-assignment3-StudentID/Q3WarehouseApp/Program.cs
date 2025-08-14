
using System;
using System.Collections.Generic;

namespace Q3WarehouseApp
{
    // a) Marker interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b) Product classes
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public int Quantity { get; set; }
        public string Brand { get; init; }
        public int WarrantyMonths { get; init; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
        }

        public override string ToString() => $"Electronic {{ Id={Id}, Name={Name}, Qty={Quantity}, Brand={Brand}, Warranty={WarrantyMonths}m }}";
    }

    public class GroceryItem : IInventoryItem
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; init; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
        }

        public override string ToString() => $"Grocery {{ Id={Id}, Name={Name}, Qty={Quantity}, Expiry={ExpiryDate:d} }}";
    }

    // e) Custom exceptions
    public class DuplicateItemException : Exception { public DuplicateItemException(string message) : base(message) {} }
    public class ItemNotFoundException : Exception { public ItemNotFoundException(string message) : base(message) {} }
    public class InvalidQuantityException : Exception { public InvalidQuantityException(string message) : base(message) {} }

    // d) InventoryRepository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with Id {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with Id {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with Id {id} not found.");
        }

        public List<T> GetAllItems() => new(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
            var item = GetItemById(id); // may throw
            item.Quantity = newQuantity;
        }
    }

    // f) WarehouseManager
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Smartphone", 10, "TechBrand", 24));
            _electronics.AddItem(new ElectronicItem(2, "Laptop", 5, "CompCo", 12));
            _groceries.AddItem(new GroceryItem(101, "Rice 5kg", 50, DateTime.Today.AddMonths(12)));
            _groceries.AddItem(new GroceryItem(102, "Milk 1L", 30, DateTime.Today.AddDays(20)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine(item);
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock increased for Id={id}. New Qty={item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error increasing stock: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Removed item Id={id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing item: {ex.Message}");
            }
        }

        public static void Main()
        {
            var mgr = new WareHouseManager();
            mgr.SeedData();

            Console.WriteLine("Grocery Items:");
            mgr.PrintAllItems(mgr._groceries);
            Console.WriteLine("\nElectronic Items:");
            mgr.PrintAllItems(mgr._electronics);

            Console.WriteLine("\n-- Exception Scenarios --");
            try
            {
                // Add duplicate
                mgr._electronics.AddItem(new ElectronicItem(1, "Tablet", 7, "TechBrand", 18));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Duplicate add error: {ex.Message}");
            }

            // Remove non-existent
            mgr.RemoveItemById(mgr._groceries, 999);

            // Update invalid quantity
            try
            {
                mgr._groceries.UpdateQuantity(101, -5);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid update error: {ex.Message}");
            }
        }
    }
}
