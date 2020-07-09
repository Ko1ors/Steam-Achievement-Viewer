using System;
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
using System.Xml;

namespace AchievementTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //XmlDocument doc = GetRequest.XmlRequest("https://steamcommunity.com/id/Ko1ors/stats/1174180/achievements/?xml=1");
            //XmlNode xRoot = doc.DocumentElement;
            //XmlNode GameNode = xRoot.SelectSingleNode("game");
            
            //TextBlock testText = new TextBlock();
            //testText.Text = GameNode.SelectSingleNode("gameName").InnerText;
            //GamesList.Children.Add(testText);
            //foreach (XmlNode n in GameNode.ChildNodes)
            //{
            //    TextBlock testText = new TextBlock();
            //    testText.Text = n.Name;
            //    GamesList.Children.Add(testText);
            //}
        }
    }
}
