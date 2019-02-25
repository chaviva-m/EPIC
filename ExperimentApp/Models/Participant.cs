using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExperimentApp.Infrastructure;

namespace ExperimentApp.Models
{
    public class Participant
    {
        public int ID { get; set; }

        //induced emotion
        private EmotionalContentEnum emotionalContent;
        public EmotionalContentEnum EmotionalContent { get { return emotionalContent; } set { emotionalContent = value; ExpOption = Convert.ToInt32(emotionalContent); } }
        //experiment option
        private int expOption ;
        [Display(Name = "Experiment Option")]
        public int ExpOption { get { return expOption; } set { expOption = value; emotionalContent = (EmotionalContentEnum)expOption; } }
        public SelectList ExpOptionList

        {
            get
            {
                List<int> Options = new List<int>();
                foreach (int content in Enum.GetValues(typeof(EmotionalContentEnum))) { Options.Add(content); }
                SelectList expOptions = new SelectList(Options);
                return expOptions;
            }
        }

        //emotions from video
        public string VideoPath { get; set; }
        public string VideoEmotionsDataPath { get; set; }
        //CHANGE this to a list of VideoEmotion / a class that holds that list
        public Dictionary<string, double> VideoEmotionFrequencies { get; set; }

        //emotions from audio
        public string AudioDataPath { get; set; }
        //CHANGE this to a list of AudioEmotion / a class that holds that list
        public Dictionary<string, double> AudioEmotionFrequencies { get; set; }

        //emotions - self report
        public SelfReportQuestionnaire SelfReportQuestionnaire { get; set; }

        //Ultimatum Game
        public int UltimatumReceivedSum { get; } = 20;

        private int ultimatumGaveSum;
        [Required(ErrorMessage = "Please enter your proposal.")]
        [Range(0, 20, ErrorMessage = "Please enter a number between 0 and 20.")]
        [Display(Name = "proposal")]
        public int UltimatumGaveSum { get { return ultimatumGaveSum; } set { ultimatumGaveSum = value; UltimatumGavePercent = (ultimatumGaveSum / (float)UltimatumReceivedSum) * 100; } }
        public float UltimatumGavePercent { get; private set; }

        //Trust Game

        public int TrustReceivedSum { get; } = 20;
        private int trustGaveSum;
        [Required(ErrorMessage = "שדה חובה")]
        [Range(0, 20, ErrorMessage = "יש להכניס מספר שלם בין 0 ל 20")]
        [Display(Name = "הצעה")]
        public int TrustGaveSum { get { return trustGaveSum; }  set { trustGaveSum = value; TrustGavePercent = (trustGaveSum / (float)TrustReceivedSum) * 100; } }
        public float TrustGavePercent { get; private set; }

    }
}