using System;
using System.Collections.Generic;
using MongoDB.Driver;
using SaluteOnline.ConfigurationService.Domain;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Extensions;

namespace SaluteOnline.ConfigurationService.DAL
{
    public static class SeedDefault
    {
        public static void Seed(string mongoPath, string mongoDbName)
        {
            var mongoDb = new MongoClient(mongoPath).GetDatabase(mongoDbName);
            SeedDefaultDashboardForSilentDon(mongoDb);
            SeedDefaultDashboardForUser(mongoDb);
        }

        private static void SeedDefaultDashboardForUser(IMongoDatabase mongoDb)
        {
            var collection = mongoDb.GetCollection<DefaultDashboardConfiguration>(
                typeof(DefaultDashboardConfiguration).ToMongoCollectionName());

            var exists = collection.Count(t => t.ForRole == Roles.User) != 0;
            if (exists)
                return;

            collection.InsertOne(new DefaultDashboardConfiguration
            {
                Guid = Guid.NewGuid(),
                LastChanged = DateTimeOffset.UtcNow,
                ForRole = Roles.User,
                Panels = new List<DashboardConfigurationItem>
                {
                    new DashboardConfigurationItem { Cols = 1, Rows = 2, Y = 0, X = 0, DragEnabled = true, ResizeEnabled = true, WidgetType = Enums.WidgetType.None },
                    new DashboardConfigurationItem { Cols = 1, Rows = 1, Y = 0, X = 1, DragEnabled = true, ResizeEnabled = true, WidgetType = Enums.WidgetType.None },
                    new DashboardConfigurationItem { Cols = 1, Rows = 1, Y = 0, X = 2, DragEnabled = true, ResizeEnabled = true, WidgetType = Enums.WidgetType.None },
                    new DashboardConfigurationItem { Cols = 1, Rows = 1, Y = 1, X = 3, DragEnabled = true, ResizeEnabled = true, WidgetType = Enums.WidgetType.None }
                }
            });
        }

        private static void SeedDefaultDashboardForSilentDon(IMongoDatabase mongoDb)
        {
            var collection = mongoDb.GetCollection<DefaultDashboardConfiguration>(
                typeof(DefaultDashboardConfiguration).ToMongoCollectionName());

            var exists = collection.Count(t => t.ForRole == Roles.SilentDon) != 0;
            if (exists)
                return;

            collection.InsertOne(new DefaultDashboardConfiguration
            {
                Guid = Guid.NewGuid(),
                LastChanged = DateTimeOffset.UtcNow,
                ForRole = Roles.SilentDon,
                Panels = new List<DashboardConfigurationItem>
                {
                    new DashboardConfigurationItem { Cols = 1, Rows = 2, Y = 0, X = 0, DragEnabled = true, ResizeEnabled = true, WidgetType = Enums.WidgetType.MyClubs },
                    new DashboardConfigurationItem { Cols = 1, Rows = 1, Y = 0, X = 1, DragEnabled = true, ResizeEnabled = true, WidgetType = Enums.WidgetType.None },
                    new DashboardConfigurationItem { Cols = 1, Rows = 1, Y = 0, X = 2, DragEnabled = true, ResizeEnabled = true, WidgetType = Enums.WidgetType.None },
                    new DashboardConfigurationItem { Cols = 1, Rows = 1, Y = 1, X = 3, DragEnabled = true, ResizeEnabled = true, WidgetType = Enums.WidgetType.None }
                }
            });
        }
    }
}
