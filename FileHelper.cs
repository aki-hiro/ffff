using ConsoleApp1.Attributes;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{

    /// <summary>
    /// ファイル操作ヘルパクラス
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// NXCSVのコンフィグ
        /// </summary>
        public static readonly CsvConfiguration NXCsvConfig = new CsvConfiguration(new CultureInfo("ja-JP", false))
        {
            //ヘッダ無（デフォルトtrue）
            HasHeaderRecord = false,
        };


        /// <summary>
        /// NXのエンコーディング
        /// </summary>
        public static readonly Encoding NXEncoding = Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// 区切り文字タブ
        /// </summary>
        public const string DELIMITER_TSV = "\t";

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        public static string[] ReadFile(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="mapper">マッピング処理</param>
        /// <returns></returns>
        public static IEnumerable<T> ReadFile<T>(string filePath, Func<string, T> mapper)
            where T : class
        {
            // マッピングして返却
            return ReadFile(filePath).Select(mapper);
        }


        /// <summary>
        /// ファイル書き込み
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        public static void ReadFile(string filePath, string txt)
        {
            File.WriteAllText(filePath, txt);
        }



        /// <summary>
        /// NXCSV読取
        /// </summary>
        /// <typeparam name="T">クラス</typeparam>
        /// <param name="filePath">パス</param>
        /// <param name="encoding">エンコーディング</param>
        /// <param name="config">設定</param>
        /// <returns></returns>
        public static IEnumerable<T> ReadNXCsv<T>(string filePath)
            where T : CsvReadBase, new()
        {
            // 読み込み処理実施
            return ReadCsv<T>(filePath, NXEncoding);
        }

        /// <summary>
        /// CSV読取
        /// </summary>
        /// <typeparam name="T">クラス</typeparam>
        /// <param name="filePath">パス</param>
        /// <param name="encoding">エンコーディング</param>
        /// <param name="hasHeader">ヘッダありかどうか</param>
        /// <param name="delimiter">区切り文字</param>
        /// <returns></returns>
        public static IEnumerable<T> ReadCsv<T>(string filePath, Encoding encoding, bool hasHeader = false, string delimiter = ",", bool isRtrim = true)
            where T : CsvReadBase, new()
        {
            var msg = $"ファイルの読み込みに失敗しました。ファイル名：{filePath}";

            try
            {
                var props = typeof(T).GetProperties().Where(p => !p.CustomAttributes.Any(a => a.AttributeType == typeof(IgnoreAttribute)));

                var list = new List<T>();

                using (var reader = CreateReader(filePath, encoding))
                using (var parser = new TextFieldParser(reader))
                {
                    parser.Delimiters = new string[] { delimiter };
                    // 終端まで処理
                    while (!parser.EndOfData)
                    {
                        // 読取
                        var line = parser.ReadFields();

                        // インスタンス生成
                        var ins = CreateIns<T>(line, props, list.Count(), isRtrim);
                        //行情報設定
                        ins.Line = string.Join(delimiter, line);
                        ins.RowIndex = list.Count();
                        list.Add(ins);
                    }
                }

                return list;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// CSV出力(NXフォーマット)
        /// </summary>
        /// <typeparam name="T">クラス</typeparam>
        /// <param name="list">一覧</param>
        /// <param name="filePath">パス</param>
        /// <returns></returns>
        public static bool WriteCsv<T>(IEnumerable<T> list, string filePath)
            where T : class
        {
            // CSV出力
            return WriteCsv<T>(list, filePath, NXEncoding);
        }


        /// <summary>
        　/// CSV出力
        　/// </summary>
        　/// <typeparam name="T">クラス</typeparam>
        　/// <param name="list">出力リスト</param>
        　/// <param name="filePath">ファイルパス</param>
        　/// <param name="encoding">エンコード</param>
        　/// <param name="isAppend">追加かどうか</param>
        　/// <param name="hasHeader">ヘッダありかどうか</param>
        　/// <param name="delimiter">区切り文字</param>
        　/// <returns></returns>
        public static bool WriteCsv<T>(IEnumerable<T> list, string filePath, Encoding encoding, bool isAppend = false, bool hasHeader = false, string delimiter = ",")
            where T : class
        {
            try
            {
                var config = CreateCsvConfig(hasHeader, delimiter);
                using (var writer = new StreamWriter(filePath, isAppend, encoding))
                {
                    using (var csv = new CsvWriter(writer, config))
                    {
                        // 書き込み処理実施
                        csv.WriteRecords(list);
                    }
                }

                return true;

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// リーダ取得
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static StreamReader CreateReader(string filePath, Encoding encoding)
        {
            return new StreamReader(filePath, encoding);
        }

        /// <summary>
        /// インスタンス生成
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="line">行</param>
        /// <param name="props">プロパティ</param>
        /// <returns></returns>
        private static T CreateIns<T>(string[] line, IEnumerable<PropertyInfo> props, int rowIndex, bool isRtrim)
            where T : CsvReadBase, new()
        {
            var ins = new T();

            ins.Datas = line;

            // 1項目ずつ処理
            foreach (var col in line.Select((r, i) => new { Index = i, Value = r }))
            {
                try
                {

                    // インデックスを一致しているプロパティを取得
                    var p = props.FirstOrDefault(d =>
                    {
                        var a = d.GetCustomAttributes(typeof(IndexAttribute), false).FirstOrDefault();

                        return (a as IndexAttribute)?.Index == col.Index;
                    });
                    // 取得できない場合
                    if (p == null)
                    {
                        continue;
                    }

                    var lengthAtr = p.GetCustomAttributes(typeof(CsvLengthAttribute), false).OfType<CsvLengthAttribute>().FirstOrDefault(); ;

                    // 値設定
                    p.SetValue(ins, GetValue(p, col.Value, lengthAtr, isRtrim));
                }
                catch (Exception e)
                {
                    throw e;
                }
            }


            return ins;
        }

        /// <summary>
        /// マイナス
        /// </summary>
        private const char STR_MINUS = '-';

        /// <summary>
        /// 値設定
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object GetValue(PropertyInfo prop, string value, CsvLengthAttribute lengthAtr, bool isNeedRTrim)
        {
            var type = prop.PropertyType;

            if (isNeedRTrim)
            {
                // トリムする
                value = value?.TrimEnd();

            }
            // null許容で値がnullの場合
            if (type != typeof(string) && string.IsNullOrEmpty(value))
            {
                return null;
            }

            var lengthErrMsg = "桁数エラー（エラー行：{0}、項目：{1}、項目値：{2}）";

            // 文字列の場合
            if (type == typeof(string))
            {
                // 最大桁数未満の場合
                if (lengthAtr == null || value.Length <= lengthAtr.MaxLength)
                {
                    return value;

                }
                // 桁数超過の場合
                else
                {
                    throw new Exception(lengthErrMsg);
                }
            }

            // shortの場合
            if (type == typeof(short) || type == typeof(short?))
            {
                var val = short.Parse(value);

                // 最大桁数未満の場合
                if (lengthAtr == null || val.ToString().TrimStart(STR_MINUS).Length <= lengthAtr.MaxLength)
                {
                    return val;

                }
                // 桁数超過の場合
                else
                {
                    throw new Exception(lengthErrMsg);
                }

            }

            // intの場合
            if (type == typeof(int) || type == typeof(int?))
            {
                var val = int.Parse(value);

                // 最大桁数未満の場合
                if (lengthAtr == null || (val.ToString().TrimStart(STR_MINUS).Length <= lengthAtr.MaxLength))
                {
                    return val;

                }
                // 桁数超過の場合
                else
                {
                    throw new Exception(lengthErrMsg);
                }

            }
            // decimalの場合
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                var val = decimal.Parse(value);

                // 整数部取得
                string intVal = value.Split('.').First();
                decimal deciVal = val % 1;

                // 小数部を文字列化（先頭-と0.を除外）
                var dec = deciVal.ToString().TrimEnd('0').TrimStart(STR_MINUS).Replace("0.", string.Empty);
                // 最大桁数未満の場合
                if (lengthAtr == null || (intVal.TrimStart(STR_MINUS).Length <= lengthAtr.MaxLength &&
                    (dec == "0" || dec.Length <= lengthAtr.MaxDeciLength)))
                {
                    return val;

                }
                // 桁数超過の場合
                else
                {
                    throw new Exception(lengthErrMsg);
                }
            }
            // longの場合
            if (type == typeof(long) || type == typeof(long?))
            {
                var val = long.Parse(value);

                // 最大桁数未満の場合
                if (lengthAtr == null || val.ToString().TrimStart(STR_MINUS).Length <= lengthAtr.MaxLength)
                {
                    return val;

                }
                // 桁数超過の場合
                else
                {
                    throw new Exception(lengthErrMsg);
                }
            }

            // 日付の場合
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                // フォーマットを取得
                var format = prop.GetCustomAttributes(typeof(FormatAttribute), false).FirstOrDefault() as FormatAttribute;
                // 変換
                return DateTime.ParseExact(value, format.Formats.First(), null);
            }

            return value;
        }

        /// <summary>
        /// コンフィグ情報生成
        /// </summary>
        /// <param name="hasHeader">ヘッダ有無</param>
        /// <param name="delimiter">区切り文字</param>
        /// <returns></returns>
        private static CsvConfiguration CreateCsvConfig(bool hasHeader, string delimiter)
        {
            return new CsvConfiguration(new CultureInfo("ja-JP", false)) { Delimiter = delimiter, HasHeaderRecord = hasHeader };
        }
    }
}
