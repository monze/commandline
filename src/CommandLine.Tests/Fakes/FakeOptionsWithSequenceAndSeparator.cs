﻿// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithSequenceAndSeparator
    {
        [Option("long-seq", Separator=';')]
        public IEnumerable<long> LongSequence { get; set; }

        [Option('s', Min = 0, Max = 100, Separator = ',')]
        public IEnumerable<string> StringSequence { get; set; } 
    }
}
