namespace AIAudioLanguageConverter
{
    public class SoundbankInput
    {
        public SoundbankLanguages[] soundbank_languages { get; set; }

        public SoundbankLanguages GetFile(uint id, int lang)
        {
            foreach (var file in soundbank_languages)
            {
                switch (lang)
                {
                    case 0 when file.english.Contains(id):
                        return file;
                    case 1 when file.french.Contains(id):
                        return file;
                    case 2 when file.italian.Contains(id):
                        return file;
                    case 3 when file.german.Contains(id):
                        return file;
                    case 4 when file.spanish.Contains(id):
                        return file;
                    case 5 when file.portuguese.Contains(id):
                        return file;
                    case 6 when file.russian.Contains(id):
                        return file;
                }
            }

            throw new KeyNotFoundException("couldn't find id=" + id + " in " + Helpers.GetLanguageString(lang));
        }
    }
}