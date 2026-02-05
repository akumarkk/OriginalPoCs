using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;

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
        var r = _cartService.AddItem(id, cartItem);
        return Ok(r);
    }

    [HttpGet("{id}")]
    public ActionResult<Cart> Get(int id)
    {
        var r = _cartService.GetCart(id);
        return Ok(r);
    }
}