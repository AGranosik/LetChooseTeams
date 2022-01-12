﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCT.Core.Shared.Exceptions
{
    public class InvalidFieldException : Exception
    {
        public InvalidFieldException(string field): base($@"Invalid field: {field}")
        {

        }
    }
}
