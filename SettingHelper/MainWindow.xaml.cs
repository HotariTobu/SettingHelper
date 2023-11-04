using SharedWPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace SettingHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel { get; }

        private MainIO IO { get; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = DataContext as MainWindowViewModel;
            IO = new MainIO(ViewModel.TreeContent);
            ViewModel.TypeList.AddRange(IO.Types);
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = ((string[])e.Data.GetData(DataFormats.FileDrop))[0].EndsWith(".xml") ? DragDropEffects.Copy : DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                IO.Load(((string[])e.Data.GetData(DataFormats.FileDrop))[0]);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) => IO.Save();
        private void ExportButton_Click(object sender, RoutedEventArgs e) => IO.Export();

        private void AddEmptyChildTo(Container parent)
        {
            System.Collections.ObjectModel.ObservableCollection<Container> containers = parent.Containers;
            if (!containers.Any() || !containers.Last().IsEmpty)
            {
                containers.Add(new Container(parent));
            }

            System.Collections.ObjectModel.ObservableCollection<Item> items = parent.Items;
            if (!items.Any() || !items.Last().IsEmpty)
            {
                items.Add(new Item(parent));
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.OldValue is Container oldValue)
            {
                if (oldValue.Containers.LastOrDefault() is Container last && last.IsEmpty)
                {
                    oldValue.Containers.Remove(last);
                }
                if (oldValue.Items.LastOrDefault() is Item item && item.IsEmpty)
                {
                    oldValue.Items.Remove(item);
                }
            }

            if (e.NewValue is Container newValue)
            {
                AddEmptyChildTo(newValue);
                ViewModel.SelectedContainer = newValue;
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    TextBox_EditEnding();
                    break;
                case Key.Delete:
                    if (sender is TextBox textBox && textBox.Tag is Container container)
                    {
                        ViewModel.SelectedContainer.Containers.Remove(container);
                        TextBox_EditEnding();
                    }
                    break;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox_EditEnding();
        }

        private void TextBox_EditEnding()
        {
            AddEmptyChildTo(ViewModel.SelectedContainer);
        }

        private void DataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                /*case Key.Up:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                    {
                        int index = (sender as DataGrid).SelectedIndex;
                        if (index <= 0)
                        {
                            break;
                        }

                        System.Collections.ObjectModel.ObservableCollection<Item> items = ViewModel.SelectedContainer.Items;
                        Item item = items[index];
                        index -= 1;
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
                        {
                            items.Remove(item);
                        }
                        else
                        {
                            item = item.Copy(item.Parent);
                        }

                        items.Insert(index, item);

                        e.Handled = true;
                    }
                    break;
                case Key.Down:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                    {
                        int index = (sender as DataGrid).SelectedIndex;
                        if (index != -1)
                        {
                            break;
                        }

                        System.Collections.ObjectModel.ObservableCollection<Item> items = ViewModel.SelectedContainer.Items;
                        Item item = items[index];
                        index += 1;
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
                        {
                            items.Remove(item);
                            
                        }
                        else
                        {
                            item = item.Copy(item.Parent);
                        }

                        if (items.Count - 1 < index)
                        {
                            items.Add(item);
                        }
                        else
                        {
                            items.Insert(index + 1, item);
                        }

                        e.Handled = true;
                    }
                    break;*/
                case Key.Delete:
                    if (sender is DataGrid dataGrid)
                    {
                        System.Collections.IList selection = dataGrid.SelectedItems;
                        Item[] items = new Item[selection.Count];
                        selection.CopyTo(items, 0);
                        ViewModel.SelectedContainer.Items.RemoveRange(items);
                        DataGrid_EditEnding();
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGrid_EditEnding();
        }

        private void DataGrid_EditEnding()
        {
            AddEmptyChildTo(ViewModel.SelectedContainer);
        }

        private readonly string ContainerDrop = "SettingHelperContainer";
        private readonly string ItemDrop = "SettingHelperItem";

        private void TreeViewItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if ((e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed) && sender is TreeViewItem item && item.Header is Container container && container.Parent != null)
            {
                DragDrop.DoDragDrop(item, new DataObject(ContainerDrop, container, true), DragDropEffects.Move | DragDropEffects.Copy);
            }
        }

        private void TreeViewItem_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(ContainerDrop) || e.Data.GetDataPresent(ItemDrop))
            {
                e.Effects = (e.KeyStates & DragDropKeyStates.ControlKey) == 0 ? DragDropEffects.Move : DragDropEffects.Copy;
                e.Handled = true;
            }
        }

        private void TreeViewItem_Drop(object sender, DragEventArgs e)
        {
            Container parent = (sender as TreeViewItem).Header as Container;

            if (e.Data.GetDataPresent(ContainerDrop) && e.Data.GetData(ContainerDrop) is Container container)
            {
                bool check(Container child) => parent == child || child.Containers.Any(grandchild => check(grandchild));
                if (parent == container.Parent || check(container))
                {
                    e.Handled = true;
                    return;
                }

                if ((e.KeyStates & DragDropKeyStates.ControlKey) == 0)
                {
                    container.Parent.Containers.Remove(container);
                }
                else
                {
                    container = container.Copy(parent);
                }

                container.Parent = parent;
                parent.Containers.Add(container);
                e.Handled = true;
            }
            else if (e.Data.GetDataPresent(ItemDrop) && e.Data.GetData(ItemDrop) is Item[] items)
            {
                if ((e.KeyStates & DragDropKeyStates.ControlKey) == 0)
                {
                    ViewModel.SelectedContainer.Items.RemoveRange(items);
                }
                else
                {
                    items = items.Select(item => item.Copy(parent)).ToArray();
                }

                foreach (Item item in items)
                {
                    item.Parent = parent;
                    parent.Items.Add(item);
                }
                e.Handled = true;
            }
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            if ((e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed) && sender is DataGridRow row)
            {
                System.Collections.IList selection = row.FindAncestor<System.Windows.Controls.Primitives.MultiSelector>().SelectedItems;
                Item[] items = new Item[selection.Count];
                selection.CopyTo(items, 0);
                DragDrop.DoDragDrop(row, new DataObject(ItemDrop, items, true), DragDropEffects.Move | DragDropEffects.Copy);
            }
        }

        private void DataGridRow_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(ItemDrop))
            {
                e.Effects = (e.KeyStates & DragDropKeyStates.ControlKey) == 0 ? DragDropEffects.Move : DragDropEffects.Copy;
                e.Handled = true;
            }
        }

        private void DataGridRow_Drop(object sender, DragEventArgs e)
        {
            int index = (sender as DataGridRow).GetIndex();
            if (e.Data.GetDataPresent(ItemDrop) && e.Data.GetData(ItemDrop) is Item[] items && index != -1)
            {
                if ((e.KeyStates & DragDropKeyStates.ControlKey) == 0)
                {
                    ViewModel.SelectedContainer.Items.RemoveRange(items);
                }
                else
                {
                    items = items.Select(item => item.Copy(item.Parent)).ToArray();
                }

                System.Collections.ObjectModel.ObservableCollection<Item> itemList = ViewModel.SelectedContainer.Items;
                if (itemList.Count - 1 < index)
                {
                    itemList.AddRange(items);
                }
                else
                {
                    itemList.InsertRange(index, items);
                }
                e.Handled = true;
            }
        }
    }
}
