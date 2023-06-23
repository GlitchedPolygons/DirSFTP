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

namespace GlitchedPolygons.DirSFTP.Services.Config;

public interface IConfigStorage
{
    Task<IEnumerable<SftpConfig>> GetAll();
    Task<SftpConfig> Get(string id);
    Task<bool> Add(SftpConfig config, bool overwriteExisting = false);
    Task<bool> Remove(string id);
    bool RemoveAll();
}