using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            var filePath = "misa1.csv";
            var csvData = FileHelper.ReadCsv<Misa>(filePath, Encoding.UTF8, false).ToList();

            foreach (var misa in csvData)
            {
                Console.WriteLine($"会社コード: {misa.KaiCode}, RowIdx: {misa.RowIndex}, Line: {misa.Line}");
            }
        }
    }
}
