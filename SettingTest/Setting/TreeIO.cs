using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SettingTest.Setting
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
                    XMLToTree(root, ___Tree);
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
                XElement root = new XElement("Tree");
                TreeToXML(root, ___Tree);
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

            public _FFF FFF => ((__FFF)___Tree.Containers["FFF"])._FFF;

            public bool AAA { get => (bool)___Tree.Items["AAA"].Obj; set => ___Tree.Items["AAA"].Obj = value; }

        private readonly __Tree ___Tree = new __Tree();


        private class __Tree : ISettingContainer
        {
            public __Tree()
            {
                Containers = new Dictionary<string, ISettingContainer>()
                {
                    { "FFF", new __FFF() },
                };

                Items = new Dictionary<string, ISettingValue>()
                {
                    { "AAA", new SettingValueBool(false) },
                };
            }
        }

        public class _FFF
        {
            public _FFF(object obj) => ___FFF = (__FFF)obj;
            private readonly __FFF ___FFF;


            public byte BBB { get => (byte)___FFF.Items["BBB"].Obj; set => ___FFF.Items["BBB"].Obj = value; }
        }

        private class __FFF : ISettingContainer
        {
            public __FFF()
            {
                Containers = new Dictionary<string, ISettingContainer>()
                {

                };

                Items = new Dictionary<string, ISettingValue>()
                {
                    { "BBB", new SettingValueByte(200) },
                };

                _FFF = new _FFF(this);
            }

            public readonly _FFF _FFF;
        }


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

        private class SettingValueByte : ISettingValue
        {
            public SettingValueByte(byte value = 0) => Value = value;
            private byte Value;

            public object Obj { get => Value; set => Value = (byte)value; }
            public string Str { get => Value.ToString(); set => byte.TryParse(value, out Value); }
        }

        private class SettingValueBool : ISettingValue
        {
            public SettingValueBool(bool value = false) => Value = value;
            private bool Value;

            public object Obj { get => Value; set => Value = (bool)value; }
            public string Str { get => Value.ToString(); set => bool.TryParse(value, out Value); }
        }
    }
}