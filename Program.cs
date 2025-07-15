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
            var filePath = @"C:\work\terashima\MISA1.csv";
            var csvData = FileHelper.ReadCsv<Misa>(filePath, Encoding.UTF8, false).ToList();

            var misaData = CreateMisa(csvData.ToList());

            FileHelper.WriteCsv(misaData, @"C:\work\terashima\misaOutputShinadai.csv");

            // 諸掛処理
            var shokakariData = CreateSyokakri(csvData.ToList());
        }

        private static IEnumerable<MisaShokakariData> CreateSyokakri(List<Misa> csvData)
        {
            var group = csvData.GroupBy(d => d.DenNo);
            var syokakariList = group.SelectMany(d => d.Where(dd => dd.RecordKbn == "S1").Select(s1 => new MisaShokakariData(d.ToList(), s1)));
            return syokakariList.ToList();
        }

        private static IEnumerable<MisaDenData> CreateMisa(List<Misa> csvData)
        {
            // CSVデータを伝票単位に
            return csvData.GroupBy(d => d.DenNo).Select(g => new MisaDenData(g.ToList()));
        }
    }
}