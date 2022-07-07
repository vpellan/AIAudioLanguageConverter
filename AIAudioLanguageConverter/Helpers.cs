using System.Diagnostics;

namespace AIAudioLanguageConverter
{
    public static class Helpers
    {
        public static void ExecuteProcess(string fileName, string arguments, bool redirect=false)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = redirect,
                UseShellExecute = false,
            };

            var process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();
            process.WaitForExit();
        }

        public static string GetLanguageString(int id)
        {
            return id switch
            {
                0 => "English(US)",
                1 => "French(France)",
                2 => "Italian",
                3 => "German",
                4 => "Spanish(Spain)",
                5 => "Portuguese(Brazil)",
                6 => "Russian",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}