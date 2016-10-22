using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueComparer
{
    public class TypeNotSupportedException : Exception
    {
        public Type Type { get; set; }
    }
}