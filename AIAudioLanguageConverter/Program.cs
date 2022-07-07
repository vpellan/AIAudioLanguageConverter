using McMaster.Extensions.CommandLineUtils;
using System.Reflection;
using Newtonsoft.Json;

namespace AIAudioLanguageConverter
{
    [Command("AIAudioManguageConverter")]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    [Subcommand(typeof(ChangeCommand), typeof(RestoreCommand))]
    class Program
    {
        public static readonly SoundbankInput ID_tables =
            JsonConvert.DeserializeObject<SoundbankInput>(File.ReadAllText(Program.AbsolutePath("export.json")));

        private static int Main(string[] args)
        {
            try
            {
                return CommandLineApplication.Execute<Program>(args);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"The file {e.FileName} cannot be found. The program will now exit.");
                return 2;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"You do not have administrator privileges.");
                return -3;
            }
            catch (Exception e)
            {
                Console.WriteLine($"FATAL ERROR: {e.Message}\n{e.StackTrace}");
                return -1;
            }
        }
        
        protected int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return 1;
        }
        private static string GetVersion()
            => typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
        
        public static string AbsolutePath(string file)
        {
            var execPath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(execPath, file);
        }
    }
}