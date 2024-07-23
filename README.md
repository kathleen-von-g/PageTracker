# Page Tracker
Helping you read more by tracking how many pages you've read per day.

![](/docs/pages-form-light.png)

## About
A hobby project using React / .NET Web API / SQL Server.

Using Mantine UI Components library https://mantine.dev/


## Helpful commands

Add a migration

`cd src/PageTracker.Api`

`dotnet ef migrations add {MigrationName} --output-dir ../PageTracker.Infrastructure/Persistence/Migrations --project ../PageTracker.Infrastructure`

`dotnet ef database update` 