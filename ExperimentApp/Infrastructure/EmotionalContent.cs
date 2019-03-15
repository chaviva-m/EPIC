using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExperimentApp.Infrastructure
{
    public enum EmotionalContentEnum
    {
        Happy,
        Sad,
        Neutral
    }

    public enum ExperimentOptionEnum
    {
        A, B, C
    }

    public class EmotionInducingContent
    {
        public static string VideoRootDir = "~/Content/videos/";

        public static Dictionary<EmotionalContentEnum, string> VideoByContent = new Dictionary<EmotionalContentEnum, string>
        {
            {EmotionalContentEnum.Happy, VideoRootDir +"UriChizkiya.mp4" }, //+ "shortTemp.mp4" }, 
            {EmotionalContentEnum.Sad, VideoRootDir + "LionKing.mp4" },
            {EmotionalContentEnum.Neutral, VideoRootDir + "GreatBarrierReefAustralia.mp4" }
        };

    }
}