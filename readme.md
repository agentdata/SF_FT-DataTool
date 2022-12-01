# Overview
There are 2 parts to the application. Server side and client side.
Server side application is written in C# using .net core and using AspNetCore builder Web Application to handle the HTTP service. Client side is built with React.js.

I started off the application with the web and react boilerplate

The server application starts by fetching the SF food truck CSV file from the file server and parses it into an in-memory location for consumption by the API.
The server application also starts up an HTTP server For serving the sf food data

Improvements

* add a daily fetch to query for new data
* add an analytics page with interesting facts about all of the food trucks such as most common food type, truck or push cart more common.. etc
* add GET query paramaters to pull specific vendors
* error handling, what if CSV is not available for download?



## API
The API only has a single GET endpoint

{host_address}:5170/foodvendor

this endpoint returns all of the food vendors in the order they were received initially, it does limit the data to more relevant info like name, address, food type, only Requested or Approved status.


## Web page
