
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class MisaShokakariData
    {
        [Ignore]
        public List<Misa> MisaData { get; set; }

        [Ignore]
        public Misa S1 { get; set; }


        [Ignore]
        public Misa A1 => MisaData.FirstOrDefault(d => d.RecordKbn == "A1");
        [Ignore]
        public Misa A2 => MisaData.FirstOrDefault(d => d.RecordKbn == "A2");
        [Ignore]
        public Misa A3 => MisaData.FirstOrDefault(d => d.RecordKbn == "A3");
        [Ignore]
        public Misa A4 => MisaData.FirstOrDefault(d => d.RecordKbn == "A4");

        [Ignore]
        public IEnumerable<Misa> L1List => MisaData.Where(d => d.RecordKbn == "L1");
        [Ignore]
        public IEnumerable<Misa> S1List => MisaData.Where(d => d.RecordKbn == "S1");
        [Ignore]
        public IEnumerable<Misa> LXList => MisaData.Where(d => d.RecordKbn == "LX");

        /// <summary>
        /// 品代タイプ
        /// </summary>
        [Index(0)]
        [Name("諸掛タイプ")]
        public string ShinaDaiType => GetShokakariType();

        /// <summary>
        /// 伝票番号
        /// </summary>
        [Index(1)]
        [Name("伝票番号")]
        public string DenNo => A1.DenNo;

        /// <summary>
        /// 複数DenID
        /// </summary>
        [Index(2)]
        [Name("複数伝票識別ID")]
        public string MultipleDenId => MisaData.All(m => m.DenId == A1.DenId) ? "×" : "〇";

        /// <summary>
        /// 伝票識別ID
        /// </summary>
        [Index(3)]
        [Name("A1伝票識別ID")]
        public string DenId => A1.DenId;

        [Index(4)]
        [Name("諸掛種類区分")]
        public string SyokakariKbn => S1.Datas[13];

        [Index(5)]
        [Name("勘定科目区分")]
        public string KmkKbn => S1.Datas[14];

        [Index(6)]
        [Name("勘定科目内分類")]
        public string KmkBnri => S1.Datas[15];

        /// <summary>
        /// 在庫直送区分(D:直送、Z:在庫、M:未着)
        /// </summary>
        [Index(7)]
        [Name("A1在庫直送区分(未使用)")]
        public string ZaikoChokusoKbn => A1.Datas[17];

        /// <summary>
        /// 売買区分
        /// </summary>
        [Index(8)]
        [Name("売買区分")]
        public string BaiBaiKbn => A1.Datas[19];

        /// <summary>
        /// 在庫口座区分
        /// </summary>
        [Index(9)]
        [Name("在庫口座区分")]
        public string ZaikoKozKbn => LXList.FirstOrDefault()?.Datas[6];

        [Index(10)]
        [Name("伝票種類区分（在庫型直送用）")]
        public string DenSriKbn => A1.Datas[59];

        /// <summary>
        /// 在庫タイプ
        /// </summary>
        [Index(11)]
        [Name("在庫タイプ")]
        public string ZaikoType
        {
            get
            {
                if (BaiBaiKbn == "C" && ZaikoKozKbn == "Z")
                {
                    return "在庫-買";
                }
                else if (BaiBaiKbn == "C" && ZaikoKozKbn == "M")
                {
                    return "在庫-売";
                }
                else if (DenSriKbn == "2")
                {
                    return "在庫変更・移動";
                }
                else if (DenSriKbn == "1")
                {
                    return "未着→在庫";
                }
                return null;
            }
        }

        /// <summary>
        /// 取引形態
        /// </summary>
        [Index(12)]
        [Name("取引形態")]
        public string ToriKeitai => A1.Datas[21];

        /// <summary>
        /// L1行数
        /// </summary>
        [Index(13)]
        [Name("L1行数")]
        public int L1Cnt => L1List.Count();

        /// <summary>
        /// LX行数
        /// </summary>
        [Index(14)]
        [Name("LX行数")]
        public int LXCnt => LXList.Count();

        /// <summary>
        /// LX行数
        /// </summary>
        [Index(15)]
        [Name("S1行数")]
        public int S1Cnt => S1List.Count();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="csvData"></param>
        /// <param name="S1"></param>
        public MisaShokakariData(List<Misa> csvData, Misa S1)
        {
            this.MisaData = csvData;
            this.S1 = S1;
        }

        public string GetShokakariType()
        {
            // 諸掛区分
            switch(SyokakariKbn)
            {
                case "0":
                    return GetNormaType();
                case "1":
                    return GetUriShinaDaiType();
                case "2":
                    return GetKaiShinadaiType();
                case "3":
                    return GetUriTesuryoType();
                case "4":
                    return GetKaiTesuryoType();
                case "5":
                    return GetUriKakeType();
                case "6":
                    return GetMaeUketype();
                case "8":
                    return GetSyusiRisokuType();
                case "9":
                    return GetSaisanRisokuType();
                case "C":
                    return GetSyanaiType();
                default:
                    return null;
            }
            return null;
        }

        private string GetSyanaiType()
        {
            if (KmkKbn == "0" && KmkBnri == "50")
            {
                return "C050";
            }
            return null;
        }

        private string GetSaisanRisokuType()
        {
            if (KmkKbn == "9")
            {
                if (new string[] { "99", "88" }.Contains(KmkBnri))
                {
                    // TODO 在庫判定
                    return $"{SyokakariKbn}{KmkKbn}{KmkBnri}";
                }
            }
            return null;
        }

        private string GetSyusiRisokuType()
        {
            if (KmkKbn == "3" || KmkKbn == "4")
            {
                if (KmkBnri.StartsWith("7"))
                {
                    // TODO 在庫判定
                    return $"{SyokakariKbn}{KmkKbn}{KmkBnri}";
                }
            }
            return null;
        }

        private string GetMaeUketype()
        {
            if (KmkKbn == "1")
            {
                if (new string[] { "00", "02", "03" }.Contains(KmkBnri))
                {
                    // TODO 在庫判定
                    return $"{SyokakariKbn}{KmkKbn}{KmkBnri}";
                }
            }
            if (KmkKbn == "2")
            {
                if (KmkBnri == "00")
                {
                    // TODO 在庫判定
                    return $"{SyokakariKbn}{KmkKbn}{KmkBnri}";
                }
            }
            return null;
        }

        private string GetUriKakeType()
        {
            if (KmkKbn == "1")
            {
                if (KmkBnri == "00")
                {
                    // TODO 在庫判定
                    return $"{SyokakariKbn}{KmkKbn}{KmkBnri}";
                }
                if (new string[] { "02", "03" }.Contains(KmkBnri))
                {
                    // TODO 在庫判定
                    return $"{SyokakariKbn}{KmkKbn}{KmkBnri}";
                }
            }
            return null;
        }

        private string GetKaiTesuryoType()
        {
            if (KmkKbn == "1")
            {
                if (new string[] { "20", "30" }.Contains(KmkBnri))
                {
                    // TODO 在庫判定
                    return $"{SyokakariKbn}{KmkKbn}{KmkBnri}";
                }
            }
            return null;
        }

        private string GetUriTesuryoType()
        {
            if (KmkKbn == "1")
            {
                if (new string[] { "20", "30" }.Contains(KmkBnri))
                {
                    // TODO 在庫判定
                    return $"{SyokakariKbn}{KmkKbn}{KmkBnri}";
                }
            }
            return null;
        }

        private string GetKaiShinadaiType()
        {
            if (KmkKbn == "2" && KmkBnri == "00")
            {
                // TODO 在庫判定
                return $"{SyokakariKbn}{KmkKbn}{KmkBnri}";
            }
            if (KmkKbn == "3" && KmkBnri == "20")
            {
                // TODO 在庫判定
                return $"{SyokakariKbn}{KmkKbn}{KmkBnri}";
            }
            if (KmkKbn == "3" && KmkBnri == "2A")
            {
                // TODO 在庫判定
                return $"{SyokakariKbn}{KmkKbn}{KmkBnri}";
            }
            return null;
        }

        private string GetUriShinaDaiType()
        {
            if (KmkKbn == "1")
            {
                if (new string[] { "00", "02", "03", "20", "30" }.Contains(KmkBnri))
                {
                    // TODO 在庫判定
                    return $"{SyokakariKbn}{KmkKbn}{KmkBnri}";
                }
            }
            return null;
        }

        private string GetNormaType()
        {
            if (KmkKbn == "1")
            {
                if (new string[] { "10", "80", "90" }.Contains(KmkBnri))
                {
                    // TODO 在庫判定
                    return $"0{KmkKbn}{KmkBnri}";
                }
                else if (KmkBnri == "F1")
                {
                    // TODO 在庫判定
                    return $"0{KmkKbn}{KmkBnri}";
                }
            } 
            else if (KmkKbn == "2")
            {
                if (new string[] { "10", "20", "30", "50", "60", "70", "80", "90" }.Contains(KmkBnri))
                {
                    if (ZaikoType == null)
                    {
                        return $"02{KmkBnri}";
                    }
                    return $"02{KmkBnri}";
                }
                else if (KmkBnri == "F1")
                {
                    if (ZaikoType == null)
                    {
                        return "01F1";
                    }
                    else
                    {
                        return "01F1";
                    }
                }
                else if (KmkBnri == "F2")
                {
                    if (ZaikoType == null)
                    {
                        return "01F2";
                    }
                    else
                    {
                        return "01F2";
                    }
                }
            }
            else if (KmkKbn == "3" || KmkKbn == "4")
            {
                if (new string[] { "10", "11", "20", "30", "60", "90" }.Contains(KmkBnri))
                {
                    if (ZaikoType == null)
                    {
                        return $"0{KmkKbn}{KmkBnri}";
                    }
                    return $"0{KmkKbn}{KmkBnri}";
                }
                else if (KmkBnri == "F1")
                {
                    if (ZaikoType == null)
                    {
                        return $"0{KmkKbn}{KmkBnri}";
                    }
                    else
                    {
                        return $"0{KmkKbn}{KmkBnri}";
                    }
                }
                else if (KmkBnri == "F2")
                {
                    if (ZaikoType == null)
                    {
                        return $"0{KmkKbn}{KmkBnri}";
                    }
                    else
                    {
                        return $"0{KmkKbn}{KmkBnri}";
                    }
                }
            }
            else if (KmkKbn == "5" || KmkKbn == "6")
            {
                if (new string[] { "10", "30", "40", "90" }.Contains(KmkBnri))
                {
                    if (ZaikoType == null)
                    {
                        return $"0{KmkKbn}{KmkBnri}";
                    }
                    return $"0{KmkKbn}{KmkBnri}";
                }
            }
            else if (KmkKbn == "7")
            {
                if (new string[] { "10", "90" }.Contains(KmkBnri))
                {
                    if (ZaikoType == null)
                    {
                        return $"0{KmkKbn}{KmkBnri}";
                    }
                    return $"0{KmkKbn}{KmkBnri}";
                }
            }
            else if (KmkKbn == "9")
            {
                if (KmkBnri == "00")
                {
                    // TODO 在庫区分
                    if (ZaikoType == null)
                    {
                        return $"0{KmkKbn}{KmkBnri}";
                    }
                    return $"0{KmkKbn}{KmkBnri}";
                }
                else if (new string[] { "10", "20", "30", "50", "60", "70", "80", "90" }.Contains(KmkBnri))
                {
                    if (ZaikoType == null)
                    {
                        return $"0{KmkKbn}{KmkBnri}";
                    }
                    return $"0{KmkKbn}{KmkBnri}";
                }
            }
            return null;
        }
    }
}
