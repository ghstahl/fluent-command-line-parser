﻿#region License
// OptionNameValidator.cs
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
using System.Linq;

namespace Fclp.Internals.Validators
{
	/// <summary>
	/// Validator to ensure a new Option has a valid long name.
	/// </summary>
	public class OptionNameValidator : ICommandLineOptionValidator
	{
		private static readonly char[] ReservedChars =
			SpecialCharacters.ValueAssignments.Union(new[] { SpecialCharacters.Whitespace }).ToArray();

		/// <summary>
		/// Verifies that the specified <see cref="ICommandLineOption"/> has a valid short/long name combination.
		/// </summary>
		/// <param name="commandLineOption">The <see cref="ICommandLineOption"/> to validate. This must not be null.</param>
		/// <exception cref="ArgumentNullException">if <paramref name="commandLineOption"/> is null.</exception>
		public void Validate(ICommandLineOption commandLineOption)
		{
			if (commandLineOption == null) throw new ArgumentNullException("commandLineOption");

			ValidateShortName(commandLineOption.ShortName);
			ValidateLongName(commandLineOption.LongName);
			ValidateShortAndLongName(commandLineOption.ShortName, commandLineOption.LongName);
		}

		private static void ValidateShortAndLongName(string shortName, string longName)
		{
			if (string.IsNullOrEmpty(shortName) && string.IsNullOrEmpty(longName))
			{
				throw new ArgumentOutOfRangeException();
			}
		}

		private static void ValidateLongName(string longName)
		{
			if (longName != null
				&& longName.Length == 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			if (ContainsReserved(longName))
			{
				throw new ArgumentOutOfRangeException();
			}
		}

		private static void ValidateShortName(string shortName)
		{
			if (shortName != null
				&& shortName.Length > 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			if (ContainsReserved(shortName))
			{
				throw new ArgumentOutOfRangeException();
			}
		}

		private static bool ContainsReserved(string value)
		{
			return value != null 
				&& ReservedChars.Any(value.Contains);
		}
	}
}