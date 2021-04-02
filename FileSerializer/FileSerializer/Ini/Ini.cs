using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace FileSerializer.INI
{
    /// <summary>
    /// INIファイルの読み書きクラス
    /// </summary>
    public static class Ini
    {

        /// <summary>
        /// INIファイルからキーの値を取得します
        /// <para>戻り値は, 取得が成功したかどうかを示します</para>
        /// </summary>
        /// <typeparam name="T">データ取得する型</typeparam>
        /// <param name="path">ファイルパス</param>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <param name="outputValue">出力値</param>
        /// <returns>取得の成功有無</returns>
        public static bool TryGetValueOrDefault<T>(string path, string sectionName, string keyName,  out T outputValue)
        {
            outputValue = default;

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return false;

            var sb = new StringBuilder(1024);

            var ret = NativeMethods.GetPrivateProfileString(sectionName, keyName, string.Empty, sb, Convert.ToUInt32(sb.Capacity), path);
            if (ret == 0 || string.IsNullOrEmpty(sb.ToString()))
                return false;

            var conv = TypeDescriptor.GetConverter(typeof(T));
            if (conv == null)
                return false;

            try
            {
                outputValue = (T)conv.ConvertFromString(sb.ToString());
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// INIファイルからキーの値を取得します
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <returns>キー値</returns>
        public static int GetIntValueOrDefault(string path, string sectionName, string keyName)
        {
            TryGetValueOrDefault(path, sectionName, keyName,  out int ret);
            return ret;
        }
        /// <summary>
        /// INIファイルからキーの値を取得します
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <returns>キー値</returns>
        public static uint GetUintValueOrDefault(string path, string sectionName, string keyName)
        {
            TryGetValueOrDefault(path, sectionName, keyName, out uint ret);
            return ret;
        }
        /// <summary>
        /// INIファイルからキーの値を取得します
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <returns>キー値</returns>
        public static double GetDoubleValueOrDefault(string path, string sectionName, string keyName)
        {
            TryGetValueOrDefault(path, sectionName, keyName, out double ret);
            return ret;
        }
        /// <summary>
        /// INIファイルからキーの値を取得します
        /// </summary>
        /// <typeparam name="T">データ取得する型</typeparam>
        /// <param name="path">ファイルパス</param>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <returns>キー値</returns>
        public static T GetValueOrDefault<T>(string path, string sectionName, string keyName) 
        {
            TryGetValueOrDefault(path, sectionName, keyName, out T ret);
            return ret;
        }

        /// <summary>
        /// INIファイルにデータを書き込みます
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <param name="outputValue">出力値</param>
        public static void SetValue(string path, string sectionName, string keyName, object outputValue)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir) &&
                !string.IsNullOrWhiteSpace(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(path))
                File.WriteAllText(path, "");

            NativeMethods.WritePrivateProfileString(sectionName, keyName, outputValue.ToString(), path);
        }
    }
}