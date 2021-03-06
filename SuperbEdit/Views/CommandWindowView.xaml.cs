﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SuperbEdit.Base;

namespace SuperbEdit.Views
{
    /// <summary>
    ///     Interaction logic for CommandWindowView.xaml
    /// </summary>
    public partial class CommandWindowView : UserControl
    {
        public CommandWindowView()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                HandleListBoxSelection(e.Key);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                ExecuteSelectedAction();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                HideMe();
            }
        }

        private void HideMe()
        {
            var ancestor = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this)) as UIElement;
            ancestor.Visibility = Visibility.Collapsed;
            TextBox.Text = "";
            ListBox.SelectedIndex = -1;
        }

        private void ExecuteSelectedAction()
        {
            var action = (ListBox.SelectedItem as IActionItem);
            ExecuteAction(action);
        }

        private void ExecuteAction(IActionItem action)
        {
            if (action != null)
            {
                action.Execute();
                HideMe();
            }
        }

        private void HandleListBoxSelection(Key key)
        {
            switch (key)
            {
                case Key.Up:
                    if (ListBox.SelectedIndex > 0)
                        ListBox.SelectedIndex--;
                    break;
                case Key.Down:
                    if (ListBox.SelectedIndex < ListBox.Items.Count - 1)
                        ListBox.SelectedIndex++;
                    break;
            }
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var action = (sender as ListBoxItem).DataContext as IActionItem;
            ExecuteAction(action);
        }
    }
}