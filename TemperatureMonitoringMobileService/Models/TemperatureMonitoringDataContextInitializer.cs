using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TemperatureMonitoringMobileService.Models
{
    public class TemperatureMonitoringDataContextInitializer : ClearDatabaseSchemaIfModelChanges<TemperatureMonitoringDataContext>
    {
        public override void InitializeDatabase(TemperatureMonitoringDataContext context)
        {
            base.InitializeDatabase(context);
        }

        protected override void Seed(TemperatureMonitoringDataContext context)
        {
            base.Seed(context);
        }

        protected override void DeleteAllResourcesFromSchema(TemperatureMonitoringDataContext context)
        {
            base.DeleteAllResourcesFromSchema(context);
        }
    }
}