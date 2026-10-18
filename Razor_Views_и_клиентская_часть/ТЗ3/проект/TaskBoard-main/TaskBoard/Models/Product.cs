namespace TaskBoard.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }

    // Уровень 1 — цена до скидки (null, если скидки нет)
    public decimal? OldPrice { get; set; }

    public string? Badge { get; set; }
    public string ImageUrl { get; set; } = "/img/placeholder.jpg";

    // ===== Уровень 3 — состояния =====
    public int Stock { get; set; } = 10;        // 0 = нет в наличии
    public bool IsHit { get; set; }             // хит продаж

    // ===== Уровень 2 — категория для фильтра =====
    public string Category { get; set; } = "Электроника";

    // ===== Уровень 1 — процент скидки (вычисляемый) =====
    public int DiscountPercent =>
        OldPrice.HasValue && OldPrice.Value > 0
            ? (int)Math.Round((1 - Price / OldPrice.Value) * 100)
            : 0;

    // ===== Уровень 3 — признак «большая скидка > 30%» =====
    public bool IsBigDiscount => DiscountPercent > 30;
}