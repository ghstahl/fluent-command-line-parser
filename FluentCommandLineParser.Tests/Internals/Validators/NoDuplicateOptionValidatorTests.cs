﻿#region License
// NoDuplicateOptionValidatorTests.cs
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

using System.Collections.Generic;
using Fclp.Internals;
using Fclp.Internals.Validators;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Fclp.Tests.Internals.Validators
{
    class NoDuplicateOptionValidatorTests
    {
        [Subject(typeof(NoDuplicateOptionValidator))]
        abstract class NoDuplicateOptionValidatorTestContext : TestContextBase<NoDuplicateOptionValidator>
        {
            protected static Mock<IFluentCommandLineParser> parser;

            Establish context = () =>
            {
                FreezeMock(out parser);
                CreateSut();
            };
        }

        sealed class Validate
        {
            [Subject("Validate")]
            abstract class ValidateTestContext : NoDuplicateOptionValidatorTestContext
            {
                protected static Mock<ICommandLineOption> option;

                Establish context = () =>
                {
                    CreateMock(out option);
                    var optionNames = new Dictionary<string, string>();
                    option.SetupGet(it => it.CaseInsensitiveOptionNames).Returns(optionNames);
                };

                Because of = () =>
                    error = Catch.Exception(() =>
                        sut.Validate(option.Object));

                protected static void SetupExistingParserOptions(params ICommandLineOption[] options)
                {
                    parser.SetupGet(it => it.Options).Returns(CreateManyAsList(options));
                }

                protected static ICommandLineOption CreateOptionWith(string shortName = null, string longName = null)
                {
                    var optionNames = new Dictionary<string, string>();
                    if(!string.IsNullOrWhiteSpace(shortName))
                        optionNames.Add(shortName,"");
                    if (!string.IsNullOrWhiteSpace(longName))
                        optionNames.Add(longName, "");

                    var existingOption = CreateMock<ICommandLineOption>();

                    existingOption.SetupGet(it => it.CaseInsensitiveOptionNames).Returns(optionNames);

                    return existingOption.Object;
                }
            }

            class when_there_have_been_no_options_setup_thus_far : ValidateTestContext
            {
                Establish context = () =>
                    parser.SetupGet(it => it.Options).Returns(CreateEmptyList<ICommandLineOption>());

                It should_not_throw_an_error = () => error.ShouldBeNull();
            }
 
        }
    }
}