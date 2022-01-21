using System;
using System.Text.RegularExpressions;

namespace DataverseGen.Core
{
    /// <summary>
    ///     Console Color Helper class that provides coloring to individual commands
    /// </summary>
    public static class ColorConsole
    {
        /// <summary>
        ///     Write with color
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void Write(string text, ConsoleColor? color = null)
        {
            if (color.HasValue)
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                if (color == oldColor)
                {
                    Console.Write(text);
                }
                else
                {
                    Console.ForegroundColor = color.Value;
                    Console.Write(text);
                    Console.ForegroundColor = oldColor;
                }
            }
            else
            {
                Console.Write(text);
            }
        }

        /// <summary>
        ///     Writes out a line with color specified as a string
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="color">A console color. Must match ConsoleColors collection names (case insensitive)</param>
        public static void Write(string text, string color)
        {
            if (string.IsNullOrEmpty(color))
            {
                Write(text);
                return;
            }

            if (!Enum.TryParse(color, true, out ConsoleColor col))
            {
                Write(text);
            }
            else
            {
                Write(text, col);
            }
        }

        /// <summary>
        ///     WriteLine with color
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void WriteLine(string text, ConsoleColor? color = null)
        {
            if (color.HasValue)
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                if (color == oldColor)
                {
                    Console.WriteLine(text);
                }
                else
                {
                    Console.ForegroundColor = color.Value;
                    Console.WriteLine(text);
                    Console.ForegroundColor = oldColor;
                }
            }
            else
            {
                Console.WriteLine(text);
            }
        }

        /// <summary>
        ///     Writes out a line with a specific color as a string
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="color">A console color. Must match ConsoleColors collection names (case insensitive)</param>
        public static void WriteLine(string text, string color)
        {
            if (string.IsNullOrEmpty(color))
            {
                WriteLine(text);
                return;
            }

            if (!Enum.TryParse(color, true, out ConsoleColor col))
            {
                WriteLine(text);
            }
            else
            {
                WriteLine(text, col);
            }
        }

        #region Wrappers and Templates

        private static readonly Lazy<Regex> ColorBlockRegEx = new Lazy<Regex>(
            () => new Regex("\\[(?<color>.*?)\\](?<text>[^[]*)\\[/\\k<color>\\]",
                RegexOptions.IgnoreCase),
            true);

        /// <summary>
        ///     Allows a string to be written with embedded color values using:
        ///     This is [red]Red[/red] text and this is [cyan]Blue[/blue] text
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="baseTextColor">Base text color</param>
        public static void WriteEmbeddedColorLine(string text, ConsoleColor? baseTextColor = null)
        {
            if (baseTextColor == null)
            {
                baseTextColor = Console.ForegroundColor;
            }

            if (string.IsNullOrEmpty(text))
            {
                WriteLine(string.Empty);
                return;
            }

            int at = text.IndexOf("[", StringComparison.Ordinal);
            int at2 = text.IndexOf("]", StringComparison.Ordinal);
            if (at == -1 || at2 <= at)
            {
                WriteLine(text, baseTextColor);
                return;
            }

            while (true)
            {
                var match = ColorBlockRegEx.Value.Match(text);
                if (match.Length < 1)
                {
                    Write(text, baseTextColor);
                    break;
                }

                // write up to expression
                Write(text.Substring(0, match.Index), baseTextColor);

                // strip out the expression
                string highlightText = match.Groups["text"].Value;
                string colorVal = match.Groups["color"].Value;

                Write(highlightText, colorVal);

                // remainder of string
                text = text.Substring(match.Index + match.Value.Length);
            }

            Console.WriteLine();
        }

        /// <summary>
        ///     Writes a line of header text wrapped in a in a pair of lines of dashes:
        ///     -----------
        ///     Header Text
        ///     -----------
        ///     and allows you to specify a color for the header. The dashes are colored
        /// </summary>
        /// <param name="headerText">Header text to display</param>
        /// <param name="wrapperChar">wrapper character (-)</param>
        /// <param name="headerColor">Color for header text (yellow)</param>
        /// <param name="dashColor">Color for dashes (gray)</param>
        public static void WriteWrappedHeader(string headerText,
                                              char wrapperChar = '-',
                                              ConsoleColor headerColor = ConsoleColor.Yellow,
                                              ConsoleColor dashColor = ConsoleColor.DarkGray)
        {
            if (string.IsNullOrEmpty(headerText))
            {
                return;
            }

            string line = new string(wrapperChar, headerText.Length);

            WriteLine(line, dashColor);
            WriteLine(headerText, headerColor);
            WriteLine(line, dashColor);
        }

        #endregion Wrappers and Templates

        #region Success, Error, Info, Warning Wrappers

        /// <summary>
        ///     Write a Error Line - Red
        /// </summary>
        /// <param name="text">Text to write out</param>
        public static void WriteError(string text)
        {
            WriteLine(text, ConsoleColor.Red);
        }

        /// <summary>
        ///     Write a Info Line - dark cyan
        /// </summary>
        /// <param name="text">Text to write out</param>
        public static void WriteInfo(string text)
        {
            WriteLine(text, ConsoleColor.DarkCyan);
        }

        /// <summary>
        ///     Write a Success Line - green
        /// </summary>
        /// <param name="text">Text to write out</param>
        public static void WriteSuccess(string text)
        {
            WriteLine(text, ConsoleColor.Green);
        }

        /// <summary>
        ///     Write a Warning Line - Yellow
        /// </summary>
        /// <param name="text">Text to Write out</param>
        public static void WriteWarning(string text)
        {
            WriteLine(text, ConsoleColor.DarkYellow);
        }

        #endregion Success, Error, Info, Warning Wrappers
    }
}