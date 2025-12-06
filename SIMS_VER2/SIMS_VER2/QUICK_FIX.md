# Quick Fix for Login Issue

## Problem
Cannot login even though password hash appears correct in database.

## Solution Steps

### 1. Verify Database
Run this to check password hash:
```sql
USE SIMS_DB2;
SELECT Username, PasswordHash, LEN(PasswordHash) AS HashLength, Role, Active 
FROM Users 
WHERE Username = 'admin';
```

Expected hash for admin123: `JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=`
Expected length: 44

### 2. Test Login via Debug Endpoint
After starting the application, test login at:
```
http://localhost:5000/api/debug/test-login/admin/admin123
```

This will show:
- If user exists
- If user is active
- Database hash vs computed hash
- Whether hashes match

### 3. Recreate Database (If Needed)
If password hash is wrong, run:
```bash
sqlcmd -S "(localdb)\mssqllocaldb" -i create_fresh_database.sql
```

### 4. Verify Hash Calculation
The hash is calculated using:
- Algorithm: SHA256
- Encoding: Base64
- Code location: `UserService.HashPassword()`

### 5. Common Issues
- **Whitespace**: Code now trims hashes before comparison
- **Encoding**: Ensure UTF-8 encoding
- **Case sensitivity**: Hash comparison is case-sensitive
- **User inactive**: Check `Active = 1` in database

## Test Credentials
- Admin: `admin` / `admin123`
- Faculty: `faculty1` / `faculty123`
- Student: `student1` / `student123`

## Debug Endpoints
- `/api/debug/test-hash/{password}` - Calculate hash for a password
- `/api/debug/check-user/{username}` - Check user details and hash
- `/api/debug/test-login/{username}/{password}` - Test login validation

