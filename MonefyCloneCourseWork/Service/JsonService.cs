using MonefyCloneCourseWork.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace MonefyCloneCourseWork.Service
{
    class JsonService : IFileService
    {
        public List<Check> Open(string fileName)
        {
            List<Check> checks = new List<Check>();
            try
            {
                DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Check>));
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    checks = jsonFormatter.ReadObject(fs) as List<Check>;
                }
                return checks;
            }
            catch(Exception)
            {
                return checks;
            }
        }

        public void Save(string fileName, List<Check> checks)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Check>));
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                jsonFormatter.WriteObject(fs, checks);
            }
        }
    }
}
