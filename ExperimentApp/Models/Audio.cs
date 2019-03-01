using ExperimentApp.Infrastructure;
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

namespace ExperimentApp.Models
{
    public class Audio
    {
        private readonly string dataRelDir = "\\Data\\Audio\\";

        public bool RunVokaturi(Participant participant)
        {
            string input_file = participant.AudioPath;
            string output_file = "AudioData" + participant.ID;
            participant.AudioDataPath = output_file;

            string inputFullPath = Directories.AudioFile + '\\' + input_file;
            string rel_output_file = dataRelDir + output_file;
            string outputFullPath = HttpContext.Current.Server.MapPath(Path.Combine("~", rel_output_file));


            string processRelPath = "Scripts\\AudioEmotionDetector\\run_vokaturi.bat";
            string processPath = HttpContext.Current.Server.MapPath(Path.Combine("~", processRelPath));

            string codeDir = Directories.AudioEmotionDetector;

            string window_title = "Python_Audio";

            Task<bool> t = new Task<bool>(() =>
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(processPath)
                    {
                        WindowStyle = ProcessWindowStyle.Minimized,
                        Arguments = String.Format("{0} {1} {2} {3}", codeDir, window_title, inputFullPath, outputFullPath),
                        UseShellExecute = true
                        //UseShellExecute = false
                    };
                    //start process
                    using (Process myProcess = Process.Start(startInfo)) { }
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
            //wait for process to finish
            do
            {
                Thread.Sleep(2000);
            } while (WindowIsOpen(window_title));

            return t.Result;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string className, string windowName);

        private bool WindowIsOpen(string windowName)
        {
            IntPtr hWnd = FindWindow(null, windowName);
            if (hWnd == IntPtr.Zero)
                return false;
            return true;
        }

        public void GetEmotionsFromFile(Participant participant)
        {
            string fileRelPath = dataRelDir + '\\' + participant.AudioDataPath;
            string filePath = HttpContext.Current.Server.MapPath(Path.Combine("~", fileRelPath));
            IEnumerable<string> lines;
            try
            {
                lines = File.ReadLines(filePath);
            } catch {
                return;
            }
            string[] data;
            foreach (var line in lines)
            {
                data = line.Split('\t');
                participant.AudioEmotions.Add(new AudioEmotion
                {
                    ParticipantID = participant.ID,
                    Name = data[0],
                    Strength = Convert.ToDouble(data[1])
                });    
            }
        }
    }
}