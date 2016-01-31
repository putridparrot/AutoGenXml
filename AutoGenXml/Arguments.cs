using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;

namespace AutoGenXml
{
    internal class Arguments
    {
        [ArgExistingFile]
        [ArgRequired]
        [ArgDescription("The class file")]
        public string ClassFile { get; set; }

        [ArgRequired]
        [ArgDescription("The type to generate XML from")]
        public string TypeName { get; set; }
    }

}
