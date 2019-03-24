using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExperimentApp.DAL;
using ExperimentApp.Infrastructure;
using ExperimentApp.Models;

namespace ExperimentApp.Controllers
{
    public class ParticipantController : Controller
    {
        private ExperimentContext db = new ExperimentContext();

        // GET: Participant
        public ActionResult Index()
        {
            return View(db.Participants.ToList());
        }

        // GET: Participant/Details/5
        public ActionResult Details(int? id)
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

        // GET: Participant/Create
        public ActionResult Create()
        {
            Participant par = new Participant();
            return View(par);
        }

        // POST: Participant/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ExpOption,ID")] Participant participant)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Participants.Add(participant);
                    db.SaveChanges();
                    return RedirectToAction("BaselineAudio", "Home", new { id = participant.ID });
                }
            } catch(DataException e)
            {
                if (db.Participants.Find(participant.ID) != null)
                {
                    TempData["ErrorMessage"] = "מספר הזיהוי שהזנת כבר קיים במערכת. אנא חזור ונסה מספר זיהוי אחר.";
                } else
                {
                    ModelState.AddModelError("", "Unable to save changes." + e.Message);
                    TempData["ErrorMessage"] = "שגיאה באתחול הנבדק " + e.Message;
                }
                return RedirectToAction("Error", "Home");

            }
            return View(participant);
        }

        // GET: Participant/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Participant/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EmotionalContent,VideoPath,VideoEmotionsDataPath,AudioDataPath,UltimatumReceivedSum,UltimatumGaveSum,UltimatumGavePercent,TrustReceivedSum,TrustGaveSum,TrustGavePercent")] Participant participant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(participant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(participant);
        }

        // GET: Participant/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Participant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Participant participant = db.Participants.Find(id);
            db.Participants.Remove(participant);
            db.SaveChanges();
            return RedirectToAction("Index");
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
