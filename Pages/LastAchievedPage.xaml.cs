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
    /// Логика взаимодействия для LastAchievedPage.xaml
    /// </summary>
    public partial class LastAchievedPage : Page
    {
        public ObservableCollection <AchievementInfo> Achievements  { get; set; }
        public LastAchievedPage()
        {
            InitializeComponent();
            Achievements = new ObservableCollection<AchievementInfo>
        {
                new AchievementInfo {ImagePath = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/1174180/3f5b6b4295a58d39b7619e5723f94b8f475dd81e.jpg", GameLogoPath = "https://steamcdn-a.akamaihd.net/steam/apps/1174180/capsule_184x69.jpg", AchievementName = "Back in the Mud", AchievementDesctiption = "Complete Chapter 1", AchievementDate = "12.12.12" },
                new AchievementInfo {ImagePath = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/1174180/3f5b6b4295a58d39b7619e5723f94b8f475dd81e.jpg", GameLogoPath = "https://steamcdn-a.akamaihd.net/steam/apps/1174180/capsule_184x69.jpg", AchievementName = "Back in the Mud", AchievementDesctiption = "Complete Chapter 1", AchievementDate = "12.12.12" },
                new AchievementInfo {ImagePath = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/1174180/3f5b6b4295a58d39b7619e5723f94b8f475dd81e.jpg", GameLogoPath = "https://steamcdn-a.akamaihd.net/steam/apps/1174180/capsule_184x69.jpg", AchievementName = "Back in the Mud", AchievementDesctiption = "Complete Chapter 1", AchievementDate = "12.12.12" },
                new AchievementInfo {ImagePath = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/1174180/3f5b6b4295a58d39b7619e5723f94b8f475dd81e.jpg", GameLogoPath = "https://steamcdn-a.akamaihd.net/steam/apps/1174180/capsule_184x69.jpg", AchievementName = "Back in the Mud", AchievementDesctiption = "Complete Chapter 1", AchievementDate = "12.12.12" },
                new AchievementInfo {ImagePath = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/1174180/3f5b6b4295a58d39b7619e5723f94b8f475dd81e.jpg", GameLogoPath = "https://steamcdn-a.akamaihd.net/steam/apps/1174180/capsule_184x69.jpg", AchievementName = "Back in the Mud", AchievementDesctiption = "Complete Chapter 1", AchievementDate = "12.12.12" },
                new AchievementInfo {ImagePath = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/1174180/3f5b6b4295a58d39b7619e5723f94b8f475dd81e.jpg", GameLogoPath = "https://steamcdn-a.akamaihd.net/steam/apps/1174180/capsule_184x69.jpg", AchievementName = "Back in the Mud", AchievementDesctiption = "Complete Chapter 1", AchievementDate = "12.12.12" }
        };
            AchievementList.ItemsSource = Achievements;
        }
    }
}
