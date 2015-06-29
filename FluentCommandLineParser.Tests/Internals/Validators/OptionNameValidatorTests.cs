#region License
// OptionNameValidatorTests.cs
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
using Fclp.Internals.Validators;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Fclp.Tests.Internals.Validators
{
    class OptionNameValidatorTests
    {
        [Subject(typeof(OptionNameValidator))]
        abstract class CommandLineOptionNameValidatorTestContext : TestContextBase<OptionNameValidator>
        {
            Establish context = () => CreateSut();
        }

        sealed class Validate
        {
            abstract class ValidateTestContext : CommandLineOptionNameValidatorTestContext
            {
                protected const string ValidShortName = WellKnownOptionNames.LittleS;
                protected const string ValidLongName = "long";

//                protected static Mock<ICommandLineOption> option;
               protected static ICommandLineOption option;
                /*
                Establish context = () =>
                    CreateMock(out option);

                Because of = () =>
                    error = Catch.Exception(() =>
                        sut.Validate(option.Object));
*/
                protected static void SetupOptionWith(string shortName = ValidShortName, string longName = ValidLongName)
                {
                    var parser = new Fclp.FluentCommandLineParser();
                    parser.Setup<string>(CaseType.CaseInsensitive, shortName, longName);
                    option = parser.Options.First();
                }
            }

            class when_the_short_name_is_null : ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith(shortName: null));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();

            }

            class when_the_short_name_is_whitespace : ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith(shortName: " "));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();
              
            }

            class when_the_short_name_contains_a_colon : ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith(shortName: ":"));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();
            }

            class when_the_short_name_contains_an_equality_sign : ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith(shortName: "="));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();
                
            }

            class when_the_short_name_is_empty : ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith(shortName: string.Empty));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();

            }

            class when_the_short_name_is_a_control_char : ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith(shortName: ((char)7).ToString(CultureInfo.InvariantCulture)));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();

            }
 
            class when_the_short_name_is_one_char : ValidateTestContext
            {
                Establish context = () =>
                    SetupOptionWith(shortName: CreateStringOfLength(1));

                It should_not_throw_an_error = () => error.ShouldBeNull();
            }

            class when_the_long_name_is_null : ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith( longName: null));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();
                
            }

            class when_the_long_name_is_whitespace : ValidateTestContext
            {
                private static Exception exception;
                private Because of = () =>
                   exception = Catch.Exception(() => SetupOptionWith(longName: " "));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();

            }

            private class when_the_long_name_contains_a_colon : ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith(longName: ValidLongName + ":"));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();

            }

            class when_the_long_name_contains_an_equality_sign : ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith(longName: ValidLongName + "="));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();

            }

            class when_the_long_name_is_empty : ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith(longName: string.Empty));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();

            }

            class when_the_long_name_is_longer_than_one_char : ValidateTestContext
            {
                Establish context = () =>
                    SetupOptionWith(longName: CreateStringOfLength(2));

                It should_not_throw_an_error = () => error.ShouldBeNull();
            }

            class when_the_long_name_contains_whitespace: ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith(longName: ValidLongName + " " + ValidLongName));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();
                
                
            }

            class when_the_long_name_is_null_and_the_short_name_is_null : ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith(shortName: null, longName: null));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();
                
            }

            class when_the_long_name_is_empty_and_the_short_name_is_empty : ValidateTestContext
            {
                private static Exception exception;

                private Because of = () =>
                    exception = Catch.Exception(() => SetupOptionWith(shortName: string.Empty, longName: string.Empty));

                private It should_have_thrown_an_exception = () =>
                    exception.ShouldNotBeNull();

                private It and_should_be_an_ArgumentException = () =>
                    exception.ShouldBeOfType<InvalidOptionNameException>();

            }
        }
    }
}