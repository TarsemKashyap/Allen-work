namespace Infrastructure
{
    public class ProcessingError
    {
        public FileInfo File { get; set; }
        public int LineNumber { get; set; }
        public Exception exception { get; set; }
        public string Data { get; set; }

    }
}
