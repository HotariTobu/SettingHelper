using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace SettingHelper
{
    public class Templates
    {
        public string Extension { get; }
        public ContainerTemplates ContainerTemplates { get; }
        public IEnumerable<TypeTemplate> Types { get; }

        public string BaseTemplate { get; }

        public string ListRegistries(string text, Container container)
        {
            foreach (Container child in container.Containers)
            {
                text += $"\r\n{ContainerTemplates.ComposeAccessorClass(child)}\r\n\r\n{ContainerTemplates.ComposeRegistryClass(child)}\r\n";
                text = ListRegistries(text, child);
            }

            return text;
        }

        public Templates()
        {
            try
            {
                Extension = File.ReadAllText("Extension.txt");
            }
            catch
            {
                Extension = ".cs";
            }

            try
            {
                BaseTemplate = File.ReadAllText($"Templates\\BaseTemplate{Extension}");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.StackTrace);
            }

            if (string.IsNullOrWhiteSpace(BaseTemplate))
            {
                quit("Templatesが見つからないか、BaseTemplateが見つからないか、BaseTemplateが空でした。");
            }

            ContainerTemplates = new ContainerTemplates(Extension);

            DirectoryInfo TypesInfo = new DirectoryInfo("Types");
            if (TypesInfo.Exists)
            {
                List<TypeTemplate> typeList = new List<TypeTemplate>();

                foreach (FileInfo fileInfo in TypesInfo.EnumerateFiles($"*{Extension}", SearchOption.AllDirectories))
                {
                    try
                    {
                        using StreamReader reader = fileInfo.OpenText();
                        typeList.Add(new TypeTemplate(Path.GetFileNameWithoutExtension(fileInfo.Name), reader.ReadLine(), reader.ReadLine(), reader.ReadToEnd()));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, e.StackTrace);
                    }
                }

                if (!typeList.Any())
                {
                    quit("Typesが空でした。");
                }

                Types = typeList;
            }
            else
            {
                quit("Typesが見つかりませんでした。");
            }

            void quit(string message)
            {
                MessageBox.Show(message);
                Application.Current.Shutdown();
            }
        }
    }

    public class ContainerTemplates
    {
        public ContainerTemplates(string extension)
        {
            DirectoryInfo TemplatesInfo = new DirectoryInfo("Templates");
            if (TemplatesInfo.Exists)
            {
                foreach (FileInfo fileInfo in TemplatesInfo.EnumerateFiles($"*{extension}", SearchOption.AllDirectories))
                {
                    try
                    {
                        using StreamReader reader = fileInfo.OpenText();

                        switch (Path.GetFileNameWithoutExtension(fileInfo.Name))
                        {
                            case "ContainerAccessorsTemplate":
                                Accessor = reader.ReadLine();
                                AccessorClass = reader.ReadToEnd();
                                break;

                            case "ContainerRegistriesTemplate":
                                Registry = reader.ReadLine();
                                RegistryClass = reader.ReadToEnd();
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, e.StackTrace);
                    }
                }
            }
        }

        private string Accessor { get; }
        private string AccessorClass { get; }
        private string Registry { get; }
        private string RegistryClass { get; }

        public string ComposeAccessor(string name, string ContainerName) => Accessor?.Replace("$Name$", name)?.Replace("$Container$", ContainerName);
        public string ComposeAccessorClass(Container container) => AccessorClass?.Replace("$Name$", container.Name)?.Replace("$Accessors$", ListAccessors(container));
        public string ComposeRegistry(string name) => Registry?.Replace("$Name$", name);
        public string ComposeRegistryClass(Container container) => RegistryClass?.Replace("$Name$", container.Name)
            ?.Replace("$Containers$", string.Join("\r\n", container.Containers.Select(child => ComposeRegistry(child.Name))))
            ?.Replace("$Items$", string.Join("\r\n", container.Items.Select(item => item.Type.ComposeRegistry(item.Name, item.Value))));

        public string ListAccessors(Container parent)
        {
            string result = string.Empty;

            foreach (Container container in parent.Containers)
            {
                result += $"{ComposeAccessor(container.Name, parent.Name)}\r\n";
            }

            foreach (Item item in parent.Items)
            {
                result += $"\r\n{item.Type.ComposeAccessor(item.Name, parent.Name)}";
            }

            return result;
        }
    }

    public class TypeTemplate
    {
        public TypeTemplate(string name, string registry, string accessor, string definition)
        {
            Name = name;
            Accessor = accessor;
            Registry = registry;
            Definition = definition;
        }

        public string Name { get; }
        private string Accessor { get; }
        private string Registry { get; }
        public string Definition { get; }

        public string ComposeAccessor(string name, string ContainerName) => Accessor?.Replace("$Name$", name)?.Replace("$Container$", ContainerName);
        public string ComposeRegistry(string name, string value) => Registry?.Replace("$Name$", name)?.Replace("$Value$", value);
    }
}
