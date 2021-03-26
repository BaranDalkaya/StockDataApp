using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using System.Reflection;

namespace CompanyOverview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public async Task<Root> GetData<T>(string url)
        {
            var aRoot = new Root();
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var ResponseString = await response.Content.ReadAsStringAsync();
                        var ResponseObject = JsonConvert.DeserializeObject<Root>(ResponseString);
                        
                        return ResponseObject;
                    }
                    return aRoot;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid ticker");
                return aRoot;
            }
        }

        private void PreviewData(Root val)
        { 
            foreach (Label label in lblvalues.Children)
            {
                var prop = GetProp(label.Name);

                if (prop != null)
                {
                    label.Content = prop.GetValue(val);
                }
            }

            tbDescription.Text = val.Description;
        }

        private PropertyInfo GetProp(string name)
        {
            string propName = name.Substring(3);
            var properties = typeof(Root).GetProperties();
            return properties.Where(x => x.Name == propName).FirstOrDefault();
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            // Please insert your unique api key from alphavantage url: https://www.alphavantage.co/
            var apiKey = "";
            var symbol = tbTicker.Text;
            var root = await GetData<Root>($"https://www.alphavantage.co/query?function=OVERVIEW&symbol={symbol}&apikey={apiKey}");
            PreviewData(root);
        }

        private void ClearControl()
        {
            tbTicker.Text = string.Empty;
            tbDescription.Text = string.Empty;
            foreach (Label label in lblvalues.Children)
            {
                label.Content = string.Empty;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControl();
        }
    }
}
