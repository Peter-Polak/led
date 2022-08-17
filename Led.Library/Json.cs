using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Library
{
    public class Json
    {
        public static T Load<T>(string fileName) where T : new()
        {
            T? obj;
            string fullFileName = $"{fileName}.json";

            if (!File.Exists(fullFileName))
            {
                File.Create(fullFileName).Close();
            }

            using (StreamReader file = new StreamReader(fullFileName))
            {
                var fileContent = file.ReadToEnd();
                obj = JsonConvert.DeserializeObject<T>(fileContent);
            }

            return obj == null ? new T() : obj;
        }

        public static void Save(object obj, string fileName)
        {
            using (StreamWriter file = new StreamWriter($"{fileName}.json"))
            {
                var json = JsonConvert.SerializeObject(obj);
                file.WriteLine(json);
            }
        }
    }
}
