namespace Personnel.Tracker.Portal.Models
{
    public class UploadedFile
    {
        public bool IsSucceeded { get; set; }
        public string OriginalFileName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
    }
}
