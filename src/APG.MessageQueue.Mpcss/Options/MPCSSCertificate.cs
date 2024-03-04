namespace APG.MessageQueue.Mpcss.Options;

public class MPCSSCertificate
{
    public string PrivateKeyToken { get; set; }
    public string PspFilePath { get; set; }
    public string mpcCertificateFile { get; set; }
    public string pspCertificateFile { get; set; }
    public string HashAlgorithm { get; set; }
}
