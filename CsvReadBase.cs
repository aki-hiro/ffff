using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace ConsoleApp1
{
    public abstract class CsvReadBase
    {
        /// <summary>
        /// カラム数
        /// </summary>
        [Ignore]
        public int ColCnt => Datas.Count();

        /// <summary>
        /// 行
        /// </summary>
        [Ignore]
        public string Line { get; set; }

        /// <summary>
        /// 行番号
        /// </summary>
        [Ignore]
        public int RowIndex { get; set; }


        /// <summary>
        /// 行情報
        /// </summary>
        [Ignore]
        public string[] Datas { get; set; }
    }
}
