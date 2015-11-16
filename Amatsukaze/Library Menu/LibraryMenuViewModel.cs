﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amatsukaze.HelperClasses;
using Amatsukaze.Model;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Timers;

namespace Amatsukaze.ViewModel
{
    class LibraryMenuViewModel : ObservableObjectClass, ViewModelBase
    {
        public string BaseName
        {
            get
            {
                return "Library Menu";
            }
        }

        public LibraryMenuViewModel(OptionsObject optionsobject, IEventAggregator eventAggregator)
        {
            this.EventAggregator = eventAggregator;
            this.optionsobject = optionsobject;
            datasource = new LibraryMenuModel(optionsobject);

            //Subscribe to events
            datasource.SendMessagetoGUI += new EventHandler(onSendMessagetoGUI);

            //Read the cache file
            datasource.ReadCacheFile();            

            animeLibraryList = datasource.AnimeLibraryList;            
        }


        #region Fields
        
        //Private copy of the message log displayed in the drop down menu. Added to in the SendMessagetoGUI event handler
        private ObservableCollection<string> libraryMessageLog = new ObservableCollection<string>();

        //Commands
        private ICommand _refreshCommand;
        private ICommand _selectAnime;

        //Private fields to keep track of the current grid column and row count
        private int gridcolumncount;
        private int gridrowcount;

        //Private field for the currently displayed anime
        private AnimeEntryObject selectedAnime;

        //GUI Toggles
        private bool messageLogToggle;
        private bool animeInfoToggle;
        #endregion    

        #region Objects

        OptionsObject optionsobject;
        private ObservableCollection<AnimeEntryObject> animeLibraryList;
        LibraryMenuModel datasource;
        public IEventAggregator EventAggregator { get; set; }

        #endregion

        #region Properties/Commands

        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(
                        p => Refresh(),
                        p => true);
                }
                return _refreshCommand;
            }
        }

        public ICommand SelectAnime
        {
            get
            {
                if (_selectAnime == null)
                {
                    _selectAnime = new RelayCommand(
                        p => Select((AnimeEntryObject)p),
                        p => true);
                }
                return _selectAnime;
            }
        }

        public ObservableCollection<AnimeEntryObject> AnimeLibraryList
        {
            get
            {
                return animeLibraryList;
            }
            set
            {
                if (animeLibraryList != value)
                {
                    animeLibraryList = value;
                    OnPropertyChanged("AnimeLibraryList");                    
                }
            }
        }

        public ObservableCollection<string> LibraryMessageLog
        {
            get
            {
                return libraryMessageLog;
            }
            set
            {
                if (libraryMessageLog != value)
                {
                    libraryMessageLog = value;
                    OnPropertyChanged("LibraryMessageLog");
                }
            }
        }

        public int GridColumnCount
        {
            get
            {
                return gridcolumncount;
            }
            set
            {
                if (gridcolumncount != value)
                {
                    gridcolumncount = value;
                    OnPropertyChanged("GridColumnCount");
                }
            }
        }

        public int GridRowCount
        {
            get
            {
                return gridrowcount;
            }
            set
            {
                if (gridrowcount != value)
                {
                    gridrowcount = value;
                    OnPropertyChanged("GridRowCount");
                }
            }
        }             

        public bool MessageLogToggle
        {
            get
            {
                return messageLogToggle;
            }
            set
            {
                if (messageLogToggle != value)
                {
                    messageLogToggle = value;
                    OnPropertyChanged("MessageLogToggle");
                }
            }
        }

        public bool AnimeInfoToggle
        {
            get
            {
                return animeInfoToggle;
            }
            set
            {
                if (animeInfoToggle != value)
                {
                    animeInfoToggle = value;
                    OnPropertyChanged("AnimeInfoToggle");
                }
            }
        }

        public AnimeEntryObject SelectedAnime
        {
            get
            {
                return selectedAnime;
            }
            set
            {
                if (selectedAnime != value)
                {
                    selectedAnime = value;
                    OnPropertyChanged("SelectedAnime");
                }
            }
        }

        #endregion

        #region Methods

        //Updates 
        public void UpdateGridIndexes (int ColumnCount)
        {
            int columncounter = 0, rowcounter = 0;
            foreach (AnimeEntryObject anime in this.AnimeLibraryList)
            {
                anime.GridColumn = columncounter;
                anime.GridRow = rowcounter;

                columncounter++;                

                if (columncounter > (ColumnCount - 1))
                {
                    rowcounter++;
                    columncounter = 0;
                }
            }
        }

        //Rescans the Cache folder for XML files
        private void Refresh()
        {
            datasource.ReadXMLDirectory();
            DisplayAreaResized(this.GridColumnCount);
        }

        //Changes the currently selected anime
        private void Select(AnimeEntryObject anime)
        {
            this.SelectedAnime = anime;
            this.AnimeInfoToggle = true;
        }

        #endregion

        #region Events/EventHandlers

        //All SendtoGUI events from the model are handled here. and they send the message to the EventAggregator which calls up the black popup menu 
        void onSendMessagetoGUI (object sender, EventArgs e)
        {
            Console.WriteLine("Library Event Fired!");
            var message = (e as MessageArgs).Message;
            LibraryMessageLog.Add(message);

            if (this.EventAggregator != null)
            {
                this.EventAggregator.PublishEvent(new MessagetoGUI() { Message = message });
            }
        }

        //Every time the size of the display area is changed in the view, the view is hardcoded to call this function to 
        //recalculate the number of columns/rows that fit and to reassign the grid index of every single picture.
        public void DisplayAreaResized(int columncount)
        {
            if (columncount != 0)
            {
                UpdateGridIndexes(columncount);
                GridColumnCount = columncount;
                if (AnimeLibraryList.Count % columncount == 0)
                {
                    GridRowCount = AnimeLibraryList.Count / columncount;
                }

                else
                {
                    GridRowCount = (AnimeLibraryList.Count / columncount) + 1;
                }
            }                        
        }      

        #endregion
   

    }
}
