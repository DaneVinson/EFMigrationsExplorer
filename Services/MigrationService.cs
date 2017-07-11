using MigrationAlpha;
using MigrationBravo;
using MigrationsLibrary.Migrations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MigrationService
    {
        public static void Migrate(bool testOnly = false)
        {
            bool firstConfiguration = true;
            foreach (var configuration in GetMigrationConfigurations())
            {
                DbMigrator migrator = new DbMigrator(configuration);

                if (firstConfiguration)
                {
                    firstConfiguration = false;
                    Console.WriteLine("---List of All Migrations---");
                    foreach (var migration in migrator.GetDatabaseMigrations())
                    {
                        Console.WriteLine($"Migration: {migration}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("---Pending Migration---");
                }

                var pendingMigrations = migrator.GetPendingMigrations();

                Console.WriteLine($"{pendingMigrations.Count()} Pending migrations for Assembly {configuration.MigrationsAssembly.GetName().Name}");

                foreach (var pendingMigration in pendingMigrations)
                {
                    if (testOnly) { Console.Write("\t{0} pending", pendingMigration); }
                    if (!testOnly)
                    {
                        string typeName = configuration.MigrationsNamespace + "." + pendingMigration.Split('_').Last();
                        var type = configuration.MigrationsAssembly.GetType(typeName);
                        migrator.Update(pendingMigration);
                        Console.Write("\t{0} applied", type);
                    }
                    Console.WriteLine();
                }
            }
        }

        private static List<MigrationsLibrary.Migrations.Configuration> GetMigrationConfigurations()
        {
            List<MigrationsLibrary.Migrations.Configuration> configurations = new List<MigrationsLibrary.Migrations.Configuration>();
            var dbConnectionInfo = new DbConnectionInfo("StuffContext");

            configurations.Add(new MigrationsLibrary.Migrations.Configuration()
            {
                MigrationsAssembly = Assembly.GetAssembly(typeof(Alpha)),
                MigrationsNamespace = "MigrationsLibrary.Migrations",
                TargetDatabase = dbConnectionInfo
            });

            configurations.Add(new MigrationsLibrary.Migrations.Configuration()
            {
                MigrationsAssembly = Assembly.GetAssembly(typeof(Bravo)),
                MigrationsNamespace = "MigrationsLibrary.Migrations",
                TargetDatabase = dbConnectionInfo
            });

            configurations.Add(new MigrationsLibrary.Migrations.Configuration()
            {
                MigrationsAssembly = Assembly.GetAssembly(typeof(MigrationsLibrary.Migrations.Configuration)),
                MigrationsNamespace = "MigrationsLibrary.Migrations",
                TargetDatabase = dbConnectionInfo
            });

            return configurations;
        }
    }
}
