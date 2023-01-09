namespace Applied_WebApplication.Data
{
    public interface IAppliedDependency
    {
        string DefaultDB { get; set; }
        string DefaultPath { get; set; }
        string UserDBPath { get; set; }
    }
}