using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAS.Padrones.Tucuman
{
    public class ExcededParsingErrorCountException : Exception
    {
        public ExcededParsingErrorCountException()
        {
        }

        public ExcededParsingErrorCountException(string message)
            : base(message)
        {
        }

        public ExcededParsingErrorCountException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
