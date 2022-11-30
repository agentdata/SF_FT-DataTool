namespace SF_FT_DataTool;

using System;
using System.Net;
using System.Text.Json;

public class FoodVendor
{
    public string? VendorId { get; set; }
    public string? LocationId { get; set;}
    public string? Applicant { get; set;}
    public string? FacilityType { get; set;}
    public string? LocationDescription { get; set;}
    public string? Address { get; set;}
    public string? Permit { get; set;}
    public string? Status { get; set;}
    public List<string>? FoodItems { get; set;}

    //helper method to seriliaze as JSON
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}

public static class FoodVendorList{
    public static List<FoodVendor>? FoodVendors {get; set;}
    public static string ToJson(){
        return JsonSerializer.Serialize(FoodVendors);
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
                // \"(37.794331003246846, -122.39581105302317)\"
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
                    var xyz = new string(tempItem.ToArray());
                    items.Add(xyz);
                    tempItem.Clear();
                }
                else
                {
                    tempItem.Add((char)line[i]);
                }

                //last char in line, write data.
                if (i == line.Length - 1)
                {
                    var xyz = new string(tempItem.ToArray());
                    items.Add(xyz);
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
            var Address = items[5 + i];
            var Permit = items[9 + i];
            var Status = items[10 + i];
            var FoodItems = ((items[11 + i]).Split(new char[] { ':', ';' })).ToList<string>();
            FoodVendor vendor = new FoodVendor
            {
                VendorId = Guid.NewGuid().ToString(),
                LocationId = LocationId,
                Applicant = Applicant,
                FacilityType = FacilityType,
                LocationDescription = LocationDescription,
                Address = Address,
                Permit = Permit,
                Status = Status,
                FoodItems = FoodItems
            };
            parsedVendorsList.Add(vendor);
        }

        FoodVendorList.FoodVendors = parsedVendorsList;
    }
}