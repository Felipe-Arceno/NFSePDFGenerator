using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NfseNacional.PdfGenerator.Lib.Models;

namespace NfseNacional.PdfGenerator.Lib.Repositories
{
    public class AppConfigRepository
    {
        private readonly string _configFilePath;
        private readonly object _lockObj = new object();

        public AppConfigRepository(string configDirectory)
        {
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }
            _configFilePath = Path.Combine(configDirectory, "app_config.json");
        }

        public AppConfig Load()
        {
            lock (_lockObj)
            {
                if (!File.Exists(_configFilePath))
                {
                    var defaultConfig = new AppConfig
                    {
                        HistoricoBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Historico")
                    };
                    Save(defaultConfig);
                    return defaultConfig;
                }

                try
                {
                    string json = File.ReadAllText(_configFilePath, Encoding.UTF8);
                    var config = JsonConvert.DeserializeObject<AppConfig>(json);
                    if (config == null || string.IsNullOrWhiteSpace(config.HistoricoBasePath))
                    {
                        config = new AppConfig
                        {
                            HistoricoBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Historico")
                        };
                    }
                    return config;
                }
                catch
                {
                    return new AppConfig
                    {
                        HistoricoBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Historico")
                    };
                }
            }
        }

        public void Save(AppConfig config)
        {
            lock (_lockObj)
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(_configFilePath, json, Encoding.UTF8);
            }
        }
    }
}
