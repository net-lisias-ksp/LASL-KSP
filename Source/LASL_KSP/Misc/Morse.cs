/*
	This file is part of L Aerospace Shared Libraries / KSP Division (LASL-KSP)
		© 2023 LisiasT : http://lisias.net <support@lisias.net>

	CrewLight is double licensed, as follows:
		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	And you are allowed to choose the License that better suit your needs.

	Crew Light /L Unleashed is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the SKL Standard License 1.0
	along with Crew Light /L Unleashed.
	If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

	You should have received a copy of the GNU General Public License 2.0
	along with Crew Light /L Unleashed.
	If not, see <https://www.gnu.org/licenses/>.

*/
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

namespace LASL.KSP.Misc
{
	public class Morse
	{
		public enum Code { dih, dah, letterspc, wordspc, symspc };

		public interface IPlay
		{
			void RisingEdge();
			void FallingEdge();
		}

		public interface ISettings
		{
			float ditDuration { get; }
			float dahDuration { get; }

			char letterSpaceChar { get; }
			float letterSpaceDuration { get; }

			char wordSpaceChar { get; }
			float wordSpaceDuration { get; }

			float symbolSpaceDuration { get; }
		}

		// Source: https://morsecode.world/international/morse.html
		private static readonly Dictionary<char, string> TABLE = new Dictionary<char, string> {
			{'A', ".-"},
			{'B', "-..."},
			{'C', "-.-."},
			{'D', "-.."},
			{'E', "."},
			{'F', "..-."},
			{'G', "--."},
			{'H', "...."},
			{'I', ".."},
			{'J', ".---"},
			{'K', "-.-"},
			{'L', ".-.."},
			{'M', "--"},
			{'N', "-."},
			{'O', "---"},
			{'P', ".--."},
			{'Q', "--.-"},
			{'R', ".-."},
			{'S', "..."},
			{'T', "-"},
			{'U', "..-"},
			{'V', "...-"},
			{'W', ".--"},
			{'X', "-..-"},
			{'Y', "-.--"},
			{'Z', "--.."},

			{'0', "-----"},
			{'1', ".----"},
			{'2', "..---"},
			{'3', "...--"},
			{'4', "....-"},
			{'5', "....."},
			{'6', "-...."},
			{'7', "--..."},
			{'8', "---.."},
			{'9', "----."},

			{'&', ".-..."},
			{'\'', ".----."},
			{'@', ".--.-."},
			{')', "-.--.-"},
			{'(', "-.--."},
			{':', "---..."},
			{',', "--..--"},
			{'=', "-...-"},
			{'!', "-.-.--"},
			{'.', ".-.-.-"},
			{'-', "-....-"},
			{'×', "-..-"},
			{'%', "------..-.-----"},
			{'+', ".-.-."},
			{'"', ".-..-."},
			{'?', "..--.."},
			{'/', "-..-."},
		};

		public struct CodePoint
		{ 
			public readonly string code;
			public readonly string meaning;
			public readonly string morse;

			internal CodePoint(string code, string meaning, string morse) : this()
			{
				this.code = code;
				this.meaning = meaning;
				this.morse = morse;
			}
		}

		public static class PROSIGN
		{
			private static readonly Dictionary<string, CodePoint> TABLE = new Dictionary<string, CodePoint>();

			internal static void Register(string code, string meaning)
			{
				TABLE[code] = new CodePoint(code, meaning, EncodeFromSymbol(code));
			}

			public static bool ContainsKey(string k) => TABLE.ContainsKey(k);
			public static string Code(string k) => TABLE[k].morse;
		}

		public static class QCODE
		{
			private static readonly Dictionary<string, CodePoint> TABLE = new Dictionary<string, CodePoint>();

			internal static void Register(string code, string meaning)
			{
				TABLE[code] = new CodePoint(code, meaning, EncodeFromSymbol(code));
			}

			public static bool ContainsKey(string k) => TABLE.ContainsKey(k);
			public static string Code(string k) => TABLE[k].morse;
		}

		public static class ABBREV
		{
			private static readonly Dictionary<string, CodePoint> TABLE = new Dictionary<string, CodePoint>();

			internal static void Register(string code, string meaning)
			{
				TABLE[code] = new CodePoint(code, meaning, EncodeFromSymbol(code));
			}

			public static bool ContainsKey(string k) => TABLE.ContainsKey(k);
			public static string Code(string k) => TABLE[k].morse;
		}

		private readonly ISettings settings;
		private string text = "";
		private string sourcecode = "";
		private readonly List<Code> morsecode = new List<Code>();

		public string Text => this.text;
		public string SourceCode => this.sourcecode;
		public List<Code> MorseCode => this.morsecode;

		public Morse(ISettings settings)
		{
			this.settings = settings;
			this.morsecode.Clear();
			this.text = "";
			this.sourcecode = "";

			PROSIGN.Register("AA", "New line");
			PROSIGN.Register("AR", "End of message");
			PROSIGN.Register("AS", "Wait");
			PROSIGN.Register("BK", "Break");
			PROSIGN.Register("BT", "New paragraph (also =)");
			PROSIGN.Register("CL", "Going off the air ('clear')");
			PROSIGN.Register("CT", "Start copying");
			PROSIGN.Register("DO", "Change to wabun code");
			PROSIGN.Register("KA", "Starting signal");
			PROSIGN.Register("KN", "Invite a specific station to transmit");
			PROSIGN.Register("SK", "End of transmission");
			PROSIGN.Register("VA", "End of transmission");
			PROSIGN.Register("VE", "Understood");
			PROSIGN.Register("SN", "Understood");
			PROSIGN.Register("SOS", "Distress message");
			PROSIGN.Register("HH", "ERROR");

			QCODE.Register("QRL?", "Is the frequency in use?");
			QCODE.Register("QRL", "The frequency is in use");
			QCODE.Register("QRM?", "Is my transmission being interfered with?");
			QCODE.Register("QRM", "Your transmission is being interfered with (1-5)");
			QCODE.Register("QRN?", "Are you troubled by static?");
			QCODE.Register("QRN", "I am troubled by static (1-5)");
			QCODE.Register("QRO?", "Shall I increase transmitter power?");
			QCODE.Register("QRO", "Increase transmitter power");
			QCODE.Register("QRP?", "Shall I decrease transmitter power?");
			QCODE.Register("QRP", "Decrease transmitter power");
			QCODE.Register("QRQ?", "Shall I send faster?");
			QCODE.Register("QRQ", "Send faster (...words per minute)");
			QCODE.Register("QRS?", "Shall I send more slowly?");
			QCODE.Register("QRS", "Send more slowly (...words per minute)");
			QCODE.Register("QRT?", "Shall I stop sending?");
			QCODE.Register("QRT", "Stop sending");
			QCODE.Register("QRU?", "Have you anything for me?");
			QCODE.Register("QRU", "I have nothing for you");
			QCODE.Register("QRV?", "Are you ready to copy?");
			QCODE.Register("QRV", "I am ready to copy");
			QCODE.Register("QRX?", "Should I wait?");
			QCODE.Register("QRX", "Wait");
			QCODE.Register("QRZ?", "Who is calling me?");
			QCODE.Register("QRZ", "You are being called by...");
			QCODE.Register("QSB?", "Are my signals fading?");
			QCODE.Register("QSB", "Your signals are fading");
			QCODE.Register("QSL?", "Do you acknowledge?");
			QCODE.Register("QSL", "I acknowledge receipt");
			QCODE.Register("QTH?", "What is your location?");
			QCODE.Register("QTH", "My location is...");

			ABBREV.Register("73", "Best regards");
			ABBREV.Register("88", "Love and kisses");
			ABBREV.Register("BCNU", "Be seeing you");
			ABBREV.Register("CQ", "Call to all stations");
			ABBREV.Register("CS", "Call sign (request)");
			ABBREV.Register("CUL", "See you later");
			ABBREV.Register("DE", "From (or 'this is')");
			ABBREV.Register("ES", "And");
			ABBREV.Register("K", "Over (invitation to transmit)");
			ABBREV.Register("OM", "Old man");
			ABBREV.Register("R", "Received / Roger");
			ABBREV.Register("RST", "Signal report");
			ABBREV.Register("UR", "You are");
		}

		public IEnumerator PlayCoroutine(IPlay ifc)
		{
			foreach (Morse.Code c in this.morsecode)
			{
				switch (c) {
					case Morse.Code.dih:
						Log.dbg("Morse Code DIH");
						ifc.RisingEdge();
						yield return new WaitForSeconds (settings.ditDuration);
						break;
					case Morse.Code.dah:
						Log.dbg("Morse Code DAH");
						ifc.RisingEdge();
						yield return new WaitForSeconds (settings.dahDuration);
						break;
					case Morse.Code.letterspc:
						Log.dbg("Morse Code <>");
						ifc.FallingEdge();
						yield return new WaitForSeconds (settings.letterSpaceDuration);
						break;
					case Morse.Code.wordspc:
						Log.dbg("Morse Code < >");
						ifc.FallingEdge();
						yield return new WaitForSeconds (settings.wordSpaceDuration);
						break;
					case Morse.Code.symspc:
						Log.dbg("Morse Code <.>");
						ifc.FallingEdge();
						yield return new WaitForSeconds (settings.symbolSpaceDuration);
						break;
					}
			}
		}

		public void Compile(string sourcecode)
		{
			this.morsecode.Clear();
			this.text = "";
			this.sourcecode = sourcecode;
			foreach (char c in sourcecode)
			{
				switch (c) {
					case '.':
						this.morsecode.Add(Morse.Code.dih);
						break;
					case '_':
						this.morsecode.Add(Morse.Code.dah);
						break;
					case '-':
						this.morsecode.Add(Morse.Code.dah);
						break;
					case ' ':
						this.morsecode.Add(Morse.Code.letterspc);
						break;
					case '|':
						this.morsecode.Add(Morse.Code.wordspc);
						break;
					default:
						break;
				}
			}
			this.morsecode.Add(Morse.Code.symspc);
		}

		public void EncodeFromAny(string source)
		{
			HashSet<char> allowed = new HashSet<char>
			{
				{ '.' },
				{ '-' },
				{ '_' },
				{ ' ' },
				{ this.settings.letterSpaceChar },
				{ this.settings.wordSpaceChar }
			};
			foreach (char c in source)
				if (!allowed.Contains(c))
				{
					this.EncodeFromText(source);
					return;
				}
			this.Compile(source);
		}

		public string EncodeFromText(string source)
		{
			source = Normalize(source);
			string r = EncodeFromText(source, this.settings);
			this.Compile(r);
			this.text = source;
			return this.text;
		}

		private static string EncodeFromText(string source, ISettings settings)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in source)
			{
				bool symbol = false;
				bool wordSpc = false;
				StringBuilder symb = new StringBuilder();
				switch (c)
				{
					case ' ' :
						sb.Append(settings.wordSpaceChar);
						wordSpc = true;
						break;
					case '<':
						symbol = true;
						wordSpc = false;
						break;
					case '>':
						if (symbol)
						{
							string sym = symb.ToString();
							symb = new StringBuilder();
							string s = null;
							if (QCODE.ContainsKey(sym)) s = QCODE.Code(sym);
							else if (PROSIGN.ContainsKey(sym)) s = PROSIGN.Code(sym);
							else if (ABBREV.ContainsKey(sym)) s = ABBREV.Code(sym);
							if (null != s)
							{
								if (!wordSpc) sb.Append(settings.wordSpaceChar);
								sb.Append(s);
								sb.Append(settings.wordSpaceChar);
							}
						}
						symbol = false;
						wordSpc = false;
						break;
					default:
						if (symbol)
							symb.Append(c);
						else if (TABLE.ContainsKey(c))
						{
							sb.Append(TABLE[c]);
							sb.Append(settings.letterSpaceChar);
						}
						break;
				}
			}
			return sb.ToString();
		}

		private static string EncodeFromSymbol(string source)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in source) if (TABLE.ContainsKey(c))
				sb.Append(TABLE[c]);
			return sb.ToString();
		}

		private static readonly Regex NORMALIZE = new Regex("^[a-zA-Z0-9&\\@():,=!.%+\"?/ -_]");
		private static string Normalize(string text)
		{
			text = Encoding.UTF8.GetString(
					Encoding.ASCII.GetBytes(text.ToUpper())
				);
			text = NORMALIZE.Replace(text, "");
			return text;
		}
	}
}
