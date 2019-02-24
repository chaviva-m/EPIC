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
        private static readonly Emotions SREs = new Emotions();

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
            bool finished = videoModel.RecordVideo(participant);
            if (finished == false)
            {
                TempData["ErrorMessage"] = "שגיאה בהקלטת הוידאו";
                return RedirectToAction("Error");
            }
            db.SaveChanges();
            return View(participant);
        }

        //public ActionResult FormJquery(int id)
        //{
        //    bool result = true;

        //    return Json(new { returnvalue = result });
        //}

        public ActionResult GetVideo(EmotionalContentEnum em)
        {
            string relativeVideoPath = EmotionInducingContent.VideoByContent[em];
            var videoPath =
               Request.MapPath(relativeVideoPath);
            FileStream fs =
               new FileStream(videoPath, FileMode.Open);
            return new FileStreamResult(fs, "video/mp4");
        }

        public ActionResult EndVideoRecording()
        {
            videoModel.StopRecording();
            return RedirectToAction("Audio");   //change to different page
        }

        public ActionResult Audio(Participant participant)
        {
            return View(participant);
        }

        public ActionResult StartAudioRecording()
        {
            return View("Audio");
        }

        public ActionResult StopAudioRecording()
        {

            return View("Audio");
        }

        [HttpGet]
        public ActionResult UltimatumGame(int? id)
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
        public ActionResult UltimatumGame(Participant participant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(participant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(participant);
        }

        [HttpGet]
        public ActionResult TrustGame(int? id)
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
        public ActionResult TrustGame(Participant participant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(participant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
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
            participant.SelfReportQuestionnaire = new SelfReportQuestionnaire();
            participant.SelfReportQuestionnaire.ParticipantID = participant.ID;
            List<SelfReportEmotion> emotions = new List<SelfReportEmotion>();
            foreach (string emotion in Emotions.SelfReportEmotions)
            {
                emotions.Add(new SelfReportEmotion { SelfReportQuestionnaireID = participant.SelfReportQuestionnaire.ID, Name = emotion });
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
            return RedirectToAction("Finish");
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