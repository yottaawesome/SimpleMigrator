# MSSQL Migrator

A simple MSSQL-to-MSSQL migrator console app written in C# for .NET Framework. It uses MSSQL Server's SQL Bulk Copy to quickly migrate data between MSSQL databases. MSSQL Migrator will automatically determine which tables and columns exist in both databases, and migrate only those. MSSQL Migrator will display its progress as it migrates rows.

## Status

__Complete.__

## How to use it

Set the source and destination connection strings in the `App.config` and you're ready to go. Remember that you will need to specify `Persist Security Info = True` in the connection string if using SQL Server authentication. The `BatchSize` setting in the `App.config` controls the batch size for the bulk copy operation -- you can increase or decrease it for your purposes.
