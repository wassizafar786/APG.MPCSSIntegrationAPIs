using System.Xml;
using APG.MessageQueue.Mpcss.Interfaces;
using APG.MessageQueue.Mpcss.Options;
using APGDigitalIntegration.Common.CommonViewModels.Common;
using APGDigitalIntegration.DomainHelper.Services;
using APGMPCSSIntegration.DomainHelper.Services;
using Microsoft.Extensions.Options;

namespace APG.MessageQueue.Mpcss.Services;

public class MPCSSMessageBuilder : IMPCSSMessageBuilder
{
    private readonly MPCSSCertificate _mpcssCertificateOptions;

    public MPCSSMessageBuilder(IOptions<MPCSSCertificate> mpcssCertificateOptions)
    {
        _mpcssCertificateOptions = mpcssCertificateOptions.Value;
    }
        
    public Envelope ConvertToExternalRequest<T>(T message, string createdDateTime, string messageId, string messageType, bool isMX)
    {
        var content = XmlSerializationHelper.Serialize(message);
        var signature = Sign(content, createdDateTime);

        return new Envelope()
        {
            Content = new XmlDocument().CreateCDataSection(content),
            Signature = signature,
            Date = createdDateTime,
            Id = messageId,
            Type = messageType,
            Format = isMX ? "MX" : "",
        };

    }
        
    private string Sign(string msg, string date)
    {
        try
        {
            var pspCertificate = new Security(_mpcssCertificateOptions.PspFilePath + _mpcssCertificateOptions.pspCertificateFile, _mpcssCertificateOptions.PrivateKeyToken, _mpcssCertificateOptions.HashAlgorithm);
            var message = $"{msg}{date}";
                
            return pspCertificate.Sign(message);
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public bool Verify(Envelope envelope)
    {
        try
        {
            var mpcCertificate = new Security(_mpcssCertificateOptions.PspFilePath + _mpcssCertificateOptions.mpcCertificateFile, _mpcssCertificateOptions.HashAlgorithm);
            return mpcCertificate.Verify(envelope.Content.Value, envelope.Signature);
        }
        catch (Exception ex)
        {
            return false;
        }

    }
        
}