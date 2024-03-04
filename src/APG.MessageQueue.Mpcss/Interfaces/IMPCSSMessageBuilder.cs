using APGDigitalIntegration.Common.CommonViewModels.Common;

namespace APG.MessageQueue.Mpcss.Interfaces;

public interface IMPCSSMessageBuilder
{
    Envelope ConvertToExternalRequest<T>(T message, string createdDateTime, string messageId, string messageType, bool isMX);
    bool Verify(Envelope envelope);
}