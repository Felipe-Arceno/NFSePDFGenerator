using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NfseNacional.PdfGenerator.Lib.Models;

namespace NfseNacional.PdfGenerator.Lib.Repositories
{
    public class HistoricoRepository
    {
        public HistoricoItem Save(string basePath, string xmlContent, string pdfSourcePath, NfseRetorno nfse, DadosMunicipio mun)
        {
            string guid = Guid.NewGuid().ToString();
            DateTime now = DateTime.Now;

            string folder = Path.Combine(
                basePath,
                now.ToString("yyyy"),
                now.ToString("MM"),
                now.ToString("dd"),
                guid
            );

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string xmlPath = Path.Combine(folder, "retorno.xml");
            File.WriteAllText(xmlPath, xmlContent ?? "", Encoding.UTF8);

            string pdfFileName = Path.GetFileName(pdfSourcePath);
            if (string.IsNullOrWhiteSpace(pdfFileName))
            {
                pdfFileName = $"DANFSe_{nfse?.InfNFSe?.NNFSe ?? "Nota"}.pdf";
            }
            string pdfDestPath = Path.Combine(folder, pdfFileName);

            if (File.Exists(pdfSourcePath) && !string.Equals(Path.GetFullPath(pdfSourcePath), Path.GetFullPath(pdfDestPath), StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(pdfSourcePath, pdfDestPath, true);
            }

            var item = new HistoricoItem
            {
                Guid = guid,
                DataHora = now,
                NumeroNota = nfse?.InfNFSe?.NNFSe ?? "",
                ChaveAcesso = nfse?.InfNFSe?.ChaveAcesso ?? "",
                TomadorNome = nfse?.InfNFSe?.Dps?.InfDps?.Toma?.XNome ?? "",
                MunicipioNome = mun?.Nome ?? "",
                CodigoIbge = nfse?.InfNFSe?.Dps?.InfDps?.CLocEmi ?? "",
                CaminhoPasta = folder,
                ArquivoXml = xmlPath,
                ArquivoPdf = pdfDestPath,
                DadosMunicipio = mun
            };

            string infoJsonPath = Path.Combine(folder, "info.json");
            string json = JsonConvert.SerializeObject(item, Formatting.Indented);
            File.WriteAllText(infoJsonPath, json, Encoding.UTF8);

            return item;
        }

        public List<HistoricoItem> GetAll(string basePath)
        {
            var list = new List<HistoricoItem>();
            if (string.IsNullOrWhiteSpace(basePath) || !Directory.Exists(basePath))
            {
                return list;
            }

            try
            {
                string[] files = Directory.GetFiles(basePath, "info.json", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    try
                    {
                        string json = File.ReadAllText(file, Encoding.UTF8);
                        var item = JsonConvert.DeserializeObject<HistoricoItem>(json);
                        if (item != null)
                        {
                            // Garante que o caminho esteja atualizado caso a pasta base tenha sido movida
                            item.CaminhoPasta = Path.GetDirectoryName(file);
                            item.ArquivoXml = Path.Combine(item.CaminhoPasta, "retorno.xml");
                            
                            // Acha o primeiro pdf na pasta se o ArquivoPdf salvo não for encontrado
                            if (!File.Exists(item.ArquivoPdf))
                            {
                                var pdfs = Directory.GetFiles(item.CaminhoPasta, "*.pdf");
                                if (pdfs.Length > 0) item.ArquivoPdf = pdfs[0];
                            }

                            list.Add(item);
                        }
                    }
                    catch
                    {
                        // Ignora arquivos corrompidos
                    }
                }
            }
            catch
            {
                // Ignora erros de permissão ou leitura de diretório
            }

            return list.OrderByDescending(i => i.DataHora).ToList();
        }

        public void Delete(HistoricoItem item)
        {
            if (item != null && !string.IsNullOrWhiteSpace(item.CaminhoPasta) && Directory.Exists(item.CaminhoPasta))
            {
                try
                {
                    Directory.Delete(item.CaminhoPasta, true);
                }
                catch
                {
                    // Ignora se não conseguir excluir todos os arquivos abertos
                }
            }
        }
    }
}
