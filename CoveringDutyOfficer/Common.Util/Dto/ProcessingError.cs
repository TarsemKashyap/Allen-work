using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ProcessingError
    {
        public FileInfo File { get; set; }
        public int LineNumber { get; set; }
        public Exception exception { get; set; }
        public string Data { get; set; }

    }
}
