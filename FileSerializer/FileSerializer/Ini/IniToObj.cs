using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

namespace FileSerializer.INI
{
    /// <summary>
    /// オブジェクトとINIファイルの変換・逆変換
    /// </summary>
    /// <typeparam name="T">インスタンスクラス</typeparam>
    public static class IniToObject<T> where T : class, new()
    {
        /// <summary>
        /// Default INI file name.
        /// </summary>
        public static string DefaultFileName => CommonFileSystem.GetIniFileName("ini");
        /// <summary>
        /// INIファイルからオブジェクトを生成
        /// セクション名はクラス名
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="instance">インスタンスオブジェクト</param>
        /// <returns></returns>
        public static bool TryDeserialize(string path, out T instance)
            => TryDeserialize(path, typeof(T).Name, out instance);
        /// <summary>
        /// Try to deserialize an object typed T.
        /// Target ini filename is "EXE NAME".ini
        /// </summary>
        /// <param name="instance">An object typed T</param>
        /// <returns>Success to deserialize</returns>
        public static bool TryDeserialize(out T instance)
            => TryDeserialize(DefaultFileName, out instance);
        /// <summary>
        /// INIファイルからオブジェクトを生成
        /// </summary>
        /// <param name="section">セクション名</param>
        /// <param name="path">パス</param>
        /// <param name="instance">インスタンスオブジェクト</param>
        /// <returns></returns>
        public static bool TryDeserialize(string path, string section, out T instance)
        {
            instance = default;
            if (!File.Exists(path))
                return false;

            try
            {
                instance = (T)Activator.CreateInstance(typeof(T));
                path = Path.GetFullPath(path);

                bool success = true;//TryGetValueOrDefaultで成功/失敗を更新
                foreach (var prop in typeof(T).GetProperties().Where(m => m.CanWrite && m.CanRead))
                {
                    success &= Ini.TryGetValueOrDefault(path, section, prop.Name, out string value);
                    var propValue = Convert.ChangeType(value, prop.PropertyType);
                    prop.SetValue(instance, propValue);
                }
                return success;
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine($"Exception: {e.Message}\nStackTrace: {e.StackTrace}");
#endif
                return false;
            }
        }

        /// <summary>
        /// オブジェクトの値をINIファイルに変換
        /// セクション名は、クラス名
        /// </summary>
        /// <param name="path">出力パス</param>
        /// <param name="obj">オブジェクト</param>
        /// <returns></returns>
        public static bool Serialize(string path, T obj) => Serialize(path, typeof(T).Name, obj);
        /// <summary>
        /// Try to serialize an object typed T.
        /// </summary>
        /// <param name="obj">An object typed T.</param>
        /// <returns></returns>
        public static bool Serialize(T obj) => Serialize(DefaultFileName, obj);
        /// <summary>
        /// オブジェクトの値をINIファイルに変換
        /// </summary>
        /// <param name="path">出力パス</param>
        /// <param name="secion">セクション名</param>
        /// <param name="obj">オブジェクト</param>
        /// <returns></returns>
        public static bool Serialize(string path, string secion, T obj)
        {
            if (obj == null)
                return false;

            try
            {
                path = Path.GetFullPath(path);

                foreach (var prop in typeof(T).GetProperties().Where(m => m.CanRead && m.CanWrite))
                {
                    if (!prop.CanWrite)
                        continue;
                    Ini.SetValue(path, secion, prop.Name, prop.GetValue(obj));
                }
                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine($"Exception: {e.Message}\nStackTrace: {e.StackTrace}");
#endif
                return false;
            }
        }
    }
}