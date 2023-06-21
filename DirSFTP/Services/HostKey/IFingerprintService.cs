namespace GlitchedPolygons.DirSFTP.Services.HostKey;

public interface IFingerprintService
{
    Task<string> GetFingerprint(string host, int port, int timeoutSeconds = 6);
    Task<bool> CheckFingerprint(string host, int port, int timeoutSeconds = 6);
    Task<IDictionary<string, string>> GetAllStoredFingerprints();
    Task<bool> DeleteStoredFingerprint(string host, int port);
    Task DeleteAllStoredFingerprints();
}