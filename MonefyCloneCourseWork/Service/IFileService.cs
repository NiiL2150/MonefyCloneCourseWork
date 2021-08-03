using MonefyCloneCourseWork.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonefyCloneCourseWork.Service
{
    public interface IFileService
    {
        List<Check> Open(string fileName);
        void Save(string fileName, List<Check> checks);
    }
}
