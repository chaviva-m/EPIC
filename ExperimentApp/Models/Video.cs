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
                        // Close process by sending a close message to its main window.
                        myProcess.CloseMainWindow();
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

            //BringWebAppToFront(); //doesn't work - not sure if it's even supposed to do what I think it's supposed to do...
            //ForceForegroundWindow();    //doesn't work - same

            //wait for recording to start
            Thread.Sleep(10000);    //change this to detect when webcam turns on (or put the webcam detection in View to automatically hit play)

            return true;            //return false if there is an exception in task / return t.Result;

        }


        //Doesn't work

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr FW, IntPtr intPtr);
        [DllImport("Kernel32.dll")]
        private static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")]
        private static extern void AttachThreadInput(uint thread1, uint thread2, Boolean bl);
        [DllImport("user32.dll")]
        private static extern void BringWindowToTop(IntPtr intPtr);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        private static void ForceForegroundWindow()
        {
            Process process = Process.GetCurrentProcess();
            IntPtr hWnd = process.MainWindowHandle;

            uint foreThread = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
            uint appThread = GetCurrentThreadId();
            const uint SW_SHOW = 5;

            if (foreThread != appThread)
            {
                AttachThreadInput(foreThread, appThread, true);
                BringWindowToTop(hWnd);
                ShowWindow(hWnd, SW_SHOW);
                AttachThreadInput(foreThread, appThread, false);
            }
            else
            {
                BringWindowToTop(hWnd);
                ShowWindow(hWnd, SW_SHOW);
            }
        }

        //Doesn't work

        private const int ALT = 0xA4;
        private const int EXTENDEDKEY = 0x1;
        private const int KEYUP = 0x2;
        private const int SHOW_MAXIMIZED = 3;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private void BringWebAppToFront()
        {
            Process process = Process.GetCurrentProcess();
            IntPtr handle = process.MainWindowHandle;

            // Show window maximized.
            ShowWindow(handle, SHOW_MAXIMIZED);

            // Simulate an "ALT" key press.
            keybd_event((byte)ALT, 0x45, EXTENDEDKEY | 0, 0);

            // Simulate an "ALT" key release.
            keybd_event((byte)ALT, 0x45, EXTENDEDKEY | KEYUP, 0);

            SetForegroundWindow(handle);
        }
    }
}
/*
    cd C:\Users\leah\Desktop\python_projects\Emotion\ 
start /MIN python emotions.py "%1" -o "%2"
ProcessStartInfo startInfo = new ProcessStartInfo(processPath);
startInfo.WindowStyle = ProcessWindowStyle.Minimized;
startInfo.Arguments = String.Format("\"{0}\" \"{1}\"", file, video);
 */
