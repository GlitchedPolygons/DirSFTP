using System.Text.Json;
using GlitchedPolygons.DirSFTP.Models;
using GlitchedPolygons.ExtensionMethods;

namespace GlitchedPolygons.DirSFTP.Services.Config;

public class ConfigStorage : IConfigStorage
{
    public async Task<IEnumerable<SftpConfig>> GetAll()
    {
        string jsonBase64 = await SecureStorage.GetAsync(Constants.PreferenceIds.SAVED_SFTP_CONNECTIONS);

        if (jsonBase64.NullOrEmpty())
        {
            return Array.Empty<SftpConfig>();
        }

        string json = Convert.FromBase64String(jsonBase64).UTF8GetString();

        return JsonSerializer.Deserialize<SftpConfig[]>(json);
    }

    public async Task<SftpConfig> Get(string id)
    {
        IEnumerable<SftpConfig> savedConnections = await GetAll();

        return savedConnections.FirstOrDefault(c => c.Id == id);
    }

    public async Task<bool> Add(SftpConfig config)
    {
        IEnumerable<SftpConfig> savedConnections = await GetAll();

        if (savedConnections.Any(c => c.Id == config.Id))
        {
            return false;
        }

        string json = JsonSerializer.Serialize(savedConnections.Concat(new[] { config }));

        string jsonBase64 = Convert.ToBase64String(json.UTF8GetBytes());

        await SecureStorage.SetAsync(Constants.PreferenceIds.SAVED_SFTP_CONNECTIONS, jsonBase64);

        return true;
    }

    public async Task<bool> Remove(string id)
    {
        IEnumerable<SftpConfig> savedConnections = await GetAll();

        if (!savedConnections.Any(c => c.Id == id))
        {
            return false;
        }

        string json = JsonSerializer.Serialize(savedConnections.Where(c => c.Id != id));

        string jsonBase64 = Convert.ToBase64String(json.UTF8GetBytes());

        await SecureStorage.SetAsync(Constants.PreferenceIds.SAVED_SFTP_CONNECTIONS, jsonBase64);

        return true;
    }

    public bool RemoveAll()
    {
        return SecureStorage.Remove(Constants.PreferenceIds.SAVED_SFTP_CONNECTIONS);
    }
}