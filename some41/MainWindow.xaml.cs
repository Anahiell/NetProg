using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace some41
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Добавим обработчик событий для текстового поля поиска
            searchTextBox.GotFocus += SearchTextBox_GotFocus;
            searchTextBox.LostFocus += SearchTextBox_LostFocus;
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // При получении фокуса убираем Watermark
            if (searchTextBox.Text == "Enter search text")
            {
                searchTextBox.Text = "";
                searchTextBox.Foreground = SystemColors.WindowTextBrush;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // При потере фокуса, если текстовое поле пустое, возвращаем Watermark
            if (string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                searchTextBox.Text = "Enter search text";
                searchTextBox.Foreground = Brushes.Gray;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch("web");
        }

        private void ImageSearchButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch("images");
        }

        private void PerformSearch(string searchType)
        {
            // Оставшаяся часть кода остается без изменений
            string searchText = searchTextBox.Text;

            // Проверяем, что введен текст
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                // Определение выбранного поискового движка
                string searchEngine = ((ComboBoxItem)searchEngineComboBox.SelectedItem)?.Content.ToString().ToLower();

                // Создаем URL для поиска
                string searchUrl = "";

                if (searchEngine == "google")
                {
                    if (searchType == "images")
                    {
                        searchUrl = $"https://www.google.com/search?q={WebUtility.UrlEncode(searchText)}&tbm=isch";
                    }
                    else
                    {
                        searchUrl = $"https://www.google.com/search?q={WebUtility.UrlEncode(searchText)}";
                    }
                }
                else if (searchEngine == "bing")
                {
                    if (searchType == "images")
                    {
                        searchUrl = $"https://www.bing.com/images/search?q={WebUtility.UrlEncode(searchText)}";
                    }
                    else
                    {
                        // Создаем URL для поиска в Bing (без ключа API)
                        searchUrl = $"https://www.bing.com/search?q={WebUtility.UrlEncode(searchText)}";
                    }
                }

                // Отображаем URL в WebBrowser
                resultWebBrowser.Navigate(searchUrl);
            }
        }
    }
}