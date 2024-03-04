using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.IAL.Internal.Viewmodel.QR;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Application.Services
{
   
    public class QRCodeAppService : IQRCodeAppService
    {
        #region Initial Initialization
        const ushort poly = 4129;
        const ushort polynomial = 0xA001;
        ushort[] table = new ushort[256];
        ushort initialValue = 0;

        private List<QR_MessageContentVM> QR_MessageContent = new List<QR_MessageContentVM>();
        private List<string> ErrorList = new List<string>();
        private BaseResponse<string> Response = new BaseResponse<string>()
        {
            Data = null,
            Message = "",
            Success = false,
            ErrorList = null
        };
        #endregion


        #region Public Methods
        public async Task<QR_MessageRequestVM> ParseQR_ISO2006(string QR)
        {
            ReadyData();
            if (!string.IsNullOrEmpty(QR))
            {
                bool containsSearchResult = false;
                int first = 0;
                int last = 0;
                var tagPlusLength = "";
                var firstTwoDigits = "";
                foreach (var item in QR_MessageContent)
                {
                    if (item.Tag == "62" || item.Tag == "59" || item.Tag == "51")
                    {
                        var xc = 23;
                    }
                    firstTwoDigits = QR.Substring(0, 2);
                    containsSearchResult = QR.Contains(firstTwoDigits);
                    if (containsSearchResult && firstTwoDigits == item.Tag)
                    {

                        if (item.MaxLength != null)
                        {
                            first = QR.IndexOf(item.Tag);
                            var length = QR.Substring(first + item.Tag.Length, 2);
                            tagPlusLength = QR.Substring(first, item.Tag.Length + (length.Length) + int.Parse(length));
                            item.Value = QR.Substring(first + item.Tag.Length + (length.Length), int.Parse(length));
                            QR = QR.Replace(tagPlusLength, "");
                        }
                        else
                        {
                            first = QR.IndexOf(item.Tag);
                            var length = QR.Substring(first + item.Tag.Length, 2);
                            tagPlusLength = QR.Substring(first, item.Tag.Length + (length.Length) + int.Parse(length));
                            item.Value = QR.Substring(first + item.Tag.Length + (length.Length), int.Parse(length));
                            QR = QR.Replace(tagPlusLength, "");
                        }
                        tagPlusLength = "";
                    }
                    if (item.Sub_QR_MessageContent != null)
                    {

                        foreach (var subItem in item.Sub_QR_MessageContent)
                        {
                            if (!string.IsNullOrEmpty(item.Value))
                            {
                                firstTwoDigits = item.Value.Substring(0, 2);
                                containsSearchResult = item.Value.Contains(firstTwoDigits);
                                if (containsSearchResult && firstTwoDigits == subItem.Tag)
                                {
                                    {
                                        first = item.Value.IndexOf(subItem.Tag);
                                        var length = item.Value.Substring(first + subItem.Tag.Length, 2);
                                        tagPlusLength = item.Value.Substring(first, subItem.Tag.Length + (length.Length) + int.Parse(length));
                                        subItem.Value = item.Value.Substring(first + subItem.Tag.Length + (length.Length), int.Parse(length));
                                        item.Value = item.Value.Replace(tagPlusLength, "");
                                    }
                                }
                            }
                        }

                    }
                }
            }
            var myModel = PassToModel();
            myModel.BICCode = GetUntilOrEmpty(myModel.MerchantId, "-");
            myModel.MerchantIdentifier = GetFrom(myModel.MerchantId, "-");

            if (!string.IsNullOrEmpty(myModel.InitiationMethod))
            {
                myModel.DataPresented = int.Parse(myModel.InitiationMethod.Substring(0, 1));
                myModel.DataProvider = int.Parse(myModel.InitiationMethod.Substring(1, 1));
            }
            return myModel;
        }
        #endregion

        #region Private Methods
        private string GetUntilOrEmpty(string text, string stopAt = "-")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }
        private string GetFrom(string text, string startFrom = "-")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(startFrom, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(charLocation + 1);
                }
            }

            return String.Empty;
        }

        private QR_MessageRequestVM PassToModel()
        {
            QR_MessageRequestVM model = new QR_MessageRequestVM();
            //var json = JSON.stringify(model);
            var json = JsonConvert.SerializeObject(model);
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var result = "";
            values.Clear();
            foreach (var item in QR_MessageContent)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    values.Add(item.Name, item.Value);
                }
                if (item.Sub_QR_MessageContent != null)
                {
                    foreach (var sub in item.Sub_QR_MessageContent)
                    {
                        if (!string.IsNullOrEmpty(sub.Value))
                        {
                            values.Add(sub.Name, sub.Value);
                        }
                    }
                }
            }
            var jsonResponse = JsonConvert.SerializeObject(values);
            model = JsonConvert.DeserializeObject<QR_MessageRequestVM>(jsonResponse);
            return model;
        }
        private void ReadyData()
        {
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.PayloadFormatIndicator, QR_Tag.PayloadFormatIndicator, QR_DataTypeEnum.N, 2, QR_UsageEnum.Mandatory, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.InitiationMethod, QR_Tag.InitiationMethod, QR_DataTypeEnum.N, 2, QR_UsageEnum.Mandatory, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.MerchantId, QR_Tag.MerchantId, QR_DataTypeEnum.ANS, 50, QR_UsageEnum.Conditional, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.MerchantTxnIdentifier, QR_Tag.MerchantTxnIdentifier, QR_DataTypeEnum.ANS, null, QR_UsageEnum.Conditional, String.Empty,
                new List<QR_MessageContentVM>()
                {
                    new QR_MessageContentVM(QR_TagName.Reserved, QR_Tag.Reserved, QR_DataTypeEnum.ANS, 10, QR_UsageEnum.Optional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.MerchantIdentifier, QR_Tag.MerchantIdentifier, QR_DataTypeEnum.ANS, 16, QR_UsageEnum.Conditional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.MerchantAliasName, QR_Tag.MerchantAliasName, QR_DataTypeEnum.ANS, 25, QR_UsageEnum.Conditional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.MerchantRef, QR_Tag.MerchantRef, QR_DataTypeEnum.ANS, 30, QR_UsageEnum.Mandatory, String.Empty, null)
                }));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.PersonalIdentifier, QR_Tag.PersonalIdentifier, QR_DataTypeEnum.ANS, null, QR_UsageEnum.Conditional, String.Empty,
                new List<QR_MessageContentVM>()
                {
                    new QR_MessageContentVM(QR_TagName.Reserved, QR_Tag.Reserved, QR_DataTypeEnum.ANS, 10, QR_UsageEnum.Optional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.MobileNumber, QR_Tag.MobileNumber, QR_DataTypeEnum.ANS, 10, QR_UsageEnum.Conditional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.PersonalAliasName, QR_Tag.PersonalAliasName, QR_DataTypeEnum.ANS, 25, QR_UsageEnum.Conditional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.IdNumber, QR_Tag.IdNumber, QR_DataTypeEnum.ANS, 10, QR_UsageEnum.Optional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.NameOnID, QR_Tag.NameOnID, QR_DataTypeEnum.ANS, 30, QR_UsageEnum.Optional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.DateOfBirth, QR_Tag.DateOfBirth, QR_DataTypeEnum.ANS, 8, QR_UsageEnum.Optional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.IDExpiry, QR_Tag.IDExpiry, QR_DataTypeEnum.ANS, 8, QR_UsageEnum.Optional, String.Empty, null)
                }));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.Filler, QR_Tag.Filler, QR_DataTypeEnum.ANS, 50, QR_UsageEnum.Optional, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.MerchantCategoryCode, QR_Tag.MerchantCategoryCode, QR_DataTypeEnum.N, 4, QR_UsageEnum.Conditional, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.TransactionCurrencyCode, QR_Tag.TransactionCurrencyCode, QR_DataTypeEnum.AN, 3, QR_UsageEnum.Conditional, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.TransactionAmount, QR_Tag.TransactionAmount, QR_DataTypeEnum.ANS, 13, QR_UsageEnum.Conditional, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.TipOrConvenienceIndicator, QR_Tag.TipOrConvenienceIndicator, QR_DataTypeEnum.N, 2, QR_UsageEnum.Optional, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.ConvenienceFeeFixed, QR_Tag.ConvenienceFeeFixed, QR_DataTypeEnum.ANS, 13, QR_UsageEnum.Conditional, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.ConvenienceFeePercentage, QR_Tag.ConvenienceFeePercentage, QR_DataTypeEnum.ANS, 5, QR_UsageEnum.Conditional, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.CountryCode, QR_Tag.CountryCode, QR_DataTypeEnum.AN, 2, QR_UsageEnum.Conditional, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.MerchantName, QR_Tag.MerchantName, QR_DataTypeEnum.ANS, 20, QR_UsageEnum.Mandatory, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.MerchantCity, QR_Tag.MerchantCity, QR_DataTypeEnum.ANS, 15, QR_UsageEnum.Conditional, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.PostalCode, QR_Tag.PostalCode, QR_DataTypeEnum.AN, null, QR_UsageEnum.Conditional, String.Empty, null));
            QR_MessageContent.Add(new QR_MessageContentVM(QR_TagName.AdditionalDataField, QR_Tag.AdditionalDataField, QR_DataTypeEnum.ANS, null,
                QR_UsageEnum.Conditional, String.Empty,
                new List<QR_MessageContentVM>()
                {
                    new QR_MessageContentVM(QR_TagName.TerminalId, QR_Tag.TerminalId, QR_DataTypeEnum.ANS, 15, QR_UsageEnum.Optional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.GroupMerchId, QR_Tag.GroupMerchId, QR_DataTypeEnum.ANS, 15, QR_UsageEnum.Optional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.ConsumerId, QR_Tag.ConsumerId, QR_DataTypeEnum.ANS, 15, QR_UsageEnum.Optional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.Filler, QR_Tag.AdditionalDataField_Filler, QR_DataTypeEnum.ANS, 15, QR_UsageEnum.Optional, String.Empty, null),
                    new QR_MessageContentVM(QR_TagName.InvoiceNo, QR_Tag.InvoiceNo, QR_DataTypeEnum.ANS, 15, QR_UsageEnum.Optional, String.Empty, null)
                }));

            QR_MessageContent.Add(new QR_MessageContentVM("CRC", "63", QR_DataTypeEnum.AN, 4, QR_UsageEnum.Mandatory, String.Empty, null));
        }
        #endregion




    }
}
