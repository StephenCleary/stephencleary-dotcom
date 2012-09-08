using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace StephenCleary.Helpers
{
    public static class CSharpHtmlHelper
    {
        private static readonly string[] Keywords = new string[]
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal",
            "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null",
            "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte",
            "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof",
            "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while",
        };

        private static readonly string[] ContextualKeywords = new string[]
        {
            "add", "alias", "ascending", "async", "await", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let",
            "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield",
        };

        private static readonly Regex Whitespace = new Regex(@"^(\s+)");
        private static readonly Regex Comment = new Regex(@"^(//.*)");
        private static readonly Regex Identifier = new Regex(@"^([A-Za-z_@][A-Za-z0-9]*)");
        private static readonly Regex Character = new Regex(@"^('.+')");
        private static readonly Regex String = new Regex(@"^("".+"")");
        private static readonly Regex VerbatimString = new Regex(@"^(@""(?:[^""]+|"""")+"")");
        private static readonly Regex HighlightedLine = new Regex(@"^`\*");
        private static readonly Regex HighlightedSpan = new Regex(@"^`!");
        private static readonly Regex TypeIdentifier = new Regex(@"^`([A-Za-z_@][A-Za-z0-9]*)`");

        private static readonly Regex[] Regexes = new Regex[]
        {
            Whitespace, Comment, Identifier, Character, String, VerbatimString, HighlightedLine, HighlightedSpan, TypeIdentifier,
        };

        private static IEnumerable<Tuple<string, Regex>> Tokenize(string line)
        {
            int index = 0;
            while (index < line.Length)
            {
                var result = Regexes.Select(regex => new { regex, match = regex.Match(line.Substring(index)) }).FirstOrDefault(x => x.match.Success);
                if (result == null)
                {
                    yield return Tuple.Create(line.Substring(index, 1), (Regex)null);
                    ++index;
                }
                else
                {
                    if (result.match.Groups.Count > 1)
                        yield return Tuple.Create(result.match.Groups[1].Value, result.regex);
                    else
                        yield return Tuple.Create(result.match.Value, result.regex);
                    index += result.match.Length;
                }
            }
        }

        private static string FormatCSharpCode(this HtmlHelper @this, string code)
        {
            var sb = new StringBuilder();
            var lines = code.Replace("\r\n", "\n").Trim().Split('\n');
            foreach (var line in lines)
            {
                bool lineHighlighted = false;
                bool inHighlightSpan = false;

                // Tokenize the string until we reach the end.
                var tokens = Tokenize(line).ToArray();
                for (var i = 0; i != tokens.Length; ++i)
                {
                    var token = tokens[i];
                    if (token.Item2 == null || token.Item2 == Whitespace)
                        sb.Append(@this.Encode(token.Item1));
                    else if (token.Item2 == Comment)
                        sb.Append("<span class=\"comment\">" + @this.Encode(token.Item1) + "</span>");
                    else if (token.Item2 == Character || token.Item2 == String || token.Item2 == VerbatimString)
                        sb.Append("<span class=\"string\">" + @this.Encode(token.Item1) + "</span>");
                    else if (token.Item2 == TypeIdentifier)
                        sb.Append("<span class=\"type\">" + @this.Encode(token.Item1) + "</span>");
                    else if (token.Item2 == HighlightedLine)
                    {
                        lineHighlighted = true;
                        sb.Append("<code class=\"highlight\">");
                    }
                    else if (token.Item2 == HighlightedSpan)
                    {
                        if (inHighlightSpan)
                            sb.Append("</span>");
                        else
                            sb.Append("<span class=\"highlight\">");
                        inHighlightSpan = !inHighlightSpan;
                    }
                    else if (token.Item2 == Identifier)
                    {
                        if (Keywords.Concat(ContextualKeywords).Contains(token.Item1))
                            sb.Append("<span class=\"keyword\">" + @this.Encode(token.Item1) + "</span>");
                        else
                            sb.Append(@this.Encode(token.Item1));
                    }
                    else
                        throw new NotImplementedException();
                }

                if (lineHighlighted)
                    sb.Append("</code>");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static HtmlString CSharp(this HtmlHelper @this, string code)
        {
            var sb = new StringBuilder();
            if (code.Contains('\n'))
            {
                sb.Append("<pre><code class=\"csharp\">");
                sb.Append(FormatCSharpCode(@this, code));
                sb.Append("</code></pre>");
            }
            else
            {
                sb.Append("<code class=\"csharp\">");
                sb.Append(FormatCSharpCode(@this, code).Trim());
                sb.Append("</code>");
            }

            return new HtmlString(sb.ToString());
        }
    }
}