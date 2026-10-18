using TaskBoard.Models;

namespace TaskBoard.Services;

public class InMemoryCartService : ICartService
{
    private readonly List<CartItem> _items = new();

    public List<CartItem> GetItems() => _items.ToList();

    public void Add(int productId, string name, decimal price)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            existing.Quantity++;
            return;
        }

        _items.Add(new CartItem
        {
            ProductId = productId,
            Name = name,
            Price = price,
            Quantity = 1
        });
    }

    public void Remove(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null) _items.Remove(item);
    }

    public void Clear() => _items.Clear();

    public decimal Total() => _items.Sum(i => i.Subtotal);
}