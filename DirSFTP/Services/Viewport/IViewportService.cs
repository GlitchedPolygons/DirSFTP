using GlitchedPolygons.DirSFTP.Models;

namespace GlitchedPolygons.DirSFTP.Services.Viewport
{
    public interface IViewportService
    {
        Task<ViewportDimensions> GetViewportDimensions();
    }
}
