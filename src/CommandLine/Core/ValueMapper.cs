﻿// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class ValueMapper
    {
        public static StatePair<IEnumerable<SpecificationProperty>> MapValues(
            IEnumerable<SpecificationProperty> specProps,
            IEnumerable<string> values,
            Func<IEnumerable<string>, System.Type, bool, Maybe<object>> converter)
        {
            var propAndErrors = MapValuesImpl(specProps, values, converter);

            return StatePair.Create(
                propAndErrors.Select(pe => pe.Item1),
                propAndErrors.Select(pe => pe.Item2)
                    .OfType<Just<Error>>().Select(e => e.Value)
                );
        }

        private static IEnumerable<Tuple<SpecificationProperty, Maybe<Error>>> MapValuesImpl(
            IEnumerable<SpecificationProperty> specProps,
            IEnumerable<string> values,
            Func<IEnumerable<string>, System.Type, bool, Maybe<object>> converter)
        {
            if (specProps.Empty()) // || values.Empty())
            {
                yield break;
            }
            var pt = specProps.First();
            var taken = values.Take(pt.Specification.GetMaxValueCount().Return(n => n, values.Count()));
            if (taken.Empty())
            {
                yield return
                    Tuple.Create(pt, MakeErrorInCaseOfMinConstraint(pt.Specification));
                yield break;
            }

            var next = specProps.Skip(1).FirstOrDefault(s => s.Specification.IsValue()).ToMaybe();
            if (!pt.Specification.IsMaxNotSpecified()
                && next.IsNothing()
                && values.Skip(taken.Count()).Any())
            {
                yield return
                    Tuple.Create<SpecificationProperty, Maybe<Error>>(
                        pt, Maybe.Just<Error>(new SequenceOutOfRangeError(NameInfo.EmptyName)));
                yield break;
            }

            yield return
                converter(taken, pt.Property.PropertyType, pt.Specification.ConversionType.IsScalar())
                    .Return(
                        converted => Tuple.Create(pt.WithValue(Maybe.Just(converted)), Maybe.Nothing<Error>()),
                        Tuple.Create<SpecificationProperty, Maybe<Error>>(
                            pt, Maybe.Just<Error>(new BadFormatConversionError(NameInfo.EmptyName))));
         
            foreach (var value in MapValuesImpl(specProps.Skip(1), values.Skip(taken.Count()), converter))
            {
                yield return value;
            }
        }

        private static Maybe<Error> MakeErrorInCaseOfMinConstraint(Specification specification)
        {
            return !specification.IsMinNotSpecified()
                ? Maybe.Just<Error>(new SequenceOutOfRangeError(NameInfo.EmptyName))
                : Maybe.Nothing<Error>();
        }
    }
}