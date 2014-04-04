using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace StephenCleary.Helpers
{
    public static class XmlFormatter
    {
        public static string Format(string xml)
        {
            var ret = new StringWriter();
            using (var escaper = new HtmlEscapingTextWriter(ret))
            using (var source = new StringReader(xml))
            using (var reader = XmlReader.Create(source))
            using (var writer = new BufferedWriter())
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            ret.Write(@"<span class=""keyword"">&lt;</span>");

                            writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                            writer.WriteAttributes(reader, true);
                            if (reader.IsEmptyElement)
                            {
                                writer.WriteEndElement();
                            }
                            writer.Flush();
                            break;
                        case XmlNodeType.Text:
                            writer.WriteString(reader.Value);
                            writer.Flush();
                            break;
                        case XmlNodeType.Whitespace:
                        case XmlNodeType.SignificantWhitespace:
                            writer.WriteWhitespace(reader.Value);
                            writer.Flush();
                            break;
                        case XmlNodeType.CDATA:
                            writer.WriteCData(reader.Value);
                            writer.Flush();
                            break;
                        case XmlNodeType.EntityReference:
                            writer.WriteEntityRef(reader.Name);
                            writer.Flush();
                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                            writer.WriteProcessingInstruction(reader.Name, reader.Value);
                            writer.Flush();
                            break;
                        case XmlNodeType.DocumentType:
                            writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                            writer.Flush();
                            break;
                        case XmlNodeType.Comment:
                            writer.WriteComment(reader.Value);
                            writer.Flush();
                            break;
                        case XmlNodeType.EndElement:
                            writer.WriteFullEndElement();
                            writer.Flush();
                            break;
                    }
                }
            }
        }

        private sealed class BufferedWriter : TextWriter
        {
            private StringWriter _writer;

            public BufferedWriter()
            {
                _writer = new StringWriter();
            }

            public override Encoding Encoding
            {
                get { return _writer.Encoding; }
            }

            public override void Write(char value)
            {
                _writer.Write(value);
            }

            public override void Write(char[] buffer)
            {
                _writer.Write(buffer);
            }

            public override void Write(char[] buffer, int index, int count)
            {
                _writer.Write(buffer, index, count);
            }

            public override void Write(string value)
            {
                _writer.Write(value);
            }

            public string Reset()
            {
                _writer.Flush();
                var ret = _writer.ToString();
                _writer = new StringWriter();
                return ret;
            }
        }

        private sealed class HtmlEscapingTextWriter : TextWriter
        {
            private readonly TextWriter _writer;

            public HtmlEscapingTextWriter(TextWriter writer)
            {
                _writer = writer;
            }

            public override Encoding Encoding
            {
                get { return _writer.Encoding; }
            }

            public override void Write(char value)
            {
                _writer.Write(HttpUtility.HtmlEncode(value.ToString()));
            }

            public override void Write(string value)
            {
                _writer.Write(HttpUtility.HtmlEncode(value));
            }
        }
    }

#if NO
    public class XmlFormatter : XmlWriter
    {
        private readonly TextWriter _writer;
        private readonly HtmlEscapingTextWriter _htmlWriter;
        private readonly XmlWriter _xmlWriter;

        public XmlFormatter(TextWriter writer)
        {
            _writer = writer;
            _htmlWriter = new HtmlEscapingTextWriter(_writer);
            _xmlWriter = XmlWriter.Create(_htmlWriter);
        }

        public override void Flush()
        {
        }

        public override string LookupPrefix(string ns)
        {
            return _xmlWriter.LookupPrefix(ns);
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            _xmlWriter.WriteBase64(buffer, index, count);
            _xmlWriter.Flush();
        }

        public override void WriteCData(string text)
        {
            _xmlWriter.WriteCData(text);
            _xmlWriter.Flush();
        }

        public override void WriteCharEntity(char ch)
        {
            throw new NotImplementedException();
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override void WriteComment(string text)
        {
            throw new NotImplementedException();
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            throw new NotImplementedException();
        }

        public override void WriteEndAttribute()
        {
            throw new NotImplementedException();
        }

        public override void WriteEndDocument()
        {
            throw new NotImplementedException();
        }

        public override void WriteEndElement()
        {
            throw new NotImplementedException();
        }

        public override void WriteEntityRef(string name)
        {
            throw new NotImplementedException();
        }

        public override void WriteFullEndElement()
        {
            throw new NotImplementedException();
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            throw new NotImplementedException();
        }

        public override void WriteRaw(string data)
        {
            throw new NotImplementedException();
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            throw new NotImplementedException();
        }

        public override void WriteStartDocument(bool standalone)
        {
            throw new NotImplementedException();
        }

        public override void WriteStartDocument()
        {
            throw new NotImplementedException();
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            throw new NotImplementedException();
        }

        public override WriteState WriteState
        {
            get { throw new NotImplementedException(); }
        }

        public override void WriteString(string text)
        {
            throw new NotImplementedException();
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            throw new NotImplementedException();
        }

        public override void WriteWhitespace(string ws)
        {
            throw new NotImplementedException();
        }
    }
#endif
}