public record FileConfig
{
    public string InboundFolder { get; set; }
    public string ProcessFolder { get; set; }
    public string SearchPattern { get; set; }

     public string EmailFrom { get; set; }
    public string SmtpHost { get; set; }
    public string SmtpPort { get; set; }
     public string SmtpUser { get; set; }
    public string SmtpPass { get; set; }
  


}

