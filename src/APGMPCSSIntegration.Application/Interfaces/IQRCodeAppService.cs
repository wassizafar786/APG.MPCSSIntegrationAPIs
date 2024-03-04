using APGDigitalIntegration.IAL.Internal.Viewmodel.QR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Application.Interfaces
{
    public interface IQRCodeAppService
    {
        Task<QR_MessageRequestVM> ParseQR_ISO2006(string QR);
    }
}
