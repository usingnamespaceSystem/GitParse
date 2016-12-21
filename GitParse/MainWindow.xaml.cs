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
using System.Net;
using System.IO;
using LitJson;
namespace GitParse
{
    public partial class MainWindow : Window
    {
        JsonData data;
        public MainWindow()
        {
            InitializeComponent();
            Parse();
        }

        protected string Get_repos(string url)
        {
            try
            {

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.UserAgent = "GIT_Parse";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                MessageBox.Show("Too much requests per hour. Try again later.");
                return String.Empty;
            }
        }

        protected void Parse()
        {
            string json = Get_repos("https://api.github.com/repositories?client_id=c24d271954910f698d45&client_secret=ca09cf626abf98c9bc39378379a6964807e0563c");

            if (json != String.Empty)
            {
                data = JsonMapper.ToObject(json);

                foreach (JsonData jsd in data)
                    listView.Items.Add(jsd["owner"]["login"].ToString());
            }
        }

        private void listView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var is_item = sender as ListViewItem;

            if (is_item != null && is_item.IsSelected)
            {
                Window profile = new Window();
                ListView list = new ListView();
                StackPanel stack = new StackPanel();

                profile.Height = 200;
                profile.Width = 330;
                profile.Title = "Profile " + listView.SelectedItem.ToString();
                profile.Topmost = true;

                stack.Height = 100;
                stack.Width = 330;

                stack.Children.Add(list);
                profile.Content = stack;

                foreach (JsonData jsd in data)
                {
                    if (jsd["owner"]["login"].ToString() == listView.SelectedItem.ToString())
                    {
                        string prof = Get_repos(jsd["owner"]["url"].ToString());

                        if (prof != String.Empty)
                        {
                            JsonData profile_data = JsonMapper.ToObject(prof);

                            if (profile_data["name"] != null)
                                list.Items.Add("Name: " + profile_data["name"].ToString());
                            else
                                list.Items.Add("Name: ");

                            if (profile_data["email"] != null)
                                list.Items.Add("E-mail: " + profile_data["email"].ToString());
                            else
                                list.Items.Add("E-mail: ");

                            if (profile_data["created_at"] != null)
                                list.Items.Add("Created: " + profile_data["created_at"].ToString());
                            else
                                list.Items.Add("Created: ");

                            if (profile_data["location"] != null)
                                list.Items.Add("Location: " + profile_data["location"].ToString());
                            else
                            break;
                        }
                    }
                }

                profile.Show();
            }
        }
    }
}

