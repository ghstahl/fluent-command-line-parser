#region License
// CommandLineOptionTests.cs
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
using Fclp;
using Fclp.Internals;
using Fclp.Internals.Parsing;
using Fclp.Internals.Parsing.OptionParsers;
using Fclp.Internals.Validators;
using Fclp.Tests;
using Moq;
using NUnit.Framework;

namespace FluentCommandLineParser.Tests.Internals
{
    /// <summary>
    /// Contains unit tests for the <see cref="CommandLineOption{T}"/> class.
    /// </summary>
    [TestFixture]
    class CommandLineOptionTests
    {


        static CommandLineOption<T> NewCommandLineOption<T>()
        {
            var mockParser = Mock.Of<ICommandLineOptionParser<T>>();
            var cmdOption = new CommandLineOption<T>(mockParser);
            return cmdOption;
        }

        #region Constructor Tests

        [Test]
        public void Ensure_Can_Be_Constructed()
        {
            var cmdOption = NewCommandLineOption<object>();
        }
        
        #endregion Constructor Tests

        [Test]
        public void Ensure_Can_Add_CaseSensitiveOptions()
        {
            var parser = new Fclp.FluentCommandLineParser();

            var cmdOptionFluent = parser.Setup<string>(
                CaseType.CaseSensitive,WellKnownOptionNames.OptionNameOne, WellKnownOptionNames.OptionNameTwo);


            var cmdOptionResult = cmdOptionFluent as ICommandLineOptionResult<string>;
            Assert.IsTrue(cmdOptionResult.CaseInsensitiveOptionNames.Count == 0);
            Assert.IsTrue(cmdOptionResult.CaseSensitiveOptionNames.Count == 2);

            Assert.IsTrue(cmdOptionResult.CaseSensitiveOptionNames.ContainsKey(WellKnownOptionNames.OptionNameOne), "Specified WellKnownOptionNames.OptionNameOne was not as expected");
            Assert.IsTrue(cmdOptionResult.CaseSensitiveOptionNames.ContainsKey(WellKnownOptionNames.OptionNameTwo), "Specified WellKnownOptionNames.OptionNameTwo was not as expected");

            Assert.IsFalse(cmdOptionResult.CaseInsensitiveOptionNames.ContainsKey(WellKnownOptionNames.OptionNameOneMixedCase), "Specified WellKnownOptionNames.OptionNameOneMixedCase was not as expected");
            Assert.IsFalse(cmdOptionResult.CaseInsensitiveOptionNames.ContainsKey(WellKnownOptionNames.OptionNameTwoMixedCase), "Specified WellKnownOptionNames.OptionNameTwoMixedCase was not as expected");
        }

        [Test]
        public void Ensure_Can_Have_CaseSensitiveOptions()
        {
            var parser = new Fclp.FluentCommandLineParser();

            var cmdOptionFluentLittleA = parser.Setup<string>(CaseType.CaseSensitive, WellKnownOptionNames.LittleA);

            var cmdOptionFluentBigA = parser.Setup<string>(CaseType.CaseSensitive, WellKnownOptionNames.BigA);



            var cmdOptionResultLittleA = cmdOptionFluentLittleA as ICommandLineOptionResult<string>;
            Assert.IsTrue(cmdOptionResultLittleA.CaseInsensitiveOptionNames.Count == 0);
            Assert.IsTrue(cmdOptionResultLittleA.CaseSensitiveOptionNames.Count == 1);
            Assert.IsTrue(cmdOptionResultLittleA.CaseSensitiveOptionNames.ContainsKey(WellKnownOptionNames.LittleA),
                "Specified WellKnownOptionNames.LittleA was not as expected");


            var cmdOptionResultBigA = cmdOptionFluentBigA as ICommandLineOptionResult<string>;
            Assert.IsTrue(cmdOptionResultBigA.CaseInsensitiveOptionNames.Count == 0);
            Assert.IsTrue(cmdOptionResultBigA.CaseSensitiveOptionNames.Count == 1);
            Assert.IsTrue(cmdOptionResultBigA.CaseSensitiveOptionNames.ContainsKey(WellKnownOptionNames.BigA),
                "Specified WellKnownOptionNames.BigA was not as expected");

            Assert.Throws<Fclp.OptionAlreadyExistsException>(() =>
            {
                var cmdOptionFluentBigA2 = parser.Setup<string>(CaseType.CaseSensitive,WellKnownOptionNames.BigA);

            });
            Assert.Throws<Fclp.OptionAlreadyExistsException>(() =>
            {
                var cmdOptionFluentBigA2 = parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.BigA);

            });

            Assert.Throws<Fclp.OptionAlreadyExistsException>(() =>
            {
                var cmdOptionFluentBigA2 = parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.BigA);
            });
            Assert.Throws<Fclp.OptionAlreadyExistsException>(() =>
            {
                var cmdOptionFluentBigA2 = parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.BigA);
            });
        }

        [Test]
        public void Ensure_Can_Add_CaseInsensitiveOptions()
        {
            var parser = new Fclp.FluentCommandLineParser();
            var cmdOptionFluent = parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.OptionNameOne, WellKnownOptionNames.OptionNameTwo);


            var cmdOptionResult = cmdOptionFluent as ICommandLineOptionResult<string>;
            Assert.IsTrue(cmdOptionResult.CaseInsensitiveOptionNames.Count == 2);

            Assert.IsTrue(cmdOptionResult.CaseInsensitiveOptionNames.ContainsKey(WellKnownOptionNames.OptionNameOne), "Specified WellKnownOptionNames.OptionNameOne was not as expected");
            Assert.IsTrue(cmdOptionResult.CaseInsensitiveOptionNames.ContainsKey(WellKnownOptionNames.OptionNameTwo), "Specified WellKnownOptionNames.OptionNameTwo was not as expected");

            Assert.IsTrue(cmdOptionResult.CaseInsensitiveOptionNames.ContainsKey(WellKnownOptionNames.OptionNameOneMixedCase), "Specified WellKnownOptionNames.OptionNameOneMixedCase was not as expected");
            Assert.IsTrue(cmdOptionResult.CaseInsensitiveOptionNames.ContainsKey(WellKnownOptionNames.OptionNameTwoMixedCase), "Specified WellKnownOptionNames.OptionNameTwoMixedCase was not as expected");
        }

        [Test]
        public void Ensure_Throws_Adding_CaseInsensitive_Invalid_Options()
        {
            var parser = new Fclp.FluentCommandLineParser();


            Assert.Throws<InvalidOptionNameException>(() =>
            {
                parser.Setup<string>(CaseType.CaseInsensitive,WellKnownOptionNames.NullOptionName);
            });
            Assert.Throws<InvalidOptionNameException>(() =>
            {
                parser.Setup<string>(CaseType.CaseInsensitive,WellKnownOptionNames.EmptyString);
            });
            Assert.Throws<InvalidOptionNameException>(() =>
            {
                parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.WhiteSpaceString);
            });
        }

        [Test]
        public void Ensure_Throws_Adding_CaseSensitive_Invalid_Options()
        {
            var parser = new Fclp.FluentCommandLineParser();
            var cmdOptionFluent = parser.Setup<string>(CaseType.CaseInsensitive, "Whatever");

            Assert.Throws<Fclp.InvalidOptionNameException>(() =>
            {
                cmdOptionFluent.AddCaseSensitiveOption(WellKnownOptionNames.NullOptionName);
            });
            Assert.Throws<Fclp.InvalidOptionNameException>(() =>
            {
                cmdOptionFluent.AddCaseSensitiveOption(WellKnownOptionNames.EmptyString);
            });
            Assert.Throws<Fclp.InvalidOptionNameException>(() =>
            {
                cmdOptionFluent.AddCaseSensitiveOption(WellKnownOptionNames.WhiteSpaceString);
            });
        }

        [Test]
        public void Ensure_Throws_Adding_CaseInsensitive_Duplicates_Option()
        {
            var parser = new Fclp.FluentCommandLineParser();
            var cmdOptionFluent = parser.Setup<string>(CaseType.CaseInsensitive, WellKnownOptionNames.OptionNameOne, WellKnownOptionNames.OptionNameTwo);


            Assert.Throws<Fclp.OptionAlreadyExistsException>(() =>
            {
                cmdOptionFluent.AddCaseInsensitiveOption(WellKnownOptionNames.OptionNameOne, WellKnownOptionNames.OptionNameTwo);
            });

            Assert.Throws<Fclp.OptionAlreadyExistsException>(() =>
            {
                cmdOptionFluent.AddCaseInsensitiveOption(WellKnownOptionNames.OptionNameOne);
            });
            Assert.Throws<Fclp.OptionAlreadyExistsException>(() =>
            {
                cmdOptionFluent.AddCaseInsensitiveOption(WellKnownOptionNames.OptionNameTwo);
            });

            Assert.Throws<Fclp.OptionAlreadyExistsException>(() =>
            {
                cmdOptionFluent.AddCaseSensitiveOption(WellKnownOptionNames.OptionNameOne, WellKnownOptionNames.OptionNameTwo);
            });

            Assert.Throws<Fclp.OptionAlreadyExistsException>(() =>
            {
                cmdOptionFluent.AddCaseSensitiveOption(WellKnownOptionNames.OptionNameOne);
            });
            Assert.Throws<Fclp.OptionAlreadyExistsException>(() =>
            {
                cmdOptionFluent.AddCaseSensitiveOption(WellKnownOptionNames.OptionNameTwo);
            });


        }

        [Test]
        public void Ensure_Throws_Adding_CaseSensitive_Duplicates_Option()
        {
            var parser = new Fclp.FluentCommandLineParser();
            var cmdOptionFluent = parser.Setup<string>(CaseType.CaseSensitive,
                WellKnownOptionNames.OptionNameOne, WellKnownOptionNames.OptionNameTwo);


            Assert.Throws<OptionAlreadyExistsException>(() =>
            {
                cmdOptionFluent.AddCaseSensitiveOption(WellKnownOptionNames.OptionNameOne, WellKnownOptionNames.OptionNameTwo);
            });

            Assert.Throws<OptionAlreadyExistsException>(() =>
            {
                cmdOptionFluent.AddCaseSensitiveOption(WellKnownOptionNames.OptionNameOne);
            });
            Assert.Throws<OptionAlreadyExistsException>(() =>
            {
                cmdOptionFluent.AddCaseSensitiveOption(WellKnownOptionNames.OptionNameTwo);
            });

            Assert.Throws<OptionAlreadyExistsException>(() =>
            {
                cmdOptionFluent.AddCaseInsensitiveOption(WellKnownOptionNames.OptionNameOne, WellKnownOptionNames.OptionNameTwo);
            });

            Assert.Throws<OptionAlreadyExistsException>(() =>
            {
                cmdOptionFluent.AddCaseInsensitiveOption(WellKnownOptionNames.OptionNameOne);
            });
            Assert.Throws<OptionAlreadyExistsException>(() =>
            {
                cmdOptionFluent.AddCaseInsensitiveOption(WellKnownOptionNames.OptionNameTwo);
            });


        }

     
        #region Bind Tests

        [Test]
        [ExpectedException(typeof (OptionSyntaxException))]
        public void Ensure_That_If_Value_Is_Null_Cannot_Be_Parsed_And_No_Default_Set_Then_optionSyntaxException_Is_Thrown()
        {
            var option = new ParsedOption();
            const string value = null;
            var mockParser = new Mock<ICommandLineOptionParser<string>>();
            mockParser.Setup(x => x.CanParse(option)).Returns(false);

            var mockValidator = new Mock<ICommandLineOptionValidator>();
            mockValidator.Setup(x => x.WhatIfAddOption(WellKnownOptionNames.LittleS, WellKnownOptionNames.OptionNameOne));

            var target = new CommandLineOption<string>(mockParser.Object);
            target.SetOptionValidator(mockValidator.Object);
            target.AddCaseInsensitiveOption(WellKnownOptionNames.LittleS, WellKnownOptionNames.OptionNameOne);

            target.Bind(option);
        }


        [Test]
        [ExpectedException(typeof(OptionSyntaxException))]
        public void Ensure_That_If_Value_Is_Empty_Cannot_Be_Parsed_And_No_Default_Set_Then_optionSyntaxException_Is_Thrown()
        {
            var option = new ParsedOption();
            const string value = "";
            var mockParser = new Mock<ICommandLineOptionParser<string>>();
            mockParser.Setup(x => x.CanParse(option)).Returns(false);
            var mockValidator = new Mock<ICommandLineOptionValidator>();
            mockValidator.Setup(x => x.WhatIfAddOption(WellKnownOptionNames.LittleS, WellKnownOptionNames.OptionNameOne));

            var target = new CommandLineOption<string>(mockParser.Object);
            target.SetOptionValidator(mockValidator.Object);
            target.AddCaseInsensitiveOption(WellKnownOptionNames.LittleS, WellKnownOptionNames.OptionNameOne);
            
            target.Bind(option);
        }


        [Test]
        [ExpectedException(typeof(OptionSyntaxException))]
        public void Ensure_That_If_Value_Is_Whitespace_Cannot_Be_Parsed_And_No_Default_Set_Then_optionSyntaxException_Is_Thrown()
        {
            var option = new ParsedOption();
            const string value = " ";
            var mockParser = new Mock<ICommandLineOptionParser<string>>();
            mockParser.Setup(x => x.CanParse(option)).Returns(false);

            var mockValidator = new Mock<ICommandLineOptionValidator>();
            mockValidator.Setup(x => x.WhatIfAddOption(WellKnownOptionNames.LittleS, WellKnownOptionNames.OptionNameOne));

            var target = new CommandLineOption<string>(mockParser.Object);
            target.SetOptionValidator(mockValidator.Object);
            target.AddCaseInsensitiveOption(WellKnownOptionNames.LittleS, WellKnownOptionNames.OptionNameOne);

            target.Bind(option);
        }
        #endregion
    }
}

