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
        private static readonly SelfReportEmotions SREs = new SelfReportEmotions();

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

        public ActionResult UltimatumGame()
        {
            return View();
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
            //add self report emotions
            var emotions = new List<SelfReportEmotion>();
            foreach (string e in SREs.Emotions)
            {
                SelfReportEmotion em = new SelfReportEmotion
                {
                    ParticipantID = participant.ID,
                    Emotion = e
                };
                emotions.Add(em);
            }
            emotions.ForEach(e => db.SelfReportEmotions.Add(e));
            db.SaveChanges();

            return View(participant);
        }

        // POST: Participant/Delete/5
        [HttpPost, ActionName("SelfReport")]
        [ValidateAntiForgeryToken]
        public ActionResult SelfReportSubmitted(int id)
        {
            Participant participant = db.Participants.Find(id);
            db.Entry(participant).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Finish");
        }

        public ActionResult Finish()
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