using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using AIAudioLanguageConverter.NPCKEditor;
using AIAudioLanguageConverter.NPCKEditor.Common;
using AIAudioLanguageConverter.NPCKEditor.ViewModels;
using McMaster.Extensions.CommandLineUtils;

namespace AIAudioLanguageConverter
{
    [Command(Description = "Create backup and change audio language")]
    public class ChangeCommand
    {
        [Required]
        [FileOrDirectoryExists]
        [Argument(0, Description = "File or folder containing pck files")]
        public string InputPath { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, Description = "Specify system/game language", ShortName = "s",
            LongName = "sys-lang")]
        public int SysLang { get; set; } = 1;

        [Required]
        [Option(CommandOptionType.SingleValue, Description = "Specify desired language", ShortName = "d",
            LongName = "dest-lang")]
        public int DestLang { get; set; } = 0;
        
        [Option(CommandOptionType.NoValue, Description = "Do not make a backup of files. Warning: You will not be able to change the language anymore", ShortName = "", LongName = "no-backup")]
        public bool NoBackup { get; set; }
        
        [Option(CommandOptionType.NoValue, Description = "Automatically accept backup overwrite", ShortName = "y", LongName = "yes")]
        public bool AutoYes { get; set; }
        
        [Option(CommandOptionType.NoValue, Description = "Automatically refuse backup overwrite", ShortName = "n", LongName = "no")]
        public bool AutoNo { get; set; }

        public int OnExecute(CommandLineApplication app)
        {
            var attr = File.GetAttributes(InputPath);
            if (!attr.HasFlag(FileAttributes.Directory))
                throw new DirectoryNotFoundException("Input is not a directory");
            if (SysLang == DestLang || SysLang is < 0 or > 6 || DestLang is < 0 or > 6)
                throw new ArgumentException("SysLang and DestLang must be between 0 and 6, and must be different");
            var backupPath = Path.Combine(InputPath, "backup_audio");
            var sysLangFile = Helpers.GetLanguageString(SysLang) + "_Dialogue.pck";
            var destLangFile = Helpers.GetLanguageString(DestLang) + "_Dialogue.pck";
            if (!NoBackup && !Directory.Exists(backupPath))
                Directory.CreateDirectory(backupPath);
            if (!NoBackup)
            {
                if (!File.Exists(Path.Combine(backupPath, sysLangFile)))
                {
                    File.Copy(Path.Combine(InputPath, sysLangFile), Path.Combine(backupPath, Path.GetFileName(sysLangFile)));
                }
                else if (AutoYes)
                {
                    File.Copy(Path.Combine(InputPath, sysLangFile),
                        Path.Combine(backupPath, Path.GetFileName(sysLangFile)), true);
                }
                else if (!AutoNo)
                {
                    var inputLoop = true;
                    while (inputLoop)
                    {
                        Console.WriteLine("Backup already exists, do you want to replace it ? [y/n]");
                        var lineRead = Console.ReadLine();
                        switch (lineRead!)
                        {
                            case "y":
                                File.Copy(Path.Combine(InputPath, sysLangFile),
                                    Path.Combine(backupPath, Path.GetFileName(sysLangFile)), true);
                                inputLoop = false;
                                break;
                            case "n":
                                inputLoop = false;
                                break;
                        }
                    }
                }
            }
            
            Console.WriteLine("Importing PCKs...");
            // Import PCK sys
            var sysreadFile = HelperFunctions.OpenFile(Path.Combine(InputPath, sysLangFile));
            var sysViewModel = new NPCKViewModel();
            var syscurrentFileName = Path.Combine(InputPath, sysLangFile).Split("\\").Last().Split(".")[0];
            sysViewModel.SetNPCK(new NPCKHeader(sysreadFile,SupportedGames.MHWorld,syscurrentFileName));
            sysreadFile.Close();

            var checkBackup = false;
            if (Directory.Exists(backupPath) &&
                File.Exists(Path.Combine(backupPath, Helpers.GetLanguageString(DestLang) + "_Dialogue.pck")))
                checkBackup = NoBackup;
            
            // Import PCK dest
            var destreadFile = !checkBackup
                ? HelperFunctions.OpenFile(Path.Combine(InputPath, destLangFile))
                : HelperFunctions.OpenFile(Path.Combine(backupPath, destLangFile));
            var destViewModel = new NPCKViewModel();
            var destcurrentFileName = Path.Combine(InputPath, destLangFile).Split("\\").Last().Split(".")[0];
            destViewModel.SetNPCK(new NPCKHeader(destreadFile,SupportedGames.MHWorld, destcurrentFileName));
            destreadFile.Close();

            Console.WriteLine("Changing WEMs...");
            var i = 0;
            var export =  Encoding.Default.GetString(File.ReadAllBytes("./export.json"));
            foreach (var id in sysViewModel.GetWemIds())
            {
                var file = Program.ID_tables.GetFile(id, SysLang);
                uint destId;
                try
                {
                    destId = GetIDFromFile(file, DestLang);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine(e.Message + " : " + e.ActualValue);
                    Console.WriteLine("Syslang=" + SysLang + "; Destlang=" + DestLang + "; id=" + id + ";");
                    continue;
                }
                
                try
                {
                    var destWem = destViewModel.npck.GetWemFromId(destId);
                    sysViewModel.ReplaceWemFromId(destWem, id);
                }
                catch (KeyNotFoundException)
                {
                    throw new KeyNotFoundException(i + ", " + id + ", dest: " + destId);
                }
                i++;
            }
            File.WriteAllText("./export.json", export);
            
            Console.WriteLine("Saving PCK...");
            sysViewModel.ExportNPCK(Path.Combine(InputPath, sysLangFile), SupportedGames.MHWorld);

            return 0;
        }

        private static uint GetIDFromFile(SoundbankLanguages file, int lang)
        {
            return lang switch
            {
                0 => file.GetTreatedFiles() >= file.english.Count
                    ? file.english[^1]
                    : file.english[file.GetTreatedFilesThenIncrement()],
                1 => file.GetTreatedFiles() >= file.french.Count
                    ? file.french[^1]
                    : file.french[file.GetTreatedFilesThenIncrement()],
                2 => file.GetTreatedFiles() >= file.italian.Count
                    ? file.italian[^1]
                    : file.italian[file.GetTreatedFilesThenIncrement()],
                3 => file.GetTreatedFiles() >= file.german.Count
                    ? file.german[^1]
                    : file.german[file.GetTreatedFilesThenIncrement()],
                4 => file.GetTreatedFiles() >= file.spanish.Count
                    ? file.spanish[^1]
                    : file.spanish[file.GetTreatedFilesThenIncrement()],
                5 => file.GetTreatedFiles() >= file.portuguese.Count
                    ? file.portuguese[^1]
                    : file.portuguese[file.GetTreatedFilesThenIncrement()],
                6 => file.GetTreatedFiles() >= file.russian.Count
                    ? file.russian[^1]
                    : file.russian[file.GetTreatedFilesThenIncrement()],
                _ => throw new KeyNotFoundException()
            };
        }
    }

    [Command(Description = "Restore files from backup folder")]
    public class RestoreCommand
    {
        [Required]
        [FileOrDirectoryExists]
        [Argument(0, Description = "File or folder containing pck files")]
        public string InputPath { get; set; }

        public int OnExecute(CommandLineApplication app)
        {
            var attr = File.GetAttributes(InputPath);
            if (!attr.HasFlag(FileAttributes.Directory))
                throw new DirectoryNotFoundException("Input is not a directory");
            var backupPath = Path.Combine(InputPath, "backup_audio");
            if (!Directory.Exists(backupPath)) throw new DirectoryNotFoundException();
            foreach (var file in Directory.GetFiles(backupPath, "*.pck"))
            {
                File.Move(file, Path.Combine(InputPath, Path.GetFileName(file)), true);
            }
            if (Directory.GetFiles(backupPath).Length == 0)
                Directory.Delete(backupPath);

            return 0;
        }
    }
}