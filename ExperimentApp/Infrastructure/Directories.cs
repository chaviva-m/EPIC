using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExperimentApp.Infrastructure
{
    public class Directories
    {
        public static string VideoEmotionDetector { get; set; } = @"C:\Users\AVIADFUX\Desktop\EPIC\Emotion-master\Emotion-master\";
        public static string AudioEmotionDetector { get; set; } = @"C:\Users\AVIADFUX\Desktop\EPIC\Vokaturi\OpenVokaturi-3-0a\Our_Code\";

        public static string AudioFile { get; set; } = @"C:\Users\AVIADFUX\Downloads\";
    }
}