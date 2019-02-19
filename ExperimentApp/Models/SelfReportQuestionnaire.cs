using ExperimentApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExperimentApp.Models
{
    public class SelfReportQuestionnaire
    {
        public int ID { get; set; }
        public int ParticipantID { get; set; }

        public List<int> Options { get; } = new List<int> { 1, 2, 3, 4, 5 };

        public virtual List<SelfReportEmotion> Emotions { get; set; }
    }
}