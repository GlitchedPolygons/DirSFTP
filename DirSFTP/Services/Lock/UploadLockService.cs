﻿/*
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

using Microsoft.AspNetCore.Components;

namespace GlitchedPolygons.DirSFTP.Services.Lock
{
    public class UploadLockService : IUploadLockService
    {
        private volatile bool isLocked;
        public bool IsLocked => isLocked;

        public event Action<bool> ChangedLockState;

        public void Lock()
        {
            if (IsLocked)
            {
                return;
            }

            isLocked = true;

            ChangedLockState?.Invoke(IsLocked);
        }

        public void Unlock()
        {
            if (!IsLocked)
            {
                return;
            }

            isLocked = false;

            ChangedLockState?.Invoke(IsLocked);
        }
    }
}
