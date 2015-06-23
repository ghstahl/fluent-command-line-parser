﻿#region License
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
		#region Constructor Tests

	    [Test]
	    public void Ensure_Can_Be_Constructed()
	    {
	        const string expectedShortName = "My short name";
	        const string expectedLongName = "My long name";
	        var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            var cmdOption = new CommandLineOption<object>(mockParser,expectedShortName, expectedLongName);

	        Assert.IsTrue(cmdOption.OptionNames.ContainsKey(expectedShortName), "Specified ShortName was not as expected");
	        Assert.IsTrue(cmdOption.OptionNames.ContainsKey(expectedLongName), "Specified LongName was not as expected");
	    }

	    [Test]
		public void Ensure_Can_Be_Constructed_With_Null_LongName()
		{
			const string expectedShortName = "My short name";
			const string expectedLongName = null;
			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            var cmdOption = new CommandLineOption<object>(mockParser, expectedShortName, expectedLongName);

            Assert.IsTrue(cmdOption.OptionNames.Count == 1);
            Assert.IsTrue(cmdOption.OptionNames.ContainsKey(expectedShortName), "Specified ShortName was not as expected");
		}

		[Test]
		public void Ensure_Can_Be_Constructed_With_Empty_LongName()
		{
			const string expectedShortName = "My short name";
			const string expectedLongName = "";
			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            var cmdOption = new CommandLineOption<object>(mockParser, expectedShortName, expectedLongName);

            Assert.IsTrue(cmdOption.OptionNames.Count == 1);
            Assert.IsTrue(cmdOption.OptionNames.ContainsKey(expectedShortName), "Specified ShortName was not as expected");
		}

		[Test]
		public void Ensure_Can_Be_Constructed_With_Whitespace_Only_LongName()
		{
			const string expectedShortName = "My short name";
			const string expectedLongName = " ";
			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            var cmdOption = new CommandLineOption<object>(mockParser, expectedShortName, expectedLongName);

            Assert.IsTrue(cmdOption.OptionNames.Count == 1);
            Assert.IsTrue(cmdOption.OptionNames.ContainsKey(expectedShortName), "Specified ShortName was not as expected");
        }

		[Test]
		public void Ensure_Can_Be_Constructed_With_Null_ShortName_And_Valid_LongName()
		{
			const string expectedShortName = null;
			const string expectedLongName = "My long name";

			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            var cmdOption = new CommandLineOption<object>(mockParser, expectedShortName, expectedLongName);

            Assert.IsTrue(cmdOption.OptionNames.Count == 1);
            Assert.IsTrue(cmdOption.OptionNames.ContainsKey(expectedLongName), "Specified LongName was not as expected");
        }

		[Test]
		public void Ensure_Can_Be_Constructed_With_Empty_ShortName_And_Valid_LongName()
		{
			const string expectedShortName = "";
			const string expectedLongName = "My long name";
			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            var cmdOption = new CommandLineOption<object>(mockParser, expectedShortName, expectedLongName);

            Assert.IsTrue(cmdOption.OptionNames.Count == 1);
            Assert.IsTrue(cmdOption.OptionNames.ContainsKey(expectedLongName), "Specified LongName was not as expected");
        }

		[Test]
		public void Ensure_Can_Be_Constructed_With_Whitespace_Only_ShortName_And_Valid_LongName()
		{
			const string expectedShortName = " ";
			const string expectedLongName = "My long name";
			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            var cmdOption = new CommandLineOption<object>(mockParser, expectedShortName, expectedLongName);

            Assert.IsTrue(cmdOption.OptionNames.Count == 1);
            Assert.IsTrue(cmdOption.OptionNames.ContainsKey(expectedLongName), "Specified LongName was not as expected");
        }

		[Test]
		public void Ensure_Throws_Constructed_With_Null_ShortName_And_Null_LongName()
		{
			const string invalidShortName = null;
			const string invalidLongName = null;
			
			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            Assert.Throws<ArgumentOutOfRangeException>(
                delegate { new CommandLineOption<object>(mockParser, invalidShortName, invalidLongName); });
		}

		[Test]
        public void Ensure_Throws_Constructed_With_Empty_ShortName_And_Null_LongName()
		{
			const string invalidShortName = "";
			const string invalidLongName = null;

			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            Assert.Throws<ArgumentOutOfRangeException>(
                delegate { new CommandLineOption<object>(mockParser, invalidShortName, invalidLongName); });
        }

		[Test]
        public void Ensure_Throws_Constructed_With_WhiteSpaceOnly_ShortName_And_Null_LongName()
		{
			const string invalidShortName = " ";
			const string invalidLongName = null;

			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            Assert.Throws<ArgumentOutOfRangeException>(
                delegate { new CommandLineOption<object>(mockParser, invalidShortName, invalidLongName); });
        }

		[Test]
        public void Ensure_Throws_Constructed_With_Null_ShortName_And_Empty_LongName()
		{
			const string invalidShortName = null;
			const string invalidLongName = "";

			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            Assert.Throws<ArgumentOutOfRangeException>(
                delegate { new CommandLineOption<object>(mockParser, invalidShortName, invalidLongName); });
        }

		[Test]
        public void Ensure_Throws_Constructed_With_Empty_ShortName_And_Empty_LongName()
		{
			const string invalidShortName = "";
			const string invalidLongName = "";

			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            Assert.Throws<ArgumentOutOfRangeException>(
                delegate { new CommandLineOption<object>(mockParser, invalidShortName, invalidLongName); });
        }

		[Test]
        public void Ensure_Throws_Constructed_With_WhiteSpaceOnly_ShortName_And_Empty_LongName()
		{
			const string invalidShortName = " ";
			const string invalidLongName = "";

			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            Assert.Throws<ArgumentOutOfRangeException>(
                delegate { new CommandLineOption<object>(mockParser, invalidShortName, invalidLongName); });
        }

		[Test]
        public void Ensure_Throws_Constructed_With_Null_ShortName_And_WhiteSpaceOnly_LongName()
		{
			const string invalidShortName = null;
			const string invalidLongName = " ";

			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            Assert.Throws<ArgumentOutOfRangeException>(
                delegate { new CommandLineOption<object>(mockParser, invalidShortName, invalidLongName); });
        }

		[Test]
        public void Ensure_Throws_Constructed_With_Empty_ShortName_And_WhiteSpaceOnly_LongName()
		{
			const string invalidShortName = "";
			const string invalidLongName = " ";

			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            Assert.Throws<ArgumentOutOfRangeException>(
                delegate { new CommandLineOption<object>(mockParser, invalidShortName, invalidLongName); });
        }

		[Test]
        public void Ensure_Throws_Constructed_With_WhiteSpaceOnly_ShortName_And_WhiteSpaceOnly_LongName()
		{
			const string invalidShortName = " ";
			const string invalidLongName = " ";

			var mockParser = Mock.Of<ICommandLineOptionParser<object>>();

            Assert.Throws<ArgumentOutOfRangeException>(
                delegate { new CommandLineOption<object>(mockParser,invalidShortName, invalidLongName); });
        }
		
		[Test]
		public void Ensure_Cannot_Be_Constructed_With_Null_Parser()
		{
			const string expectedShortName = "My short name";
			const string expectedLongName = "My long name";

            Assert.Throws<ArgumentNullException>(
                delegate { new CommandLineOption<object>(null,expectedShortName, expectedLongName); });
        }

		#endregion Constructor Tests

		#region HasLongName Tests

		[Test]
		public void Ensure_Returns_False_If_Null_LongName_Provided()
		{
            ICommandLineOption cmdOption = new CommandLineOption<string>(Mock.Of<ICommandLineOptionParser<string>>(), WellKnownOptionNames.LittleS, null);
            Assert.IsTrue(cmdOption.OptionNames.ContainsKey(WellKnownOptionNames.LittleS));
            Assert.IsTrue(cmdOption.OptionNames.Count == 1);
		}

		[Test]
		public void Ensure_Returns_False_If_WhiteSpace_LongName_Provided()
		{
            ICommandLineOption cmdOption = new CommandLineOption<string>(Mock.Of<ICommandLineOptionParser<string>>(), WellKnownOptionNames.LittleS, " ");
            Assert.IsTrue(cmdOption.OptionNames.ContainsKey(WellKnownOptionNames.LittleS));
            Assert.IsTrue(cmdOption.OptionNames.Count == 1);
        }

		[Test]
		public void Ensure_Returns_False_If_Empty_LongName_Provided()
		{
            ICommandLineOption cmdOption = new CommandLineOption<string>(Mock.Of<ICommandLineOptionParser<string>>(), WellKnownOptionNames.LittleS, string.Empty);
            Assert.IsTrue(cmdOption.OptionNames.ContainsKey(WellKnownOptionNames.LittleS));
            Assert.IsTrue(cmdOption.OptionNames.Count == 1);
        }

		[Test]
		public void Ensure_Returns_True_If_LongName_Provided()
		{
			ICommandLineOption cmdOption = new CommandLineOption<string>(Mock.Of<ICommandLineOptionParser<string>>(),WellKnownOptionNames.LittleS, "long name" );
            Assert.IsTrue(cmdOption.OptionNames.ContainsKey(WellKnownOptionNames.LittleS));
            Assert.IsTrue(cmdOption.OptionNames.ContainsKey("long name"));
        }

		#endregion HasLongName Tests

		#region Bind Tests

		[Test]
		[ExpectedException(typeof(OptionSyntaxException))]
		public void Ensure_That_If_Value_Is_Null_Cannot_Be_Parsed_And_No_Default_Set_Then_optionSyntaxException_Is_Thrown()
		{
			var option = new ParsedOption();
			const string value = null;
			var mockParser = new Mock<ICommandLineOptionParser<string>>();
			mockParser.Setup(x => x.CanParse(option)).Returns(false);

            var target = new CommandLineOption<string>(mockParser.Object, WellKnownOptionNames.LittleS, "long name");

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

			var target = new CommandLineOption<string>(mockParser.Object,WellKnownOptionNames.LittleS, "long name");

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

			var target = new CommandLineOption<string>(mockParser.Object,WellKnownOptionNames.LittleS, "long name" );

			target.Bind(option);
		}
		#endregion
	}
}

