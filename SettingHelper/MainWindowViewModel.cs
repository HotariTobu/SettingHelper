using SharedWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SettingHelper
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region == TreeContent ==

        private readonly ObservableCollection<Container> _TreeContent = new ObservableCollection<Container>();
        public ObservableCollection<Container> TreeContent => _TreeContent;

        #endregion
        #region == SelectedContainer ==

        private Container _SelectedContainer;
        public Container SelectedContainer
        {
            get => _SelectedContainer;
            set
            {
                if (_SelectedContainer != value)
                {
                    _SelectedContainer = value;
                    if (_SelectedContainer == null)
                    {
                        SetError(nameof(SelectedContainer), "Error: Null Reference");
                    }
                    else
                    {
                        ClearErrror(nameof(SelectedContainer));
                    }
                    RaisePropertyChanged(nameof(SelectedContainer));
                }
            }
        }

        #endregion
        #region == TypeList ==

        private readonly ObservableCollection<TypeTemplate> _TypeList = new ObservableCollection<TypeTemplate>();
        public ObservableCollection<TypeTemplate> TypeList => _TypeList;

        #endregion

        public MainWindowViewModel()
        {
            Container root = new Container(null, "Namespace");
            root.IsSelected = true;
            TreeContent.Add(root);
            SelectedContainer = root;
        }
    }

    public class Container : ViewModelBase
    {
        public Container(Container parent, string name = "")
        {
            Parent = parent;
            Name = name;
        }

        public Container Parent { get; set; }
        #region == Name ==

        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    if (_Name == null)
                    {
                        SetError(nameof(Name), "Error: Null Reference");
                    }
                    else
                    {
                        ClearErrror(nameof(Name));
                    }
                    RaisePropertyChanged(nameof(Name));
                    RaisePropertyChanged(nameof(IsValid));
                }
            }
        }

        #endregion
        #region == IsSelected ==

        private bool _IsSelected;
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    RaisePropertyChanged(nameof(IsSelected));
                }
            }
        }

        #endregion
        public bool IsValid => !(!Name.IsConsistedOfAlphabetAndUnderscore() || (Parent != null && (Parent.Containers.Where(x => x != this).Any(x => x.Name.Equals(Name)) || Parent.Items.Any(x => x.Name.Equals(Name)))));
        public bool IsEmpty => !(!string.IsNullOrWhiteSpace(Name) || Containers.Any(x => !x.IsEmpty) || Items.Any(x => !x.IsEmpty));
        public Container Copy(Container parent)
        {
            Container copy = new Container(parent, Name);
            copy.Containers.AddRange(Containers.Select(x => x.Copy(copy)));
            copy.Items.AddRange(Items.Select(x => x.Copy(copy)));
            return copy;
        }

        #region == Containers ==

        private readonly ObservableCollection<Container> _Containers = new ObservableCollection<Container>();
        public ObservableCollection<Container> Containers => _Containers;

        #endregion
        #region == Items ==

        private readonly ObservableCollection<Item> _Items = new ObservableCollection<Item>();
        public ObservableCollection<Item> Items => _Items;

        #endregion
    }

    public class Item : ViewModelBase
    {
        public Item(Container parent, string name = "", TypeTemplate type = null, string value = "")
        {
            Parent = parent;
            Name = name;
            Type = type;
            Value = value;
        }

        public Container Parent { get; set; }
        #region == Name ==

        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    if (_Name == null)
                    {
                        SetError(nameof(Name), "Error: Null Reference");
                    }
                    else
                    {
                        ClearErrror(nameof(Name));
                    }
                    RaisePropertyChanged(nameof(Name));
                    RaisePropertyChanged(nameof(IsValid));
                }
            }
        }

        #endregion
        #region == Type ==

        private TypeTemplate _Type;
        public TypeTemplate Type
        {
            get => _Type;
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    if (_Type == null)
                    {
                        SetError(nameof(Type), "Error: Null Reference");
                    }
                    else
                    {
                        ClearErrror(nameof(Type));
                    }
                    RaisePropertyChanged(nameof(Type));
                    RaisePropertyChanged(nameof(IsValid));
                }
            }
        }

        #endregion
        public string Value { get; set; }
        public bool IsValid => !(!Name.IsConsistedOfAlphabetAndUnderscore() || Type == null || (Parent != null && (Parent.Containers.Any(x => x.Name.Equals(Name)) || Parent.Items.Where(x => x != this).Any(x => x.Name.Equals(Name)))));
        public bool IsEmpty => string.IsNullOrWhiteSpace(Name) && Type == null && string.IsNullOrWhiteSpace(Value);
        public Item Copy(Container parent) => new Item(parent, Name, Type, Value);
    }

    static class StringExtension
    {
        private static readonly Regex Regex = new Regex(@"\D.*");
        public static bool IsConsistedOfAlphabetAndUnderscore(this string value) => !string.IsNullOrWhiteSpace(value) && Regex.Replace(value, string.Empty, 1).Length == 0;
    }
}
