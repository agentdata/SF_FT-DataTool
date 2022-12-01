namespace SF_FT_DataTool;

using Geolocation;
using System;
using System.Collections;
using System.Net;
using System.Text.Json;
using static SF_FT_DataTool.FoodVendorNotificationRegistration;
public enum QueryType { Food, Distance, Either }
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

    //Simulate adding a new vendor with the provided name, food items and lat/lon
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

public class FoodVendorNotificationRegistration
{
    public string? PhoneNumber { get; set; }
    public List<string>? FoodItems { get; set; }
    public string? Longitude { get; set; }
    public string? Latitude { get; set; }
    public double? DistanceInMiles { get; set; }
    public QueryType QueryType { get; set; }

}

public static class FoodVendorNotificationRegistrations
{
    public static List<FoodVendorNotificationRegistration> Registrations { get; set; } = new List<FoodVendorNotificationRegistration>();

    public static void addNewRegistration(string _PhoneNumber, string _FoodItems, string _Longitude, string _Latitude, double _DistanceInMiles, string _QueryType)
    {
        QueryType QueryTypeValue;
        switch (_QueryType)
        {
            case "Food": QueryTypeValue = QueryType.Food; break;
            case "Distance": QueryTypeValue = QueryType.Distance; break;
            case "Either": QueryTypeValue = QueryType.Either; break;
            default: throw new Exception();
        }

        FoodVendorNotificationRegistration newRegistration = new FoodVendorNotificationRegistration
        {
            PhoneNumber = _PhoneNumber,
            FoodItems = _FoodItems.Split(':').ToList<string>(),
            Longitude = _Longitude,
            Latitude = _Latitude,
            DistanceInMiles = _DistanceInMiles,
            QueryType = QueryTypeValue
        };
        Registrations.Add(newRegistration);
    }

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

    //geolocation nuget package to help calculate distance between coordinates
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

    public static bool FoodTypeComparison(List<string> VendorFood, List<string> RegistrationFood)
    {
        bool FoodMatched = false;
        ////check Food items for match between Registration and vendor
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


    public static void SendNotification(FoodVendor Vendor, FoodVendorNotificationRegistration Registration)
    {
        //send notice to phone number of match
        //Registration.PhoneNumber;
        Console.WriteLine("Notifiction sent to :" + Registration.PhoneNumber);
    }
}

public static class FoodVendorUpdate
{
    // reference from https://stackoverflow.com/questions/11082062/how-to-read-a-csv-file-from-a-url
    public static string GetCSV(string url)
    {
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

        StreamReader sr = new StreamReader(resp.GetResponseStream());
        string results = sr.ReadToEnd();
        sr.Close();

        return results;
    }

    //fetches the latest data from DataSF downloadable CSV
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

                FoodVendorList.FoodVendors.Add(vendor);
            }
        }
    }
}

