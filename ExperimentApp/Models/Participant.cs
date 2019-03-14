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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        //induced emotion
        private EmotionalContentEnum emotionalContent;
        public EmotionalContentEnum EmotionalContent { get { return emotionalContent; } set { emotionalContent = value; ExpOption = (ExperimentOptionEnum)Convert.ToInt32(emotionalContent); } }
        //experiment option
        private ExperimentOptionEnum expOption;
        [Display(Name = "Experiment Option")]
        public ExperimentOptionEnum ExpOption { get { return expOption; } set { expOption = value; emotionalContent = (EmotionalContentEnum)Convert.ToInt32(expOption); } }

        //emotions from video
        public string VideoPath { get; set; }
        public string VideoWithLabelsPath { get; set; }
        public string VideoDataPath { get; set; }
        public virtual List<VideoEmotion> VideoEmotions { get; set; }

        //emotions from audio
        public string AudioPath { get; set; }
        public string AudioDataPath { get; set; }
        public virtual List<AudioEmotion> AudioEmotions { get; set; } = new List<AudioEmotion>();

        //emotions - self report
        public SelfReportQuestionnaire SelfReportQuestionnaire { get; set; }

        //Ultimatum Game
        public int UltimatumReceivedSum { get; } = 10;
        private int ultimatumGaveSum;
        [Required(ErrorMessage = "שדה חובה")]
        [Range(0, 10, ErrorMessage = "יש להכניס מספר שלם בין 0 ל 10")]
        [Display(Name = "הצעה")]
        public int UltimatumGaveSum { get { return ultimatumGaveSum; } set { ultimatumGaveSum = value; UltimatumGavePercent = (ultimatumGaveSum / (float)UltimatumReceivedSum) * 100;} }
        public float UltimatumGavePercent { get; private set; }

        //Trust Game
        public int TrustReceivedSum { get; } = 10;
        private int trustGaveSum;
        [Required(ErrorMessage = "שדה חובה")]
        [Range(0, 10, ErrorMessage = "יש להכניס מספר שלם בין 0 ל 10")]
        [Display(Name = "הצעה")]
        public int TrustGaveSum { get { return trustGaveSum; }  set { trustGaveSum = value; TrustGavePercent = (trustGaveSum / (float)TrustReceivedSum) * 100; this.getRewardVal(trustGaveSum); } }
        public float TrustGavePercent { get; private set; }
        public string trustGotSum;
        public string TrustGotSum { get { return trustGotSum; } set { trustGotSum = value; } }

        private string totalAward;
        public string TotalAward { get { return totalAward; } set { totalAward = value; } }

        public void getRewardVal(int trustGave)
        {
            trustGave = trustGave * 3;
            int has = this.UltimatumReceivedSum - this.ultimatumGaveSum;
            Random random = new Random();
            int n = random.Next(15 - has, 25 - has);
            if (n > trustGave)
            {
                this.TotalAward = (has + trustGave).ToString();
                this.TrustGotSum = trustGave.ToString();
                return;
            }
            this.TrustGotSum = n.ToString();
            this.TotalAward = (has + n).ToString();
            return;
        }

    }
}