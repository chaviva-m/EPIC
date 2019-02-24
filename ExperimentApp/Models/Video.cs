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
        private EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.AutoReset);
        private static readonly Object obj = new Object();
        private bool ProcessSuccess = true;

        public void StopRecording()
        {
            //signal to stop process
            ewh.Set();
            //wait for participant to internalize
            Thread.Sleep(1000);
        }

        public bool RecordVideo(Participant participant)
        {
            /*NOTICE: bash file changes directory to emotions.py's root directory. Need to change this.
             Maybe we can put python project directory in this project so that we can give relative path?*/

            string file = "VideoData" + participant.ID;  // give root directory of where we want to store the data
            participant.VideoPath = file;
            string video = "Video" + participant.ID;     // give root directory of where we want to store the data
            participant.VideoEmotionsDataPath = video;
            string processRelPath = "Scripts\\VideoEmotionDetector\\run_emotions.bat";
            string processPath = HttpContext.Current.Server.MapPath(Path.Combine("~", processRelPath));

            string python_window = "Python Script";
            string webcam_window = "camera";

            ProcessSuccess = true;
            Task t = new Task(() =>
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(processPath);
                    startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    startInfo.Arguments = String.Format("\"{0}\" \"{1}\"", file, video);
                    //start process
                    using (Process myProcess = Process.Start(startInfo))
                    {
                        //wait for signal
                        ewh.WaitOne();
                        // Close process by sending a close message to its window.
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

    }
}