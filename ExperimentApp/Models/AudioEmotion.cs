using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExperimentApp.Models
{
    public class AudioEmotion
    {
        public int ID { get; set; }
        public int ParticipantID { get; set; }
        public string Name { get; set; }
        public double Strength { get; set; }
    }
}