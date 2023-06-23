/*
    DirSFTP
    Copyright (C) 2023  Raphael Beck

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

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

    public async Task<bool> Add(SftpConfig config, bool overwriteExisting = false)
    {
        IEnumerable<SftpConfig> savedConnections = await GetAll();

        if (!overwriteExisting && savedConnections.Any(c => c.Id == config.Id))
        {
            return false;
        }

        savedConnections = savedConnections
            .Where(c => c.Id != config.Id)
            .Concat(new[] { config })
            .OrderBy(c => c.Host);

        string json = JsonSerializer.Serialize(savedConnections);

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