
namespace keep2shareSDK
{
  public   class ReportStatus
    {
        public int ProgressPercentage { get; set; }
        public long BytesTransferred { get; set; }
        public long TotalBytes { get; set; }
        public string TextStatus { get; set; }
        public bool Finished { get; set; }
    }
}
