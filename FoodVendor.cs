namespace SF_FT_DataTool;

using Geolocation;
using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static SF_FT_DataTool.FoodVendorNotificationRegistration;
public enum QueryType { Food, Distance, Either }

/// <summary>
/// A Food Vendor object Loosely matching the Food Vendor data from SF food vendor CSV file.
/// There is are some fields missing from this representation, but important core fields for interested parties are stored and record.
/// </summary>
public class FoodVendor
{
    public string? VendorId { get; set; }
    public string? LocationId { get; set; }
    public string? Applicant { get; set; }
    public string? FacilityType { get; set; }
    public string? LocationDescription { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? Address { get; set; }
    public string? Permit { get; set; }
    public string? Status { get; set; }
    public List<string>? FoodItems { get; set; }

    //helper method to seriliaze as JSON
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}


/// <summary>
/// Static class which holds a list of <FoodVendor> objects and provides some functions to work with the food vendor data.
/// </summary>
public static class FoodVendorList
{
    public static List<FoodVendor> FoodVendors { get; set; } = new List<FoodVendor>();

    //Serialize entire list as JSON
    public static string AllToJson()
    {
        return JsonSerializer.Serialize(FoodVendors);
    }

    //Filter for APPROVED only and serialize list as JSON
    public static string ApprovedToJson()
    {
        var ApprovedFoodVendors = FoodVendors.FindAll(FoodVendor => FoodVendor.Status.CompareTo("APPROVED") == 0);
        return JsonSerializer.Serialize(ApprovedFoodVendors);
    }
    //Filter for Requested only and serialize list as JSON
    public static string RequestedToJson()
    {
        var ApprovedFoodVendors = FoodVendors.FindAll(FoodVendor => FoodVendor.Status.CompareTo("REQUESTED") == 0);
        return JsonSerializer.Serialize(FoodVendors);
    }

    //Filter for Requested only and return as new list
    public static List<FoodVendor> NewRequestedToList()
    {
        var ApprovedFoodVendors = FoodVendors.FindAll(FoodVendor => FoodVendor.Status.CompareTo("REQUESTED") == 0);
        return ApprovedFoodVendors;
    }

    /// <summary>
    /// Simulate adding a new vendor with the provided name, food items and lat/lon
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="_FoodItems"></param>
    /// <param name="_Latitude"></param>
    /// <param name="_Longitude"></param>
    internal static void SimulateNewVendor(string Name, string _FoodItems, string _Latitude, string _Longitude)
    {
        var LocationId = Guid.NewGuid().ToString();
        var Applicant = Name;
        var FacilityType = "Push Cart";
        var LocationDescription = "Corner of poke and tilapia";
        var Latitude = _Latitude;
        var Longitude = _Longitude;
        var Address = "52 poke street";
        var Permit = Guid.NewGuid().ToString();
        var Status = "REQUESTED";
        var FoodItems = ((_FoodItems).Split(new char[] { ':', ';' })).ToList<string>();

        FoodVendor vendor = new FoodVendor
        {
            VendorId = Guid.NewGuid().ToString(),
            LocationId = LocationId,
            Applicant = Applicant,
            FacilityType = FacilityType,
            LocationDescription = LocationDescription,
            Latitude = Latitude,
            Longitude = Longitude,
            Address = Address,
            Permit = Permit,
            Status = Status,
            FoodItems = FoodItems
        };
        FoodVendorList.FoodVendors.Add(vendor);


        FoodVendorNotificationRegistrations.CheckRegistrationsForMatch(vendor);

    }
}

/// <summary>
/// Food Vendor Notification Registration.
/// </summary>
public class FoodVendorNotificationRegistration
{
    public string? WehbookAddress { get; set; }
    public List<string>? FoodItems { get; set; }
    public string? Id { get; set; }
    public string? Longitude { get; set; }
    public string? Latitude { get; set; }
    public double? DistanceInMiles { get; set; }
    public QueryType QueryType { get; set; }

    public void UpdateNotificationRegistration(string _WehbookAddress, List<string>? _FoodItems, string _Longitude, string _Latitude, double _DistanceInMiles, string _QueryType)
    {
        this.WehbookAddress = _WehbookAddress;
        this.FoodItems = _FoodItems;
        this.Longitude = _Longitude;
        this.Latitude = _Latitude;
        this.DistanceInMiles = _DistanceInMiles;
        this.QueryType = QueryEnumHelper.GetQueryType(_QueryType);
    }
}

/// <summary>
/// Helper class for QueryTyp Enum used in Food Vendor Notification Registration.
/// </summary>
public static class QueryEnumHelper
{
    public static QueryType GetQueryType(string _QueryType)
    {
        QueryType QueryTypeValue;
        switch (_QueryType)
        {
            case "Food": QueryTypeValue = QueryType.Food; break;
            case "Distance": QueryTypeValue = QueryType.Distance; break;
            case "Either": QueryTypeValue = QueryType.Either; break;
            default: throw new Exception();
        }
        return QueryTypeValue;
    }
}

/// <summary>
/// This static class contains a list of <FoodVendorNotificationRegistration> objects named Registrations and provides some methods for functionality with the List.
/// </summary>
public static class FoodVendorNotificationRegistrations
{
    //for webhook sending
    private static readonly HttpClient client = new HttpClient();
    public static List<FoodVendorNotificationRegistration> Registrations { get; set; } = new List<FoodVendorNotificationRegistration>();

    /// <summary>
    /// Adds a new <FoodVendorNotificationRegistration> to the Registrations List.
    /// </summary>
    /// <param name="_WehbookAddress"></param>
    /// <param name="_FoodItems"></param>
    /// <param name="_Longitude"></param>
    /// <param name="_Latitude"></param>
    /// <param name="_DistanceInMiles"></param>
    /// <param name="_QueryType"></param>
    /// <returns>The new ID associated with this Registration.</returns>
    public static string addNewRegistration(string _WehbookAddress, string _FoodItems, string _Longitude, string _Latitude, double _DistanceInMiles, string _QueryType)
    {
        string Id = Guid.NewGuid().ToString();
        FoodVendorNotificationRegistration newRegistration = new FoodVendorNotificationRegistration
        {
            WehbookAddress = _WehbookAddress,
            Id = Id,
            FoodItems = _FoodItems.Split(':').ToList<string>(),
            Longitude = _Longitude,
            Latitude = _Latitude,
            DistanceInMiles = _DistanceInMiles,
            QueryType = QueryEnumHelper.GetQueryType(_QueryType)
        };
        Registrations.Add(newRegistration);
        return Id;
    }

    /// <summary>
    /// This method is called when a new <FoodVendor> is added. It compares the new <FoodVendor> with all existing NotificationRegistration objects.
    /// Comparing if the distance or food list are a match.
    /// </summary>
    /// <param name="Vendor"></param>
    public static void CheckRegistrationsForMatch(FoodVendor Vendor)
    {
        foreach (FoodVendorNotificationRegistration Registration in Registrations)
        {
            bool Matched = false;
            //compare with each registration for a match
            if (Registration.QueryType == QueryType.Either)
            {
                if (FoodTypeComparison(Vendor.FoodItems, Registration.FoodItems) || WithinRequestedDistance(Vendor, Registration)) { Matched = true; }
            }
            else if (Registration.QueryType == QueryType.Food)
            {
                if (FoodTypeComparison(Vendor.FoodItems, Registration.FoodItems)) { Matched = true; }
            }
            else if (Registration.QueryType == QueryType.Distance)
            {
                if (WithinRequestedDistance(Vendor, Registration)) { Matched = true; }
            }

            if (Matched)
            {
                SendNotification(Vendor, Registration);
            }
        }
    }

    /// <summary>
    /// This function checks the distance between Vendor and Registration by utilizing the
    /// geolocation nuget package which calculates the distance between coordinates.
    /// </summary>
    /// <param name="Vendor"></param>
    /// <param name="Registration"></param>
    /// <returns>bool which indicates true if the distance in Miles between Vendor and registration is less than or equal to the distance specified in the Registration object.</returns>
    public static bool WithinRequestedDistance(FoodVendor Vendor, FoodVendorNotificationRegistration Registration)
    {
        //check distance between Registration and vendor and compare to distanceInMiles
        Coordinate vendorCoordinate = new Coordinate(Convert.ToDouble(Vendor.Latitude), Convert.ToDouble(Vendor.Longitude));
        Coordinate registrationCoordinate = new Coordinate(Convert.ToDouble(Registration.Latitude), Convert.ToDouble(Registration.Longitude));
        var CalculatedDistanceInMiles = GeoCalculator.GetDistance(vendorCoordinate, registrationCoordinate, 1);

        if (CalculatedDistanceInMiles >= Registration.DistanceInMiles)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// This method compares food item lists for a match.
    /// </summary>
    /// <param name="VendorFood"></param>
    /// <param name="RegistrationFood"></param>
    /// <returns>bool which indicates true if the Vendor food is found in the notification registration.</returns>
    public static bool FoodTypeComparison(List<string> VendorFood, List<string> RegistrationFood)
    {
        bool FoodMatched = false;
        //check Food items for match between Registration and vendor
        VendorFood.ForEach(VendorFoodItem =>
        {
            RegistrationFood.ForEach(RegistrationFoodItem =>
            {
                if (RegistrationFoodItem.CompareTo(VendorFoodItem) == 0)
                {
                    FoodMatched = true;
                }
            });
        });

        return FoodMatched;
    }

    /// <summary>
    /// This method sends a webhook with data to the address found in the Registration.
    /// </summary>
    /// <param name="Vendor"></param>
    /// <param name="Registration"></param>
    public static void SendNotification(FoodVendor Vendor, FoodVendorNotificationRegistration Registration)
    {
        //send notice to phone number of match
        //Registration.PhoneNumber;

        var values = new Dictionary<string, string>
          {
              { "Message", "New Food vendor has been added which matches your registration." },
              { "Vendor", JsonSerializer.Serialize(Vendor) }
          };

        var content = new StringContent(JsonSerializer.Serialize(values), Encoding.UTF8, "application/json");
        var response = client.PostAsync(Registration.WehbookAddress, content);

        //var responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine("Notifiction sent to: " + Registration.WehbookAddress);
    }

    //Filter for Requested only and serialize list as JSON
    public static string AllToJson()
    {
        return JsonSerializer.Serialize(Registrations);
    }


    /// <summary>
    /// Removes the NotificationRegistration in the list identified by the specified Guid formatted ID.
    /// </summary>
    /// <param name="_Id"></param>
    /// <returns>returns a string intended for the client side indicating if the registration was removed or not.</returns>
    public static string RemoveRegistration(string? _Id)
    {
        var tempRegistration = Registrations.Find(Registration => Registration.Id.CompareTo(_Id) == 0);

        if (Registrations.Remove(tempRegistration)) { return "Removed successfully"; }
        else { return "failed to find or remove the provided ID"; }

    }

    /// <summary>
    /// This Method updates a NotificationRegistration
    /// </summary>
    /// <param name="_Id"></param>
    /// <param name="_WehbookAddress"></param>
    /// <param name="_FoodItems"></param>
    /// <param name="_Longitude"></param>
    /// <param name="_Latitude"></param>
    /// <param name="_DistanceInMiles"></param>
    /// <param name="_QueryType"></param>
    /// <returns>Returns a bool which is true if the Registration was updated and false if there was an issue.</returns>
    internal static bool UpdateNotificationRegistration(string? _Id, string _WehbookAddress, string _FoodItems, string _Longitude, string _Latitude, double _DistanceInMiles, string _QueryType)
    {
        try
        {
            FoodVendorNotificationRegistration RegistrationToUpdate = Registrations.Find(Registration => Registration.Id.CompareTo(_Id) == 0);

            RegistrationToUpdate.UpdateNotificationRegistration(
                 _WehbookAddress,
                 _FoodItems.Split(':').ToList<string>(),
                 _Longitude,
                 _Latitude,
                 _DistanceInMiles,
                 _QueryType
                );
            return true;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// This class is used for working with the SF food vendor  CSV data.
/// </summary>
public static class FoodVendorUpdate
{
    /// <summary>
    /// This method fetches and reads a file, in this case a CSV
    /// reference from https://stackoverflow.com/questions/11082062/how-to-read-a-csv-file-from-a-url
    /// </summary>
    /// <param name="url"></param>
    /// <returns>Returns a string which contains the contents of the CSV</returns>
    public static string GetCSV(string url)
    {
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

        StreamReader sr = new StreamReader(resp.GetResponseStream());
        string results = sr.ReadToEnd();
        sr.Close();

        return results;
    }

    /// <summary>
    /// Fetches the current CSV file from DataSF, parses the data out to fit the <FoodVendor> Object.
    /// This method is intended to pull the data initially, it does not update the data.
    /// //TODO Update this method to update the FoodVendor list instead of just add, 
    /// </summary>
    public static void FetchVendors()
    {
        List<FoodVendor> parsedVendorsList = new List<FoodVendor>();
        string fileList = GetCSV("https://data.sfgov.org/api/views/rqzj-sfat/rows.csv");

        //split csv up by line for processing
        string[] strings = fileList.Split(new string[] { "\n" }, StringSplitOptions.None);

        List<string> items = new List<string>();

        //parse out ( and " blocks which may contain a comma and mess up splitting by comma as this is a csv
        foreach (string line in strings)
        {
            List<char> tempItem = new List<char>();
            //inBlock indicates we are reading from within () or "", and ignore commas
            var inBlock = false;
            //use | for default
            char endBlockChar = '|';
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '(' && endBlockChar == '|')
                {
                    tempItem.Add((char)line[i]);
                    inBlock = true;
                    endBlockChar = ')';
                }
                else if (line[i] == ')' && endBlockChar == ')')
                {
                    tempItem.Add((char)line[i]);
                    inBlock = false;
                    endBlockChar = '|';
                }
                else if (line[i] == '"' && endBlockChar == '|')
                {
                    tempItem.Add((char)line[i]);
                    inBlock = true;
                    endBlockChar = '"';
                }
                else if (line[i] == '"' && endBlockChar == '"')
                {
                    tempItem.Add((char)line[i]);
                    inBlock = false;
                    endBlockChar = '|';
                }
                else if (line[i] == ',' && !inBlock)
                {
                    items.Add(new string(tempItem.ToArray()));
                    tempItem.Clear();
                }
                else
                {
                    tempItem.Add((char)line[i]);
                }

                //last char in line, write data.
                if (i == line.Length - 1)
                {
                    items.Add(new string(tempItem.ToArray()));
                    tempItem.Clear();
                }
            }
        }

        //create FoodVendor objects from CSV data and add to list
        for (int i = 29; i < items.Count; i += 29)
        {
            var LocationId = items[i];
            var Applicant = items[1 + i];
            var FacilityType = items[2 + i];
            var LocationDescription = items[4 + i];
            var Latitude = items[14 + i];
            var Longitude = items[15 + i];
            var Address = items[5 + i];
            var Permit = items[9 + i];
            var Status = items[10 + i];
            var FoodItems = ((items[11 + i]).Split(new char[] { ':', ';' })).ToList<string>();

            //Exclude vendors that are expired or suspended
            if (Status.CompareTo("EXPIRED") != 0 && Status.CompareTo("SUSPEND") != 0)
            {
                FoodVendor vendor = new FoodVendor
                {
                    VendorId = Guid.NewGuid().ToString(),
                    LocationId = LocationId,
                    Applicant = Applicant,
                    FacilityType = FacilityType,
                    LocationDescription = LocationDescription,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    Address = Address,
                    Permit = Permit,
                    Status = Status,
                    FoodItems = FoodItems
                };
                //TODO check if foodvendor already exists before.
                FoodVendorList.FoodVendors.Add(vendor);
            }
        }
    }
}