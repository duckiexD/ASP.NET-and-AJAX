using Microsoft.AspNetCore.Mvc;
using TaskBoard.Services;

namespace TaskBoard.Controllers;

public class CatalogController : Controller
{
    private readonly IProductService _products;
    private readonly ICartService _cart;

    public CatalogController(IProductService products, ICartService cart)
    {
        _products = products;
        _cart = cart;
    }

    // GET /catalog?category=Электроника
    public IActionResult Index(string? category)
    {
        ViewBag.Categories = _products.GetCategories();
        ViewBag.CurrentCategory = category ?? "all";

        var products = _products.GetByCategory(category);
        return View(products);
    }

    // POST /catalog/add/1 — старая серверная форма (можно оставить)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(int id)
    {
        var product = _products.GetById(id);
        if (product == null) return NotFound();
        if (product.Stock == 0) return BadRequest();

        _cart.Add(product.Id, product.Name, product.Price);
        TempData["CartMessage"] = "Товар добавлен в корзину";
        return RedirectToAction(nameof(Index));
    }

    // ============================================================
    // Задание 1.1. AJAX-поиск: HTML-фрагмент
    // ============================================================
    [HttpGet]
    public IActionResult Search(string? query)
    {
        var products = _products.GetAll();

        if (!string.IsNullOrWhiteSpace(query))
        {
            query = query.ToLower();
            products = products.Where(p =>
                p.Name.ToLower().Contains(query) ||
                p.Description.ToLower().Contains(query)).ToList();
        }

        return PartialView("_ProductList", products);
    }

    // ============================================================
    // Задание 1.2. AddToCart: JSON
    // ВАЖНО: пишем И в ICartService, И в Session — чтобы
    // /cart видел товар и бейдж считался корректно.
    // ============================================================
    [HttpPost]
    public IActionResult AddToCart(int id)
    {
        var product = _products.GetById(id);
        if (product == null)
            return Json(new { success = false, message = "Товар не найден" });

        // 1. Серверная корзина (то, что читает /cart)
        _cart.Add(product.Id, product.Name, product.Price);

        // 2. Session — для AJAX-счётчика
        var cart = HttpContext.Session.GetString("Cart");
        var items = string.IsNullOrEmpty(cart)
            ? new List<int>()
            : cart.Split(',').Select(int.Parse).ToList();

        items.Add(id);
        HttpContext.Session.SetString("Cart", string.Join(',', items));

        return Json(new
        {
            success = true,
            cartCount = items.Count,
            productName = product.Name
        });
    }

    // ============================================================
    // Задание 1.3. GetCartCount: JSON
    // ============================================================
    [HttpGet]
    public IActionResult GetCartCount()
    {
        var cart = HttpContext.Session.GetString("Cart");
        var count = string.IsNullOrEmpty(cart) ? 0 : cart.Split(',').Length;
        return Json(new { count });
    }
}