using DashboardAdmin.Models;
using DashboardAdmin.Services;
using Microsoft.AspNetCore.Mvc;

namespace DashboardAdmin.Controllers;

public class DashboardController : Controller
{
    private readonly IDashboardRepository _repository;

    public DashboardController(IDashboardRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        var cards = _repository.GetAllCards().ToList();

        ViewData["ReportPeriod"] = "Сентябрь 2067";
        ViewBag.Currency = "RUB";

        return View(cards);
    }

    //  Средний уровень: печатный режим
    public IActionResult Print()
    {
        var cards = _repository.GetAllCards().ToList();

        ViewData["Mode"] = "Print";
        ViewData["ReportPeriod"] = "Сентябрь 2067";
        ViewBag.Currency = "RUB";

        return View("Index", cards);
    }
}