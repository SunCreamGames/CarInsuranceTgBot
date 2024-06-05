using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindeePictureProcessing
{
    public class PictureProcessException : Exception
    {
        public PictureProcessException(string errorMessage) : base(errorMessage)
        {

        }
    }
}
