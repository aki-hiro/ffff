using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class MisaDenData
    {

        public List<Misa> MisaData { get; set; }


        public Misa A1 => MisaData.FirstOrDefault(d => d.RecordKbn == "A1");
        public Misa A2 => MisaData.FirstOrDefault(d => d.RecordKbn == "A2");
        public Misa A3 => MisaData.FirstOrDefault(d => d.RecordKbn == "A3");
        public Misa A4 => MisaData.FirstOrDefault(d => d.RecordKbn == "A4");

        public IEnumerable<Misa> L1List => MisaData.Where(d => d.RecordKbn == "L1");
        public IEnumerable<Misa> S1List => MisaData.Where(d => d.RecordKbn == "S1");
        public IEnumerable<Misa> LXList => MisaData.Where(d => d.RecordKbn == "LX");

        /// <summary>
        /// 品代タイプ
        /// </summary>
        [Index(0)]
        public int? ShinaDaiType => GetShinaDaiType();

        /// <summary>
        /// 伝票番号
        /// </summary>
        [Index(1)]
        public string DenNo => A1.DenNo;

        /// <summary>
        /// 複数DenID
        /// </summary>
        [Index(2)]
        public string MultipleDenId => MisaData.All(m => m.DenId == A1.DenId) ? "×" : "〇";

        /// <summary>
        /// 伝票識別ID
        /// </summary>
        [Index(3)]
        public string DenId => A1.DenId;

        /// <summary>
        /// 在庫直送区分
        /// </summary>
        [Index(4)]
        public string ZaikoChikusoKbn => A1.Datas[17];
        /// <summary>
        /// 在庫型統計作成区分
        /// </summary>
        [Index(5)]
        public string ZaikoGataToukeiSakuseiKbn => A1.Datas[25];

        /// <summary>
        /// 在庫種類区分
        /// </summary>
        [Index(6)]
        public string DenSyuruiKbn => A1.Datas[71];

        /// <summary>
        /// 売買区分
        /// </summary>
        [Index(7)]
        public string BaiBaiKbn => A1.Datas[19];

        /// <summary>
        /// 通貨
        /// </summary>
        [Index(8)]
        public string Currency
        {
            get
            {
                if (BaiBaiKbn == "A")
                {
                    return A3.Datas[17];
                }
                else if (BaiBaiKbn == "B")
                {
                    return A3.Datas[17];
                }
                else if (BaiBaiKbn == "C")
                {
                    return A3.Datas[21];
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 円貨か？
        /// </summary>
        public bool IsJPY => Currency == "JPY";

        /// <summary>
        /// 取引形態
        /// </summary>
        [Index(9)]
        public string ToriKeitai => A1.Datas[21];

        /// <summary>
        /// L1行数
        /// </summary>
        [Index(10)]
        public int L1Cnt => L1List.Count();

        /// <summary>
        /// LX行数
        /// </summary>
        [Index(11)]
        public int LXCnt => LXList.Count();

        /// <summary>
        /// LX行数
        /// </summary>
        [Index(12)]
        public int S1Cnt => S1List.Count();


        public MisaDenData(List<Misa> csvData)
        {
            MisaData = csvData;
        }

        public int? GetShinaDaiType()
        {
            if (ZaikoChikusoKbn == "D")
            { 
                if (ZaikoGataToukeiSakuseiKbn == "")
                {
                    if (DenSyuruiKbn == "")
                    {
                        if (BaiBaiKbn == "A")
                        {
                            return IsJPY ? 1 : 2;
                        }
                        else if (BaiBaiKbn == "B")
                        {
                            return IsJPY ? 3 : 4;
                        }
                        else if (BaiBaiKbn == "C")
                        {
                            return IsJPY ? 5 : 6;
                        }
                    }
                }
                else if (ZaikoGataToukeiSakuseiKbn == "Y")
                {
                    if (DenSyuruiKbn == "E")
                    {
                        if (BaiBaiKbn == "B")
                        {
                            return IsJPY ? 7 : 8;
                        }
                        else if (BaiBaiKbn == "C")
                        {
                            return IsJPY ? 9 : 10;
                        }
                    }
                    else if (DenSyuruiKbn == "1")
                    {
                        if (BaiBaiKbn == "C" && IsJPY)
                        {
                            return 11;
                        }
                    }
                    else if (DenSyuruiKbn == "2")
                    {
                        if (BaiBaiKbn == "C" && IsJPY)
                        {
                            return 12;
                        }
                    }
                    else if (DenSyuruiKbn == "6")
                    {
                        if (BaiBaiKbn == "C" && IsJPY)
                        {
                            return 13;
                        }
                    }
                    else if (DenSyuruiKbn == "7")
                    {
                        if (BaiBaiKbn == "C" && IsJPY)
                        {
                            return 14;
                        }
                    }
                }
            }
            return null;
        }
    }
}
