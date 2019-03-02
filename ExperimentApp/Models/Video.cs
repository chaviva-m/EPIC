using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExperimentApp.Infrastructure;

namespace ExperimentApp.Models
{
    public class Video
    {
        private readonly string dataRelDir = "\\Data\\Video\\";
        private EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.AutoReset);
        private static readonly Object obj = new Object();
        private bool ProcessSuccess = true;
        private readonly string python_window = "python_script";
        private readonly string webcam_window = "camera";

        public void StopRecording()
        {
            //signal to stop process
            ewh.Set();
            //wait for process to finish
            do
            {
                Thread.Sleep(2000);
            } while (WindowIsOpen(webcam_window));
        }

        public bool RecordVideo(Participant participant)
        {
            /*NOTICE: bash file changes directory to emotions.py's root directory. Need to change this.
             Maybe we can put python project directory in this project so that we can give relative path?*/

            string file = "VideoData" + participant.ID;  // give root directory of where we want to store the data
            participant.VideoDataPath = file;
            string video = "Video" + participant.ID;     // give root directory of where we want to store the data
            participant.VideoPath = video + ".avi";

            string dataRootDir = HttpContext.Current.Server.MapPath(Path.Combine("~", dataRelDir));

            string processRelPath = "Scripts\\VideoEmotionDetector\\run_emotions.bat";
            string processPath = HttpContext.Current.Server.MapPath(Path.Combine("~", processRelPath));

            string codeDir = Directories.VideoEmotionDetector;


            ProcessSuccess = true;
            Task t = new Task(() =>
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(processPath)
                    {
                        WindowStyle = ProcessWindowStyle.Minimized,
                        Arguments = String.Format("{0} {1} {2} {3} {4}", codeDir, python_window,
                        dataRootDir + file, dataRootDir + video, webcam_window)
                    };
                    //start process
                    using (Process myProcess = Process.Start(startInfo))
                    {
                        //wait for signal
                        ewh.WaitOne();
                        // Close Emotion by sending a close message to its window.
                        SearchAndClose(python_window);
                    }
                }
                catch (Exception e)
                {
                    lock (obj)
                    {
                        ProcessSuccess = false;
                    }
                    Console.WriteLine("The following exception was raised: ");
                    Console.WriteLine(e.Message);
                }
            });
            t.Start();
            //check if camera window opened
            Thread.Sleep(10000);
            while (!WindowIsOpen(webcam_window))
            {
                if (!WindowIsOpen(python_window))
                {
                    lock (obj)
                    {
                        ProcessSuccess = false;
                    }
                    break;
                }
                Thread.Sleep(3000);
            }
            return ProcessSuccess;            

        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string className, string windowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int WM_CLOSE = 0x10;
        private const int WM_QUIT = 0x12;

        private bool WindowIsOpen(string windowName)
        {
            IntPtr hWnd = FindWindow(null, windowName);
            if (hWnd == IntPtr.Zero)
                return false;
            return true;
        }

        private void SearchAndClose(string windowName)
        {
            IntPtr hWnd = FindWindow(null, windowName);
            if (hWnd == IntPtr.Zero)
                throw new Exception("Couldn't find window!");
            SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        public void GetEmotionsFromFile(Participant participant)
        {
            string fileRelPath = dataRelDir + '\\' + participant.VideoDataPath;
            string filePath = HttpContext.Current.Server.MapPath(Path.Combine("~", fileRelPath));
            List<string> emotionsLabels;
            try
            { 

            using (StreamReader sr = File.OpenText(filePath))
            {
                emotionsLabels = sr.ReadLine().Split('\t').ToList();
            }        
            emotionsLabels = emotionsLabels.GetRange(1, emotionsLabels.Count() - 1); //remove "prediction" (1st word)
            var emotionsVector = File.ReadLines(filePath).Select(line => line.Split('\t')[0]).ToList();
            emotionsVector = emotionsVector.GetRange(1, emotionsVector.Count() - 1); //remove "prediction" (1st line)
            var groups = emotionsVector.ToLookup(i => i);
            int vectorLength = emotionsVector.Count;
            //calculate emotion frequencies in video
            double count;
            double freq;
            foreach (string emotion in emotionsLabels)
            {
                if (groups[emotion].Any()) { count = groups[emotion].Count(); }
                else { count = 0; }
                freq = count / vectorLength;
                //add emotion frequency to participant
                participant.VideoEmotions.Add(new VideoEmotion
                {
                    ParticipantID = participant.ID,
                    Name = emotion,
                    Strength = freq
                });
            }
            } catch (Exception e)
            {
                return;
            }

        }
    }
}
