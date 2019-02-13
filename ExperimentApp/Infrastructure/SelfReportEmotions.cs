using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExperimentApp.Infrastructure
{
    public class SelfReportEmotions
    {
        public List<string> Emotions { get; set; } = new List<string>
        {
            "Interested מעונין", "Sad עצוב", "Arousal התרגשות",
            "Disgust גועל", "Surprised מופתע", "Contentment סיפוק",
            "Fear פחד", "Amusement בידור", "Confusion בלבול",
            "Embarrassment מבוכה", "Relief הקלה", "Contempt זלזול",
            "Pain כאב", "Tension מתיחות", "Happinness שמחה",
            "Anger כעס"
        };
    }
}