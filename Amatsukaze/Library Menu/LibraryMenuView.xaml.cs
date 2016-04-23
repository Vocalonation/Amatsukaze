﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Amatsukaze.ViewModel;


namespace Amatsukaze.View
{
    /// <summary>
    /// Interaction logic for LibraryMenuView.xaml
    /// </summar>y
    public partial class LibraryMenuView : UserControl
    {
        
        public LibraryMenuView()
        {
            InitializeComponent();              
        }        

        #region Event handlers

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {      
            //For reassigning all of the indexes whenever the window is changed
            var datacontext = DataContext as LibraryMenuViewModel;
            int columncount = ((int)DisplayArea.ActualWidth - 20) / 180;
            if(datacontext.CurrentView == "All")
            {
                datacontext.LibraryViewAreaResized(columncount);
            }
            else if (datacontext.CurrentView == "Season")
            {
                datacontext.SeasonSortListAreaResized(columncount);
            }
            else if (datacontext.CurrentView == "Search")
            {
                datacontext.SearchResultListAreaResized(columncount);
            }
            
            
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //For Initializing the Grid based on the usercontrol width
            var datacontext = DataContext as LibraryMenuViewModel;
            int columncount = ((int)DisplayArea.ActualWidth -20) / 180;
            datacontext.LibraryViewAreaResized(columncount);
            datacontext.SeasonSortListAreaResized(columncount);
        }
        #endregion

        #region objects

        #endregion        

        private void ActivityLog_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as LibraryMenuViewModel;
            switch (datacontext.MessageLogToggle)
            {
                case true:
                    datacontext.MessageLogToggle = false;
                    break;
                case false:
                    datacontext.MessageLogToggle = true;
                    break;
            }
        }

        //Placeholder event handler. Will be replaced with an Icommand later
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as LibraryMenuViewModel;
            datacontext.AnimeInfoToggle = false;
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textbox = sender as TextBox;
            textbox.Text = string.Empty;
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textbox = sender as TextBox;
            textbox.Text = "Search";
        }
    }

    public class UnsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value as string;
            if (input == "") return DependencyProperty.UnsetValue;
            else return input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageBoxWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            var input = value as double?;
            input = input * 90 / 100;
            return input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GridResizedEventArgs : EventArgs
    {
        public int Columncount { get; set; }
    }    
}
