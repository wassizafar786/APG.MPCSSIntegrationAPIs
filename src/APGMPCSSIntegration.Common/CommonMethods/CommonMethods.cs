using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace APGDigitalIntegration.Common.CommonMethods
{
    public static class CommonMethods
    {
        public static string GetXMLAttributeValue(XElement xml, string attributeName)
        {
            var xmlString = xml.ToString();
            string attributeValue = string.Empty;
            if (xmlString != null && xmlString.Contains(attributeName))
            {
                #region replace extra added values
                xmlString = xmlString.Replace("<TotalInterbankSettelmentAmount Ccy=\"OMR\">", "<TotalInterbankSettelmentAmount>");
                xmlString = xmlString.Replace("<RtrdIntrBkSttlmAmt Ccy=\"OMR\">", "<RtrdIntrBkSttlmAmt>");
                #endregion

                int startIndex = xmlString.IndexOf($"<{attributeName}");
                int endIndex = xmlString.IndexOf($"</{attributeName}>");
                attributeValue = xmlString.Substring(startIndex, endIndex - startIndex).Replace($"<{attributeName}>", string.Empty);
            }
            return attributeValue;
        }
    }
}
