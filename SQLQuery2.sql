SELECT Id FROM AspNetUsers WHERE Email = 'reluxaccs@gmail.com';

SELECT Id FROM AspNetRoles WHERE Name = 'Admin';

INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ('USER_ID', 'ROLE_ID');