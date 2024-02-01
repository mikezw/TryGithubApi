using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
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

namespace GithubHostTry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        private bool isRunning = false;
        private Dictionary<string, PingReply> status = new Dictionary<string, PingReply>();
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            this.DataContext = viewModel;
            allIP = new List<string>();
        }

        private List<string> allIP;

        public List<string> AllIP
        {
            get { return allIP; }
            set
            {
                allIP = value;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                isRunning = true;
                Brush temp = GetButton.Background;
                GetButton.Background= new SolidColorBrush(Colors.LightGreen);
                MetaData? metaData = await viewModel.ReadData();
                AllIP = viewModel.GetIPs(metaData).ToList();
                //ping all
                status.Clear();
                foreach (var ip in AllIP)
                {
                    PingReply replay = await Ping(ip);
                    Debug.Write($"Ping {ip} {replay.Status} {replay.RoundtripTime}ms");
                    if (!status.ContainsKey(ip))
                    {
                        status.Add(ip, replay);
                    }
                    else
                    {
                        status[ip] = replay;
                    }
                }
                List<string> results = status.Select(x => $"{x.Key}    {x.Value.Status}    {x.Value.RoundtripTime}ms").ToList();
                File.WriteAllLines("FileResult.txt", results);
                GetButton.Background = temp;
                ResultList.ItemsSource = status.ToList() ;
                isRunning = false;
            }
        }

        public async Task<PingReply> Ping(string ip)
        {
            Task<PingReply> task = Task.Factory.StartNew(() =>
            {
                PingOptions options = new PingOptions(32, true);
                Ping ping = new Ping();
                PingReply replay = ping.Send(ip, 5000, new byte[] { 0x00 }, options);
                return replay;
            });
            return await task;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (status.Count>0)
            {
                var selected = (KeyValuePair<string, PingReply>)ResultList.SelectedItem  ;
                viewModel.BackHosts();
                viewModel.AddIpToHosts(new string[] { selected .Key});
                viewModel.CopyBackHosts();
            }
        }
    }
}