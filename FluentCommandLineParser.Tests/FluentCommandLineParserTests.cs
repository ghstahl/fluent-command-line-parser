﻿#region License
// FluentCommandLineParserTests.cs
// Copyright (c) 2013, Simon Williams
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provide
// d that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the
// following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
// the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Fclp.Internals;
using Fclp.Internals.Errors;
using Fclp.Tests.FluentCommandLineParser;
using Moq;
using NUnit.Framework;

namespace Fclp.Tests
{
    /// <summary>
    /// Contains unit tests for the <see cref="FluentCommandLineParser"/> class.
    /// </summary>
    [TestFixture]
    public class FluentCommandLineParserTests
    {
        #region HelperMethods

        /// <summary>
        /// Helper method to return the parser as its interface
        /// </summary>
        static IFluentCommandLineParser CreateFluentParser()
        {
            return new Fclp.FluentCommandLineParser();
        }

        static void CallParserWithAllKeyVariations(IFluentCommandLineParser parser, string key, string value, Action<string[], ICommandLineParserResult> assertCallback)
        {
            foreach (string[] args in CreateAllKeyVariations(key, value))
                assertCallback(args, parser.Parse(args));
        }

        static IEnumerable<string[]> CreateAllKeyVariations(string key, string value)
        {
            var keys = new[] { "-", "--", "/" };
            var valueIdentifiers = new[] { '=', ':' };

            foreach (string k in keys)
            {
                foreach (char valueIdentifier in valueIdentifiers)
                {
                    yield return new[] { k + key + valueIdentifier + value };
                }

                yield return new[] { k + key, value };
            }
        }

        static string FormatArgs(string[] args)
        {
            return "Executed with args: " + string.Join(" ", args);
        }

        static void RunTest<T>(string value, T expected)
        {
            var parser = CreateFluentParser();
            T actual = default(T);

            parser.Setup<T>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS, "long")
                .Callback(val =>
                {
                    actual = val;
                });

            var assert = new Action<string[], ICommandLineParserResult>((args, result) =>
            {
                string msg = FormatArgs(args);
                Assert.AreEqual(expected, actual, msg);
                Assert.IsFalse(result.HasErrors, msg);
                Assert.IsFalse(result.Errors.Any(), msg);
            });

            CallParserWithAllKeyVariations(parser, WellKnownOptionNames.LittleS, value, assert);
            CallParserWithAllKeyVariations(parser, "long", value, assert);
        }

        #endregion

        #region Description Tests

        [Test]
        public void Ensure_Description_Can_Be_Set()
        {
            var parser = CreateFluentParser();

            const string expected = "my description";

            var cmdOption = parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS).WithDescription(expected);

            var actual = ((ICommandLineOption)cmdOption).Description;

            Assert.AreSame(expected, actual);
        }

        #endregion Description Tests

        #region Top Level Tests

        #region String Option

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_String_When_Using_Short_option()
        {
            const string expected = "my-expected-string";
            RunTest(expected, expected);
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_String_When_Using_Long_option()
        {
            const string expected = "my-expected-string";
            const string key = "string";
            string actual = null;

            var parser = CreateFluentParser();

            parser
                .Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS, key)
                .Callback(val => actual = val);

            CallParserWithAllKeyVariations(parser, key, expected, (args, result) =>
            {
                string msg = "Executed with args: " + FormatArgs(args);
                Assert.AreEqual(expected, actual, msg);
                Assert.IsFalse(result.HasErrors, msg);
                Assert.IsFalse(result.Errors.Any(), msg);
            });
        }

        #endregion String Option

        #region Int32 Option

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Int32_When_Using_Short_option()
        {
            const int expected = int.MaxValue;
            RunTest(expected.ToString(CultureInfo.InvariantCulture), expected);
            //const string shortKey = WellKnownOptionNames.LittleI;
            //int actual = default(int);

            //var parser = CreateFluentParser();

            //parser
            //    .Setup<int>(shortKey)
            //    .Callback(val => actual = val);

            //CallParserWithAllKeyVariations(parser, shortKey, expected.ToString(CultureInfo.InvariantCulture), (args, result) =>
            //{
            //    string msg = "Executed with args: " + FormatArgs(args);
            //    Assert.AreEqual(expected, actual, msg);
            //    Assert.IsFalse(result.HasErrors, msg);
            //    Assert.IsFalse(result.Errors.Any(), msg);
            //});
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Int32_When_Using_Long_option()
        {
            const int expected = int.MaxValue;
            const string shortKey = WellKnownOptionNames.LittleI;
            const string longKey = "int32";
            int actual = default(int);

            var parser = CreateFluentParser();

            parser
                .Setup<int>(CaseType.CaseInsensitive, shortKey, longKey)
                .Callback(val => actual = val);

            CallParserWithAllKeyVariations(parser, longKey, expected.ToString(CultureInfo.InvariantCulture), (args, result) =>
            {
                string msg = "Executed with args: " + FormatArgs(args);
                Assert.AreEqual(expected, actual, msg);
                Assert.IsFalse(result.HasErrors, msg);
                Assert.IsFalse(result.Errors.Any(), msg);
            });
        }

        [Test]
        public void Ensure_Negative_Integer_Can_Be_Specified_With_Unix_Style()
        {
            var parser = CreateFluentParser();

            int actual = 0;

            parser.Setup<int>(CaseType.CaseInsensitive, "integer")
                  .Callback(i => actual = i);

            var result = parser.Parse(new[] { "--integer", "--", "-123" });

            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.EmptyArgs);
            Assert.IsFalse(result.HelpCalled);

            Assert.AreEqual(-123, actual);
        }

        #endregion Int32 Option

        #region Double Option

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Double_When_Using_Short_option()
        {
            const double expected = 1.23456789d;
            RunTest(expected.ToString(CultureInfo.InvariantCulture), expected);
            //const string shortKey = WellKnownOptionNames.LittleD;
            //double actual = default(double);

            //var parser = CreateFluentParser();

            //parser
            //    .Setup<double>(shortKey)
            //    .Callback(val => actual = val);

            //CallParserWithAllKeyVariations(parser, shortKey, expected.ToString(CultureInfo.InvariantCulture), (args, result) =>
            //{
            //    Assert.AreEqual(expected, actual, FormatArgs(args));
            //    Assert.IsFalse(result.HasErrors, FormatArgs(args));
            //    Assert.IsFalse(result.Errors.Any(), FormatArgs(args));
            //});
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Double_When_Using_Long_option()
        {
            const double expected = 1.23456789d;
            const string shortKey = WellKnownOptionNames.LittleD;
            const string longKey = "double";
            double actual = default(double);

            var parser = CreateFluentParser();

            parser
                .Setup<double>(CaseType.CaseInsensitive, shortKey, longKey)
                .Callback(val => actual = val);

            CallParserWithAllKeyVariations(parser, longKey, expected.ToString(CultureInfo.InvariantCulture), (args, result) =>
            {
                Assert.AreEqual(expected, actual, FormatArgs(args));
                Assert.IsFalse(result.HasErrors, FormatArgs(args));
                Assert.IsFalse(result.Errors.Any(), FormatArgs(args));
            });
        }

        [Test]
        public void Ensure_Negative_Double_Can_Be_Specified_With_Unix_Style()
        {
            var parser = CreateFluentParser();

            double actual = 0;

            parser.Setup<double>(CaseType.CaseInsensitive, "double")
                  .Callback(i => actual = i);

            var result = parser.Parse(new[] { "--double", "--", "-123.456" });

            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.EmptyArgs);
            Assert.IsFalse(result.HelpCalled);

            Assert.AreEqual(-123.456, actual);
        }

        #endregion Double Option

        #region Enum Option

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Enum_When_Using_Short_option()
        {
            const TestEnum expected = TestEnum.Value1;

            TestEnum actual = TestEnum.Value0;

            var parser = CreateFluentParser();

            parser
                .Setup<TestEnum>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleE)
                .Callback(val => actual = val);

            parser.Parse(new[] { "-e", expected.ToString() });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Enum_When_Using_Long_option()
        {
            const TestEnum expected = TestEnum.Value1;

            TestEnum actual = TestEnum.Value0;

            var parser = CreateFluentParser();

            parser
                .Setup<TestEnum>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleE, "enum")
                .Callback(val => actual = val);

            parser.Parse(new[] { "--enum", expected.ToString() });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Enum_When_Using_Short_option_And_Int32_Enum()
        {
            const TestEnum expected = TestEnum.Value1;

            TestEnum actual = TestEnum.Value0;

            var parser = CreateFluentParser();

            parser
                .Setup<TestEnum>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleE)
                .Callback(val => actual = val);

            parser.Parse(new[] { "-e", ((int)expected).ToString(CultureInfo.InvariantCulture) });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Enum_When_Using_Long_option_And_Int32_Enum()
        {
            const TestEnum expected = TestEnum.Value1;

            TestEnum actual = TestEnum.Value0;

            var parser = CreateFluentParser();

            parser
                .Setup<TestEnum>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleE, "enum")
                .Callback(val => actual = val);

            parser.Parse(new[] { "--enum", ((int)expected).ToString(CultureInfo.InvariantCulture) });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Enum_When_Using_Short_option_And_Lowercase_String()
        {
            const TestEnum expected = TestEnum.Value1;

            TestEnum actual = TestEnum.Value0;

            var parser = CreateFluentParser();

            parser
                .Setup<TestEnum>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleE)
                .Callback(val => actual = val);

            parser.Parse(new[] { "-e", expected.ToString().ToLowerInvariant() });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Enum_When_Using_Long_option_And_Int32_Enum_And_Lowercase_String()
        {
            const TestEnum expected = TestEnum.Value1;

            TestEnum actual = TestEnum.Value0;

            var parser = CreateFluentParser();

            parser
                .Setup<TestEnum>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleE, "enum")
                .Callback(val => actual = val);

            parser.Parse(new[] { "--enum", expected.ToString().ToLowerInvariant() });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Enum_When_Using_Short_option_And_Uppercase_String()
        {
            const TestEnum expected = TestEnum.Value1;

            TestEnum actual = TestEnum.Value0;

            var parser = CreateFluentParser();

            parser
                .Setup<TestEnum>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleE)
                .Callback(val => actual = val);

            parser.Parse(new[] { "-e", expected.ToString().ToUpperInvariant() });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Enum_When_Using_Long_option_And_Int32_Enum_And_Uppercase_String()
        {
            const TestEnum expected = TestEnum.Value1;

            TestEnum actual = TestEnum.Value0;

            var parser = CreateFluentParser();

            parser
                .Setup<TestEnum>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleE, "enum")
                .Callback(val => actual = val);

            parser.Parse(new[] { "--enum", expected.ToString().ToUpperInvariant() });

            Assert.AreEqual(expected, actual);
        }

        #region Enum Flags Option

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_EnumFlag_When_Using_Short_option()
        {
            const TestEnumFlag expected = TestEnumFlag.Value1;

            var actual = TestEnumFlag.Value0;

            var parser = CreateFluentParser();

            parser
                .Setup<TestEnumFlag>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleE)
                .Callback(val => actual = val);

            parser.Parse(new[] { "-e", expected.ToString() });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_EnumFlag_When_Using_Short_option_And_A_List()
        {
            var actual = TestEnumFlag.Value0;

            var parser = CreateFluentParser();

            parser
                .Setup<TestEnumFlag>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleE)
                .Callback(val => actual = val);

            parser.Parse(new[] { "-e", TestEnumFlag.Value1.ToString(), TestEnumFlag.Value2.ToString() });

            Assert.AreEqual(3, (int)actual);
            Assert.IsTrue(actual.HasFlag(TestEnumFlag.Value1));
            Assert.IsTrue(actual.HasFlag(TestEnumFlag.Value2));
            Assert.IsFalse(actual.HasFlag(TestEnumFlag.Value64));
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_EnumFlag_When_Using_Short_option_And_A_List_With_0()
        {
            var actual = TestEnumFlag.Value0;

            var parser = CreateFluentParser();

            parser
                .Setup<TestEnumFlag>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleE)
                .Callback(val => actual = val);

            parser.Parse(new[] { "-e", TestEnumFlag.Value1.ToString(), TestEnumFlag.Value2.ToString(), TestEnumFlag.Value0.ToString(), TestEnumFlag.Value64.ToString() });

            Assert.AreEqual(67, (int)actual);
            Assert.IsTrue(actual.HasFlag(TestEnumFlag.Value1));
            Assert.IsTrue(actual.HasFlag(TestEnumFlag.Value2));
            Assert.IsTrue(actual.HasFlag(TestEnumFlag.Value64));
            Assert.IsTrue(actual.HasFlag(TestEnumFlag.Value0));
            Assert.IsFalse(actual.HasFlag(TestEnumFlag.Value8));
            Assert.IsFalse(actual.HasFlag(TestEnumFlag.Value32));
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_EnumFlag_When_Using_Short_option_And_A_List_Of_String_Values()
        {
            var args = new[] { "--direction", "South", "East" };

            var actual = Direction.North;

            var p = CreateFluentParser();

            p.Setup<Direction>(CaseType.CaseInsensitive, "direction")
             .Callback(d => actual = d);

            p.Parse(args);

            Assert.IsFalse(actual.HasFlag(Direction.North));
            Assert.IsTrue(actual.HasFlag(Direction.East));
            Assert.IsTrue(actual.HasFlag(Direction.South));
            Assert.IsFalse(actual.HasFlag(Direction.West));
        }

        [Flags]
        public enum Direction
        {
            North = 1,
            East = 2,
            South = 4,
            West = 8,
        }

        #endregion Enum Flags Option

        #endregion Enum Option

        #region DateTime Option

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_DateTime_When_Using_Short_option()
        {
            var expected = new DateTime(2012, 2, 29, 01, 01, 01);
            RunTest(expected.ToString("yyy-MM-ddThh:mm:ss", CultureInfo.InvariantCulture), expected);
            //DateTime actual = default(DateTime);

            //var parser = CreateFluentParser();

            //parser
            //    .Setup<DateTime>("dt")
            //    .Callback(val => actual = val);

            //var result = parser.Parse(new[] { "-dt", expected.ToString("yyyy-MM-ddThh:mm:ss", CultureInfo.CurrentCulture) });

            //Assert.AreEqual(expected, actual);
            //Assert.IsFalse(result.HasErrors);
            //Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_DateTime_When_Using_Long_option()
        {
            var expected = new DateTime(2012, 2, 29, 01, 01, 01);

            DateTime actual = default(DateTime);

            var parser = CreateFluentParser();

            parser
                .Setup<DateTime>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleD, "datetime")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--datetime", expected.ToString("yyyy-MM-ddThh:mm:ss", CultureInfo.CurrentCulture) });

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        #endregion DateTime Option

        #region int? Option

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Nullable_Int32_When_Valid_Value_Is_Provided()
        {
            int? expected = 1;
            int? actual = null;

            var parser = CreateFluentParser();

            parser.Setup<int?>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleI, "integer")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] {"--integer", "1"});

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Nullable_Int32_When_InValid_Value_Is_Provided()
        {
            int? expected = null;
            int? actual = null;

            var parser = CreateFluentParser();

            parser.Setup<int?>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleI, "integer")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] {"--integer", "abc"});

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Nullable_Int32_When_Null_Is_Provided()
        {
            int? expected = null;
            int? actual = null;

            var parser = CreateFluentParser();

            parser.Setup<int?>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleI, "integer")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] {"--integer"} );

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        #endregion

        #region double? Option

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Nullable_Double_When_Valid_Value_Is_Provided()
        {
            double? expected = 1.23456789d;
            double? actual = null;

            var parser = CreateFluentParser();

            parser.Setup<double?>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleD, "double")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--double", expected.Value.ToString(CultureInfo.InvariantCulture) });

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Nullable_Double_When_InValid_Value_Is_Provided()
        {
            double? expected = null;
            double? actual = null;

            var parser = CreateFluentParser();

            parser.Setup<double?>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleD, "double")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--double", "not-a-double" });

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Nullable_Double_When_No_Value_Is_Provided()
        {
            double? expected = null;
            double? actual = null;

            var parser = CreateFluentParser();

            parser.Setup<double?>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleD, "double")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--double" });

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        #endregion

        #region DateTime? Option

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Nullable_DateTime_When_Valid_Value_Is_Provided()
        {
            DateTime? expected = new DateTime(2012, 2, 29, 01, 01, 01);
            DateTime? actual = null;

            var parser = CreateFluentParser();

            parser
                .Setup<DateTime?>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleD, "datetime")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--datetime", expected.Value.ToString("yyyy-MM-ddThh:mm:ss", CultureInfo.CurrentCulture) });

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Nullable_DateTime_When_InValid_Value_Is_Provided()
        {
            DateTime? expected = null;
            DateTime? actual = null;

            var parser = CreateFluentParser();

            parser
                .Setup<DateTime?>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleD, "datetime")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--datetime", "not-a-date-time" });

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Nullable_DateTime_When_No_Value_Is_Provided()
        {
            DateTime? expected = null;
            DateTime? actual = null;

            var parser = CreateFluentParser();

            parser
                .Setup<DateTime?>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleD, "datetime")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--datetime" });

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        #endregion

        #region bool? Option

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Nullable_Bool_When_Valid_Value_Is_Provided()
        {
            bool? expected = true;
            bool? actual = null;

            var parser = CreateFluentParser();

            parser.Setup<bool?>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleB, "bool")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--bool", "true" });

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Nullable_Bool_When_InValid_Value_Is_Provided()
        {
            bool? expected = null;
            bool? actual = null;

            var parser = CreateFluentParser();

            parser.Setup<bool?>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleB, "bool")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--bool", "not-a-bool" });

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Nullable_Bool_When_No_Value_Is_Provided()
        {
            bool? expected = null;
            bool? actual = null;

            var parser = CreateFluentParser();

            parser.Setup<bool?>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleB, "bool")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--bool" });

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        #endregion

        #region Uri Option

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Uri_When_Valid_Value_Is_Provided()
        {
            const string expected = "https://github.com/fclp/fluent-command-line-parser";
            Uri actual = null;

            var parser = CreateFluentParser();

            parser.Setup<Uri>(CaseType.CaseInsensitive, "u", "uri")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--uri", expected });

            Assert.AreEqual(expected, actual.AbsoluteUri);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Uri_When_InValid_Value_Is_Provided()
        {
            Uri actual = null;

            var parser = CreateFluentParser();

            parser.Setup<Uri>(CaseType.CaseInsensitive, "u", "uri")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--uri", "not-a-uri" });

            Assert.IsNull(actual);
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual(result.Errors.Count(), 1);
        }

        [Test]
        public void Ensure_Parser_Calls_The_Callback_With_Expected_Uri_When_No_Value_Is_Provided()
        {
            Uri actual = null;

            var parser = CreateFluentParser();

            parser.Setup<Uri>(CaseType.CaseInsensitive, "u", "uri")
                .Callback(val => actual = val);

            var result = parser.Parse(new[] { "--uri" });

            Assert.IsNull(actual);
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual(result.Errors.Count(), 1);
        }

        #endregion

        #region Long Option Only

        [Test]
        public void Can_have_long_option_only()
        {
            var parser = CreateFluentParser();
            var s = "";

            parser.Setup<string>(CaseType.CaseInsensitive, "my-feature")
                  .Callback(val => s = val);

            var result = parser.Parse(new[] { "--my-feature", "somevalue" });

            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.EmptyArgs);
            Assert.IsFalse(result.HelpCalled);

            Assert.AreEqual("somevalue", s);
        }

        [Test]
        public void Can_have_single_character_long_option()
        {
            var parser = CreateFluentParser();
            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS);
        }

        #endregion

        #region Required

        [Test]
        public void Ensure_Expected_Error_Is_Returned_If_A_Option_Is_Required_And_Null_Args_Are_Specified()
        {
            var parser = CreateFluentParser();

            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS)
                .Required();

            var result = parser.Parse(null);

            Assert.IsTrue(result.HasErrors);

            Assert.AreEqual(1, result.Errors.Count());

            Assert.IsInstanceOf(typeof(ExpectedOptionNotFoundParseError), result.Errors.First());
        }

        [Test]
        public void Ensure_Expected_Error_Is_Returned_If_A_Option_Is_Required_And_Empty_Args_Are_Specified()
        {
            var parser = CreateFluentParser();

            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS)
                .Required();

            var result = parser.Parse(new string[0]);

            Assert.IsTrue(result.HasErrors);

            Assert.AreEqual(1, result.Errors.Count());

            Assert.IsInstanceOf(typeof(ExpectedOptionNotFoundParseError), result.Errors.First());
        }

        [Test]
        public void Ensure_Expected_Error_Is_Returned_If_Required_Option_Is_Provided()
        {
            var parser = CreateFluentParser();

            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS)
                .Required();

            var result = parser.Parse(new[] { "-d" });

            Assert.IsTrue(result.HasErrors);

            Assert.AreEqual(1, result.Errors.Count());

            Assert.IsInstanceOf(typeof(ExpectedOptionNotFoundParseError), result.Errors.First());
        }

        [Test]
        public void Ensure_No_Error_Returned_If_Required_Option_Is_Not_Provided()
        {
            var parser = CreateFluentParser();

            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS);

            var result = parser.Parse(new[] { "-d" });

            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        [ExpectedException(typeof(OptionAlreadyExistsException))]
        public void Ensure_Expected_Exception_Thrown_If_Adding_A_Option_With_A_ShortName_Which_Has_Already_Been_Setup()
        {
            var parser = CreateFluentParser();

            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS, "string");

            parser.Setup<int>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS, "int32");
        }

        [Test]
        [ExpectedException(typeof(OptionAlreadyExistsException))]
        public void Ensure_Expected_Exception_Thrown_If_Adding_A_Option_With_A_ShortName_And_LongName_Which_Has_Already_Been_Setup()
        {
            var parser = CreateFluentParser();

            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS, "string");

            parser.Setup<int>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS, "string");
        }

        [Test]
        [ExpectedException(typeof(OptionAlreadyExistsException))]
        public void Ensure_Expected_Exception_Thrown_If_Adding_A_Option_With_A_LongName_Which_Has_Already_Been_Setup()
        {
            var parser = CreateFluentParser();

            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS, "string");

            parser.Setup<int>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleI, "string");
        }

        #endregion

        #region Default

        [Test]
        public void Ensure_Default_Value_Returned_If_No_Value_Specified()
        {
            var parser = CreateFluentParser();

            const string expected = "my expected value";
            string actual = null;

            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS)
                .Callback(s => actual = s)
                .SetDefault(expected);

            var result = parser.Parse(new[] { "-s" });

            Assert.AreSame(expected, actual);
            Assert.IsTrue(result.HasErrors);
        }

        [Test]
        public void Ensure_Default_Value_Returned_If_No_Option_Or_Value_Specified()
        {
            var parser = CreateFluentParser();

            const string expected = "my expected value";
            string actual = null;

            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS)
                .Callback(s => actual = s)
                .SetDefault(expected);

            var result = parser.Parse(new string[0]);

            Assert.AreSame(expected, actual);
            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());
        }

        #endregion

        #region No Args

        [Test]
        public void Ensure_Can_Specify_Empty_Args()
        {
            var parser = CreateFluentParser();

            var result = parser.Parse(new string[0]);

            Assert.IsFalse(result.HasErrors);
            Assert.IsTrue(result.EmptyArgs);
            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public void Ensure_Can_Specify_Null_Args()
        {
            var parser = CreateFluentParser();

            var result = parser.Parse(null);

            Assert.IsFalse(result.HasErrors);
            Assert.IsTrue(result.EmptyArgs);
            Assert.IsFalse(result.Errors.Any());
        }

        [Test]
        public void Ensure_Defaults_Are_Called_When_Empty_Args_Specified()
        {
            var parser = CreateFluentParser();

            const int expectedInt = 123;
            const double expectedDouble = 123.456;
            const string expectedString = "my string";
            const bool expectedBool = true;

            int actualInt = 0;
            double actualDouble = 0;
            string actualString = null;
            bool actualBool = false;

            parser.Setup<int>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleI).Callback(i => actualInt = i).SetDefault(expectedInt);
            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS).Callback(s => actualString = s).SetDefault(expectedString);
            parser.Setup<bool>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleB).Callback(b => actualBool = b).SetDefault(expectedBool);
            parser.Setup<double>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleD).Callback(d => actualDouble = d).SetDefault(expectedDouble);

            var result = parser.Parse(null);

            Assert.IsFalse(result.HasErrors);
            Assert.IsTrue(result.EmptyArgs);
            Assert.AreEqual(expectedInt, actualInt);
            Assert.AreEqual(expectedDouble, actualDouble);
            Assert.AreEqual(expectedString, actualString);
            Assert.AreEqual(expectedBool, actualBool);
        }

        #endregion No Args

        #region Example

        [Test]
        public void Ensure_Example_Works_As_Expected()
        {
            const int expectedRecordId = 10;
            const string expectedValue = "Mr. Smith";
            const bool expectedSilentMode = true;
            const bool expectedSwitchA = true;
            const bool expectedSwitchB = true;
            const bool expectedSwitchC = false;
            const bool expectedSwitchLittleD = true;
            const bool expectedSwitchBigD = true;

            var args = new[] { "-r", expectedRecordId.ToString(CultureInfo.InvariantCulture)
                , "-v", "\"Mr. Smith\"", "--silent", "-ab", "-c-", "-d", "-D" };

            var recordId = 0;
            string newValue = null;
            var inSilentMode = false;
            var switchA = false;
            var switchB = false;
            var switchC = true;
            var switchLittleD = false;
            var switchBigD = false;

            var parser = CreateFluentParser();

            parser.Setup<bool>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleA)
                  .Callback(value => switchA = value);

            parser.Setup<bool>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleB)
                  .Callback(value => switchB = value);

            parser.Setup<bool>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleC)
                  .Callback(value => switchC = value);

            // create a new Option using a short and long name
            parser.Setup<int>(CaseType.CaseInsensitive, "r", "record")
                .AddCaseSensitiveOption("BigR")
                .WithDescription("The record id to update (required)")
                .Callback(record => recordId = record)
                // use callback to assign the record value to the local RecordID property
                .Required(); // fail if this Option is not provided in the arguments

            parser.Setup<bool>(CaseType.CaseInsensitive, "silent")
                  .WithDescription("Execute the update in silent mode without feedback (default is false)")
                  .Callback(silent => inSilentMode = silent)
                  .SetDefault(false); // explicitly set the default value to use if this Option is not specified in the arguments


            parser.Setup<string>(CaseType.CaseInsensitive, "v", "value")
                    .WithDescription("The new value for the record (required)") // used when help is requested e.g -? or --help 
                    .Callback(value => newValue = value)
                    .Required();

            parser.Setup<bool>(CaseType.CaseSensitive,"d", "dvalue")
                .WithDescription("The new value for the record (required)")
                // used when help is requested e.g -? or --help 
                .Callback(value => switchLittleD = value)
                .SetDefault(false);

            parser.Setup<bool>(CaseType.CaseSensitive,"D", "DVALUE")
                .WithDescription("The new value for the record (required)")
                // used when help is requested e.g -? or --help 
                .Callback(value => switchBigD = value)
                .SetDefault(false);


            // do the work
            ICommandLineParserResult result = parser.Parse(args);

            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.Errors.Any());

            Assert.AreEqual(expectedRecordId, recordId);
            Assert.AreEqual(expectedValue, newValue);
            Assert.AreEqual(expectedSilentMode, inSilentMode);
            Assert.AreEqual(expectedSwitchA, switchA);
            Assert.AreEqual(expectedSwitchB, switchB);
            Assert.AreEqual(expectedSwitchC, switchC);
            Assert.AreEqual(expectedSwitchBigD, switchBigD);
            Assert.AreEqual(expectedSwitchLittleD, switchLittleD);
        }

        #endregion

        #region Setup Help

        [Test]
        public void Setup_Help_And_Ensure_It_Is_Called_With_Custom_Formatter()
        {
            var parser = new Fclp.FluentCommandLineParser();

            var formatter = new Mock<ICommandLineOptionFormatter>();

            var args = new[] { "/help", WellKnownOptionNames.LittleI, WellKnownOptionNames.LittleS };
            const string expectedCallbackResult = "blah";
            string callbackResult = null;

            parser.SetupHelp("?", "HELP", "h")
                    .Callback(s => callbackResult = s)
                    .WithCustomFormatter(formatter.Object);

            parser.Setup<int>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleI);
            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS);

            formatter.Setup(x => x.Format(parser.Options)).Returns(expectedCallbackResult);

            var result = parser.Parse(args);

            Assert.AreSame(expectedCallbackResult, callbackResult);
            Assert.IsFalse(result.HasErrors);
            Assert.IsTrue(result.HelpCalled);
        }

        [Test]
        public void Setup_Help_And_Ensure_It_Is_Called()
        {
            var parser = new Fclp.FluentCommandLineParser();

            var formatter = new Mock<ICommandLineOptionFormatter>();

            var args = new[] { "/help", WellKnownOptionNames.LittleI, WellKnownOptionNames.LittleS };
            const string expectedCallbackResult = "blah";
            bool wasCalled = false;

            parser.SetupHelp("?", "HELP", "h")
                    .Callback(() => wasCalled = true);

            parser.Setup<int>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleI);
            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS);

            formatter.Setup(x => x.Format(parser.Options)).Returns(expectedCallbackResult);

            var result = parser.Parse(args);

            Assert.IsTrue(wasCalled);
            Assert.IsFalse(result.HasErrors);
            Assert.IsTrue(result.HelpCalled);
        }

        [Test]
        public void Setup_Help_With_Symbol()
        {
            var parser = CreateFluentParser();

            string callbackResult = null;

            parser.SetupHelp("?").Callback(s => callbackResult = s);

            var args = new[] { "-?" };

            var result = parser.Parse(args);

            Assert.IsTrue(result.HelpCalled);
            Assert.IsNotNullOrEmpty(callbackResult);
        }

        [Test]
        public void Setup_Help_And_Ensure_It_Can_Be_Called_Manually()
        {
            var parser = CreateFluentParser();

            string callbackResult = null;

            parser.SetupHelp("?").Callback(s => callbackResult = s);

            parser.HelpOption.ShowHelp(parser.Options);

            Assert.IsNotNullOrEmpty(callbackResult);           
        }

        [Test]
        public void Generic_Setup_Help_And_Ensure_It_Can_Be_Called_Manually()
        {
            var parser = new FluentCommandLineParser<TestApplicationArgs>();
            parser.Setup(arg => arg.NewValue)
                .As(CaseType.CaseInsensitive, "v", "ValUe");
            string callbackResult = null;
            parser.Setup(arg => arg.Silent)
                .As(CaseType.CaseInsensitive, "s", "Silent");

            parser.SetupHelp("?").Callback(s => callbackResult = s);

            parser.HelpOption.ShowHelp(parser.Options);

            string[] args = new[] {"-V", "The Value","-s"};
            parser.Parse(args);

            Assert.IsTrue(System.String.CompareOrdinal("The Value", parser.Object.NewValue)==0);
            Assert.IsTrue(parser.Object.Silent);
            Assert.IsNotNullOrEmpty(callbackResult);           
        }

        #endregion

        #region Case Sensitive

        [Test]
        public void Ensure_Short_Options_Ignore_Case_When_Disabled()
        {
            var parser = CreateFluentParser();

            const string expectedValue = "expected value";

            string actualValue = null;

            parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.LittleS).Callback(str => actualValue = str).Required();

            var result = parser.Parse(new[] { "--S", expectedValue });

            Assert.IsFalse(result.HasErrors);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Ensure_Long_Options_Ignore_Case_When_Disabled()
        {
            var parser = CreateFluentParser();
 
            const string expectedValue = "expected value";

            string actualValue = null;

            parser.Setup<string>(CaseType.CaseInsensitive, "longoption").Callback(str => actualValue = str).Required();

            var result = parser.Parse(new[] { "--LONGOPTION", expectedValue });

            Assert.IsFalse(result.HasErrors);
            Assert.AreEqual(expectedValue, actualValue);
        }

        #endregion


        #region Addtional Arguments

        [Test]
        public void Ensure_Additional_Arguments_Callback_Called_When_Additional_Args_Provided()
        {
            var parser = CreateFluentParser();

            var capturedAdditionalArgs = new List<string>();

            parser.Setup<string>(CaseType.CaseInsensitive, "my-option")
                  .CaptureAdditionalArguments(capturedAdditionalArgs.AddRange);

            var result = parser.Parse(new[] { "--my-option", "value", "--", "addArg1", "addArg2" });

            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.EmptyArgs);
            Assert.IsFalse(result.HelpCalled);

            Assert.AreEqual(2, capturedAdditionalArgs.Count());
            Assert.IsTrue(capturedAdditionalArgs.Contains("addArg1"));
            Assert.IsTrue(capturedAdditionalArgs.Contains("addArg2"));
        }

        [Test]
        public void Ensure_Additional_Arguments_Callback_Not_Called_When_No_Additional_Args_Provided()
        {
            var parser = CreateFluentParser();

            bool wasCalled = false;

            parser.Setup<string>(CaseType.CaseInsensitive, "my-option")
                  .CaptureAdditionalArguments(addArgs => wasCalled = true);

            var result = parser.Parse(new[] { "--my-option", "value" });

            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.EmptyArgs);
            Assert.IsFalse(result.HelpCalled);

            Assert.IsFalse(wasCalled);
        }

        [Test]
        public void Ensure_Additional_Arguments_Callback_Not_Called_When_No_Additional_Args_Follow_A_Double_Dash()
        {
            var parser = CreateFluentParser();

            bool wasCalled = false;

            parser.Setup<string>(CaseType.CaseInsensitive, "my-option")
                  .CaptureAdditionalArguments(addArgs => wasCalled = true);

            var result = parser.Parse(new[] { "--my-option", "value", "--" });

            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.EmptyArgs);
            Assert.IsFalse(result.HelpCalled);

            Assert.IsFalse(wasCalled);
        }

        [Test]
        public void Ensure_Stable_When_Additional_Args_Are_Provided_But_Capture_Additional_Arguments_Has_Not_Been_Setup()
        {
            var parser = CreateFluentParser();
            parser.Setup<string>(CaseType.CaseInsensitive, new[] { "my-option" });

            var result = parser.Parse(new[] { "--my-option", "value", "--", "addArg1", "addArg2" });

            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.EmptyArgs);
            Assert.IsFalse(result.HelpCalled);
        }

        [Test]
        public void Ensure_Additional_Args_Can_Be_Captured_For_Different_Options()
        {
            var parser = CreateFluentParser();

            var option1AddArgs = new List<string>();
            var option2AddArgs = new List<string>();

            string option1Value = null;
            string option2Value = null;
            parser.Setup<string>(CaseType.CaseInsensitive, new[] { "option-one" })
                  .Callback(s => option1Value = s)
                  .CaptureAdditionalArguments(option1AddArgs.AddRange);

            parser.Setup<string>(CaseType.CaseInsensitive, new[] { "option-two" })
                  .Callback(s => option2Value = s)
                  .CaptureAdditionalArguments(option2AddArgs.AddRange);

            var result = parser.Parse(new[] { "--option-one", "value-one", "addArg1", "addArg2", "--option-two", "value-two", "addArg3", "addArg4" });

            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.EmptyArgs);
            Assert.IsFalse(result.HelpCalled);

            Assert.AreEqual("value-one", option1Value);
            Assert.AreEqual("value-two", option2Value);

            Assert.AreEqual(2, option1AddArgs.Count());
            Assert.IsTrue(option1AddArgs.Contains("addArg1"));
            Assert.IsTrue(option1AddArgs.Contains("addArg2"));

            Assert.AreEqual(2, option2AddArgs.Count());
            Assert.IsTrue(option2AddArgs.Contains("addArg3"));
            Assert.IsTrue(option2AddArgs.Contains("addArg4"));
        }

        #endregion

        #region Lists

        [Test]
        public void Ensure_Can_Parse_Mulitple_Arguments_Containing_Negative_Integers_To_A_List()
        {
            var parser = CreateFluentParser();

            var actual = new List<int>();
            parser.Setup<List<int>>(CaseType.CaseInsensitive, new[] { "integers" })
                  .Callback(actual.AddRange);

            var result = parser.Parse(new[] { "--integers", "--", "123", "-123", "-321", "321" });

            Assert.IsFalse(result.HasErrors);
            Assert.IsFalse(result.EmptyArgs);
            Assert.IsFalse(result.HelpCalled);

            Assert.AreEqual(4, actual.Count());
            Assert.IsTrue(actual.Contains(123));
            Assert.IsTrue(actual.Contains(-123));
            Assert.IsTrue(actual.Contains(-321));
            Assert.IsTrue(actual.Contains(321));
        }

        #endregion

        #endregion Top Level Tests

        #region Duplicate Options Tests

        [Test]
        public void Ensure_First_Value_Is_Stored_When_Duplicate_Options_Are_Specified()
        {
            var parser = CreateFluentParser();

            int? number = 0;
            parser.Setup<int>(CaseType.CaseInsensitive, "n").Callback(n => number = n);

            parser.Parse(new[] { "/n=1", "/n=2", "-n=3", "--n=4" });

            Assert.AreEqual(1, number);
        }

        #endregion

        [Test]
        public void when_args_contains_a_single_switch()
        {
            var parser = CreateFluentParser();

            int? number = 0;
            parser.Setup<int>(CaseType.CaseInsensitive, "b").Callback(n => number = n);

            parser.Parse(new[] { "/b=1" });

            Assert.AreEqual(1, number);
        }

        [Test]
        public void when_args_are_stacked()
        {
            var parser = CreateFluentParser();

            int? number = 0;
            parser.Setup<int>(CaseType.CaseInsensitive, "a").Required().Callback(n => number = n);
            int? number2 = 0;
            parser.Setup<int>(CaseType.CaseInsensitive, "b").Required().Callback(n => number2 = n);

            parser.Parse(new[] { "/ab=1" });

            Assert.AreEqual(1, number);
            Assert.AreEqual(1, number2);
        }

        [Test]
        public void when_args_are_stacked_with_rogue_characters()
        {
            var parser = CreateFluentParser();

            int? number = 0;
            parser.Setup<int>(CaseType.CaseInsensitive, "a").Required().Callback(n => number = n);
            int? number2 = 0;
            parser.Setup<int>(CaseType.CaseInsensitive, "b").Required().Callback(n => number2 = n);

            var result  = parser.Parse(new[] { "/abc=1" });

            Assert.AreEqual(0, number);
            Assert.AreEqual(0, number2);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.Count() == 2);
        }

    }
}

