# Reko-Market-Mvp

## Project description

**What are we making?**<br>
The goal is to create a functional product/app using industry standard tools and practices, mainly for educational and portfolio purposes.
The idea is based on a design study focused on improving the end-user / supplier experience provided by REKO-Ringen.

**What is REKO-Ringen?**<br>
REKO-ringen stands for fair trade/consumption and is a way to sell and buy local food directly from the farmer/producer. REKO-ringen enables farmers/producers to sell their ecological products and communicate directly with consumers. There are several REKO-ring locations in norway.

Today, REKO-ringen use private Facebook groups for each location to sell and buy products. Farmers/producers creates a Facebook post announcing their products, and consumers comment on the post to reserve/buy food.  
The market is physical once a month, the farmers who are ready with their products annonces their goods. Often you have to pay on VIPPS to the producer beforehand, but not always.
Our solution will focus on clean architecture, both regarding design and code principles to simplify the cumbersome Facebook solution REKO-ringen uses for their consumers today. 
The outcome will give us knowledge in common data processes, integration and testing.

**Frontend**<br>
Focus on creating clean, maintanable and readable code. Creating reusable components in Tailwind CSS, following the DRY principle. The goal is to have one frontend, with two different role experiences (end-user + supplier). Ideally there will also be a frontend administrator interface. The frontend will read data from backend using exposed API endpoints.
Frontend should include:
- Landing Page
- Role Based Login / Registration
- Product visualization
- User/profile page
    - Option to delete profile
    - Option to change username
- 

**Backend**<br>
Design and implement a clean-architecture based policy evaluation engine in C#. 
Focused on SOLID, KISS, YAGNI, DRY principles, test-driven development, and maintainable decision logic.
Demonstrate real-world access control and rule evaluation patterns used in enterprise systems.
Backend should include:
- Persistant database
- API endpoints
- CRUD operations (Create, Read, Update, Delete)
    - Users
    - Products
- Login management
- Error handling
- App Settings?

## Project Management

Kanban board in MS Planner connected to Teams channel

## Tech stack

***Version Control***
- Git

***Documentation***
- Markdown

***Backend***
- ASP .NET Core

***Database***
- Entity Framework Core
- SQLite

***Frontend / UI***
- React Native
- Tailwind CSS
- TypeScript

***Unit + Integration Testing***
- xUnit
- Moq
- Jest
- React Testing Library

***CI/CD***
- Docker
- Azure

## Software/Tools

- Visual Studio Code
- Visual Studio Community
- Git / Github

## Features

**Must have**
- Inventory Item (Id, Price, Category, Weight)
- User (Id, UserName, Role (*End-user*, *Supplier*, *Admin*))
- Location (County, City, Address)
- Database
    - Items
    - Users
    - Location
- API Endpoints (CRUD) for
    - Items
    - Users
- Landing page
- Some Unit / Integration testing

**Should have**
- User Login
- Multiple Pages
    - Login / Registration
    - Home
    - Profile
    - Products 
- Purchase product (End-user)
- Add new product (Supplier)
    - Options to edit product details (price, weight etc..)
- Routing of groups and middleware to separate users
- More Unit / Integration testing

**Nice to have**
- Shopping Cart
- Payment solution
- Full test coverage
- Publish to App Store / Google Play