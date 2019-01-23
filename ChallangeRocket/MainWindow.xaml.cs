using CsQuery;
using System;
using System.Collections.Generic;
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

namespace ChallangeRocket
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Update();
            var timer = new Timer(10000);
            timer.Elapsed += async (s, args) => await Update();
            timer.Start();
        }

        private async Task Update()
        {
            var dom = CQ.CreateFromUrl("https://challengerocket.com/bankitup2018/works");
            var elements = dom.Select("#all > div.challenge-group > section > div > div > ul > li");
            var url = elements.Select("h4 > a");
            var titles = url.Elements.Select(x => x.InnerText).ToList();
            var votes = elements.Select(".icon-vote").Elements.Select(x => x.InnerText).ToList();

            var projects = new List<Project>();
            for (int i = 0; i < elements.Length; i++)
            {
                var votesTyped = int.Parse(votes[i].Split(':')[1]);
                projects.Add(new Project()
                {
                    Votes = votesTyped,
                    Name = titles[i]
                });
            }

            var twoFirst = projects.OrderByDescending(x => x.Votes).Take(2).ToList().OrderByDescending(x => x.Name);

            var sum = twoFirst.Sum(x => x.Votes);
            var first = twoFirst.First().Votes;

            await App.Current.Dispatcher.Invoke(async () =>
            {
                gauge.From = 0;
                gauge.Value = first;
                gauge.To = sum;
                tb_firstTeam.Text = twoFirst.First().Name;
                tb_firstTeam_Votes.Text = twoFirst.First().Votes.ToString();
                tb_secondTeam.Text = twoFirst.Skip(1).First().Name;
                tb_secondTeam_Votes.Text = twoFirst.Skip(1).First().Votes.ToString();
                tb_difference.Text = (twoFirst.First().Votes - twoFirst.Skip(1).First().Votes).ToString();
                tb_lastUpdate.Text = DateTime.Now.ToString();
            });
        }
    }
    public class Project
    {
        public int Votes { get; set; }
        public string Name { get; set; }
    }
}
