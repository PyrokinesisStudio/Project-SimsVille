﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TSO.Files.FAR1;
using System.IO;
using TSO.Files.formats.iff;
using TSO.Files.formats.iff.chunks;

namespace sims.debug
{
    public partial class Form1 : Form
    {
        private FAR1Archive Far;
        private string Userpath, Famdir, Housedir;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            DirectoryInfo dirinfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            foreach (DirectoryInfo dir in dirinfo.GetDirectories())
                listBox2.Items.Add(dir.FullName);

        }

        private sbyte GetCategoryName(string name)
        {
            sbyte category = -1;

            if (name.Contains("Door") || name.Contains("door"))
                category = 0;
            if (name.Contains("Window") || name.Contains("window"))
                category = 1;
            else if (name.Contains("Stair"))
                category = 2;
            else if (name.Contains("Shrub") || name.Contains("Tree") || name.Contains("Flower"))
                category = 3;
            else if (name.Contains("Fireplace"))
                category = 4;
            else if (name.Contains("Pool"))
                category = 5;
            else if (name.Contains("Column"))
                category = 7;
            else if (name.Contains("Chair") || name.Contains("Sofa") || name.Contains("Bed") || name.Contains("bed") || name.Contains("bench"))
                category = 12;
            else if (name.Contains("Table") || name.Contains("Counter") || name.Contains("counter") || name.Contains("Saloon")
                || name.Contains("Horn") || name.Contains("Desk") || name.Contains("desk") || name.Contains("table") || name.Contains("Dining"))
                category = 13;
            else if (name.Contains("Dishwasher") || name.Contains("Machine") || name.Contains("Stove") || name.Contains("stove")
                || name.Contains("Fridge") || name.Contains("Shower") || name.Contains("tub") || name.Contains("Toilet") || name.Contains("shower") || name.Contains("sink"))
                category = 14;
            else if (name.Contains("tv") || name.Contains("Stereo") || name.Contains("Computer") || name.Contains("Dance") || name.Contains("Mechanical")
                || name.Contains("Teleport") || name.Contains("Cash"))
                category = 15;
            else if (name.Contains("Piano") || name.Contains("Book") || name.Contains("Mirror") || name.Contains("Telescope") || name.Contains("mirror") || name.Contains("book")
                || name.Contains("Chemistry") || name.Contains("Guitar") || name.Contains("mirror"))
                category = 16;
            else if (name.Contains("Sculpture") || name.Contains("Plant") || name.Contains("Painting") || name.Contains("Clock")
                || name.Contains("Rug") || name.Contains("Ball") || name.Contains("Christmas") || name.Contains("awning") || name.Contains("Sign"))
                category = 17;
            else if (name.Contains("Cloth") || name.Contains("Dress") || name.Contains("Can") || name.Contains("Bar")
                || name.Contains("Trash") || name.Contains("Alarm") || name.Contains("Trunk") || name.Contains("DisplayCase"))
                category = 18;
            else if (name.Contains("Lamp") || name.Contains("Light") || name.Contains("lamp") || name.Contains("Menorah"))
                category = 19;
            else if (name.Contains("Pet") || name.Contains("Dog") || name.Contains("Cat"))
                category = 20;
            else if (name.Contains("Job"))
                category = 21;
            else if (name.Contains("Restaurant"))
                category = 22;

            return category;

        }


        private void CreateTableFile(string path)
        {

            InfoTable infotable = new InfoTable();
            infotable.Items = new List<TableItem>();

            //Far = new FAR1Archive(path, 0);
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach(FileInfo file in dir.GetFiles())
                if (file.Extension == ".iff")
               {
               var iff = new Iff(path + "/" + file.Name);
               ulong FileID = 0;


               foreach (OBJD obj in iff.List<OBJD>())
               if (obj.IsMultiTile && obj.SubIndex == -1)
                   {

                   FileID = obj.GUID;
                   string name = Path.GetFileNameWithoutExtension(file.Name);

                    infotable.Items.Add(new TableItem()
                            {
                                GUID = FileID.ToString("X"),
                                Name = name,
                                

                            });

                    listBox1.Items.Add(file.Name + " " + FileID);
                    InfoTable.Save("table.xml", infotable);

                   }
               else if (!obj.IsMultiTile)
               {

                   FileID = obj.GUID;
                   string name = Path.GetFileNameWithoutExtension(file.Name);

                   infotable.Items.Add(new TableItem()
                   {
                       GUID = FileID.ToString("X"),
                       Name = name,

                   });

                    listBox1.Items.Add(file.Name + " " + FileID);
                    InfoTable.Save("table.xml", infotable);
                        }
          
                }
        }

        private void CreateCatalogFile(string path)
        {

            CatalogTable catalog = new CatalogTable();
            catalog.Items = new List<CatalogItem>();

            //Far = new FAR1Archive(path, 0);
            DirectoryInfo dir = new DirectoryInfo(path);


           foreach(FileInfo file in dir.GetFiles())
               if (file.Extension == ".iff")
               {
               var iff = new Iff(path + "/" + file.Name);
               ulong FileID = 0;


               foreach (OBJD obj in iff.List<OBJD>())
               if (obj.IsMultiTile && obj.SubIndex == -1)
                   {

                   FileID = obj.GUID;
                   string name = Path.GetFileNameWithoutExtension(file.Name);

                    catalog.Items.Add(new CatalogItem()
                            {
                                GUID = FileID.ToString("X"),
                                Name = name,
                                Category = GetCategoryName(name),
                                Price = obj.Price

                            });

                    listBox1.Items.Add(file.Name + " " + FileID);
                    CatalogTable.Save("catalog.xml", catalog);

                   }
               else if (!obj.IsMultiTile)
               {

                   FileID = obj.GUID;
                   string name = Path.GetFileNameWithoutExtension(file.Name);

                   catalog.Items.Add(new CatalogItem()
                   {
                       GUID = FileID.ToString("X"),
                       Name = name,
                       Category = GetCategoryName(name),
                       Price = obj.Price

                   });

                   listBox1.Items.Add(file.Name + " " + FileID);
                   CatalogTable.Save("catalog.xml", catalog);

               }

               }

        }


        private void button1_Click(object sender, EventArgs e)
        {

            listBox1.Items.Clear();

            string file = "";

            if (listBox2.SelectedItem != null)

                file = listBox2.SelectedItem.ToString();

            CreateTableFile(file);

            
                

        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            string file = "";

            if (listBox2.SelectedItem != null)

                file = listBox2.SelectedItem.ToString();

            CreateCatalogFile(file);
        }

        private void button3_Click(object sender, EventArgs e)
        {

            List<FarEntry> Resource = new List<FarEntry>();
            FAR1Archive Archive = new FAR1Archive("UIGraphics.far", 0);
                    //m_Archives.Add(path, Archive);
             Resource = Archive.GetAllFarEntries();

             foreach (var entry in Resource)
                 {

                     listBox1.Items.Add(entry.Filename);

                 if (entry.Filename.Contains("setup.bmp"))
                     MessageBox.Show(entry.Filename);

                 }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            uint id = 0;
            string name = "";

            RoofTable roofs = new RoofTable();
            roofs.Items = new List<RoofItem>();

            DirectoryInfo dir = new DirectoryInfo("Roofs");

            foreach (FileInfo file in dir.GetFiles())
                if (file.Extension == ".bmp")
                {
                    name = Path.GetFileNameWithoutExtension(file.Name);

                    roofs.Items.Add(new RoofItem()
                    {
                        ID = id,
                        Name = name,
                        Category = 6,
                        Price = 0

                    });


                    id++;

                }

            RoofTable.Save("roofs.xml", roofs);

        }

        private void LoadHouses(string path)
        {
            Housedir = path + "/Houses";

            DirectoryInfo housedirinfo = new DirectoryInfo(Housedir);
            label5.Text = housedirinfo.Name;


            foreach (FileInfo file in housedirinfo.GetFiles())
                if (file.Extension == ".iff" && file.Name.Contains("House") && !file.Name.Contains("House00") && !file.Name.Contains("House11"))
                {


                    string house = file.Name.Split('_')[0];

                    listBox3.Items.Add(file.Name);



                }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int current = comboBox1.SelectedIndex;

            
            Userpath = "UserData/";

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

            listBox3.Items.Clear();
            int current = comboBox1.SelectedIndex;

            if (current == 0)
                LoadHouses(Userpath);

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }



    }
}