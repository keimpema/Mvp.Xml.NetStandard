using System;
using System.Xml;
using System.Text;
using System.Globalization;

namespace Mvp.Xml.XPointer
{
	/// <summary>
	/// XPointer lexical analyzer.
	/// </summary>
	internal class XPointerLexer
	{
	    private readonly string ptr;
		private int ptrIndex;
	    private char currChar;

	    private string ParseName()
		{
			int start = ptrIndex - 1;
			int len = 0;
			while (LexUtils.IsNcNameChar(currChar))
			{
				NextChar(); len++;
			}
			return ptr.Substring(start, len);
		}

	    public XPointerLexer(string p)
		{
		    ptr = p ?? throw new ArgumentNullException("pointer", Properties.Resources.NullXPointer);
			NextChar();
		}

	    public bool NextChar()
		{
			if (ptrIndex < ptr.Length)
			{
				currChar = ptr[ptrIndex++];
				return true;
			}

		    currChar = '\0';
		    return false;
		}

		public LexKind Kind { get; private set; }

	    public int Number { get; private set; }

	    public string NcName { get; private set; }

	    public string Prefix { get; private set; }

	    public bool CanBeSchemaName { get; private set; }

	    public void SkipWhiteSpace()
		{
			while (LexUtils.IsWhitespace(currChar))
			{
			    NextChar();
			}
		}

		public bool NextLexeme()
		{
			switch (currChar)
			{
				case '\0':
					Kind = LexKind.Eof;
					return false;
				case '(':
				case ')':
				case '=':
				case '/':
					Kind = (LexKind)Convert.ToInt32(currChar);
					NextChar();
					break;
				case '^':
					NextChar();
					if (currChar == '^' || currChar == '(' || currChar == ')')
					{
						Kind = LexKind.EscapedData;
						NextChar();
					}
					else
					{
					    throw new XPointerSyntaxException(Properties.Resources.CircumflexCharMustBeEscaped);
					}

				    break;
				default:
					if (char.IsDigit(currChar))
					{
						Kind = LexKind.Number;
						int start = ptrIndex - 1;
						int len = 0;
						while (char.IsDigit(currChar))
						{
							NextChar(); len++;
						}
						Number = XmlConvert.ToInt32(ptr.Substring(start, len));
						break;
					}
					else if (LexUtils.IsStartNameChar(currChar))
					{
						Kind = LexKind.NcName;
						Prefix = string.Empty;
						NcName = ParseName();
						if (currChar == ':')
						{
							//QName?
							NextChar();
							Prefix = NcName;
							Kind = LexKind.QName;
							if (LexUtils.IsStartNcNameChar(currChar))
							{
							    NcName = ParseName();
							}
							else
							{
							    throw new XPointerSyntaxException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidNameToken, Prefix, currChar));
							}
						}
						CanBeSchemaName = currChar == '(';
						break;
					}
					else if (LexUtils.IsWhitespace(currChar))
					{
						Kind = LexKind.Space;
						while (LexUtils.IsWhitespace(currChar))
						{
						    NextChar();
						}

					    break;
					}
					else
					{
						Kind = LexKind.EscapedData;
						break;
					}
			}
			return true;
		}

		public string ParseEscapedData()
		{
			int depth = 0;
			StringBuilder sb = new StringBuilder();
			while (true)
			{
				switch (currChar)
				{
					case '^':
						if (!NextChar())
						{
						    throw new XPointerSyntaxException(Properties.Resources.UnexpectedEndOfSchemeData);
						}
						else if (currChar == '^' || currChar == '(' || currChar == ')')
						{
						    sb.Append(currChar);
						}
						else
						{
						    throw new XPointerSyntaxException(Properties.Resources.CircumflexCharMustBeEscaped);
						}

					    break;
					case '(':
						depth++;
						goto default;
					case ')':
						if (depth-- == 0)
						{
							//Skip ')'
							NextLexeme();
							return sb.ToString();
						}
						else
						{
						    goto default;
						}
				    default:
						sb.Append(currChar);
						break;
				}
				if (!NextChar())
				{
				    throw new XPointerSyntaxException(Properties.Resources.UnexpectedEndOfSchemeData);
				}
			}
		}

		public enum LexKind
		{
			NcName = 'N',
			QName = 'Q',
			LrBracket = '(',
			RrBracket = ')',
			Circumflex = '^',
			Number = 'd',
			Eq = '=',
			Space = 'S',
			Slash = '/',
			EscapedData = 'D',
			Eof = 'E'
		}
	}
}
