using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace FileSerializer.XML
{
    /// <summary>
    /// 指定オブジェクトとXMLファイルの変換・逆変換
    /// </summary>
    /// <typeparam name="T">インスタンスクラス</typeparam>
    public class XmlToObject<T> where T : class//, new()配列も対象にしたいためこの制約は無し
    {
        private static readonly string _xmlFile = FileSerializer.CommonFileSystem.GetObjectFileName<T>("xml");
        /// <summary>
        /// 指定クラスのオブジェクトをxmlに保存する
        /// </summary>
        /// <param name="obj">保存オブジェクト</param>
        /// <param name="path">保存先のファイルパス</param>
        /// <returns></returns>
        public static bool Serialize(string path, T obj)
        {
            // Check arguments.
            if (obj == null || string.IsNullOrWhiteSpace(path))
                return false;

            var dirName = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(dirName) &&//この場合は引数がファイル名のみだった場合
                !Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    serializer.Serialize(sw, obj);
                    sw.Close();
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

        /// <summary>
        /// Object serialization to XML file.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Serialize(T obj) => Serialize(_xmlFile, obj);
        /// <summary>
        /// 指定クラスのオブジェクト(配列対応)をxmlから復元する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryDeserialize(string path, out T instance)
        {
            instance = null;

            if (!File.Exists(path))
                return false;

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var sr = new StreamReader(path, Encoding.UTF8))
                {
                    instance = (T)serializer.Deserialize(sr);
                    sr.Close();
                    return true;
                }
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
        /// Try to deserialize.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryDeserialize(out T instance) => TryDeserialize(_xmlFile, out instance);
    }
}