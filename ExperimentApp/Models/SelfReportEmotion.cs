using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExperimentApp.Models
{
    public class SelfReportEmotion
    {
        public int ID { get; set; }
        public int SelfReportQuestionnaireID { get; set; }
        public string Name { get; set; }
        public int Strength { get; set; }
    }
}