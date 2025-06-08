-- Verifique o ID do usuário
SELECT Id FROM AspNetUsers WHERE Email = 'reluxaccs@gmail.com';

-- Verifique o ID da role "Admin"
SELECT Id FROM AspNetRoles WHERE Name = 'Admin';

-- Insira o usuário na role (substitua USER_ID e ROLE_ID)
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ('USER_ID', 'ROLE_ID');