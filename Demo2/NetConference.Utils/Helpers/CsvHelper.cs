using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetConference.Utils.Helpers
{
    public static class CsvHelper
    {
        public static IEnumerable<T> ReadCsvFileWithMapperAndHeader<T, Y>(Stream stream) where T : class where Y : ClassMap<T>
        {
            var result = new List<T>();
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            {
                using (var csvReader = new CsvReader(sr))
                {
                    csvReader.Configuration.RegisterClassMap<Y>();
                    csvReader.Configuration.HasHeaderRecord = true;
                    csvReader.Configuration.Delimiter = ";";                    

                    while (csvReader.Read())
                    {
                        result.Add(csvReader.GetRecord<T>());
                    }

                    return result;
                }
            }
        }
    }
}
