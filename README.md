# Music Event Management System

[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](#)
[![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](#)
[![MySQL](https://img.shields.io/badge/MySQL-4479A1?style=for-the-badge&logo=mysql&logoColor=white)](#)
[![Bootstrap](https://img.shields.io/badge/Bootstrap_5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](#)
[![SendGrid](https://img.shields.io/badge/SendGrid-009DD9?style=for-the-badge&logo=twilio&logoColor=white)](#)
[![OpenAI](https://img.shields.io/badge/OpenAI-412991?style=for-the-badge&logo=openai&logoColor=white)](#)
[![Perplexity](https://img.shields.io/badge/Perplexity_AI-222222?style=for-the-badge&logo=perplexity&logoColor=white)](#)

> **Swinburne University of Technology — COS20007**  
> A comprehensive web-based platform for organizing, managing, and attending music events. Built with ASP.NET Core 8 Razor Pages, EF Core, MySQL, and powered by Perplexity AI & OpenAI integrations. Features include PDF ticket generation, automated email notifications, and an intelligent chatbot for event recommendations.

---

## Table of Contents

- [Disclaimer](#disclaimer)
- [Features](#features)
- [Project Modules](#project-modules)
- [Tech Stack](#tech-stack)
- [Quick Start](#quick-start)
- [Environment Configuration](#environment-configuration)
- [Important Note](#important-note)
- [Documentation & Resources](#documentation--resources)
- [Acknowledgements](#acknowledgements)

---

## Disclaimer

- **Educational Purpose:** This project was developed solely as a personal project for academic purposes as part of the COS20007 coursework at Swinburne University of Technology.
- **AI Accuracy:** The platform uses experimental AI features powered by Perplexity and OpenAI APIs. Event descriptions and recommendations may contain inaccuracies.
- **Data Privacy:** As this is an educational prototype, users are strongly advised **not** to input or upload highly sensitive, personal, or confidential information (including real financial details or passwords).
- **No Warranty:** This software is provided "as is" and without any guarantees or warranties of any kind.

---

## Features

- **Event Management (CRUD):** Organizers can create, edit, manage, and delete music events with varying pricing tiers and custom required fields.
- **Ticketing System:** Users can register for events, leading to automatic PDF e-ticket generation via QuestPDF.
- **AI Integrations:**
  - **Auto-Generated Descriptions:** Automatically create engaging event descriptions using Perplexity AI.
  - **AI Chatbot & Recommendations:** Ask the chatbot for music event suggestions tailored to user preferences.
- **Email Notifications:** Automated HTML email templates sent via SendGrid for registrations, payments, and event updates.
- **Authentication & Authorization:** Secure login/register system using ASP.NET Core Identity with role-based access control (Admin vs User).
- **Interactive Dashboard:** Admins can view analytics, manage users, and oversee all registrations.

---

## Project Modules

| Module               | Description                                                                 |
| -------------------- | --------------------------------------------------------------------------- |
| `Areas/Identity`     | Handles User Registration, Login, Logout, and profile management.           |
| `Models`             | EF Core entity classes mapping to the MySQL database (MusicEvent, User, etc).|
| `Pages/Admin`        | Admin dashboard, CRUD tools for events, and registration management.        |
| `Pages/Events`       | Public-facing event listings, detailed views, and the checkout/registration flow. |
| `Services`           | Core logic including AI integrations, Email sending, PDF generation, and Repositories. |

---

## Tech Stack

| Layer          | Technology                                   |
| -------------- | -------------------------------------------- |
| **Framework**  | ASP.NET Core 8.0 (Razor Pages)               |
| **Language**   | C# 12                                        |
| **Database**   | MySQL (Pomelo.EntityFrameworkCore.MySql)     |
| **Auth**       | ASP.NET Core Identity                        |
| **Email API**  | Twilio SendGrid                              |
| **AI APIs**    | Perplexity AI (Sonar), OpenAI (gpt-4o-mini)  |
| **PDF Engine** | QuestPDF                                     |
| **Frontend**   | HTML5, CSS3, Bootstrap 5, jQuery, Chart.js   |

---

## Quick Start

### 0. Clone the Repository

Clone the project to your local machine:

```bash
git clone https://github.com/YOUR_USERNAME/Music-Event-Management-App.git
cd Music-Event-Management-App/MusicEventManagementSystem
```

### 1. Database Setup

Ensure you have a local MySQL server running. Then, execute the provided SQL script to create and seed the database:

1. Open your MySQL client (e.g., MySQL Workbench).
2. Execute the script located at `SQL/Music.sql`.

Alternatively, use Entity Framework Core tools to update the database:
```bash
dotnet ef database update
```

### 2. Run the Application

Restore dependencies and start the web server:

```bash
dotnet restore
dotnet run
```

The application will typically be accessible at `https://localhost:7143` or `http://localhost:5143`.

---

## Environment Configuration

Before running the application, you **must** configure your API keys and database connection string.
Create an `appsettings.json` file in the `MusicEventManagementSystem` folder based on the provided `appsettings.Example.json`.

**`appsettings.json`**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=musiceventdb;Uid=root;Pwd=YOUR_MYSQL_PASSWORD;"
  },
  "SendGridSettings": {
    "ApiKey": "YOUR_SENDGRID_API_KEY",
    "FromName": "Music Event System",
    "FromEmail": "your-email@example.com"
  },
  "AiSettings": {
    "ApiKey": "YOUR_PERPLEXITY_API_KEY",
    "ApiUrl": "https://api.perplexity.ai/chat/completions",
    "Model": "sonar"
  },
  "OpenAISettings": {
    "ApiKey": "YOUR_OPENAI_API_KEY",
    "Model": "gpt-4o-mini"
  }
}
```

---

## Important Note

> - You **must** populate the `SendGridSettings:ApiKey` for the email registration confirmation feature to work. You can get an API key from [SendGrid](https://sendgrid.com/).
> - The **AI capabilities** rely on valid API keys for both Perplexity and OpenAI. Place them in `AiSettings` and `OpenAISettings` respectively.
> - For security reasons, the `appsettings.json` file is ignored by Git to prevent your API keys from being leaked. **Never commit your actual API keys to a public repository!**

---

## Documentation & Resources

For further reading or troubleshooting, please refer to the official documentation for the technologies used:

- **[ASP.NET Core Razor Pages](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/)**
- **[Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)**
- **[Pomelo MySQL Provider](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)**
- **[QuestPDF Documentation](https://www.questpdf.com/)**
- **[SendGrid API Reference](https://docs.sendgrid.com/api-reference/how-to-use-the-sendgrid-v3-api)**

---

## Acknowledgements

Developed for **COS20007** coursework at **Swinburne University of Technology** by Nguyễn Thành Kiên (105507742). Special thanks to Ms. Ly Quynh Tran and tutors for their guidance throughout the coursework.