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

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using GlitchedPolygons.DirSFTP.Services.Config;
using GlitchedPolygons.DirSFTP.Services.HostKey;
using GlitchedPolygons.DirSFTP.Services.Lock;
using GlitchedPolygons.DirSFTP.Services.Viewport;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;

namespace GlitchedPolygons.DirSFTP;

public static class MauiProgram
{
    private static Mutex mutex;

    public static MauiApp CreateMauiApp()
    {
        if (!IsSingleInstance())
        {
            Process.GetCurrentProcess().Kill();
        }

        MauiAppBuilder builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddTransient<IConfigStorage, ConfigStorage>();
        builder.Services.AddTransient<IViewportService, ViewportService>();
        builder.Services.AddTransient<IFingerprintService, FingerprintService>();
        builder.Services.AddSingleton<IUploadLockService, UploadLockService>();
        builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        return builder.Build();
    }

    private static bool IsSingleInstance()
    {
        mutex = new Mutex(false, Assembly.GetExecutingAssembly().FullName);
        GC.KeepAlive(mutex);

        try
        {
            return mutex.WaitOne(0, false);
        }
        catch (AbandonedMutexException)
        {
            mutex.ReleaseMutex();
            return mutex.WaitOne(0, false);
        }
    }
}