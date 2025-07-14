using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Attributes
{
    /// <summary>
    /// CSV桁数
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvLengthAttribute : Attribute
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="intDigit">整数桁</param>
        /// <param name="deciDigit">小数桁</param>
        public CsvLengthAttribute(int maxLength, int maxDeciLength = 0)
        {
            MaxLength = maxLength;
            MaxDeciLength = maxDeciLength;
        }

        
        /// <summary>
        /// 整数桁数
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// 小数桁数
        /// </summary>
        public int MaxDeciLength { get; set; }


    }
}
