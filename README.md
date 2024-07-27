# Page Tracker
Helping you read more by tracking how many pages you've read per day.

![](/docs/pages-form-light.png)

## About
A hobby project using React / .NET Web API / SQL Server.

Using Mantine UI Components library https://mantine.dev/

How I'm organising and tracking my own requirements https://kathleen-von-gnielinski.notion.site/Page-Tracker-d39aa2e60d4741be8f99fa3811d92bd3

## Helpful commands

Add a migration

`cd src/PageTracker.Api`

`dotnet ef migrations add {MigrationName} --output-dir ../PageTracker.Infrastructure/Persistence/Migrations --project ../PageTracker.Infrastructure`

`dotnet ef database update` 

Build the migrations docker image from the root of the project

`docker build . -f ./src/PageTracker.Infrastructure/Dockerfile`