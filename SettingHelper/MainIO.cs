using SharedWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SettingHelper
{
    class MainIO
    {
        private Templates Templates { get; }
        public IEnumerable<TypeTemplate> Types { get; }
        private string Path { set; get; }

        private ObservableCollection<Container> TreeContent { get; }
        private Container Root
        {
            get => TreeContent.FirstOrDefault();
            set
            {
                while (TreeContent.Count > 1)
                {
                    TreeContent.RemoveAt(0);
                }

                if (TreeContent.Count == 0)
                {
                    TreeContent.Add(value);
                }
                else
                {
                    TreeContent[0] = value;
                }
            }
        }

        public MainIO(ObservableCollection<Container> treeContent)
        {
            Templates = new Templates();
            Types = Templates.Types;
            Path = System.IO.Path.GetFullPath("Setting.xml");

            TreeContent = treeContent;
        }

        public void Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (File.Exists(path) && XDocument.Load(path)?.Root is XElement root)
            {
                Root = new Container(null, root.Name.LocalName);
                XMLToTree(root, Root);
                Root.IsSelected = true;
                Path = path;
            }
        }

        public void Save()
        {
            XElement root = new XElement(Root.Name);
            TreeToXML(root, Root);
            new XDocument(root).Save(Path);
        }

        public void Export()
        {
            Container root = Extract(Root);
            HashSet<TypeTemplate> typeList = new HashSet<TypeTemplate>();
            ScanTypes(typeList, root);
            File.WriteAllText(System.IO.Path.ChangeExtension(Path, Templates.Extension),
            Templates.BaseTemplate.Replace("$Namespace$", root.Name)
            .Replace("$Accessors$", Templates.ContainerTemplates.ListAccessors(root))
            .Replace("$Registries$", $"\r\n{string.Join("\n", Templates.ContainerTemplates.ComposeRegistryClass(root).Split("\n").Where(line => !line.Contains("$RootSkip$")))}\r\n{Templates.ListRegistries(string.Empty, root).Replace("$RootSkip$", "")}")
            .Replace("$Definitions$", string.Join("\r\n\r\n", typeList.Select(type => type.Definition))));
        }

        private void XMLToTree(XElement element, Container parent)
        {
            if (int.TryParse(element.Attribute("Count").Value, out int count))
            {
                IEnumerable<XElement> elements = element.Elements();

                foreach (XElement child in elements.Take(count))
                {
                    Container container = new Container(parent, child.Name.LocalName);
                    XMLToTree(child, container);
                    parent.Containers.Add(container);
                }

                foreach (XElement child in elements.Skip(count))
                {
                    string typeName = child.Attribute("Type").Value;
                    parent.Items.Add(new Item(parent, child.Name.LocalName, Types.FirstOrDefault(type => type.Name.Equals(typeName)), child.Value));
                }
            }
        }

        private void TreeToXML(XElement element, Container parent)
        {
            int count = 0;

            foreach (Container container in parent.Containers)
            {
                if (!string.IsNullOrWhiteSpace(container.Name))
                {
                    XElement child = new XElement(container.Name);
                    element.Add(child);
                    TreeToXML(child, container);
                    count++;
                }
            }

            element.SetAttributeValue("Count", count);

            foreach (Item item in parent.Items)
            {
                if (!string.IsNullOrWhiteSpace(item.Name))
                {
                    XElement child = new XElement(item.Name);
                    element.Add(child);
                    child.Value = item.Value;
                    child.SetAttributeValue("Type", item.Type.Name);
                }
            }
        }

        private Container Extract(Container parent)
        {
            Container result = new Container(parent, parent.Name);
            result.Containers.AddRange(parent.Containers.Where(container => container.IsValid).Select(child => Extract(child)));
            result.Items.AddRange(parent.Items.Where(item => item.IsValid));
            return result;
        }

        private void ScanTypes(HashSet<TypeTemplate> typeList, Container container)
        {
            foreach (Container child in container.Containers)
            {
                ScanTypes(typeList, child);
            }

            foreach (Item item in container.Items)
            {
                typeList.Add(item.Type);
            }
        }
    }
}
