using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Mvc;

namespace CartApplApiCtrl;

[ApiController]
[Route("api/[controller]")]
public class CartController: ControllerBase
{
    private CartService _cartService;
    public CartController(CartService cartService )
    {
        _cartService = cartService;
    }


    [HttpPost("add/{id}")]
    public ActionResult<Cart> Add(int id, [FromBody]CartItem cartItem)
    {
        Console.WriteLine($"no multiple carts[todo] : {id}");
        var r = _cartService.AddItem(cartItem);
        return Ok(r);
    }

    [HttpGet("{id}")]
    public ActionResult<Cart> Get(int id)
    {
        Console.WriteLine($"get no multiple carts[todo] : {id}");
        var r = _cartService.GetCart();
        return Ok(r);
    }
}