﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Windows.Forms;

namespace Amatsukaze.View
{
    /// <summary>
    /// Interaction logic for FolderMenuView.xaml
    /// </summary>
    public partial class FolderMenuView : System.Windows.Controls.UserControl
    {
        public FolderMenuView()
        {
            InitializeComponent();
        }

        private void openAddFolderDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.ShowDialog();

            var obj = this.DataContext as FolderMenuViewModel;
            FolderEntity newEntity = new FolderEntity();
            newEntity.

            obj.Folders.Add();
       
            obj.Test = dialog.SelectedPath;

            
        }
    }
}
