# SimpleMigrator
A simple MSSQL migrator console app written in C#.
 
# What it does
Simple Migrator uses MSSQL Server's SQL Bulk Copy to quickly migrate data between databases. Simple Migrator will automatically determine which tables and columns exist in both databases, and migrate only those.

# Use case
If you need to migrate data or restore data from another database after wiping the data out of it.

# How to use it
Set the source and destination connection strings in the App.config and you're ready to go. Remember that you will need to specify "Persist Security Info = True" in the connection string if using SQL Server authentication.
