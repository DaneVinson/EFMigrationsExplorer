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
                    foreach (var migration in migrator.GetDatabaseMigrations())
                    {
                        Console.WriteLine($"Migration: {migration}");
                    }
                }

                Console.WriteLine($"Pending migrations for Assembly {configuration.MigrationsAssembly.GetName().Name}");

                foreach (var pendingMigration in migrator.GetPendingMigrations())
                {
                    Console.WriteLine($"----{pendingMigration}");
                    if (!testOnly)
                    {
                        string typeName = configuration.MigrationsNamespace + "." + pendingMigration.Split('_').Last();
                        var type = configuration.MigrationsAssembly.GetType(typeName);
                        Console.WriteLine(type);
                        migrator.Update(pendingMigration);
                    }
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
