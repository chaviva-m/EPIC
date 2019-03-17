using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExperimentApp.Infrastructure
{
    public class Emotions
    {
        public static List<string> SelfReportEmotions { get; set; } = new List<string>
        {
            "Apathy אדישות", "Sadness עצב", "Calm רוגע",
            "Amusement בידור","Pain כאב", "Happiness שמחה",
        };
    }
}