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


//|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
//||||                                                                                                                   ||||
//|||| Copyright notice of the original (unmodified) MIT-licensed source file which this one was originally based on:    ||||
//||||                                                                                                                   ||||
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||


/*
 
The MIT License (MIT)

Copyright (c) 2016 Ioannis G.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

// https://github.com/JohnTheGr8/Renci.SshNet.Async

using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace GlitchedPolygons.DirSFTP.ExtensionMethods
{
    public static class SftpClientExtensionMethods
    {
        public static Task<IEnumerable<SftpFile>> ListDirectoryAsync
        (
            this SftpClient client,
            string path, 
            Action<int> listCallback = null,
            TaskFactory<IEnumerable<SftpFile>> factory = null,
            TaskCreationOptions creationOptions = default,
            TaskScheduler scheduler = null
        )
        {
            return (factory ??= Task<IEnumerable<SftpFile>>.Factory)
                .FromAsync
                (
                  client.BeginListDirectory(path, null, null, listCallback),
                  client.EndListDirectory,
                  creationOptions,
                  scheduler ?? factory.Scheduler ?? TaskScheduler.Current
                );
        }

        public static Task DownloadAsync
        (
            this SftpClient client,
            string path, 
            Stream output,
            TaskFactory factory = null,
            TaskCreationOptions creationOptions = default,
            TaskScheduler scheduler = null)
        {
            return (factory ??= Task.Factory)
                .FromAsync
                (
                    client.BeginDownloadFile(path, output),
                    client.EndDownloadFile,
                    creationOptions, 
                    scheduler ?? factory.Scheduler ?? TaskScheduler.Current
                );
        }

        public static Task DownloadAsync
        (
            this SftpClient client,
            string path, 
            Stream output, 
            Action<ulong> downloadCallback,
            TaskFactory factory = null,
            TaskCreationOptions creationOptions = default,
            TaskScheduler scheduler = null
        )
        {
            return (factory ??= Task.Factory)
                .FromAsync
                (
                    client.BeginDownloadFile(path, output, null, null, downloadCallback),
                    client.EndDownloadFile,
                    creationOptions, 
                    scheduler ?? factory.Scheduler ?? TaskScheduler.Current
                );
        }

        public static Task UploadAsync
        (
            this SftpClient client,
            Stream input, 
            string path, 
            Action<ulong> uploadCallback = null,
            TaskFactory factory = null,
            TaskCreationOptions creationOptions = default,
            TaskScheduler scheduler = null
        )
        {
            return (factory ??= Task.Factory)
                .FromAsync
                (
                    client.BeginUploadFile(input, path, null, null, uploadCallback),
                    client.EndUploadFile,
                    creationOptions, scheduler ?? factory.Scheduler ?? TaskScheduler.Current
                );
        }

        public static Task UploadAsync
        (
            this SftpClient client,
            Stream input, 
            string path, 
            bool overwriteExistingFiles, 
            Action<ulong> uploadCallback = null,
            TaskFactory factory = null,
            TaskCreationOptions creationOptions = default,
            TaskScheduler scheduler = null
        )
        {
            return (factory ??= Task.Factory)
                .FromAsync
                (
                    client.BeginUploadFile(input, path, overwriteExistingFiles, null, null, uploadCallback),
                    client.EndUploadFile,
                    creationOptions, 
                    scheduler ?? factory.Scheduler ?? TaskScheduler.Current
                );
        }
    }
}
