## Online Contest Management System - Backend API

This project provides a comprehensive backend API for managing online contests.  It offers a platform for organizations and individuals to create, manage, and promote contests, including streamlined registration and payment processing.

### Table of Contents

- [Product Overview](#product-overview)
- [Key Features](#key-features)
- [Technologies](#technologies)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [License](#license)

### Product Overview

This platform manages the entire lifecycle of online contests: creation, publishing, promotion, registration management, and payment processing.  The primary goal is to simplify contest management for organizers and improve accessibility for participants.

### Key Features

#### For Contest Organizers:

- **Create New Contests:** Create contests with details like name, description, dates, rules, skill requirements, and attachments (guides, images, videos). Set participation fees and configure online payment options.
- **Manage Contests:**  Access a registration dashboard to track participants, payment statuses, and personal details. Approve or reject contestant applications and send automated notifications.
- **Analytics Tools:** Track registration statistics, analyze participant demographics (age, gender, location), and monitor revenue from paid contests.

#### For Users:

- **Account Registration:** Register via email or social login (e.g., Facebook, Google).
- **Browse and Register for Contests:** Search and filter contests by criteria (category, age, etc.). View contest details and register, making online payments if required.
- **Profile Management:** Track registered contests, edit profiles, and update personal details.

#### For System Administrators:

- **Contest Approval:** Review and approve new contests.
- **Registration Statistics:** Monitor the number of contests and registered users.
- **Revenue Tracking:** Track overall system revenue.

### Technologies

- **C#**: Core backend API language.
- **ASP.NET Core**: API framework.
- **MongoDB**: NoSQL database for contest data.
- **JWT Authentication**: JSON Web Tokens for authentication and authorization.
- **CloudinaryDotNet**: For image and video uploads.
- **FirebaseAdmin**: For potentially handling notifications.
- **Google.Cloud.Storage.V1**: For potentially handling cloud storage.
- **Microsoft.AspNetCore.Authentication.Google**: For Google authentication.
- **SendGrid**: For email notifications.
- **PayOS**: For payment processing.
- **Swagger**: API documentation.
- **Github Actions**: CI/CD pipeline.


### Project Structure

- **Controllers**: API controllers handling requests.
- **Data**:
    - **Models**: Data models for contests and other entities.
    - **Repositories**: Data access layer for MongoDB interaction.
- **Infrastructure**:
    - **Services**: Business logic and data handling.
- **Models**: Data transfer objects (DTOs) for communication between client and API.
- **Templates**: Email templates for confirmations and other notifications.


### Getting Started

1. **Prerequisites:**
   - .NET 8 SDK
   - MongoDB
2. **Clone the repository:**
   - `git clone https://github.com/sloweyyy/OnlineContestManagementSystem-be`
3. **Install dependencies:**
   - `dotnet restore`
4. **Configure MongoDB connection:**
   - Update the MongoDB connection string in `.env` file.  
5. **Run the application:**
   - `dotnet run`

### Usage

- **Authentication:** Obtain a JWT token for API requests.
- **Authorization:**  JWT-based user IDs control access.

### License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
