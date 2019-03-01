using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using ExperimentApp.DAL;
using ExperimentApp.Infrastructure;
using ExperimentApp.Models;

namespace ExperimentApp.Controllers
{
    public class HomeController : Controller
    {
        private ExperimentContext db = new ExperimentContext();
        private static readonly Video videoModel = new Video();
        private static readonly Audio audioModel = new Audio();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Start(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }

        [HttpGet]
        public ActionResult Video(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            bool result = videoModel.RecordVideo(participant);
            if (result == false)
            {
                TempData["ErrorMessage"] = "שגיאה בהקלטת הוידאו";
                return RedirectToAction("Error");
            }
            db.Entry(participant).State = EntityState.Modified;
            db.SaveChanges();
            return View(participant);
        }



        public ActionResult GetVideo(EmotionalContentEnum em)
        {
            string relativeVideoPath = EmotionInducingContent.VideoByContent[em];
            var videoPath =
               Request.MapPath(relativeVideoPath);
            FileStream fs =
               new FileStream(videoPath, FileMode.Open);
            return new FileStreamResult(fs, "video/mp4");
        }

        public ActionResult EndVideoRecording(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            videoModel.StopRecording();
            //save emotions in database
            videoModel.GetEmotionsFromFile(participant);
            db.Entry(participant).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Audio", new { id = participant.ID });
        }

        public ActionResult Audio(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            //add audio file name
            participant.AudioPath = "Audio" + participant.ID + ".wav";
            db.Entry(participant).State = EntityState.Modified;
            db.SaveChanges();
            return View(participant);
        }

        public ActionResult FinishedAudioRecording(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            bool result = audioModel.RunVokaturi(participant);
            if (result) {
                //save emotions in database
                audioModel.GetEmotionsFromFile(participant);
            }
            db.Entry(participant).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("SelfReport", new { id = participant.ID });
        }


        [HttpGet]
        public ActionResult GameA(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }

        [HttpPost]
        public ActionResult GameA(Participant participant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(participant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GameB", new { id = participant.ID });
            }

            return View(participant);
        }

        [HttpGet]
        public ActionResult GameB(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }

        [HttpPost]
        public ActionResult GameB(Participant participant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(participant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Finish");
            }

            return View(participant);
        }

        public ActionResult SelfReport(int? id)
        {
            //find participant in database
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }

            //add self report questionnaire
            participant.SelfReportQuestionnaire = new SelfReportQuestionnaire
            {
                ParticipantID = participant.ID
            };
            List<SelfReportEmotion> emotions = new List<SelfReportEmotion>();
            foreach (string emotion in Emotions.SelfReportEmotions)
            {
                emotions.Add(new SelfReportEmotion { ParticipantID = participant.ID, Name = emotion });
            }
            participant.SelfReportQuestionnaire.Emotions = emotions;

            //save changes in database
            db.Entry(participant).State = EntityState.Modified;
            db.SaveChanges();

            return View(participant.SelfReportQuestionnaire);
        }

        // POST
        [HttpPost, ActionName("SelfReport")]
        [ValidateAntiForgeryToken]
        public ActionResult SelfReportSubmitted(SelfReportQuestionnaire selfReportQuestionnaire)
        {
            if (ModelState.IsValid)
            {
                //save changes in database
                foreach (SelfReportEmotion emotion in selfReportQuestionnaire.Emotions)
                {
                    db.Entry(emotion).State = EntityState.Modified;
                }
                db.Entry(selfReportQuestionnaire).State = EntityState.Modified;
                db.SaveChanges();
            }
            int participantID = selfReportQuestionnaire.ParticipantID;
            return RedirectToAction("GameA", new { id = participantID });
        }

        public ActionResult Finish()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}