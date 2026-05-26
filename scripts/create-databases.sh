#!/bin/bash

echo "Criando bancos de dados..."

# AuthService Database
docker exec -i sqlserver-auth /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" -C -Q "
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AuthDb')
BEGIN
    CREATE DATABASE AuthDb;
    PRINT 'Database AuthDb criado com sucesso.';
END
ELSE
BEGIN
    PRINT 'Database AuthDb já existe.';
END
"

# CampaignService Database
docker exec -i sqlserver-campaign /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" -C -Q "
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CampaignDb')
BEGIN
    CREATE DATABASE CampaignDb;
    PRINT 'Database CampaignDb criado com sucesso.';
END
ELSE
BEGIN
    PRINT 'Database CampaignDb já existe.';
END
"

# DonationService Database
docker exec -i sqlserver-donation /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" -C -Q "
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DonationDb')
BEGIN
    CREATE DATABASE DonationDb;
    PRINT 'Database DonationDb criado com sucesso.';
END
ELSE
BEGIN
    PRINT 'Database DonationDb já existe.';
END
"

echo "Bancos de dados criados com sucesso!"
