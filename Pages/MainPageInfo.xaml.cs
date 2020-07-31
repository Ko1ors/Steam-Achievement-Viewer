﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AchievementTest.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPageInfo.xaml
    /// </summary>
    public partial class MainPageInfo : Page
    {
        public MainPageInfo()
        {
            InitializeComponent();
            if (Manager.profile != null)
            {
                UpdateProfileName(Manager.profile.SteamID);
                UpdateGameCount(Manager.gamesList.Games.Game.Count);
                //int AchievementC=0;
                //foreach(Game g in Manager.gamesList.Games.Game)
                //{
                //    foreach(Achievement a in g.Achievements.Achievement)
                //    {
                //        AchievementC++;
                //    }
                //}
                //UpdateAchievementCount(AchievementC);
            }
        }
        public void UpdateProfileName(string Name)
        {
            ProfileName.Text = Name;
        }
        public void UpdateGameCount(int GameCount)
        {
            GamesCount.Text = GameCount.ToString();
        }
        public void  UpdateAchievementCount(int Achievements)
        {
            AchievementCount.Text = Achievements.ToString();
        }
        public void UpdateAchieved(int Achieved)
        {
            AchievementAchieved.Text = Achieved.ToString();
        }
        private void UpdateProgressBar(int Progress)
        {
            AchievementProgressCircle.Value = Progress;
        }
    }
}
