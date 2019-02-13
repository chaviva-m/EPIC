using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExperimentApp.Models
{
    public class SelfReportEmotion
    {
        public int ID { get; set; }
        public int ParticipantID { get; set; }

        public string Emotion { get; set; }
        public int EmotionStrength { get; set; }
        public List<int> Options { get { List<int> l = new List<int> { 1, 2, 3, 4, 5 }; return l; } }
    }
}