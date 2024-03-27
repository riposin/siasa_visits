# Visits
The idenrollvists application (a.k.a. Visits) consists of two independent parts, on the one hand the mobile part, hereinafter the "Vmobile" and on the other hand the web part, hereinafter the "Vweb".

Vweb is the first in the visit process and is the one contained in this repository, it is used to send an invitation to a person and then that person generates the QR code that will be used in the facility to be registered using Vmobile.

Vweb is ready to work with SQLite and Microsoft SQL Server, hereinafter SQLS, by making a few simple changes.

To run the version of SQLite or move from SQLS to SQLite, make sure you have SQLite x86 fully installed, i.e. install the assemblies in the Global Assembly Cache and the Components for Visual Studio. The SQLite version of Vweb can only be built/debugged using VS2019 exclusively, there is a way to make it work with VS2022 but it was not tested as part of the project.
For more detail visit https://github.com/ErikEJ/SqlCeToolbox/wiki/EF6-workflow-with-SQLite-DDEX-provider.

As of March 27, 2024, the project is configured/ready to work with SQLS.

# Run Vweb for SQLite
1. Clone the repo.
2. Execute this PowerShell script ~/Visits/App_Data/db_script_new.ps1
3. Follow the few steps indicated in by the script.
4. Visual Studio is usually configured to restore NuGet packages; if that is not your case, restore the packages manually.
5. Clean solution.
6. Build solution.
7. Run.

# Run Vweb for SQLS
1. Clone the repository
2. Open ~/Visits/App_Data/db_script_new.sql
3. Adjust the “SQL UPDATES” for the SMTP server, this can be done after.
4. Execute the script in the target SQLS server.
5. In Web.config adjust the visitsEntities connection string to match with your server.
6. Visual Studio is usually configured to restore NuGet packages; if that is not your case, restore the packages manually.
7. Clean solution.
8. Build solution.
9. Run.

# For new controllers
If your controller will serve views and use the Visits layout, be sure to add code to make Vweb settings and Vweb locales available to the views of the controller. See the PreregistrationController.cs for more detail.

# For changes in DB
Make sure you apply the same changes to both SQL scripts, for SQLite and SQLS.

# Change from SQLS to SQLite
0. Clone the repo.
1. Generate the DB for the target DBMS.
	1. Execute this PowerShell script ~/Visits/App_Data/db_script_new.ps1
2. Open with VS2019 the Visits project that is within the Visits solution.
3. Delete Models/visits.edmx
4. In Web.Config, section Connection Strings (CS)
	1. For visitsEntitiesSQLite, change “|DataDirectory|” by the absolute path to visits.db
	2. Change CS name: visitsEntities to visitsEntitiesSQLS
	3. Change CS name: visitsEntitiesSQLite to visitsEntities
5. Generate the EDMX
	1. Add a new item to Models: Right click over Models directory -> Add -> New Item -> Visual C# -> Data -> ADO.NET Entity Data model -> Name: visits
	2. Select “EF Designer from database” and Next.
	3. Click “New Connection” button.
	4. Select the Data Source type “SQLite Provider (Simple for EF6 by ErikEJ)” -> Ok.
	5. Type the ConnectionString field, make sure to adjust the path to the proper one: data source=C:\Users\rpool\dev\Visits\Visits\App_Data\visits.db
	6. Click Ok.
	7. Select all and only Tables
	8. Set name for the namespace to visitsModel and then click finish.
	9. Save the EDMX diagram.
	10. In Web.config change the visitsEntities connection and remove the absolute path to the visits.db file and put |DataDirectory|, without backslash.

# Change from SQLite to SQLS
0. Clone the repo.
1. Generate the DB for the target DBMS.
	1. Execute this SQL script in the server ~/Visits/App_Data/db_script_new.sql
2. Open with VS2019 the Visits project that is within the Visits solution.
3. Delete Models/visits.edmx
4. In Web.Config, section Connection Strings (CS)
	1. Change CS name: visitsEntities to visitsEntitiesSQLite
	2. Change CS name: visitsEntitiesSQLS to visitsEntities
5. Generate the EDMX
	1. Add a new item to Models: Right click over Models directory -> Add -> New Item -> Visual C# -> Data -> ADO.NET Entity Data model -> Name: visits
	2. Select “EF Designer from database” and Next.
	3. Click “New Connection” button.
	4. Select the Data Source type “Microsoft SQL Server” -> Ok.
	5. Configure the connection to the SQLServer and the visits database.
	6. Click Ok.
	7. Select all and only Tables
	8. Set name for the namespace to visitsModel and then click finish.
	9. Save the EDMX diagram.