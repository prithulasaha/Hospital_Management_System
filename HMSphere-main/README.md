# HMSphere - Hospital Management System (V.1)

HMSphere is a web-based hospital management system built using **ASP.NET Core MVC**. The project streamlines hospital operations by managing patient appointments, medical records, and staff scheduling. The system offers role-based access control, ensuring that patients, doctors, and administrators have access only to the features relevant to their roles.

## Table of Contents

1. [Project Features](#project-features)
2. [Technologies Used](#technologies-used)
3. [Setup Instructions](#setup-instructions)
4. [UI](#ui)
5. [Usage](#usage)
6. [Team](#contributers-team)
7. [Contribution Guidelines](#contribution-guidelines)
8. [Contact Information](#contact-information)

## Project Features

#### Patient Management
- Patients can book and manage appointments.
- Access to personal medical records.
#### Doctor Features
- View and update patient records.
- Manage schedules.
#### Administrative Features
- Manage doctor schedules and patient records.
- Oversee staff schedules and hospital operations.
#### Role-Based Access Control
Ensures each user role (patients, doctors, administrators) accesses only the data relevant to their role.

## Technologies Used

- **ASP.NET Core MVC**: For building the web application.
- **Entity Framework Core**: For database management and interactions.
- **MS SQL Server**: Database provider.
- **ASP.NET Core Identity**: Role-based authentication and authorization.
- **MailKit and MimeKit**: For email notifications and services.
- **AutoMapper and Repository Pattern**: For efficient model mapping and service architecture.
- **Clean Architecture**: For a layered project structure ensuring scalability and maintainability.

## Setup Instructions

1. **Clone the Repository**:
```bash
git clone https://github.com/mohdali300/HMSphere.git
cd HMSphere
```
2. **Set Up the Database**:
    - Ensure **MS SQL Server** is installed and running.
    - Update the `appsettings.json` file with your database connection string:
      ```json
      "ConnectionStrings": {
        "DefaultConnection": "Data Source=.;Initial Catalog=HMSphereDB;Integrated Security=True;Trust Server Certificate=True;"
      }
      ```
3. **Run Database Migrations**:
    - Open a terminal in the project directory and run:
      ```bash
      dotnet ef database update
      ```
4. **Install Dependencies**:
    - Restore NuGet packages:
      ```bash
      dotnet restore
      ```
5. **Run the Application**:
    - Start the application using:
      ```bash
      dotnet run
      ```
    - Open your browser and navigate to `http://localhost:(your local)`.
6. **Default Roles and Users**:
    - run data seed in program.cs.
    - Create accounts for testing or log in with predefined credentials.

## Usage

1. **Patient Portal**: Patients can log in to:
   - Book and view upcoming appointments.
   - Access their medical records.
2. **Doctor Dashboard**: Doctors can:
   - View their schedules and assigned patients.
   - Update patient records.
3. **Admin Panel**: Administrators can:
   - Manage schedules for doctors and staff.
   - Oversee patient records and hospital operations.

## UI
some UIs:

<img src="HMSphere.MVC/wwwroot/images/ui/Screenshot 2024-12-19 221905.png" alt="1" width="350"/> 
<img src="HMSphere.MVC/wwwroot/images/ui/Screenshot 2024-12-16 200225.png" alt="1" width="350"/>
<img src="HMSphere.MVC/wwwroot/images/ui/Screenshot 2024-12-16 200324.png" alt="1" width="350"/>
<img src="HMSphere.MVC/wwwroot/images/ui/Screenshot 2024-12-16 200701.png" alt="1" width="350"/>
<img src="HMSphere.MVC/wwwroot/images/ui/Screenshot 2024-12-16 200343.png" alt="1" width="350"/>
<img src="HMSphere.MVC/wwwroot/images/ui/Screenshot 2024-12-16 200810.png" alt="1" width="350"/>
<img src="HMSphere.MVC/wwwroot/images/ui/Screenshot 2024-12-20 002019.png" alt="1" width="350"/>
<img src="HMSphere.MVC/wwwroot/images/ui/Screenshot 2024-12-20 002141.png" alt="1" width="350"/>

## Contributers Team
- [**Mohamed Ali**](https://github.com/mohdali300)
- [**Ahmed Nasser**](https://github.com/AhmedNasser03)
- [**Rawan Adel**](https://github.com/R0wanAdel)
- [**Eman Hamam**](https://github.com/EmanHamam)
- [**Nermeen Ashraf**](https://github.com/0xNermeenaa)
- [**Nahla Mohamed**](https://github.com/nahla-mo)

## Contribution Guidelines

We welcome contributions to enhance HMSphere! If you'd like to contribute:
1. Star and Fork the repository.
2. Create a new branch for your feature or bugfix.
3. Submit a pull request describing your changes.

## Contact Information

If you have any questions or suggestions, feel free to contact us.


