using TaskBoard.Models;

namespace TaskBoard.Services;

public interface ICartService
{
    List<CartItem> GetItems();
    void Add(int productId, string name, decimal price);
    void Remove(int productId);
    void Clear();
    decimal Total();
}