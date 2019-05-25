# MSSQL Migrator
A simple MSSQL migrator console app written in C#.

## Status
__Complete.__

## What it does
Simple Migrator uses MSSQL Server's SQL Bulk Copy to quickly migrate data between MSSQL databases. Simple Migrator will automatically determine which tables and columns exist in both databases, and migrate only those. SimpleMigrator will display its progress as it migrates rows.

## Use case
If you need to migrate data or restore data from another database after wiping the data out of it.

## How to use it
Set the source and destination connection strings in the App.config and you're ready to go. Remember that you will need to specify "Persist Security Info = True" in the connection string if using SQL Server authentication. The BatchSize setting in the App.config controls the batch size for the bulk copy operation -- you can increase or decrease it for your purposes.
