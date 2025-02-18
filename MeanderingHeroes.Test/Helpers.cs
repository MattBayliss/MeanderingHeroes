﻿using HexCore;
using LaYumba.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LaYumba.Functional.F;

namespace MeanderingHeroes.Test
{
    internal static class Helpers
    {
        

        internal static T AssertIsSome<T>(Option<T> value)
        {
            if (value == None)
            {
                throw new ArgumentException($"Expected value to be Some, but was None");
            }
            return value.AsEnumerable().Single();
        }
    }
}
