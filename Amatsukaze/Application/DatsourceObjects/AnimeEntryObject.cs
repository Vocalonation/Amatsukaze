﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using Amatsukaze.HelperClasses;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace Amatsukaze.ViewModel
{
    public class AnimeEntryObject : ObservableObjectClass
    {
        //Object to hold the properties for one anime (based on the return from myanimelist
        #region Properties

        //Properties used by Amatsukaze
        public int id { get; set; }
        public string title { get; set; }
        public string english { get; set; }
        public string synonyms { get; set; }
        public int episodes { get; set; }
        public double score { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string synopsis { get; set; }
        public string image { get; set; }        

        //Staff
        public List<AnimeStaff> Staff { get; set; }

        //Character List    
        public List<AnimeCharacter> Characters { get; set; } 

        //Episode list
        public List<Episode> Episodes { get; set; }

        //Sources used
        public List<string> Sources { get; set; }

        //Properties for Amatsukaze GUI

        //Anime Cover Image
        private string imagePath;
        public string ImagePath
        {
            get
            {
                return imagePath;
            }
            set
            {
                if (imagePath != value)
                {
                    imagePath = value;
                    OnPropertyChanged("ImagePath");
                }
            }
        }

        //For the image grid layout
        private int gridcolumn;
        public int GridColumn
        {
            get
            {
                return gridcolumn;
            }
            set
            {
                if (gridcolumn != value)
                {
                    gridcolumn = value;
                    OnPropertyChanged("GridColumn");
                }
            }
        }      

        private int gridrow { get; set; }
        public int GridRow
        {
            get
            {
                return gridrow;
            }
            set
            {
                if (gridrow != value)
                {
                    gridrow = value;
                    OnPropertyChanged("GridRow");
                }
            }
        }        


        //Constructors

        public AnimeEntryObject()
        {
        }

        public AnimeEntryObject(MALDataSource datasource)
        {
            if (datasource == null)
            {
                return;
            }

            this.Sources = new List<string>();
            this.Sources.Add("MAL");

            PropertyInfo[] properties = datasource.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                PropertyInfo targetproperty = this.GetType().GetProperty(property.Name);
                targetproperty.SetValue(this, property.GetValue(datasource, null), null);
            }
        }

        public AnimeEntryObject(AniDBDataSource datasource)
        {
            if (datasource == null)
            {
                return;
            }

            //Can't use reflection because the names are all different, so have to use a lamo copy constructor
            this.id = datasource.Id;
            this.type = datasource.Type;
            this.start_date = datasource.StartDate;
            this.end_date = datasource.EndDate;
            this.title = datasource.Title;
            this.score = datasource.StandardRating;
            this.english = datasource.EnglishTitle;
            this.synonyms = datasource.Synonyms;
            this.image = datasource.Picture;
            this.synopsis = datasource.Synopsis;

            this.Staff = datasource.Staff;
            this.Characters = datasource.Characters;
            this.Episodes = datasource.Episodes;

            AssignGridRowColumn(this.Staff);
            AssignGridRowColumn(this.Characters);

            this.Sources = new List<string>();
            this.Sources.Add("AniDB");
        }

        public AnimeEntryObject(MALDataSource MALdatasource, AniDBDataSource AniDBdatasource)
        {
            if (MALdatasource == null || AniDBdatasource == null)
            {
                return;
            }

            PropertyInfo[] properties = MALdatasource.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                PropertyInfo targetproperty = this.GetType().GetProperty(property.Name);
                targetproperty.SetValue(this, property.GetValue(MALdatasource, null), null);
            }

            this.synopsis = AniDBdatasource.Synopsis;
            this.Staff = AniDBdatasource.Staff;
            this.Characters = AniDBdatasource.Characters;
            this.Episodes = AniDBdatasource.Episodes;

            AssignGridRowColumn(this.Staff);
            AssignGridRowColumn(this.Characters);
        }

        //Methods

        public AnimeEntryObject Clone()
        {
            return (AnimeEntryObject)this.MemberwiseClone();
        }

        private void AssignGridRowColumn(List<AnimeCharacter> CharacterList)
        {
            int columncounter = 0, rowcounter = 0;
            foreach (AnimeCharacter Character in CharacterList)
            {
                Character.GridColumn = columncounter;
                Character.GridRow = rowcounter;

                columncounter++;

                if (columncounter > (4 - 1))
                {
                    rowcounter++;
                    columncounter = 0;
                }
            }
        }

        public void MergeAnimeCover(string ImagePath)
        {
            lock(this)
            {
                this.ImagePath = ImagePath;
            }            
        }

        public void MergeCharacters(List<AnimeCharacter> Characters)
        {
            lock(this)
            {
                this.Characters = Characters;
            }

            //Add AniDB just in case because characters can only be added from AniDB
            if (this.Sources.Contains("AniDB") == false) this.Sources.Add("AniDB");
        }

        public void MergeInfo(MALDataSource Input, OptionsObject OptionsObject)
        {
            //Return if OptionsObject doesn't have MALDataSource enabled. 
            if (OptionsObject.UseMALDataSource == false) return;
            
            //Try to merge the property if the property is not defined on AnimeEntryObject
            PropertyInfo[] InputProperties = Input.GetType().GetProperties();

            foreach (PropertyInfo property in InputProperties)
            {
                PropertyInfo AnimeEntryObjectProperty = this.GetType().GetProperty(property.Name);                

                if(AnimeEntryObjectProperty.GetValue(this, null) == null && property.GetValue(Input, null) != null)
                {
                    AnimeEntryObjectProperty.SetValue(this, property.GetValue(Input, null), null);
                }
            }

            if (this.Sources.Contains("MAL") == false) this.Sources.Add("MAL");

            //Temporary Workaround
            this.image = Input.image;
            this.synopsis = Input.synopsis;
        }

        public void MergeInfo(AniDBDataSource Input, OptionsObject OptionsObject)
        {
            //Return if OptionsObject doesn't have AniDB enabled. 
            if (OptionsObject.UseAniDBDataSource == false) return;

            //Try to merge the property if the property is not defined on AnimeEntryObject
            PropertyInfo[] InputProperties = Input.GetType().GetProperties();

            foreach (PropertyInfo property in InputProperties)
            {
                PropertyInfo AnimeEntryObjectProperty = this.GetType().GetProperty(property.Name);

                if (AnimeEntryObjectProperty.GetValue(this, null) == null && property.GetValue(Input, null) != null)
                {
                    AnimeEntryObjectProperty.SetValue(this, property.GetValue(Input, null), null);
                }
            }

            if (this.Sources.Contains("AniDB") == false) this.Sources.Add("AniDB");            
        }

        private void AssignGridRowColumn(List<AnimeStaff> StaffList)
        {
            int columncounter = 0, rowcounter = 0;
            foreach (AnimeStaff Staff in StaffList)
            {
                Staff.GridColumn = columncounter;
                Staff.GridRow = rowcounter;

                columncounter++;

                if (columncounter > (4 - 1))
                {
                    rowcounter++;
                    columncounter = 0;
                }
            }
        }        

        [Conditional("DEBUG")]
        public void ContentsDump()
        {
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                Console.WriteLine("{0}: {1}", property.Name, property.GetValue(this, null));
            }
        }
        #endregion
    }
}
