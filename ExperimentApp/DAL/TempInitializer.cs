using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExperimentApp.Models;
using ExperimentApp.Infrastructure;


namespace ExperimentApp.DAL
{
    public class TempInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ExperimentContext>
    {
        protected override void Seed(ExperimentContext context)
        {
            var participants = new List<Participant>
            {
                new Participant{EmotionalContent=EmotionalContentEnum.Happy, VideoPath="parVideo.mp4",
                VideoEmotionsDataPath="parVideoData", VideoEmotionFrequencies=new Dictionary<string, double>{ { "happy", 0.52 } },
                AudioDataPath="parAudio.wav", AudioEmotionFrequencies=new Dictionary<string, double>{{"happy",0.4}},
                UltimatumGaveSum=2,
                TrustGaveSum=10 }
            };

            participants.ForEach(s => context.Participants.Add(s));
            context.SaveChanges();

        }
    }
}