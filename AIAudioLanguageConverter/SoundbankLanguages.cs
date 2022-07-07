namespace AIAudioLanguageConverter
{
    public class SoundbankLanguages
    {
        public string filename { get; }
        public List<uint> english { get; set; } = new();
        public List<uint> french { get; set; } = new();
        public List<uint> german { get; set; } = new();
        public List<uint> italian { get; set; } = new();
        public List<uint> portuguese { get; set; } = new();
        public List<uint> russian { get; set; } = new();
        public List<uint> spanish { get; set; } = new();

        public SoundbankLanguages(string filename)
        {
            this.filename = filename;
        }

        private int _treatedFiles = 0;

        public int GetTreatedFiles()
        {
            return _treatedFiles;
        }

        public void IncrementTreatedFiles()
        {
            _treatedFiles++;
        }

        public int GetTreatedFilesThenIncrement()
        {
            var temp = _treatedFiles;
            _treatedFiles++;
            return temp;
        }
    }
}