using Renci.SshNet.Sftp;

namespace GlitchedPolygons.DirSFTP.Services;

public interface ISftpService
{
    IEnumerable<SftpFile> ListAllFiles(string remoteDirectory = ".");
    void UploadFile(string localFilePath, string remoteFilePath);
    void DownloadFile(string remoteFilePath, string localFilePath);
    void DeleteFile(string remoteFilePath);
}