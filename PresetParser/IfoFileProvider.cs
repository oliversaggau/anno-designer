using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace PresetParser
{
    public class IfoFileProvider : IIfoFileProvider
    {
        public XmlDocument GetIfoFileContent(string basePath, string variationFilename, string annoVersion)
        {
            var result = new XmlDocument();

            var pathToFileDirectory = Path.Combine(Path.GetDirectoryName(variationFilename), Path.GetFileNameWithoutExtension(variationFilename));
            var pathToFile = Path.Combine(basePath + "/", string.Format("{0}.ifo", pathToFileDirectory));

            if (File.Exists(pathToFile))
            {
                if (annoVersion.Equals(Constants.ANNO_VERSION_117, StringComparison.OrdinalIgnoreCase))
                {
                    // some .ifo files in Anno 117 use non-standard XML end-tags </>
                    // we need to check and fix those before loading the XmlDocument
                    string content = File.ReadAllText(pathToFile);

                    if (content.Contains("</>"))
                    {
                        content = FixNonStandardEndTags(content);
                    }

                    using (TextReader reader = new StringReader(content))
                    {
                        result.Load(reader);
                    }
                }
                else
                {
                    result.Load(pathToFile);
                }

                return result;
            }

            return result;
        }

        private static string FixNonStandardEndTags(string input)
        {
            Regex tags = new Regex(@"<([a-zA-Z0-9_]+)>", RegexOptions.Compiled);
            StringBuilder output = new StringBuilder();
            Stack<string> stack = new Stack<string>();
            string line;

            using (StringReader reader = new StringReader(input))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    Match match = tags.Match(line);

                    if (match.Success)
                    {
                        if (!line.TrimEnd().EndsWith("/>")) stack.Push(match.Groups[1].Value);
                        else line = line.Replace("</>", $"</{match.Groups[1].Value}>");
                    }
                    else if (line.Trim() == "</>")
                    {
                        line = $"</{stack.Pop()}>";
                    }

                    output.AppendLine(line);
                }
            }

            return output.ToString();
        }
    }
}
