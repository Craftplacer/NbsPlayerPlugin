namespace NbsPlayerPlugin
{
    public class NbsFile
    {
        public float Delay => 20 / Speed;
        public string Description { get; internal set; }
        public NbsLayer[] Layers { get; internal set; }
        public short Length { get; internal set; }
        public string OriginalSongAuthor { get; internal set; }
        public string SongAuthor { get; internal set; }
        public string SongName { get; internal set; }
        public float Speed => Tempo / 100f;
        public float Tempo { get; internal set; }
        public string FileName { get; set; }

        public string GetLabel()
        {
            bool songNameSpecified = !string.IsNullOrWhiteSpace(SongName);
            bool songAuthorSpecified = !string.IsNullOrWhiteSpace(OriginalSongAuthor);

            if (songNameSpecified || songAuthorSpecified)
            {
                string songName = songNameSpecified ? SongName : "Unknown";
                string songAuthor = songAuthorSpecified ? OriginalSongAuthor : "Unknown";
                return songAuthor + " - " + songName;
            }
            else
            {
                return FileName;
            }
        }
    }
}