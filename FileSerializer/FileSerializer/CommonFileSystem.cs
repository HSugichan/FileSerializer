using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace FileSerializer
{
    static class CommonFileSystem
    {
        /// <summary>
        /// Gets ini filename.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetIniFileName(string extension = "ini")
            => Path.ChangeExtension(Assembly.GetEntryAssembly().Location, extension);
        /// <summary>
        /// Gets xml filename.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetObjectFileName<T>(string extension = "xml")
        {
            if (extension.StartsWith("."))
                extension = extension.Substring(1);

            string typeName = typeof(T).Name;
            if (typeName.EndsWith("[]"))//If class type is array.
                typeName = $"ArrayOf{typeName.Replace("[]", "")}";

            return $@"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\{typeName}.{extension}";
        }
    }
}