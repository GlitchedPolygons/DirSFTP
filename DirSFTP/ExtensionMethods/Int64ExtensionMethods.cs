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

namespace GlitchedPolygons.DirSFTP.ExtensionMethods
{
    public static class Int64ExtensionMethods
    {
        private static readonly string[] SIZE_SUFFIX_STRINGS = { " B", " KiB", " MiB", " GiB", " TiB", " PiB", " EiB" };

        public static string ToFileSizeString(this Int64 bytes)
        {
            if (bytes == 0)
            {
                return "0" + SIZE_SUFFIX_STRINGS[0];
            }

            int i = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));

            double n = Math.Round(bytes / Math.Pow(1024, i), 1);

            return (Math.Sign(bytes) * n).ToString(System.Globalization.CultureInfo.InvariantCulture) + SIZE_SUFFIX_STRINGS[i];
        }

        public static string ToFileSizeString(this UInt64 bytes)
        {
            if (bytes > Int64.MaxValue)
            {
                // It would take more than 22000 years to upload > 8 EiB of data over SFTP if your upload speed was truly 100 Mbps (and that's faster than most SFTP connections).
                // But whatever, we'll leave this here anyway <:'D

                return $"> {ToFileSizeString(Int64.MaxValue)}";
            }

            return ToFileSizeString((Int64)bytes);
        }
    }
}
