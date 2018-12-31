using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortMImage.Exceptions
{
    public class NoSuchCategoryException : Exception
    {
        public NoSuchCategoryException()
        {
        }

        public NoSuchCategoryException(string message)
            :base(message)
        {
        }
    }
}
