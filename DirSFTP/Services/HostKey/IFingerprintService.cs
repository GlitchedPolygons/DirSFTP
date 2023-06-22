namespace GlitchedPolygons.DirSFTP.Services.HostKey;

public interface IFingerprintService
{
    Task<string> GetFingerprint(string host, int port, int timeoutSeconds = 6);
    Task<bool> CheckFingerprint(string host, int port, int timeoutSeconds = 6);
    Task<IDictionary<string, string>> GetAllStoredFingerprints();
    Task<bool> RemoveStoredFingerprint(string hostId);
    Task<bool> RemoveStoredFingerprint(string host, int port);
    Task RemoveAllStoredFingerprints();
}