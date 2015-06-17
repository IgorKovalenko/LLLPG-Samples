﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Loyc.Syntax.Lexing;

namespace Loyc.Syntax
{
	/// <summary>
	/// A base class designed for LLLPG parsers. It contains all the methods that 
	/// code generated by the Loyc LL(k) Parser Generator might invoke.
	/// </summary>
	/// <typeparam name="Token">A list of characters</typeparam>
	/// <remarks>
	/// This code originally came from Loyc.Syntax.dll, but was modified to 
	/// eliminate dependencies on Loyc.Essentials.dll.
	/// <para/>
	/// BaseParser assumes that the token type can be represented as an integer.
	/// Many parsers, however, will use an "enum TokenType" or something like 
	/// that instead of an integer. When using LLLPG with this base class and
	/// a non-integer token type, you must tell LLLPG to cast the token type
	/// to an integer when calling the <c>Match(...)</c> methods, like so:
	/// <code>
	///     LLLPG (parser(laType(YourTokenType), matchType(int)))
	///     {
	///         rule ...;
	///         rule ...;
	///         ...
	///     } // add semicolon here if using LES
	/// </code> 
	/// </remarks>
	public abstract class BaseParser<Token>
	{
		protected static HashSet<T> NewSet<T>(params T[] items) { return new HashSet<T>(items); }

		protected BaseParser() { EOF = EofInt(); }

		protected Token _lt0;
		/// <summary>Next token to parse (set to LT(0) whenever InputPosition is changed).</summary>
		public Token LT0 { [DebuggerStepThrough] get { return _lt0; } }

		private int _inputPosition = 0;
		/// <summary>Current position of the next token to be parsed.</summary>
		protected int InputPosition
		{
			[DebuggerStepThrough]
			get { return _inputPosition; }
			set {
				_inputPosition = value;
				_lt0 = LT(0);
			}
		}

		int EOF;
		/// <summary>Returns the value used for EOF (normally 0)</summary>
		protected abstract int EofInt();
		/// <summary>Returns the token type of _lt0 (normally _lt0.TypeInt)</summary>
		protected abstract int LA0Int { get; }
		/// <summary>Returns the token at lookahead i (e.g. <c>Source[InputPosition + i]</c>
		/// if the tokens come from a list called Source) </summary>
		protected abstract Token LT(int i);
		/// <summary>Records an error or throws an exception. When called by 
		/// BaseParser, inputPosition is always equal to <see cref="InputPosition"/>.</summary>
		protected abstract void Error(int inputPosition, string message);
		/// <summary>Returns a string representation of the specified token type.
		/// These strings are used in error messages.</summary>
		protected abstract string ToString(int type);

		protected void Skip()
		{
			// Called when prediction already verified the input (and LA(0) is not saved, so we return void)
			Debug.Assert(!(LA0Int == EOF));
			InputPosition++;
		}

		#region Normal matching

		protected Token MatchAny()
		{
			Token lt = _lt0;
			InputPosition++;
			return lt;
		}
		protected Token Match(HashSet<int> set, bool inverted = false)
		{
			Token lt = _lt0;
			if (set.Contains(LA0Int) == inverted)
				Error(false, set);
			else
				InputPosition++;
			return lt;
		}
		protected Token Match(int a)
		{
			Token lt = _lt0; int la = LA0Int;
			if (!(la == a))
				Error(false, a);
			else
				InputPosition++;
			return lt;
		}
		protected Token Match(int a, int b)
		{
			Token lt = _lt0; int la = LA0Int;
			if (!(la == a) && !(la == b))
				Error(false, a, b);
			else
				InputPosition++;
			return lt;
		}
		protected Token Match(int a, int b, int c)
		{
			Token lt = _lt0; int la = LA0Int;
			if (!(la == a) && !(la == b) && !(la == c))
				Error(false, a, b, c);
			else
				InputPosition++;
			return lt;
		}
		protected Token Match(int a, int b, int c, int d)
		{
			Token lt = _lt0; int la = LA0Int;
			if (!(la == a) && !(la == b) && !(la == c) && !(la == d))
				Error(false, a, b, c, d);
			else
				InputPosition++;
			return lt;
		}
		protected Token MatchExcept()
		{
			Token lt = _lt0; int la = LA0Int;
			if ((la == EOF))
				Error(true);
			else
				InputPosition++;
			return lt;
		}
		protected Token MatchExcept(int a)
		{
			Token lt = _lt0; int la = LA0Int;
			if ((la == a) || (la == EOF))
				Error(true, a);
			else
				InputPosition++;
			return lt;
		}
		protected Token MatchExcept(int a, int b)
		{
			Token lt = _lt0; int la = LA0Int;
			if ((la == a) || (la == b) || (la == EOF))
				Error(true, a, b);
			else
				InputPosition++;
			return lt;
		}
		protected Token MatchExcept(int a, int b, int c)
		{
			Token lt = _lt0; int la = LA0Int;
			if ((la == a) || (la == b) || (la == c) || (la == EOF))
				Error(true, a, b, c);
			else
				InputPosition++;
			return lt;
		}
		protected Token MatchExcept(int a, int b, int c, int d)
		{
			Token lt = _lt0; int la = LA0Int;
			if ((la == a) || (la == b) || (la == c) || (la == d) || (la == EOF))
				Error(true, a, b, c);
			else
				InputPosition++;
			return lt;
		}

		#endregion

		#region Try-matching

		protected struct SavePosition : IDisposable
		{
			BaseParser<Token> _parser;
			int _oldPosition;
			public SavePosition(BaseParser<Token> parser, int lookaheadAmt)
				{ _parser = parser; _oldPosition = parser.InputPosition; parser.InputPosition += lookaheadAmt; }
			public void Dispose() { _parser.InputPosition = _oldPosition; }
		}
		protected bool TryMatch(HashSet<int> set, bool inverted = false)
		{
			if (set.Contains(LA0Int) == inverted)
				return false;
			else
				InputPosition++;
			return true;
		}
		protected bool TryMatch(int a)
		{
			if (!(LA0Int == a))
				return false;
			else
				InputPosition++;
			return true;
		}
		protected bool TryMatch(int a, int b)
		{
			Token lt = _lt0; int la = LA0Int;
			if (!(la == a) && !(la == b))
				return false;
			else
				InputPosition++;
			return true;
		}
		protected bool TryMatch(int a, int b, int c)
		{
			Token lt = _lt0; int la = LA0Int;
			if (!(la == a) && !(la == b) && !(la == c))
				return false;
			else
				InputPosition++;
			return true;
		}
		protected bool TryMatchExcept()
		{
			if ((LA0Int == EOF))
				return false;
			else
				InputPosition++;
			return true;
		}
		protected bool TryMatchExcept(int a)
		{
			Token lt = _lt0; int la = LA0Int;
			if ((la == EOF) || (la == a))
				return false;
			else
				InputPosition++;
			return true;
		}
		protected bool TryMatchExcept(int a, int b)
		{
			Token lt = _lt0; int la = LA0Int;
			if ((la == EOF) || (la == a) || (la == b))
				return false;
			else
				InputPosition++;
			return true;
		}
		protected bool TryMatchExcept(int a, int b, int c)
		{
			Token lt = _lt0; int la = LA0Int;
			if ((la == EOF) || (la == a) || (la == b) || (la == c))
				return false;
			else
				InputPosition++;
			return true;
		}

		#endregion

		protected void Error(bool inverted, params int[] expected) { Error(inverted, (IEnumerable<int>)expected); }
		protected virtual void Error(bool inverted, IEnumerable<int> expected)
		{
			Error(InputPosition, string.Format("Error: '{0}': expected {1}", ToString(LA0Int), ToString(inverted, expected)));
		}
		protected virtual string ToString(bool inverted, IEnumerable<int> expected)
		{
			int plural = expected.Take(2).Count();
			if (plural == 0)
				return string.Format(inverted ? "anything" : "nothing");
			else if (inverted)
				return string.Format("anything except {0}", ToString(false, expected));
			else if (plural == 1)
				return ToString(expected.First());
			else
				return string.Join("|", expected.Select(e => ToString(e)));
		}
		protected virtual void Check(bool expectation, string expectedDescr = "")
		{
			if (!expectation)
				Error(InputPosition, string.Format("An expected condition was false: {0}", expectedDescr));
		}
	}
}