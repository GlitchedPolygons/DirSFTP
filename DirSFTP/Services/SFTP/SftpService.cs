using GlitchedPolygons.DirSFTP.Models;
using GlitchedPolygons.ExtensionMethods;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace GlitchedPolygons.DirSFTP.Services;

public class SftpService : ISftpService
{
    private readonly SftpConfig config;
    private readonly ILogger? logger;

    public SftpService(SftpConfig sftpConfig, ILogger? logger = null)
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
            logger?.LogError(exception, "Failed in listing files under [{remoteDirectory}]", remoteDirectory);
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
            logger?.LogInformation("Finished uploading the file [{localFilePath}] to [{remoteFilePath}]", localFilePath, remoteFilePath);
        }
        catch (Exception exception)
        {
            logger?.LogError(exception, "Failed in uploading the file [{localFilePath}] to [{remoteFilePath}]", localFilePath, remoteFilePath);
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
            logger?.LogInformation("Finished downloading the file [{localFilePath}] from [{remoteFilePath}]", localFilePath, remoteFilePath);
        }
        catch (Exception exception)
        {
            logger?.LogError(exception, "Failed in downloading the file [{localFilePath}] from [{remoteFilePath}]", localFilePath, remoteFilePath);
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
            logger?.LogInformation("File [{remoteFilePath}] is deleted.", remoteFilePath);
        }
        catch (Exception exception)
        {
            logger?.LogError(exception, "Failed deleting the file [{remoteFilePath}]", remoteFilePath);
        }
        finally
        {
            client.Disconnect();
        }
    }
}