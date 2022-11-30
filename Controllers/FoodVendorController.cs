using Microsoft.AspNetCore.Mvc;

namespace SF_FT_DataTool.Controllers;

[ApiController]
[Route("[controller]")]
public class FoodVendorController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return FoodVendorList.ToJson();
    }
}
