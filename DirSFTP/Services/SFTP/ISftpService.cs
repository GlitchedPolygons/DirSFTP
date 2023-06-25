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

using Renci.SshNet.Sftp;

namespace GlitchedPolygons.DirSFTP.Services.SFTP;

public interface ISftpService
{
    bool StillConnected { get; }
    bool Exists(string remotePath);
    IEnumerable<SftpFile> ListAll(string remoteDirectory = ".");
    Task<IEnumerable<SftpFile>> ListAllAsync(string remoteDirectory = ".");
    bool CreateDirectory(string remoteDirectory);
    void UploadFile(string localFilePath, string remoteFilePath);
    Task UploadFileAsync(string localFilePath, string remoteFilePath, bool overwriteExistingFiles = false, Action<ulong> uploadCallback = null);
    void DownloadFile(string remoteFilePath, string localFilePath);
    bool Rename(string remotePath, string newRemotePath);
    bool ChangePermissions(string remotePath, short newPermissions, bool recursively = false);
    bool Delete(SftpFile file);
}