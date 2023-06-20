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

using GlitchedPolygons.DirSFTP.Models;
using GlitchedPolygons.ExtensionMethods;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace GlitchedPolygons.DirSFTP.Services.SFTP;

public class SftpService : ISftpService
{
    private readonly SftpConfig config;
    private readonly ILogger logger;

    public SftpService(SftpConfig sftpConfig, ILogger logger = null)
    {
        config = sftpConfig;
        this.logger = logger;

        if (config.Port is < 0 or >= 65536)
        {
            throw new ArgumentOutOfRangeException(nameof(sftpConfig.Port), $"{nameof(SftpService)}::ctor: The passed port number is outside of the allowed range of values [0; 65535]");
        }
    }

    private SftpClient Create()
    {
        if (config.PrivateKey.NotNullNotEmpty())
        {
            using Stream privateKeyStream = new MemoryStream(config.PrivateKey.UTF8GetBytes());

            return new SftpClient
            (
                config.Host,
                config.Port == 0 ? 22 : config.Port,
                config.Username,
                config.PrivateKeyPassphrase.NullOrEmpty()
                    ? new PrivateKeyFile(privateKeyStream)
                    : new PrivateKeyFile(privateKeyStream, config.PrivateKeyPassphrase)
            );
        }

        return new SftpClient
        (
            config.Host,
            config.Port == 0 ? 22 : config.Port,
            config.Username,
            config.Password
        );
    }

    public IEnumerable<SftpFile> ListAllFiles(string remoteDirectory = ".")
    {
        using SftpClient client = Create();

        try
        {
            client.Connect();
            return client.ListDirectory(remoteDirectory);
        }
        catch (Exception exception)
        {
            logger?.LogError(exception, "Failed in listing files under [{RemoteDirectory}]", remoteDirectory);
            return Array.Empty<SftpFile>();
        }
        finally
        {
            client.Disconnect();
        }
    }

    public void UploadFile(string localFilePath, string remoteFilePath)
    {
        using SftpClient client = Create();

        try
        {
            client.Connect();
            using FileStream fileStream = File.OpenRead(localFilePath);
            client.UploadFile(fileStream, remoteFilePath);
            logger?.LogInformation("Finished uploading the file [{LocalFilePath}] to [{RemoteFilePath}]", localFilePath, remoteFilePath);
        }
        catch (Exception exception)
        {
            logger?.LogError(exception, "Failed in uploading the file [{LocalFilePath}] to [{RemoteFilePath}]", localFilePath, remoteFilePath);
        }
        finally
        {
            client.Disconnect();
        }
    }

    public void DownloadFile(string remoteFilePath, string localFilePath)
    {
        using SftpClient client = Create();

        try
        {
            client.Connect();
            using FileStream fileStream = File.Create(localFilePath);
            client.DownloadFile(remoteFilePath, fileStream);
            logger?.LogInformation("Finished downloading the file [{LocalFilePath}] from [{RemoteFilePath}]", localFilePath, remoteFilePath);
        }
        catch (Exception exception)
        {
            logger?.LogError(exception, "Failed in downloading the file [{LocalFilePath}] from [{RemoteFilePath}]", localFilePath, remoteFilePath);
        }
        finally
        {
            client.Disconnect();
        }
    }

    public void DeleteFile(string remoteFilePath)
    {
        using SftpClient client = Create();

        try
        {
            client.Connect();
            client.DeleteFile(remoteFilePath);
            logger?.LogInformation("File [{RemoteFilePath}] is deleted", remoteFilePath);
        }
        catch (Exception exception)
        {
            logger?.LogError(exception, "Failed deleting the file [{RemoteFilePath}]", remoteFilePath);
        }
        finally
        {
            client.Disconnect();
        }
    }
}