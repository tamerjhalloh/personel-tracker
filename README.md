# personel-tracker

This is a small project to track personnel check in / check out.

Key Features
* User can check in / check out
* Admin can track personnel actions.


## How to run the project
To run the project you have to run 2 projects
* Personnel.Tracker.WebApi
* Personnel.Tracker.Portal

before running `Personnel.Tracker.WebApi` project, database connection string has to be modified to be able to run the project. It is located in [appsettings.json](https://github.com/tamerjhalloh/personel-tracker/blob/master/Personnel.Tracker.Portal/appsettings.json) file.
before running `Personnel.Tracker.Portal` project, you have to change `Personnel.Tracker.WebApi` endpoint if it changed. It is located in [appsettings.json](https://github.com/tamerjhalloh/personel-tracker/blob/master/Personnel.Tracker.WebApi/appsettings.json) file. appsettings.json => restEase => services section.

## Default admin login 
When you run the project a default db and asmin user will be created
Username: admin@admin.com
Password: admin2021




