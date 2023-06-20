using GlitchedPolygons.DirSFTP.Models;

namespace GlitchedPolygons.DirSFTP.Services.Config;

public interface IConfigStorage
{
    Task<IEnumerable<SftpConfig>> GetAll();
    Task<SftpConfig> Get(string id);
    Task<bool> Add(SftpConfig config);
    Task<bool> Remove(string id);
    bool RemoveAll();
}