using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace SF_FT_DataTool.Controllers;

[ApiController]
[Route("[controller]")]
public class FoodVendorController : Controller
{
    /// <summary>
    /// This endpoint returns a json formatted list containing all the vendors with a status of "Requested". 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("ApprovedVendors")]
    public string GetApprovedVendors()
    {
        return FoodVendorList.ApprovedToJson();
    }

    /// <summary>
    /// This endpoint returns a json formatted list containing all the vendors with a status of "Requested". 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("RequestedVendors")]
    public string GetRequestedVendors()
    {
        return FoodVendorList.RequestedToJson();
    }

    /// <summary>
    /// This endpoint returns a json formatted list containing all NotificationRegistrations.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("NotificationRegistrations")]
    public string GetNotificationRegistrations()
    {
        return FoodVendorNotificationRegistrations.AllToJson();
    }


    /// <summary>
    /// This Endpoint simulates a new Food Vendor being added to the list, requires Name, FoodItems list and Latitude/Longitude coordinates.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("SimulateNewVendor")]
    public string SimulateNewVendor()
    {
        //validations
        try
        {
            FoodVendorList.SimulateNewVendor(
                Name: Request.Form["Name"].ToString(),
                _FoodItems: Request.Form["FoodItems"].ToString(),
                _Latitude: Request.Form["Latitude"].ToString(),
                _Longitude: Request.Form["Longitude"].ToString());
        }
        catch (Exception e)
        {
            return JsonResponses.BuildError("SimulateNewVendor failed to add new vendor.");
        }
        return JsonResponses.BuildGoodResponse("New Vendor addition simulated.");
    }

    /// <summary>
    /// This endpoint adds a new NotificationRegistration to the list.
    /// </summary>
    /// <returns>Returns the ID of the newly added NotificationRegistration</returns>
    [HttpPost]
    [Route("NotificationRegistrations")]
    public string NewFoodVendorNotifications()
    {
        //validate parsing distance as double and querytype as a one of the proper enums Food, Distance, Either

        QueryType QueryTypeValue;
        try
        {
            QueryTypeValue = QueryEnumHelper.GetQueryType(Request.Form["Type"].ToString());
        }
        catch (Exception e)
        {
            return JsonResponses.BuildError("Type must be one of the following {Food, Distance, Either}");
        }


        double DistanceInMilesValue = -1;
        try
        {
            DistanceInMilesValue = Convert.ToDouble(Request.Form["DistanceInMiles"].ToString());
            if(DistanceInMilesValue < 0)
            {
                return JsonResponses.BuildError("DistanceInMiles needs to be a positive number.");
            }

        }
        catch (Exception e)
        {
            return JsonResponses.BuildError("DistanceInMiles needs to be a number.");
        }


        return (JsonResponses.BuildGoodResponse("New Registration added successfuly, Id: "+FoodVendorNotificationRegistrations.addNewRegistration(
            _WehbookAddress: Request.Form["WehbookAddress"].ToString(),
            _FoodItems: Request.Form["FoodItems"].ToString(),
            _Longitude: Request.Form["Longitude"].ToString(),
            _Latitude: Request.Form["Latitude"].ToString(),
            _DistanceInMiles: DistanceInMilesValue,
            _QueryType: Request.Form["Type"].ToString())));
    }


    /// <summary>
    /// This endpoint updates a NotificationRegistration identified by the provided ID. All NotificationRegistration Details are required.
    /// </summary>
    /// <param name="Id"></param>
    /// <returns>returns a string with results of the udpate.</returns>
    [HttpPut]
    [Route("NotificationRegistrations/{id?}")]
    public string UpdateNotificationRegistration(string? Id)
    {
        //validate ID is guid format.
        try
        {
            Guid.Parse(Id);
        }
        catch (Exception e)
        {
            return JsonResponses.BuildError("Id provided is not a valid GUID");
        }

        double DistanceInMilesValue = -1;
        try
        {
            DistanceInMilesValue = Convert.ToDouble(Request.Form["DistanceInMiles"].ToString());
            if (DistanceInMilesValue < 0)
            {
                return JsonResponses.BuildError("DistanceInMiles needs to be a positive number.");
            }

        }
        catch (Exception e)
        {
            return JsonResponses.BuildError("DistanceInMiles needs to be a number.");
        }

        if(FoodVendorNotificationRegistrations.UpdateNotificationRegistration(
            Id,
            _WehbookAddress: Request.Form["WebhookAddress"].ToString(),
            _FoodItems: Request.Form["FoodItems"].ToString(),
            _Longitude: Request.Form["Longitude"].ToString(),
            _Latitude: Request.Form["Latitude"].ToString(),
            _DistanceInMiles: DistanceInMilesValue,
            _QueryType: Request.Form["Type"].ToString()))
        {
            return JsonResponses.BuildGoodResponse("NotificationRegistration updated successfully.");
        }
        else
        {
            return JsonResponses.BuildError("Something went wrong updating this notificationRegistration.");
        }
    }

    /// <summary>
    /// This endpoint deletes a NotificationRegistration identified by the provided ID.
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("NotificationRegistrations/{id?}")]
    public string DeleteNotificationRegistration(string? Id)
    {
        //validate ID is guid format.
        try
        {
            Guid.Parse(Id);
        }
        catch (Exception e)
        {
            return JsonResponses.BuildError("Id provided is not a valid GUID");
        }
        return JsonResponses.BuildGoodResponse(FoodVendorNotificationRegistrations.RemoveRegistration(Id));   
    }
}
