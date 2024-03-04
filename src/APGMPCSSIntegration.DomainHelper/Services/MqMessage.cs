using Apache.NMS;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGMPCSSIntegration.DomainHelper
{
    public class MqMessage
    {
        public const string HEADER_MESSAGE_ID = "messageId";
        public const string HEADER_MESSAGE_TYPE = "messageType";
        public const string HEADER_DIGITAL_SIGNATURE = "digitalSignature";
        private IMessage message;

        public Dictionary<string, string> Headers { get; set; }

        public object Contents { get; set; }

        public string MessageId
        {
            get
            {
                if (this.Headers.Any())
                    return this.Headers["messageId"];
                return "";
            }
            set
            {
                this.Headers["messageId"] = value;
            }
        }

        public string MessageType
        {
            get
            {
                if (this.Headers.Any())
                    return this.Headers["messageType"];
                return "";
            }
            set
            {
                this.Headers["messageType"] = value;
            }
        }

        public string Signature
        {
            get
            {
                if (this.Headers.Any())
                    return this.Headers["digitalSignature"];
                return "";
            }
            set
            {
                this.Headers["digitalSignature"] = value;
            }
        }

        public IMessage Message
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
            }
        }

        public string NMSMessageId
        {
            get
            {
                return this.message == null ? (string)null : this.message.NMSMessageId;
            }
        }

        public string NMSCorrelationID
        {
            get
            {
                return this.message == null ? (string)null : this.message.NMSCorrelationID;
            }
        }

        public MqMessage(IMessage message)
        {
            this.message = message;
            if (message is ITextMessage textMessage)
                this.Contents = (object)textMessage.Text;
            if (message is IBytesMessage bytesMessage)
                this.Contents = (object)bytesMessage.Content;
            // this.Headers = message.Properties.ToDictionary();
        }

        public MqMessage(object content, string messageId, string messageType)
        {
            this.Headers = new Dictionary<string, string>();
            this.Contents = content;
            this.MessageId = messageId;
            this.MessageType = messageType;
        }

        public MqMessage(object content, Dictionary<string, string> headers)
        {
            this.Contents = content;
            this.Headers = headers;
        }

        public bool IsReply
        {
            get
            {
                return this.Contents is string contents && (contents.Contains("<FIToFICstmrCdtTrf>") || contents.Contains("{2:O103") || contents.Contains("<FIToFICstmrDrctDbt>") || contents.Contains("{2:O104"));
            }
        }
    }
}
