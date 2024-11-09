## Online Contest Management System

This project implements a comprehensive backend API to manage online contests, offering a user-friendly platform for organizations, companies, or individuals to create, promote, and manage contests, with streamlined registration and payment functionalities.

### Product Overview

The platform serves as a solution for managing the entire lifecycle of online contests, including creation, posting, promotion, registration management, and payment processing. The primary goal is to simplify contest management for organizers and to enhance accessibility for participants.

### Key Features

#### For Contest Organizers:

- **Create New Contests**:
    - Organizers can create contests with comprehensive details such as name, description, start and end dates, rules, and specific skill requirements.
    - Allows uploading of attachments like guides, images, or video demonstrations.
    - Set participation fees (if applicable) and configure online payment options.
  
- **Manage Contests**:
    - Registration Dashboard: Track participant lists, payment status, and personal details.
    - Application Approval: Manage and approve contestant applications.
    - Automated Notifications: Send confirmation emails and schedule notifications to contestants.

- **Analytics Tools**:
    - Registration Statistics: Track the number of registrations.
    - Participant Demographics: Analyze age, gender, and geographical data.
    - Revenue Tracking: View statistics for paid contests.

#### For Users:

- **Account Registration**:
    - Users can register via email or log in using social accounts (e.g., Facebook, Google).

- **Browse and Register for Contests**:
    - Search and filter contests based on criteria such as category, age requirements, etc.
    - View contest details, including description, prizes, requirements, and deadlines.
    - Register by completing necessary details and make online payments (if required).

- **Profile Management**:
    - Users can track registered contests, edit their profiles, and update personal details.

#### For System Administrators:

- **Contest Approval**: Review and approve registered contests.
- **Registration Statistics**: Monitor the number of created contests and registered users.
- **Revenue Tracking**: Track system revenue generated from contest fees.

### 3. Technologies

- **C#**: Core language for backend API development.
- **ASP.NET Core**: Framework for building the API.
- **MongoDB**: NoSQL database to store contest data.
- **JWT Authentication**: Authentication and authorization via JSON Web Tokens.

### 4. Project Structure

- **Controllers**: API controllers to handle requests.
- **Data**:
  - **Models**: Data models for contests and other entities.
  - **Repositories**: Data access layer to interact with MongoDB.
- **Infrastructure**:
  - **Services**: Contains business logic and data handling.
- **Models**: Data transfer models between client and API.

### 5. Getting Started

1. **Prerequisites**:
   - .NET 8 SDK
   - MongoDB
2. **Clone the repository**:
   - `git clone https://github.com/sloweyyy/OnlineContestManagementSystem-be`
3. **Install dependencies**:
   - `dotnet restore`
4. **Configure MongoDB connection**:
   - Update the MongoDB connection string in `appsettings.json`.
5. **Move to project directory**:
   - `cd OnlineContestManagementSystem`
6. **Run the application**:
   - `dotnet run`

### Usage

- **Authentication**: Obtain a valid JWT token for API requests.
- **Authorization**: Only contest creators can update or delete their contests. The API uses JWT-based user ID for authorization.

### License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

