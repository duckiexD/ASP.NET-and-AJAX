using DashboardAdmin.Models;

namespace DashboardAdmin.Services;

public class InMemoryDashboardRepository : IDashboardRepository
{
    private static readonly List<DashboardCard> _cards = new()
    {
        new DashboardCard
        {
            Id = 1,
            Title = "Выручка",
            Value = 1_488_670m,
            Trend = Trend.Up,
            Unit = "руб.",
            Description = "Общая выручка за текущий месяц"
        },
        new DashboardCard
        {
            Id = 2,
            Title = "Количество заказов",
            Value = 6_767m,
            Trend = Trend.Up,
            Unit = "шт.",
            Description = "Число оформленных заказов"
        },
        new DashboardCard
        {
            Id = 3,
            Title = "Средний чек",
            Value = 1_488m,
            Trend = Trend.Stable,
            Unit = "руб.",
            Description = "Средняя стоимость одного заказа"
        },
        new DashboardCard
        {
            Id = 4,
            Title = "Возвраты",
            Value = -228_228m,
            Trend = Trend.Down,
            Unit = "руб.",
            Description = "Сумма возвратов клиентам"
        },
        new DashboardCard
        {
            Id = 5,
            Title = "Конверсия",
            Value = 6m,
            Trend = Trend.Up,
            Unit = "%",
            Description = "Процент посетителей, оформивших заказ"
        },
        new DashboardCard
        {
            Id = 6,
            Title = "Отток клиентов",
            Value = 6.7m,
            Trend = Trend.Down,
            Unit = "%",
            Description = "Доля клиентов, прекративших пользоваться сервисом"
        }
    };

    public IEnumerable<DashboardCard> GetAllCards() => _cards;

    public DashboardCard? GetById(int id) =>
        _cards.FirstOrDefault(c => c.Id == id);

    public IEnumerable<DashboardCard> GetLatest(int count) =>
        _cards.OrderByDescending(c => c.Id).Take(count);
}