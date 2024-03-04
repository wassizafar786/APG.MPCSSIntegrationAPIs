namespace APGDigitalIntegration.BackgroundJobs.Interfaces
{
    public interface ICreateDTFJobService : IDisposable
    {
        void CreateDTFAudit();
    }
}
