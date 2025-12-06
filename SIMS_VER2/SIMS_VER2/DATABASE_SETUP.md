# Database Setup Guide

## Complete Database Initialization

This guide will help you set up the SIMS database from scratch.

## Quick Start

### Option 1: Using SQL Script (Recommended)

1. **Run the complete initialization script:**
   ```bash
   sqlcmd -S "(localdb)\mssqllocaldb" -i init_database_complete.sql
   ```

2. **Or using SQL Server Management Studio:**
   - Open SSMS
   - Connect to `(localdb)\mssqllocaldb`
   - Open `init_database_complete.sql`
   - Execute (F5)

### Option 2: Using Entity Framework Migrations

```bash
cd SIMS_VER2/SIMS_VER2
dotnet ef database drop --force
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Then run the application once to seed data (DbInitializer will run automatically).

## Database Structure

### Tables Created

1. **Users**
   - Id (PK, Identity)
   - Username (Unique, Required, MaxLength: 50)
   - PasswordHash (Required, MaxLength: 255)
   - Role (Required, int: 0=Admin, 1=Student, 2=Faculty)
   - Active (Default: true)

2. **Students**
   - Id (PK, Identity)
   - StudentCode (Unique, Required, MaxLength: 10, Format: BDXXXXX)
   - FullName (Required, MaxLength: 100)
   - DateOfBirth (Nullable, datetime2)
   - Gender (Nullable, MaxLength: 10)
   - Email (Nullable, MaxLength: 100)
   - Phone (Nullable, MaxLength: 20)
   - Address (Nullable, MaxLength: 200)
   - Program (Nullable, MaxLength: 50)
   - Intake (Nullable, MaxLength: 20)
   - Status (Default: 'ACTIVE', MaxLength: 20)
   - UserId (FK to Users, Nullable, ON DELETE SET NULL)

3. **Courses**
   - Id (PK, Identity)
   - CourseCode (Unique, Required, MaxLength: 20)
   - CourseName (Required, MaxLength: 100)
   - Description (Nullable, MaxLength: 500)
   - Credits (Required, int)
   - Program (Nullable, MaxLength: 50)
   - Type (Nullable, MaxLength: 20)

4. **CourseOfferings**
   - Id (PK, Identity)
   - CourseId (FK to Courses, Required, ON DELETE NO ACTION)
   - Semester (Required, MaxLength: 20)
   - Year (Required, int)
   - ClassCode (Required, MaxLength: 20)
   - InstructorUserId (FK to Users, Nullable, ON DELETE SET NULL)
   - MaxStudents (Required, int)
   - Status (Default: 'OPEN', MaxLength: 20)

5. **Enrollments**
   - Id (PK, Identity)
   - StudentId (FK to Students, Required, ON DELETE CASCADE)
   - CourseOfferingId (FK to CourseOfferings, Required, ON DELETE CASCADE)
   - Status (Default: 'ENROLLED', MaxLength: 20)
   - Grade (Nullable, decimal(5,2))

### Indexes Created

- `IX_Users_Username` - Unique index on Username
- `IX_Students_StudentCode` - Unique index on StudentCode
- `IX_Courses_CourseCode` - Unique index on CourseCode
- `IX_Enrollments_StudentId_CourseOfferingId` - Unique composite index

### Constraints

- **Foreign Keys:**
  - Students.UserId → Users.Id (SET NULL on delete)
  - CourseOfferings.CourseId → Courses.Id (NO ACTION on delete)
  - CourseOfferings.InstructorUserId → Users.Id (SET NULL on delete)
  - Enrollments.StudentId → Students.Id (CASCADE on delete)
  - Enrollments.CourseOfferingId → CourseOfferings.Id (CASCADE on delete)

- **Unique Constraints:**
  - Username
  - StudentCode
  - CourseCode
  - (StudentId, CourseOfferingId) composite

## Seed Data

The script creates the following sample data:

### Users (4)
- **admin** (Role: Admin, Password: admin123)
- **faculty1** (Role: Faculty, Password: faculty123)
- **faculty2** (Role: Faculty, Password: faculty123)
- **student1** (Role: Student, Password: student123)

### Students (5)
- BD00001 - John Doe (Linked to student1 user)
- BD00002 - Jane Smith
- BD00003 - Bob Johnson
- BD00004 - Alice Williams
- BD00005 - Charlie Brown

### Courses (5)
- CS101 - Introduction to Programming
- CS201 - Data Structures
- CS301 - Database Systems
- IT101 - Web Development
- CS401 - Software Engineering

### Course Offerings (5)
- CS101-F24-01 (Fall 2024, Instructor: faculty1)
- CS201-F24-01 (Fall 2024, Instructor: faculty1)
- CS301-F24-01 (Fall 2024, Instructor: faculty2)
- IT101-F24-01 (Fall 2024, Instructor: faculty2)
- CS401-S25-01 (Spring 2025, Instructor: faculty1)

### Enrollments (6)
- John Doe enrolled in CS101 and CS201
- Jane Smith enrolled in CS101 and CS301
- Bob Johnson enrolled in IT101
- Alice Williams enrolled in IT101

## Password Hash Algorithm

Passwords are hashed using:
- **Algorithm:** SHA256
- **Encoding:** Base64

All default passwords ending with "123" have the same hash: `jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=`

## Connection String

The default connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SIMS_DB2;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

## Verification

After running the script, verify the database:

```sql
USE SIMS_DB2;
SELECT 'Users' AS TableName, COUNT(*) AS Count FROM Users
UNION ALL SELECT 'Students', COUNT(*) FROM Students
UNION ALL SELECT 'Courses', COUNT(*) FROM Courses
UNION ALL SELECT 'CourseOfferings', COUNT(*) FROM CourseOfferings
UNION ALL SELECT 'Enrollments', COUNT(*) FROM Enrollments;
```

Expected results:
- Users: 4
- Students: 5
- Courses: 5
- CourseOfferings: 5
- Enrollments: 6

## Troubleshooting

### If database already exists:
The script will drop and recreate it. Make sure to backup any important data first.

### If you get permission errors:
Run SQL Server Management Studio or sqlcmd as Administrator.

### If password hash doesn't match:
The hash is calculated using SHA256 + Base64. Make sure the algorithm matches the one in `UserService.cs`.

## Next Steps

1. Run the application: `dotnet run`
2. Navigate to the login page
3. Login with one of the default accounts
4. Start using the system!

