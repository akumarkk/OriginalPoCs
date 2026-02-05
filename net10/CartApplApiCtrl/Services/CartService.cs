namespace CartApplApiCtrl;

public class CartService
{
    private readonly Cart _cart = new Cart();

    public Cart AddItem(CartItem newItem)
    {
        var existingItem = _cart.Items.FirstOrDefault(i => i.Id == newItem.Id);

        if (existingItem != null)
        {
            existingItem.Quantity += newItem.Quantity;
        }
        else
        {
            _cart.Items.Add(newItem);
        }

        return _cart;
    }

    public Cart GetCart() => _cart;
}