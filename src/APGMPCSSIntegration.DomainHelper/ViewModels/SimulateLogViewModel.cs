using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;

namespace APGDigitalIntegration.DomainHelper.ViewModels
{
    public class SimulateLogViewModel
    {
        public LogOperation LogOperation { get; set; }
        public Guid Id { get; set; }
        public string RefId { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public string Request { get; set; }
    }
}
