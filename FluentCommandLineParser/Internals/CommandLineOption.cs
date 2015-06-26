#region License
// CommandLineOption.cs
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
using System.Linq;
using Fclp.Internals.Extensions;
using Fclp.Internals.Parsing;
using Fclp.Internals.Parsing.OptionParsers;
using Fclp.Internals.Validators;

namespace Fclp.Internals
{
    /// <summary>
    /// A command line Option
    /// </summary>
    /// <typeparam name="T">The type of value this Option requires.</typeparam>
    public class CommandLineOption<T> : ICommandLineOptionResult<T>
    {
        #region Constructors

        private void InternalConstructor(ICommandLineOptionParser<T> parser)
        {

            if (parser == null)
                throw new ArgumentNullException("parser");
            this.Parser = parser;
        }

        /// <summary>
        /// Construct with string array of names
        /// </summary>
        /// <param name="parser"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CommandLineOption(ICommandLineOptionParser<T> parser)
        {
            InternalConstructor(parser);
        }

        #endregion

        #region Properties

        private Dictionary<string, string> _caseInsensitiveOptionsDictionary;

        private Dictionary<string, string> CaseInsensitiveOptionsDictionary
        {
            get
            {
                return _caseInsensitiveOptionsDictionary
                       ??
                       (_caseInsensitiveOptionsDictionary =
                           new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase));
            }
        }
        private Dictionary<string, string> _caseSensitiveOptionsDictionary;

        private Dictionary<string, string> CaseSensitiveOptionsDictionary
        {
            get
            {
                return _caseSensitiveOptionsDictionary
                       ??
                       (_caseSensitiveOptionsDictionary =
                           new Dictionary<string, string>(StringComparer.InvariantCulture));
            }
        }

        /// <inheritdoc/>
        /// <summary>
        /// Gets or sets the parser to use for this <see cref="CommandLineOption{T}"/>.
        /// </summary>
        ICommandLineOptionParser<T> Parser { get; set; }

        /// <summary>
        /// Gets the description set for this <see cref="CommandLineOption{T}"/>.
        /// </summary>
        public string Description { get; set; }

        internal Action<T> ReturnCallback { get; set; }

        internal Action<IEnumerable<string>> AdditionalArgumentsCallback { get; set; }

        internal T Default { get; set; }

        /// <summary>
        /// Gets whether this <see cref="ICommandLineOption"/> is required.
        /// </summary>
        public bool IsRequired { get; set; }



        /// <summary>
        /// Gets whether this <see cref="ICommandLineOption"/> has a default value setup.
        /// </summary>
        public bool HasDefault { get; set; }

        /// <summary>
        /// Gets the setup <see cref="System.Type"/> for this option.
        /// </summary>
        public Type SetupType
        {
            get
            {
                var type = typeof (T);
                var genericArgs = type.GetGenericArguments();
                return genericArgs.Any() ? genericArgs.First() : type;
            }
        }

        /// <summary>
        /// Returns the case sensitive options
        /// </summary>
        public IDictionary<string, string> CaseSensitiveOptionNames
        {
            get { return CaseSensitiveOptionsDictionary; }
        }

        /// <summary>
        /// Returns a Dictionary of option names
        /// </summary>
        public IDictionary<string, string> CaseInsensitiveOptionNames
        {
            get { return CaseInsensitiveOptionsDictionary; }
        }

        /// <summary>
        /// Gets whether this <see cref="ICommandLineOption"/> has a callback setup.
        /// </summary>
        public bool HasCallback
        {
            get { return this.ReturnCallback != null; }
        }

        /// <summary>
        /// Gets whether this <see cref="ICommandLineOption"/> has an additional arguments callback setup.
        /// </summary>
        public bool HasAdditionalArgumentsCallback
        {
            get { return this.AdditionalArgumentsCallback != null; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Binds the specified <see cref="System.String"/> to the Option.
        /// </summary>
        /// <param name="value">The <see cref="System.String"/> to bind.</param>
        public void Bind(ParsedOption value)
        {
            if (this.Parser.CanParse(value) == false) throw new OptionSyntaxException();

            this.Bind(this.Parser.Parse(value));

            this.BindAnyAdditionalArgs(value);
        }

        /// <summary>
        /// Binds the default value for this <see cref="ICommandLineOption"/> if available.
        /// </summary>
        public void BindDefault()
        {
            if (this.HasDefault)
                this.Bind(this.Default);
        }


        void Bind(T value)
        {
            if (this.HasCallback)
                this.ReturnCallback(value);
        }

        void BindAnyAdditionalArgs(ParsedOption option)
        {
            if (!this.HasAdditionalArgumentsCallback) return;

            if (option.AdditionalValues.Any())
            {
                this.AdditionalArgumentsCallback(option.AdditionalValues);
            }
        }

        /// <summary>
        /// Adds the specified description to the <see cref="ICommandLineOptionFluent{T}"/>.
        /// </summary>
        /// <param name="description">The <see cref="System.String"/> representing the description to use. This should be localised text.</param>
        /// <returns>A <see cref="ICommandLineOptionFluent{T}"/>.</returns>
        public ICommandLineOptionFluent<T> WithDescription(string description)
        {
            this.Description = description;
            return this;
        }

        /// <summary>
        /// Declares that this <see cref="ICommandLineOptionFluent{T}"/> is required and a value must be specified to fulfil it.
        /// </summary>
        /// <returns>A <see cref="ICommandLineOptionFluent{T}"/>.</returns>
        public ICommandLineOptionFluent<T> Required()
        {
            this.IsRequired = true;
            return this;
        }

        /// <summary>
        /// Specifies the method to invoke when the <see cref="ICommandLineOptionFluent{T}"/>. 
        /// is parsed. If a callback is not required either do not call it, or specify <c>null</c>.
        /// </summary>
        /// <param name="callback">The return callback to execute with the parsed value of the Option.</param>
        /// <returns>A <see cref="ICommandLineOptionFluent{T}"/>.</returns>
        public ICommandLineOptionFluent<T> Callback(Action<T> callback)
        {
            this.ReturnCallback = callback;
            return this;
        }

        /// <summary>
        /// Specifies the default value to use if no value is found whilst parsing this <see cref="ICommandLineOptionFluent{T}"/>.
        /// </summary>
        /// <param name="value">The value to use.</param>
        /// <returns>A <see cref="ICommandLineOptionFluent{T}"/>.</returns>
        public ICommandLineOptionFluent<T> SetDefault(T value)
        {
            this.Default = value;
            this.HasDefault = true;
            return this;
        }

        /// <summary>
        /// Specified the method to invoke with any addition arguments parsed with the Option.
        /// If additional arguments are not required either do not call it, or specify <c>null</c>.
        /// </summary>
        /// <param name="callback">The return callback to execute with the parsed addition arguments found for this Option.</param>
        /// <returns>A <see cref="ICommandLineOptionFluent{T}"/>.</returns>
        public ICommandLineOptionFluent<T> CaptureAdditionalArguments(Action<IEnumerable<string>> callback)
        {
            this.AdditionalArgumentsCallback = callback;
            return this;
        }

        /// <summary>
        /// Adds case sensitive option only if it doesn't exist in the case sensitive or case insensitive dictionary
        /// </summary>
        /// <param name="optionNames"></param>
        /// <returns>A <see cref="ICommandLineOptionFluent{T}"/>.</returns>
        public ICommandLineOptionFluent<T> AddCaseSensitiveOption(params string[] optionNames)
        {
            if (optionNames == null)
                throw new ArgumentNullException("optionNames");

            _optionValidator.WhatIfAddOption(optionNames);

            foreach (var name in optionNames)
            {
                CaseSensitiveOptionsDictionary.Add(name, "");
            }
            return this;
        }

        /// <summary>
        /// Adds case sensitive option only if it doesn't exist in the case sensitive or case insensitive dictionary
        /// </summary>
        /// <param name="optionNames"></param>
        /// <returns>A <see cref="ICommandLineOptionFluent{T}"/>.</returns>
        public ICommandLineOptionFluent<T> AddCaseInsensitiveOption(params string[] optionNames)
        {
            if (optionNames == null)
                throw new ArgumentNullException("optionNames");
            
            _optionValidator.WhatIfAddOption(optionNames);

            foreach (var name in optionNames)
            {
                CaseInsensitiveOptionsDictionary.Add(name, "");
            }
            return this;
        }

        #endregion Methods

        ICommandLineOptionValidator _optionValidator;

        /// <summary>
        /// Sets the option validator so that prior to accepting any option name it gets checked
        /// </summary>
        /// <param name="optionValidator"></param>
        public void SetOptionValidator(ICommandLineOptionValidator optionValidator)
        {
            _optionValidator = optionValidator;
        }
    }
}
