using ExperimentApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExperimentApp.Models
{
    public class SelfReportQuestionnaire
    {
        public int ID;
        public int ParticipantID;

        private static SelfReportEmotions SREs = new SelfReportEmotions();
        public virtual ICollection<SelfReportEmotion> Emotions
        {
            get
            {
                ICollection<SelfReportEmotion> emotions = new List<SelfReportEmotion>();
                foreach (string e in SREs.Emotions)
                {
                    SelfReportEmotion em = new SelfReportEmotion
                    {
                        Emotion = e
                    };
                    emotions.Add(em);
                }
                return emotions;
            }
        }
    }
}