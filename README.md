What is the APG Project?
=====================
The APG Project is a Payment Gateway Project written in .NET Core

The goal of this project is implement the different payment gateway integrations, it represnts Terminal Management System, Merchant Management System and Trabsactions Management System as well.

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/6518989bea914b348c92385dda05f93d)](https://www.codacy.com/manual/EduardoPires/EquinoxProject?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=EduardoPires/EquinoxProject&amp;utm_campaign=Badge_Grade)
[![Build status](https://ci.appveyor.com/api/projects/status/rl2ja69994rt3ei6?svg=true)](https://ci.appveyor.com/project/EduardoPires/equinoxproject)
![.NET Core](https://github.com/EduardoPires/EquinoxProject/workflows/.NET%20Core/badge.svg)
[![License](https://img.shields.io/github/license/eduardopires/equinoxproject.svg)](LICENSE)
[![Issues open](https://img.shields.io/github/issues/eduardopires/equinoxproject.svg)](https://huboard.com/EduardoPires/EquinoxProject/)


## How to use:
- You will need the latest Visual Studio 2022 and the latest .NET Core SDK.
- ***Please check if you have installed the same runtime version (SDK) described in global.json***
- The latest SDK and tools can be downloaded from https://dot.net/core.

Also you can run the APG Project in Visual Studio Code (Windows, Linux or MacOS).

To know more about how to setup your enviroment visit the [Microsoft .NET Download Guide](https://www.microsoft.com/net/download)

## Technologies implemented:

- ASP.NET 6.0 (with .NET Core 6.0)
 - ASP.NET MVC Core 
 - ASP.NET WebApi Core with JWT Bearer Authentication
 - ASP.NET Identity Core
- Entity Framework Core 6.0
- .NET Core Native DI
- AutoMapper
- FluentValidator
- MediatR
- Swagger UI with JWT support
- .NET DevPack
- .NET DevPack.Identity

## Architecture:

- Full architecture with responsibility separation concerns, SOLID and Clean Code
- Domain Driven Design (Layers and Domain Model Pattern)
- Domain Events
- Domain Notification
- Domain Validations
- CQRS (Imediate Consistency)
- Event Sourcing
- Unit of Work
- Repository

## News

**v1.7 - 04/06/2021**
- Migrated for .NET 6.0
- All dependencies is up to date

**v1.6 - 06/09/2020**
- Full Refactoring (consistency, events, validation, identity)
- Added [NetDevPack](https://github.com/NetDevPack) and saving a hundreds of code lines
- All dependencies is up to date

**v1.5 - 01/22/2020**
- Migrated for .NET Core 3.1.1
- All dependencies is up to date
- Added JWT (Bearer) authentication for WebAPI
- Added JWT support in Swagger

**v1.4 - 02/14/2019**
- Migrated for .NET Core 2.2.1
- All dependencies is up to date
- Improvements for last version of MediatR (Notifications and Request)

**v1.3 - 05/22/2018**
- Migrated for .NET Core 2.1.2
- All dependencies is up to date
- Improvements in Automapper Setup
- Improvements for last version of MediatR (Notifications and Request)
- Code improvements in general

**v1.2 - 08/15/2017**
- Migrated for .NET Core 2.0 and ASP.NET Core 2.0
- Adaptations for the new Identity Authentication Model

**v1.1 - 08/09/2017**
- Adding WebAPI service exposing the application features
- Adding Swagger UI for better viewing and testing
- Adding MediatR for Memory Bus Messaging

## Disclaimer:
- **NOT** intended to be a definitive solution
- Beware to use in production way
- Maybe you don't need a lot of implementations that is included, try avoid the **over engineering**

## About the next versions
Watch our [RoadMap](https://github.com/EduardoPires/EquinoxProject/wiki/RoadMap) to know the new changes

## Pull-Requests 
Make a contact! Don't submit PRs for extra features, all the new features are planned
