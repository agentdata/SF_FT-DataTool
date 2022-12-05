# SF Food Vendor data API Overview

This is a casual project to show aptitude in coding web and server application.
Objective: Download CSV file from SF Food vendors https://data.sfgov.org/api/views/rqzj-sfat/rows.csv and provide some interesting API.

The application starts up by downloading the SF food vendor CSV and consuming the data into a static in-memory list.
From the API you may GET Approved or Requested vendors, you may also create a notification registration in order to be notified if any newly added Food Vendors offer a particular food item OR are within specified Miles of your coordinates or both.
Notifications are sent out via webhook for simplicity of testing here, but in a real setting sending out via SMS or Email would be ideal.


There are 2 applications in this repo, the web API server in root dir and the client web app under /ClientApp dir.
Server side application is written in C# using .net core and AspNetCore to handle the HTTP service.
Client web app is built with React.js.


## How to build and run
Here are commands to get this application up and running.
Only one **pre-req**, a recent version of .net (dotnet).

- Clone the repo
    - Browse to the root of the repo in your cli.
    - Type command 'dotnet run', hit enter. This should build and run the server and web application.
    - OR
    - Open .sln file in visual studio and hit the green start button.
- To see the web app browse to https://localhost:7203/ which should tell the engine to start proxy for site and then redirect to https://localhost:44438/  The vendor tab is the only page done.
- Import Postman collection for easy API testing.
- The path for testing may be
    * GET Approved Vendors to see that Food vendor data was pulled in and available for consumption.
    * POST New Notification Registration
        * When you POST a Notification Registration in Postman the ID is stored for the PUT and DEL requests until another POST is completed.
    * GET Notification Registrations (see that the notification registration is stored)
    * PUT Notification Registration (updates the registration data so that it will match with the default new simulated vendor)
        * For reference the Distance between the PUT notification registration and Simulate Vendor is exactly .5mi so anything above will fail and anything .5 or less will match.
    * Browse to https://webhook.site/#!/a0fb1606-0093-4605-b50f-da740f5b7199/b99a56b0-f918-412f-ada2-cf9be3dc149a/1 To view webhooks generated when new vendor has matched one or more conditions.
    * POST Simulate New Vendor Added
        * See webhooks address above to see the webhook received.
    * DEL Notification Registration
    * GET Notification Registration (to see that Notification Registration has been removed)

You may edit the postman requests to change the food items or distance in miles to check that it matches or doesn't match. The Simulate Food Vendor request allows you to update the name, food items (separated by :), and Longitude/latitude.


## API
The API has a few endpoints for retrieving vendor data, and CRUD for Notification Registrations.

### GET
There are a few Get endpoints,
* ApprovedVendors/
This endpoint returns all of the food vendors in the order they were received initially, it does limit the data to more relevant info like name, address, food type, only Requested or Approved status.
* RequestedVendors/
Same as above but returns all presently "requested" vendors in the db.
* NotificationRegistrations/
This Endpoint returns all of the notification registrations.

### POST
* NotificationRegistrations/
This returns a list of all Notification Registrations
* SimulateNewVendor/
This simulates a new vendor being added into the system since this is something that would presumably happen very seldom with the production and testing is important.


### PUT
* NotificationRegistrations/{id?}
This endpoint updates an existing NotificationRegistration

### DELETE
* NotificationRegistrations/{id?}
This endpoint deletes a NotificationRegistration out of the system


## Web app
The Vendor tab shows a table of Approved vendors. Not much else in the web app right now.
I was planning to update the table for sort and filter using https://react-table-v7.tanstack.com/docs/api/useGlobalFilter 


## Possible Improvements

* Add interface to web app to add and check NotificationRegistrations.
* Add a daily fetch to query for new data.
* Add an analytics page with interesting facts about all of the food trucks such as most common food type, truck or push cart more common.. etc
* Add GET query paramaters to pull specific vendors or vendors with particular food type
* Error handling, what if CSV is not available for download?.
* Separate Notification, Food Vendor, SF Food vendor data downloader/parser classes into different Files.
* Add an IsActive field to NotificationRegistration objects and then instead of directly updating Notification Registration Add a new Active Row and set old row to inactive, when deleting just mark row as inactive. This preserves history which could be interesting.
* Move CSV web address to .env file for easy changing.
* Change distance checking to use street address not Lon/Lat coordinates, easier for humans.
* Change distance checking to street distance not point to point.

