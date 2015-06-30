using System;
using System.Collections.Generic;
using System.Linq;
using Fclp.Internals.Extensions;
using NUnit.Framework;

namespace Fclp.Tests.Internals.CommandLineParserEngine
{
    /// <summary>
    /// Tests for Uris
    /// </summary>
    [TestFixture]
    public class CommandLineParserEngineMark2NunitTests
    {

        [Test]
        public void when_args_contains_negative_argument_seperated_with_a_colon()
        {
            var parser = new Fclp.FluentCommandLineParser();
            int myInt = 0;
            parser.Setup<int>(CaseType.CaseInsensitive, "int")
                .Callback(value => myInt = value);

            var args = new[] {"--int:-1"};

            parser.Parse(args);
            Assert.IsTrue(myInt == -1);

        }

        [Test]
        public void when_args_contains_negative_argument_seperated_with_a_equals()
        {
            var parser = new Fclp.FluentCommandLineParser();
            int myInt = 0;
            parser.Setup<int>(CaseType.CaseInsensitive, "int")
                .Callback(value => myInt = value);

            var args = new[] { "--int=-123" };

            parser.Parse(args);
            Assert.IsTrue(myInt == -123);

        }

        [Test]
        public void when_args_contains_negative_arguments_seperated_with_double_dash()
        {
            var parser = new Fclp.FluentCommandLineParser();
            int myInt = 0;
            parser.Setup<int>(CaseType.CaseInsensitive, "int")
                .Callback(value => myInt = value);

            var args = new[] { "--int","--","-4321" };

            parser.Parse(args);
            Assert.IsTrue(myInt == -4321);

        }
    }
}
