using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace $Namespace$
{
    public class Setting
    {
        private string Path { get; }

        public Setting(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException();
            }

            Path = path;

            try
            {
                if (XDocument.Load(path)?.Root is XElement root)
                {
                    XMLToTree(root, ___$Namespace$);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public bool Save() => Save(Path);
        public bool Save(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            try
            {
                XElement root = new XElement("$Namespace$");
                TreeToXML(root, ___$Namespace$);
                new XDocument(root).Save(path);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }

        private void XMLToTree(XElement element, ISettingContainer container)
        {
            foreach (string key in container.Containers.Keys)
            {
                XElement child = element.Element(key);
                if (child != null)
                {
                    XMLToTree(child, container.Containers[key]);
                }
            }

            foreach (string key in container.Items.Keys)
            {
                XElement child = element.Element(key);
                if (child != null)
                {
                    container.Items[key].Str = child.Value;
                }
            }
        }

        private void TreeToXML(XElement element, ISettingContainer container)
        {
            foreach (string key in container.Containers.Keys)
            {
                XElement child = new XElement(key);
                element.Add(child);
                TreeToXML(child, container.Containers[key]);
            }

            foreach (string key in container.Items.Keys)
            {
                XElement child = new XElement(key);
                element.Add(child);
                child.Value = container.Items[key].Str;
            }
        }

$Accessors$

        private readonly __$Namespace$ ___$Namespace$ = new __$Namespace$();

$Registries$

        private class ISettingContainer
        {
            public Dictionary<string, ISettingContainer> Containers { get; protected set; }
            public Dictionary<string, ISettingValue> Items { get; protected set; }
        }

        private interface ISettingValue
        {
            public object Obj { get; set; }
            public string Str { get; set; }
        }

$Definitions$
    }
}