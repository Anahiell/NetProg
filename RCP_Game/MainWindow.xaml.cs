using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Net.Sockets;
using System.Linq;
using System.IO;
namespace RCP_Game
{
    public partial class MainWindow : Window
    {
        private TcpClient tcpClient;
        private NetworkStream clientStream;
        private bool isConnected = false;
        private string serverIP = "127.0.0.1";
        private int serverPort = 8888;
        private DispatcherTimer resultTimer;
        private int playerNumber;

        public int CurrentImageIndex { get; set; } = 0;
        public string[] Images { get; set; } = { "rock.png", "paper.webp", "cut.webp" };
        public bool IsGameStarted { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            playerNumber = 0;
        }

        private void UpdateImage(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                Console.WriteLine("Error: Empty tag");
                return;
            }

            string imageName;

            switch (tag.ToLower())
            {
                case "rock":
                    imageName = "rock.png";
                    break;

                case "paper":
                    imageName = "paper.webp";
                    break;

                case "scissors":
                    imageName = "cut.webp";
                    break;

                default:
                    Console.WriteLine($"Error: Unknown tag - {tag}");
                    return;
            }

            string imagePath = $"C:\\Users\\user\\Desktop\\сетквое\\NetProg\\RCP_Game\\Source\\{imageName}";

            if (File.Exists(imagePath))
            {
                Dispatcher.Invoke(() =>
                {
                    CenterImage.Source = new BitmapImage(new Uri(imagePath));
                });
            }
            else
            {
                Console.WriteLine($"Error: Image file not found - {imagePath}");
            }
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                try
                {
                    await ConnectAsync();

                    await SendRequestAsync("get_player_number");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Connection error: {ex.Message}");
                }
            }
            else
            {
                Disconnect();
            }
        }

        private async Task ConnectAsync()
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIP, serverPort);

                clientStream = tcpClient.GetStream();
                isConnected = true;
                StatusTextBlock.Text = "Connected to the server";
                ConnectButton.Content = "Disconnect";

                await SendRequestAsync("get_player_number");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to the server: {ex.Message}");
                MessageBox.Show($"Connection error: {ex.Message}");
            }
        }

        private async void GameButton_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                if (IsGameStarted)
                {
                    Button clickedButton = (Button)sender;
                    string move = clickedButton.Tag.ToString();

                    await SendRequestAsync($"move:{move}");

                    await ReceiveAndProcessResponseAsync();
                }
                else
                {
                    MessageBox.Show("The game has not started. Click 'Start' to begin.");
                }
            }
            else
            {
                MessageBox.Show("Not connected to the server.");
            }
        }

        private async Task SendRequestAsync(string request)
        {
            byte[] requestBytes = Encoding.UTF8.GetBytes(request);
            await clientStream.WriteAsync(requestBytes, 0, requestBytes.Length);
        }

        private async Task ReceiveAndProcessResponseAsync()
        {
            byte[] responseBytes = new byte[4096];
            int bytesRead = await clientStream.ReadAsync(responseBytes, 0, responseBytes.Length);
            string response = Encoding.UTF8.GetString(responseBytes, 0, bytesRead);

            if (response.StartsWith("game_result:"))
            {
                string[] parts = response.Split(':');
                string gameResult = parts[1];
                string tag = parts[2];

                await UpdateGameResult(gameResult, tag);
            }
            else
            {
            }
        }

        private async Task UpdateGameResult(string gameResult, string tag)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                UpdateImage(tag);
                InfoTextBlock.Text = $"Результат: {gameResult}";
            });
        }


        private async void Disconnect()
        {
            if (isConnected)
            {
                tcpClient.Close();
                clientStream.Close();
                isConnected = false;
                StatusTextBlock.Text = "Disconnected";
                ConnectButton.Content = "Connect";
            }
        }
        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            IsGameStarted = true;
            await SendRequestAsync("start_game");

            InfoTextBlock.Text = "Статус";

            await ReceiveAndProcessResponseAsync();
        }

        private async void SurrenderButton_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected && IsGameStarted)
            {
                await SendRequestAsync("surrender");

                InfoTextBlock.Text = "Вы сдались. Игра окончена!";
                resultTimer.Stop();
            }
            else
            {
                MessageBox.Show("Не подключено к серверу или игра не начата.");
            }
        }
    }
}
