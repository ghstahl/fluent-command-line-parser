﻿#region License
// SettingUpAShortOptionTestContext.cs
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

using System.Globalization;
using System.Linq;
using Fclp.Internals;
using Machine.Specifications;

namespace Fclp.Tests.FluentCommandLineParser
{
	namespace TestContext
	{
		[Subject(Subjects.setup_new_option)]
		public abstract class SettingUpAShortOptionTestContext : FluentCommandLineParserTestContext
		{
			protected const string invalid_short_name_that_is_whitespace = " ";
			protected const string invalid_short_name_with_colon = ":";
			protected const string invalid_short_name_with_equality_sign = "=";
			const char _char7 = (char)7;
			protected static readonly string invalid_short_name_that_is_a_control_char = _char7.ToString(CultureInfo.InvariantCulture);
			protected const string valid_short_name = WellKnownOptionNames.LittleS;

			protected static ICommandLineOption option;

			protected static void SetupOptionWith(string shortName)
			{
				CatchAnyError(() =>
				{
                    var ret = sut.Setup<TestType>().AddCaseInsensitiveOption(shortName);
					option = sut.Options.SingleOrDefault(x => ReferenceEquals(x, ret));
				});
			}
		}
	}
}