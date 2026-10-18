namespace TaskBoard.Models;

public class CartViewModel
{
    public List<CartItem> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);
    public bool IsEmpty => Items.Count == 0;
}