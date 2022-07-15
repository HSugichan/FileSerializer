using System;
using System.Runtime.InteropServices;
using System.IO;

namespace FileSerializer.BIN
{
    /// <summary>
    /// 構造体とバイナリーファイルの変換・逆変換
    /// </summary>
    /// <typeparam name="T">構造体の型</typeparam>
    public static class BinToStruct<T> where T : struct
    {
        /// <summary>
        /// Byte配列から構造体へ逆変換
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T Invert(byte[] buffer)
        {
            GCHandle gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T structIns = (T)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(T));
            gch.Free();

            return structIns;
        }
        /// <summary>
        /// 構造体からバイト配列へ変換
        /// </summary>
        /// <param name="structValue"></param>
        /// <returns></returns>
        public static byte[] Convert(T structValue)
        {
            int size = Marshal.SizeOf(structValue);
            byte[] by = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(structValue, ptr, false);

            //アンマネージデータをマネージのbyte[]にコピーする
            Marshal.Copy(ptr, by, 0, size);
            Marshal.FreeHGlobal(ptr);
            return by;
        }
        /// <summary>
        /// Default BIN file name.
        /// </summary>
        public static string DefaultFileName { get; } = CommonFileSystem.GetObjectFileName<T>("bin");

        /// <summary>
        /// バイナリーファイルを読み取って
        /// 指定した型の構造体に変換
        /// </summary>
        /// <param name="path"></param>
        /// <param name="structure"></param>
        /// <returns></returns>
        public static bool TryDeserialize(string path, out T structure)
        {
            structure = default;

            var file = new FileInfo(path);

            if (!file.Exists)
                return false;

            var binData = new byte[file.Length];
            var binRead = BinData.Read(path, out binData);
            if (!binRead)
                return false;

            structure = Invert(binData);
            return true;
        }
        /// <summary>
        /// Deserialize structure from specified file.
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        public static bool TryDeserialize(out T structure) => TryDeserialize(DefaultFileName, out structure);
        /// <summary>
        /// 指定した型の構造体をバイナリーファイルに保存
        /// </summary>
        /// <param name="path"></param>
        /// <param name="structure"></param>
        /// <returns></returns>
        public static bool Serialize(string path, T structure)
            => BinData.Write(path, Convert(structure));
        /// <summary>
        /// Serialize structure to file.
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        public static bool Serialize(T structure) => Serialize(DefaultFileName, structure);
    }
}