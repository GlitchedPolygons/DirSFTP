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