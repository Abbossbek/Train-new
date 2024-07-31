using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Train
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string videosPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pages/videos");
            if (!System.IO.Directory.Exists(videosPath))
            {
                System.IO.Directory.CreateDirectory(videosPath);
            }
            var videos = System.IO.Directory.GetFiles(videosPath);
            var videosHtmlPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pages/videos.html");
            var html = File.ReadAllText(videosHtmlPath);
            html = html.Replace("[]", JsonSerializer.Serialize(videos.Select(x=>Path.GetFileName(x))));
            File.WriteAllText(videosHtmlPath, html);
        }
    }
}
