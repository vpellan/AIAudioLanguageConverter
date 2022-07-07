using System.Collections.ObjectModel;
using AIAudioLanguageConverter.NPCKEditor.Common;

namespace AIAudioLanguageConverter.NPCKEditor.ViewModels
{
    public class NPCKViewModel
    {
        public NPCKHeader npck = null;//shouldn't have to bind directly to this

        public ObservableCollection<Wem> wems 
        { 
            get => npck == null ? new ObservableCollection<Wem>() : new ObservableCollection<Wem>(npck.WemList);
            set => npck.WemList = new List<Wem>(value);
        }
        public ObservableCollection<string> languages
        { 
            get => npck == null ? new ObservableCollection<string>() : new ObservableCollection<string>(npck.GetLanguages());
            set => throw new NotImplementedException();
        }

        public NPCKViewModel()
        {
            npck = null;
        }

        public void SetNPCK(NPCKHeader file)
        {
            npck = file;
        }

        public void AddWems(IEnumerable<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                var newWem = HelperFunctions.MakeWems(fileName, HelperFunctions.OpenFile(fileName));
                npck.WemList.Add(newWem);
            }
        }

        public void ReplaceWem(Wem newWem,int index)
        {
            newWem.id = npck.WemList[index].id;
            newWem.languageEnum = npck.WemList[index].languageEnum;
            npck.WemList[index] = newWem;
        }
        
        public void ReplaceWemFromId(Wem newWem,uint id)
        {
            var index = npck.GetWemIndexFromId(id);
            var wemCpy = (Wem)newWem.Clone();
            wemCpy.id = npck.WemList[index].id;
            wemCpy.languageEnum = npck.WemList[index].languageEnum;
            npck.WemList[index] = wemCpy;
        }
        
        public void DeleteWem(int index)
        {
            npck.WemList.RemoveAt(index);
        }

        public void ExportWems(string savePath)
        {
            foreach (var newWem in npck.WemList)
            {
                var name = savePath + "\\" + newWem.name + ".wem";
                var bw = new BinaryWriter(new FileStream(name, FileMode.OpenOrCreate));
                bw.Write(newWem.file);
                bw.Close();
            }
        }

        public void ExportNPCK(string fileName, SupportedGames mode)
        {
            npck.ExportFile(fileName);
            if (mode is SupportedGames.RE2DMC5 or SupportedGames.RE3R or SupportedGames.MHRise or SupportedGames.RE8)
            {
                npck.ExportHeader(fileName + ".nonstream");
            }
        }

        public void IDReplace(string[] id2)
        {
            for (var i = 0; i < id2.Length; i++)
            {
                try
                {
                    npck.WemList[i].id = Convert.ToUInt32(id2[i]);
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }
        }

        public void ExportLabels(SupportedGames mode, string currentFileName, List<uint> changedIds)
        {
            npck.labels.Export(Directory.GetCurrentDirectory() + "/" + mode.ToString() + "/PCK/" + currentFileName + ".lbl", npck.WemList, changedIds);
        }

        public List<uint> GetWemIds()
        {
            return npck.WemList.Select(t => t.id).ToList();
        }
    }
}
