using System.IO;
using System.Diagnostics;
using System;

namespace FileSerializer.BIN
{
    /// <summary>
    /// バイナリーファイルのReadとWrite
    /// </summary>
    public static class BinData
    {
        /// <summary>
        /// バイナリーファイルのRead
        /// </summary>
        /// <param name="path"></param>
        /// <param name="binData"></param>
        /// <returns></returns>
        public static bool Read(string path, out byte[] binData)
        {
            binData = null;
            if (!File.Exists(path))
                return false;
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(fs))
                {
                    binData = new byte[fs.Length];
                    reader.Read(binData, 0, binData.Length);
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
        /// バイナリーデータのWrite
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool Write(string path, byte[] data)
        {
            if (string.IsNullOrWhiteSpace(path) || data == null)
                return false;

            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(dir) ||
                !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fs))
            {
                try
                {
                    writer.Write(data, 0, data.Length);
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
}