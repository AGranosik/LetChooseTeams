﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCT.Core.Entites.Tournament.Exceptions
{
    public class TournamentLimitCannotBeExceededException : Exception
    {
        public TournamentLimitCannotBeExceededException() : base()
        {

        }
    }
}