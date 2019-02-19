using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExperimentApp.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ExperimentApp.DAL
{
    public class ExperimentContext : DbContext
    {
        public ExperimentContext() : base("ExperimentContext") { }  //see Tutorial Connection Strings and Models

        public DbSet<Participant> Participants { get; set; }
        public DbSet<SelfReportEmotion> SelfReportEmotions { get; set; }
        public DbSet<SelfReportQuestionnaire> SelfReportQuestionnaires { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}