# Overview

This is a casual project to show aptitude in coding web and server application.
Objective: Collect CSV file from SF Food vendors repository and provide some interesting API.

The application starts up by pulling the CSV and consuming the data into a static in-memory list and a recurring task will pull the CSV once an hour for any new additions.
From the API you may pull Approved or Requested vendors, you may also create a notification registration in order to be notified if any newly added Food Vendors offer a particular food item OR are within specified Miles of your coordinates or both.
Notifications are intended to be texted out to the recipient.


There are 2 applications in this repo, the web API server and the client web app.
Server side application is written in C# using .net core and AspNetCore to handle the HTTP service. Client web app is built with React.js.

I started off the application with the web and react boilerplate.


Possible Improvements

* add a daily fetch to query for new data
* add an analytics page with interesting facts about all of the food trucks such as most common food type, truck or push cart more common.. etc
* add GET query paramaters to pull specific vendors or vendors with particular food type
* error handling, what if CSV is not available for download?



## API
The API has a few endpoints.

###GET
* 
This endpoint returns all of the food vendors in the order they were received initially, it does limit the data to more relevant info like name, address, food type, only Requested or Approved status.

###PUT

###DELETE



## Web page
