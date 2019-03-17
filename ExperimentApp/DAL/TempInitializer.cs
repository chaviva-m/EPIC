using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExperimentApp.Models;
using ExperimentApp.Infrastructure;


namespace ExperimentApp.DAL
{
    public class TempInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ExperimentContext>
    {
        protected override void Seed(ExperimentContext context)
        {}
    }
}