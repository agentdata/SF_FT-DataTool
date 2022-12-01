using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace SF_FT_DataTool.Controllers;

[ApiController]
[Route("[controller]")]
public class FoodVendorController : Controller
{
    [HttpGet]
    [Route("ApprovedVendors")]
    public string GetApprovedVendors()
    {
        return FoodVendorList.ApprovedToJson();
    }
    [HttpGet]
    [Route("RequestedVendors")]
    public string GetRequestedVendors()
    {
        return FoodVendorList.RequestedToJson();
    }

    [HttpPost]
    [Route("SimulateNewVendor")]
    public string SimulateNewVendor()
    {
        FoodVendorList.SimulateNewVendor(Name: Request.Form["Name"].ToString(), _FoodItems: Request.Form["FoodItems"].ToString(), _Latitude: Request.Form["Latitude"].ToString(), _Longitude: Request.Form["Longitude"].ToString());
        return "New Vendor addition simulated.";
    }


    [HttpPost]
    [Route("FoodVendorNotifications")]
    public void NewFoodVendorNotifications()
    {
        //validate parsing distance as double and querytype as a one of the proper enums Food, Distance, Either

        //QueryType QueryTypeValue;
        //try
        //{
        //    switch (Request.Form["Type"].ToString())
        //    {
        //        case "Food": QueryTypeValue = QueryType.Food; break;
        //        case "Distance": QueryTypeValue = QueryType.Distance; break;
        //        case "Either": QueryTypeValue = QueryType.Either; break;
        //        default: throw new Exception();
        //    }
        //}
        //catch (Exception e)
        //{
        //    Console.WriteLine("{0} Exception caught.", e);
        //}


        double DistanceInMilesValue = -1;
        try
        {
            DistanceInMilesValue = Convert.ToDouble(Request.Form["DistanceInMiles"].ToString());

        }
        catch (Exception e)
        {
            Console.WriteLine("{0} Exception caught.", e);
        }


        FoodVendorNotificationRegistrations.addNewRegistration(
            _PhoneNumber: Request.Form["PhoneNumber"].ToString(),
            _FoodItems: Request.Form["FoodItems"].ToString(),
            _Longitude: Request.Form["Longitude"].ToString(),
            _Latitude: Request.Form["Latitude"].ToString(),
            _DistanceInMiles: DistanceInMilesValue,
            _QueryType: Request.Form["Type"].ToString());
        //FoodVendorNotifications
        Console.WriteLine("posted something");
    }
}
