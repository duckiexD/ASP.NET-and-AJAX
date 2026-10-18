using Microsoft.AspNetCore.Mvc;
using TaskBoard.Models;
using TaskBoard.Services;

namespace TaskBoard.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cart;

    public CartController(ICartService cart)
    {
        _cart = cart;
    }

    // GET /cart
    public IActionResult Index()
    {
        var vm = new CartViewModel
        {
            Items = _cart.GetItems()
        };
        return View(vm);
    }

    // POST /cart/remove/1
    // Удаляем и из ICartService, и из Session — чтобы бейдж синхронизировался.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int id)
    {
        _cart.Remove(id);

        // Синхронизация Session: удаляем id из строки
        var cart = HttpContext.Session.GetString("Cart");
        if (!string.IsNullOrEmpty(cart))
        {
            var items = cart.Split(',').Select(int.Parse).ToList();
            items.RemoveAll(x => x == id);
            HttpContext.Session.SetString("Cart", string.Join(',', items));
        }

        return RedirectToAction(nameof(Index));
    }

    // POST /cart/confirm
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Confirm()
    {
        _cart.Clear();
        HttpContext.Session.Remove("Cart");    // ← очищаем бейдж

        TempData["OrderSuccess"] = "Заказ оформлен! Спасибо за покупку.";
        return RedirectToAction(nameof(Index));
    }
}