# Restaurant Reservations
A small application to store restaurant reservations.

Problem Statement:
==================

[WPF Applicants] Design a WPF application for handling restaurant reservations. 
The input for the application is an XML file containing information about the tables in the restaurant and the maximum occupancy per table. (please see the attached tables.xml file)
The application should handle the following functionality for a single day. Assume that the restaurant operates from 10am to 10pm.

	a. Add reservation
	b. Delete reservation
	c. Edit reservation.

While designing your application make the following considerations-

	a. Recovery if the app crashes. (Reservations should not be lost)
   b. Please make sure that the UI is always responsive.

Assumptions:
============
1. Table data input file will be in correct format.
2. 

Issues that I fixed before moving forward:
==========================================

The input file Tables.xml was not in correct format:
1. Root 'Tables' element didn't have correct closing tag.
2. All 'Table' elements didn't have closing tags.

I fixed it to read properly through XmlSerializer class.

Prerequisites:
===============

I have used couple of third party APIs in my solution. I have added them through NuGet Package manager directly from Project references in Solution.
NuGet Packages is used for development:
1. NLog for logging machanism.
2. AutoFac for Dependency Injections and IoC.
3. MahApp.Metro for Metro style UI.

Please do check following points before using the application:
1. Make sure you are connected to internet.
2. Build the code first which will download all NuGet packages required for this application.

Application Supports following actions:
=======================================
1. User can add/remove/update reservations.
2. Summary of available tables.
3. Summary of total reservation for today and future.
4. Tooltip on each reservation to see the details.
5. Settings to change the input/output folder path.
6. Saving settings.
7. Saving reservation.
8. Not allowing user to add/update reservations in past.
9. Not allowing user to add/update reservations when store is closed.
10.Async implementation to make UI responsive.
11.Loading and Saving data in non-UI thread.
12.User cannot open multiple instances of an application.

Architecture related points:
===============================

1.	Implemented project under MVVM design pattern.
2.	Separated Models and XML operations into separate class library.
3.	XMLOperations are done to Serialize and Deserialize objects like, Tables, Reservations and Settings.
4.	Application mutex bound so only one instance can be used at a time.
5.	NLog configured to store logs on exception.
6.	MahApp dll is used to make application's look and feel like Metro theme based application.
7.	AutoFac used for Dependency Injections and IoC.
8.	Settings window implemented to store default file and folder path for the application. By default all data will be stored under C:/ProgramData/Reservations folder.
9.	Custom ToolTips are available on controls.
10.	XAML based validation has been implemented.
11.	Custom XAML components like styles, tooltips, imagesources, converters, validations are stored into Resource Dictionary which we have merged into main dictionary.
12.	Images are downloaded from internet. No copyrights have been violated.


Pending Tasks:
==================
•	Unit Test cases for the application.


