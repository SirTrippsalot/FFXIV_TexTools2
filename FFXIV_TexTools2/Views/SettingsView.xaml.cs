// FFXIV TexTools
// Copyright © 2017 Rafael Gonzalez - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using FFXIV_TexTools2.Helpers;
using FFXIV_TexTools2.Resources;
using FFXIV_TexTools2.ViewModel;
using FFXIV_TexTools2;
using System.Windows.Controls;
using FolderSelect;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System;
using Newtonsoft.Json;

namespace FFXIV_TexTools2.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            DataContext = Properties.Settings.Default;

            var savePath = Path.GetFullPath(Properties.Settings.Default.Save_Directory);
            saveDir.Text = savePath;

            var ffxivPath = Path.GetFullPath(Properties.Settings.Default.FFXIV_Directory);
            FFXIVDir.Text = ffxivPath;


        }

        private void Menu_ProblemCheck_Click(object sender, RoutedEventArgs e)
        {
            if (Helper.CheckIndex())
            {
                if (MessageBox.Show("The index file does not have access to the modded dat file. \nFix now?", "Found an Issue", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    Helper.FixIndex();
                }
            }
            else
            {
                MessageBox.Show("No issues were found \nIf you are still experiencing issues, please submit a bug report.", "No Issues Found", MessageBoxButton.OK, MessageBoxImage.None);
            }
        }
        private void Menu_BugReport_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://ffxivtextools.dualwield.net/bug_report.html");
        }
        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            About a = new About();
            a.Owner = FFXIV_TexTools2.MainWindow.GetWindow(this);
            a.Show();
        }
        private void Menu_English_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Changing language requires the application to restart. \nRestart now?", "Language Change", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                Properties.Settings.Default.Language = "en";
                Properties.Settings.Default.Save();
                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }

        }
        private void Menu_Japanese_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Changing language requires the application to restart. \nRestart now?", "Language Change", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                Properties.Settings.Default.Language = "ja";
                Properties.Settings.Default.Save();
                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }

        }
        private void Menu_French_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Changing language requires the application to restart. \nRestart now?", "Language Change", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                Properties.Settings.Default.Language = "fr";
                Properties.Settings.Default.Save();
                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }

        }
        private void Menu_German_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Changing language requires the application to restart. \nRestart now?", "Language Change", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                Properties.Settings.Default.Language = "de";
                Properties.Settings.Default.Save();
                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }

        }
        private void Menu_StartOver_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Starting over will:\n\n" +
                "Restore index files to their original state.\n" +
                "Delete all mods and create new .dat files.\n" +
                "Delete all .modlist file entries.\n\n" +
                "Do you want to start over?", "Start Over", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                RevertAll();
                string[] indexFiles = new string[]
                {
                    Directory.GetCurrentDirectory() + "/Index_Backups/040000.win32.index",
                    Directory.GetCurrentDirectory() + "/Index_Backups/040000.win32.index2",
                    Directory.GetCurrentDirectory() + "/Index_Backups/060000.win32.index",
                    Directory.GetCurrentDirectory() + "/Index_Backups/060000.win32.index2"
                };
                foreach (var i in indexFiles)
                {
                    if (File.Exists(i))
                    {
                        File.Copy(i, Properties.Settings.Default.FFXIV_Directory + "/" + Path.GetFileName(i), true);
                    }
                }
                foreach (var datName in Info.ModDatDict)
                {
                    var datPath = string.Format(Info.datDir, datName.Key, datName.Value);
                    if (File.Exists(datPath))
                    {
                        File.Delete(datPath);
                    }
                }
                File.Delete(Info.modListDir);
                MakeModContainers();
            }
        }
        /// <summary>
        /// Creates files that will contain modded information
        /// </summary>
        private void MakeModContainers()
        {
            foreach (var datName in Info.ModDatDict)
            {
                var datPath = string.Format(Info.datDir, datName.Key, datName.Value);
                if (!File.Exists(datPath))
                {
                    CreateDat.MakeDat();
                    CreateDat.ChangeDatAmounts();
                }
            }
            if (!File.Exists(Info.modListDir))
            {
                CreateDat.CreateModList();
            }
        }
        private void Menu_ModList_Click(object sender, RoutedEventArgs e)
        {
            ModList ml = new ModList();
            ml.Show();
        }
       

        private void Menu_RevertAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RevertAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("[Main] Error Accessing .modlist File \n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RevertAll()
        {
            JsonEntry modEntry = null;
            string line;
            using (StreamReader sr = new StreamReader(Info.modListDir))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    modEntry = JsonConvert.DeserializeObject<JsonEntry>(line);
                    if (modEntry.originalOffset != 0)
                    {
                        Helper.UpdateIndex(modEntry.originalOffset, modEntry.fullPath, modEntry.datFile);
                        Helper.UpdateIndex2(modEntry.originalOffset, modEntry.fullPath, modEntry.datFile);
                    }
                }
            }
        }
        private void Menu_ReapplyAll_Click(object sender, RoutedEventArgs e)
        {
            JsonEntry modEntry = null;
            string line;
            try
            {
                using (StreamReader sr = new StreamReader(Info.modListDir))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        modEntry = JsonConvert.DeserializeObject<JsonEntry>(line);
                        if (modEntry.originalOffset != 0)
                        {
                            Helper.UpdateIndex(modEntry.modOffset, modEntry.fullPath, modEntry.datFile);
                            Helper.UpdateIndex2(modEntry.modOffset, modEntry.fullPath, modEntry.datFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("[Main] Error Accessing .modlist File \n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void FFXIVDirButton_Click(object sender, RoutedEventArgs e)
        {
            FolderSelectDialog folderSelect = new FolderSelectDialog()
            {
                Title = "Select ffxiv folder"
            };
            var result = folderSelect.ShowDialog();
            if (result)
            {
                Properties.Settings.Default.FFXIV_Directory = folderSelect.FileName;
                Properties.Settings.Default.Save();

                FFXIVDir.Text = folderSelect.FileName;
            }
        }

        private void SaveDirButton_Click(object sender, RoutedEventArgs e)
        {
            FolderSelectDialog folderSelect = new FolderSelectDialog()
            {
                Title = "Select save folder"
            };
            var result = folderSelect.ShowDialog();
            if (result)
            {
                Properties.Settings.Default.Save_Directory = folderSelect.FileName;
                Properties.Settings.Default.Save();

                saveDir.Text = folderSelect.FileName;
            }
        }


    }
}
