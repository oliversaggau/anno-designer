using System;
using System.Collections.Generic;
using System.Xml;

namespace PresetParser.Models
{
    internal class Asset
    {
        private readonly XmlNode asset;

        public Asset(XmlNode asset, XmlDocument assets, XmlDocument templates)
        {
            this.asset = asset;

            if (asset["BaseAssetGUID"] != null)
            {
                string baseAssetGuid = asset["BaseAssetGUID"].InnerText;
                this.BaseAsset = assets.SelectSingleNode($"//Asset[Values/Standard/GUID[text()='{baseAssetGuid}']]");
                if (BaseAsset == null) throw new BaseAssetMissingException();
                this.TemplateName = BaseAsset["Template"].InnerText;
            }
            else if (asset["Template"] != null)
            {
                this.TemplateName = asset["Template"].InnerText;
            }

            this.Template = !string.IsNullOrEmpty(TemplateName)
                ? templates.SelectSingleNode($"//Template[Name[text()='{TemplateName}']]")
                : null;
        }

        public XmlNode BaseAsset
        {
            get;
        }

        public XmlNode Template
        {
            get;
        }

        public string TemplateName
        {
            get;
        }

        public string GetValue(string path)
        {
            return SelectNode(path)?.InnerText;
        }

        public bool TryGetValue(string path, out string result)
        {
            result = GetValue(path);
            return !string.IsNullOrEmpty(result);
        }

        public XmlNode SelectNode(string path)
        {
            XmlNode result = asset["Values"].SelectSingleNode(path);
            if (result == null && BaseAsset != null) result = BaseAsset["Values"]?.SelectSingleNode(path);
            if (result == null) result = Template["Properties"]?.SelectSingleNode(path);
            return result;
        }

        public IEnumerable<XmlNode> SelectNodes(string path)
        {
            XmlNodeList result = asset["Values"].SelectNodes(path);
            if (result == null && BaseAsset != null) result = BaseAsset["Values"]?.SelectNodes(path);
            if (result == null) result = Template["Properties"]?.SelectNodes(path);

            if (result != null)
            {
                foreach (XmlNode node in result)
                {
                    yield return node;
                }
            }
        }
    }

    internal class BaseAssetMissingException : InvalidOperationException
    {
        public BaseAssetMissingException()
        {
        }
    }
}
