namespace CartApplApiCtrl;
public class Cart
{
    public int Id {get; set;}
    public List<CartItem> Items { get; set; } = new List<CartItem>();
    public decimal GrandTotal => Items.Sum(i => i.TotalPrice);
}