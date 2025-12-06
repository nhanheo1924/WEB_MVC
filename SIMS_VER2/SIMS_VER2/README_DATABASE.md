# Database Initialization Guide

## SQL Script Location
The database initialization script is located at: `init_database.sql`

## How to Run the SQL Script

### Option 1: Using SQL Server Management Studio (SSMS)
1. Open SQL Server Management Studio
2. Connect to your SQL Server instance (LocalDB or SQL Server)
3. Open the file `init_database.sql`
4. Execute the script (F5 or Execute button)

### Option 2: Using Command Line (sqlcmd)
```bash
sqlcmd -S (localdb)\mssqllocaldb -i init_database.sql
```

### Option 3: Using Azure Data Studio
1. Open Azure Data Studio
2. Connect to your database server
3. Open `init_database.sql`
4. Run the script

## What the Script Does

1. **Drops existing database** (if exists) - `SIMS_DB2`
2. **Creates new database** - `SIMS_DB2`
3. **Creates all tables** with proper constraints:
   - Users
   - Students
   - Courses
   - CourseOfferings
   - Enrollments
4. **Creates indexes** for performance:
   - Unique index on Username
   - Unique index on StudentCode
   - Unique index on CourseCode
   - Unique composite index on Enrollment (StudentId, CourseOfferingId)
5. **Inserts seed data**:
   - 4 Users (1 Admin, 2 Faculty, 1 Student)
   - 5 Students
   - 5 Courses
   - 5 Course Offerings
   - 6 Enrollments

## Default Login Credentials

After running the script, you can login with:

- **Admin**: 
  - Username: `admin`
  - Password: `admin123`

- **Faculty**: 
  - Username: `faculty1` or `faculty2`
  - Password: `faculty123`

- **Student**: 
  - Username: `student1`
  - Password: `student123`

## Password Hash Calculation

The password hashes in the SQL script are calculated using:
- Algorithm: SHA256
- Encoding: Base64

To calculate a new password hash, you can use the `DbInitializer.HashPassword()` method in C# or any SHA256 + Base64 tool.

## Database Connection String

Update your `appsettings.json` if needed:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SIMS_DB2;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

## Verification

After running the script, you should see:
- UserCount: 4
- StudentCount: 5
- CourseCount: 5
- OfferingCount: 5
- EnrollmentCount: 6

## Notes

- The script uses `ON DELETE CASCADE` for Enrollments
- The script uses `ON DELETE SET NULL` for optional foreign keys
- The script uses `ON DELETE RESTRICT` for Course-CourseOffering relationship
- All password hashes are the same for simplicity (in production, use unique hashes)

