﻿// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Core;
using Xunit;

namespace CommandLine.Tests.Unit.Core
{
    public class TokenizerTests
    {
        [Fact]
        public void Explode_scalar_with_separator_in_odd_args_input_returns_sequence()
        {
            // Fixture setup
            var expectedTokens = new[] { Token.Name("i"), Token.Value("10"), Token.Name("string-seq"),
                Token.Value("aaa"), Token.Value("bb"),  Token.Value("cccc"), Token.Name("switch"), };
            var specs = new[] { new OptionSpecification(string.Empty, "string-seq",
                false, string.Empty, -1, -1, ',', null, typeof(IEnumerable<string>), string.Empty, string.Empty, new List<string>())};

            // Exercize system
            var result =
                Tokenizer.ExplodeOptionList(
                    StatePair.Create(
                        Enumerable.Empty<Token>().Concat(new[] { Token.Name("i"), Token.Value("10"),
                            Token.Name("string-seq"), Token.Value("aaa,bb,cccc"), Token.Name("switch") }),
                        Enumerable.Empty<Error>()),
                        optionName => NameLookup.WithSeparator(optionName, specs, StringComparer.InvariantCulture));

            // Verify outcome
            Assert.True(expectedTokens.SequenceEqual(result.Value));

            // Teardown
        }

        [Fact]
        public void Explode_scalar_with_separator_in_even_args_input_returns_sequence()
        {
            // Fixture setup
            var expectedTokens = new[] { Token.Name("x"), Token.Name("string-seq"),
                Token.Value("aaa"), Token.Value("bb"),  Token.Value("cccc"), Token.Name("switch"), };
            var specs = new[] { new OptionSpecification(string.Empty, "string-seq",
                false, string.Empty, -1, -1, ',', null, typeof(IEnumerable<string>), string.Empty, string.Empty, new List<string>())};

            // Exercize system
            var result =
                Tokenizer.ExplodeOptionList(
                    StatePair.Create(
                        Enumerable.Empty<Token>().Concat(new[] { Token.Name("x"),
                            Token.Name("string-seq"), Token.Value("aaa,bb,cccc"), Token.Name("switch") }),
                        Enumerable.Empty<Error>()),
                        optionName => NameLookup.WithSeparator(optionName, specs, StringComparer.InvariantCulture));

            // Verify outcome
            Assert.True(expectedTokens.SequenceEqual(result.Value));

            // Teardown
        }
    }
   
}
