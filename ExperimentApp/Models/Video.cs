using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExperimentApp.Infrastructure;

namespace ExperimentApp.Models
{
    public class Video
    {
        EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.AutoReset);

        public void StopRecording()
        {
            //signal to stop process
            ewh.Set();
            //wait for recording to stop
            Thread.Sleep(3000);    //change this to detect when webcam turns off
        }

        public bool RecordVideo(Participant participant)
        {
            /*NOTICE: bash file changes directory to emotions.py's root directory. Need to change this.
             Maybe we can put python project directory in this project so that we can give relative path?*/

            //file and video paths - not actually sure if this updates it in the database
            string file = "VideoData" + participant.ID;  // give root directory of where we want to store the data
            participant.VideoPath = file;
            string video = "Video" + participant.ID;     // give root directory of where we want to store the data
            participant.VideoEmotionsDataPath = video;
            string processRelPath = "Scripts\\VideoEmotionDetector\\run_emotions.bat";
            string processPath = HttpContext.Current.Server.MapPath(Path.Combine("~", processRelPath));

            Task<bool> t = new Task<bool>(() =>
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(processPath);
                    startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    startInfo.Arguments = String.Format("\"{0}\" \"{1}\"", file, video);
                    //start process
                    using (Process myProcess = Process.Start(startInfo))
                    {
                        //wait for signnal
                        ewh.WaitOne();
                        // Close process by sending a close message to its window.
                        SearchAndClose("Python Script");
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("The following exception was raised: ");
                    Console.WriteLine(e.Message);
                    return false;
                }
            });
            t.Start();

            //wait for recording to start
            Thread.Sleep(10000);    //change this to detect when webcam turns on (or put the webcam detection in View to automatically hit play)

            return true;            //return false if there is an exception in task / return t.Result;

        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string className, string windowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int WM_CLOSE = 0x10;
        private const int WM_QUIT = 0x12;

        private void SearchAndClose(string windowName)
        {
            IntPtr hWnd = FindWindow(null, windowName);
            if (hWnd == IntPtr.Zero)
                throw new Exception("Couldn't find window!");
            SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

    }
}