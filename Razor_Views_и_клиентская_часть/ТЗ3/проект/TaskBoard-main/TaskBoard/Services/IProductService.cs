using TaskBoard.Models;

namespace TaskBoard.Services;

public interface IProductService
{
    List<Product> GetAll();
    Product? GetById(int id);

    // ===== Уровень 2 — фильтр по категории и список категорий =====
    List<Product> GetByCategory(string? category);
    List<string> GetCategories();
}