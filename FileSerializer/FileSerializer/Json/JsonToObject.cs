using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace FileSerializer.JSON
{
    /// <summary>
    /// Serializer class to JSON file.
    /// Ref: https://dev.classmethod.jp/articles/c-sharp-json/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class JsonToObject<T> where T : class//配列対応のためnew()はつけない
    {
        /// <summary>
        /// Default JSON file name.
        /// </summary>
        public static string DefaultFileName => FileSerializer.CommonFileSystem.GetObjectFileName<T>("json");
        /// <summary>
        /// Serialize to JSON file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool Serialize(string path, T instance)
        {
            // Check arguments.
            if (instance == null || string.IsNullOrWhiteSpace(path))
                return false;

            var dirName = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(dirName) &&//この場合は引数がファイル名のみだった場合
                !Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);
            try
            {
                var resolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };

                var json = JsonConvert.SerializeObject(instance, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = resolver
                });    
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception: {e.Message}\nStackTrace: {e.StackTrace}");
                return false;
            }
        }
        /// <summary>
        /// Serialize to JSON file.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool Serialize(T instance) => Serialize(DefaultFileName, instance);
        /// <summary>
        /// Try to deserialize instance typed "T", from JSON file specified path
        /// </summary>
        /// <param name="path">JSON file</param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryDeserialize(string path, out T instance)
        {
            instance = null;

            if (!File.Exists(path))
                return false;

            try
            {
                var resolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
                var json = File.ReadAllText(path);
                instance = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = resolver
                });   
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception: {e.Message}\nStackTrace: {e.StackTrace}");
                return false;
            }
        }
        /// <summary>
        /// Try to deserialize instance typed "T", from JSON file
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryDeserialize(out T instance) => TryDeserialize(DefaultFileName, out instance);
    }
}