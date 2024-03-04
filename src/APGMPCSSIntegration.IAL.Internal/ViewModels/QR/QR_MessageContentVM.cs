using APGMPCSSIntegration.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.QR
{
    public class QR_MessageContentVM
    {
        public QR_MessageContentVM(
            string name, string tag, QR_DataTypeEnum dataType,
            int? maxLength, QR_UsageEnum usage, string description,
            List<QR_MessageContentVM> sub_QR_MessageContent)
        {
            Name = name;
            Tag = tag;
            DataType = dataType;
            MaxLength = maxLength;
            Usage = usage;
            Description = description;
            Sub_QR_MessageContent = sub_QR_MessageContent;
        }
        public string Name { get; set; }
        public string Tag { get; set; }
        public QR_DataTypeEnum DataType { get; set; }
        public int? MaxLength { get; set; }
        public QR_UsageEnum Usage { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }

        public List<QR_MessageContentVM> Sub_QR_MessageContent { get; set; }

    }
}
