using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для RareAchievements.xaml
    /// </summary>
    public partial class RareAchievements : Page
    {
        ObservableCollection<AchievementInfo> Achievemetns = new ObservableCollection<AchievementInfo>
        {
           new AchievementInfo {AchievementName = "Lorem ipsum dolor sit amet, co, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", ImagePath="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/218620/544df59059853491170562bf03b7bbb7954dd16a.jpg" },
           new AchievementInfo {AchievementName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", ImagePath="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/218620/544df59059853491170562bf03b7bbb7954dd16a.jpg" },
           new AchievementInfo {AchievementName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esunt in culpa qui officia deserunt mollit anim id est laborum.", ImagePath="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/218620/544df59059853491170562bf03b7bbb7954dd16a.jpg" },
           new AchievementInfo {AchievementName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", ImagePath="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/218620/544df59059853491170562bf03b7bbb7954dd16a.jpg" },
           new AchievementInfo {AchievementName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", ImagePath="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/218620/544df59059853491170562bf03b7bbb7954dd16a.jpg" },
           new AchievementInfo {AchievementName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", ImagePath="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/218620/544df59059853491170562bf03b7bbb7954dd16a.jpg" },
           new AchievementInfo {AchievementName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla roident, sunt in culpa qui officia deserunt mollit anim id est laborum.", ImagePath="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/218620/544df59059853491170562bf03b7bbb7954dd16a.jpg" },
           new AchievementInfo {AchievementName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", ImagePath="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/218620/544df59059853491170562bf03b7bbb7954dd16a.jpg" },
           new AchievementInfo {AchievementName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", ImagePath="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/218620/544df59059853491170562bf03b7bbb7954dd16a.jpg" },
           new AchievementInfo {AchievementName = "Lorem ipsum dolor sit am sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", ImagePath="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/218620/544df59059853491170562bf03b7bbb7954dd16a.jpg" },
           new AchievementInfo {AchievementName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", ImagePath="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/218620/544df59059853491170562bf03b7bbb7954dd16a.jpg" }
        };
        public RareAchievements()
        {
            InitializeComponent();
            AchievementTable.ItemsSource = Achievemetns;
        }
    }
}
