using TaskBoard.Models;

namespace TaskBoard.Services;

public class InMemoryProductService : IProductService
{
    private readonly List<Product> _products = new()
    {
        new Product
        {
            Id = 1, Name = "Наушники Zowie Mashu",
            Description = "Полноразмерные мониторные наушники с микрофоном",
            Price = 3999m,
            OldPrice = 5130m,           // Уровень 1 — старая цена
            ImageUrl = "/img/naushniki.jpg",
            Badge = "Хит",
            Category = "Электроника",
            Stock = 5,
            IsHit = true                 // Уровень 3 — хит
        },
        new Product
        {
            Id = 2, Name = "Смартфон Iphone 18 Pro 256 Гб Чёрный",
            Description = "6Флагманский смартфон с переменной диафрагмой камеры 48 Мп, чипом A20 Pro и экраном 6,3 дюйма с ProMotion",
            Price = 165000m,
            ImageUrl = "/img/iphone18.jpg",
            Category = "Электроника",
            Stock = 12
        },
        new Product
        {
            Id = 3, Name = "Тематическое худи Team Spirit Dragon Day",
            Description = "Лимитированное худи в честь побед Team Spirit: свободный крой, качественный хлопок, стойкий принт с логотипом команды и арт-дизайном, посвящённым «Дню Дракона»",
            Price = 8990m,
            OldPrice = 9990m,
            ImageUrl = "/img/hoodiespirit.jpg",
            Badge = "Скидка",
            Category = "Одежда",
            Stock = 10,
            IsHit = true
        },
        new Product
        {
            Id = 3, Name = "Киберспортивная организация Team Spirit",
            Description = "Профессиональная киберспортивная организация",
            Price = 2535999999m,
            ImageUrl = "/img/spirit.jpg",
            Category = "Другое",
            Stock = 0                    // Уровень 3 — нет в наличии
        },
        new Product
        {
            Id = 4, Name = "Игровое джерси Team Spirit",
            Description = "Хлопок 100%, размеры S–XL",
            Price = 2499m,
            OldPrice = 3990m,
            ImageUrl = "/img/1.jpg",
            Category = "Одежда",
            Stock = 50
        },
        new Product
        {
            Id = 5, Name = "Ноутбук MSI Katana 17 HX B14WFK-096CN",
            Description = "(Intel Core i7 14650HX/17.3\" 2560x1440/16Gb/2048Gb SSD/NVIDIA GeForce RTX 5060 8Gb/Win 11 Home)",
            Price = 169990m,
            OldPrice = 182300m,
            ImageUrl = "/img/notebookmsi.jpg",
            Category = "Электроника",
            Stock = 8,
            IsHit = true                 // Уровень 3 — хит
        },
    };

    public List<Product> GetAll() => _products.ToList();

    public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

    // ===== Уровень 2 — фильтрация по категории =====
    public List<Product> GetByCategory(string? category)
    {
        if (string.IsNullOrWhiteSpace(category) || category == "all")
            return GetAll();

        return _products
            .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // ===== Уровень 2 — список уникальных категорий =====
    public List<string> GetCategories()
        => _products.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();
}