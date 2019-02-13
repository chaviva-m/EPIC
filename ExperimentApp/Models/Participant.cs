﻿using System;
using System.Collections.Generic;
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
        public virtual ICollection<SelfReportEmotion> SelfReportEmotions { get; set; }

        //Ultimatum Game
        public int UltimatumReceivedSum { get; set; }
        private int ultimatumGaveSum;
        public int UltimatumGaveSum { get { return ultimatumGaveSum; } set { ultimatumGaveSum = value; UltimatumGavePercent = (ultimatumGaveSum / (float)UltimatumReceivedSum) * 100; } }
        public float UltimatumGavePercent { get; private set; }

        //Trust Game
        public int TrustReceivedSum { get; set; }
        private int trustGaveSum;
        public int TrustGaveSum { get { return trustGaveSum; }  set { trustGaveSum = value; TrustGavePercent = (trustGaveSum / (float)TrustReceivedSum) * 100; } }
        public float TrustGavePercent { get; private set; }
    }
}