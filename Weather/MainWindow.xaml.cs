using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Weather
{
    public partial class MainWindow : Window
    {
        private const string SinoptikUrl = "https://sinoptik.ua/";
        private HttpClient httpClient;

        public MainWindow()
        {
            InitializeComponent();
            httpClient = new HttpClient();
        }



        private async Task<string> GetWeatherDataAsync(string cityName)
        {
            // Замените пробелы в названии города на символы "-", чтобы соблюсти формат URL
            string formattedCityName = cityName.Replace(" ", "-");

            // Формируем URL
            string apiUrl = $"{SinoptikUrl}погода-{formattedCityName}";

            // Остальной код остается без изменений
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string htmlContent = await response.Content.ReadAsStringAsync();
                return htmlContent;
            }
            else
            {
                throw new HttpRequestException($"Failed to get weather data. Status code: {response.StatusCode}");
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedCityItem = (ComboBoxItem)cityComboBox.SelectedItem;
            if (selectedCityItem == null)
            {
                MessageBox.Show("Please select a city.");
                return;
            }

            string cityName = selectedCityItem.Content.ToString();

            try
            {
                string weatherData = await GetWeatherDataAsync(cityName);
                weatherResultTextBlock.Text = weatherData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting weather data: {ex.Message}");
            }
        }
    }
}
