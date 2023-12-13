using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TCP_UDP_HMW
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string ipAddress = "127.0.0.1";
            int port = 8888;
            // Запуск сервера
            Process serverProcess = new Process();
            serverProcess.StartInfo.FileName = @"C:\Users\user\Desktop\сетквое\NetProg\Server\bin\Debug\\Server.exe"; // Замените на фактический путь
            serverProcess.StartInfo.Arguments = $"{ipAddress} {port}";
            serverProcess.Start();

            await Task.Delay(2000);

            // Запуск первого WPF клиента
            Process wpfClientProcess1 = new Process();
            wpfClientProcess1.StartInfo.FileName = @"C:\Users\user\Desktop\сетквое\NetProg\RCP_Game\bin\Debug\RCP_Game.exe";
            wpfClientProcess1.StartInfo.Arguments = $"{ipAddress} {port}";
            wpfClientProcess1.Start();

            // Ожидание завершения всех процессов
            serverProcess.WaitForExit();
            wpfClientProcess1.WaitForExit();
        }
    }
}
