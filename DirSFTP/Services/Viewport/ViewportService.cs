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
