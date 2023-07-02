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

using GlitchedPolygons.ExtensionMethods;

namespace GlitchedPolygons.DirSFTP.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        private static readonly char[] ILLEGAL_FILE_NAME_CHARS = new char[]
        {
            '/',
            '<',
            '>',
            ':',
            '\"',
            '|',
            '?',
            '*',
            '<',
        };

        private static readonly string[] ILLEGAL_FILE_NAMES = new string[]
        {
            "CON",  "PRN",  "AUX",  "NUL",
            "COM1", "COM2", "COM3", "COM4", 
            "COM5", "COM6", "COM7", "COM8", 
            "COM9", "LPT1", "LPT2", "LPT3", 
            "LPT4", "LPT5", "LPT6", "LPT7", 
            "LPT8", "LPT9"
        };

        public static bool IsValidFileName(this string str)
        {
            if (str.NullOrEmpty())
            {
                return false;
            }

            if (str.EndsWith('.') || str.EndsWith(' '))
            {
                return false;
            }

            if (str.Any(ILLEGAL_FILE_NAME_CHARS.Contains))
            {
                return false;
            }

            if (ILLEGAL_FILE_NAMES.Contains(str.ToUpperInvariant()))
            {
                return false;
            }

            return true;
        }
    }
}
