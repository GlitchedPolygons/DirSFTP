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
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlitchedPolygons.DirSFTP.Services.Viewport
{
    public class ViewportService : IViewportService
    {
        private readonly IJSRuntime jsRuntime;

        public ViewportService(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task<ViewportDimensions> GetViewportDimensions()
        {
            ViewportDimensions dimensions = await jsRuntime.InvokeAsync<ViewportDimensions>("getWindowDimensions");
            return dimensions;
        }
    }
}
