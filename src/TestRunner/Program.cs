using System;
using System.IO;
using NfseNacional.PdfGenerator.Lib.Models;
using NfseNacional.PdfGenerator.Lib.Parsers;
using NfseNacional.PdfGenerator.Lib.Pdf;

namespace TestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            // XML de exemplo: 1º argumento, ou o sample versionado no repositório.
            string xmlPath = args.Length > 0
                ? args[0]
                : LocalizarSample();

            // Pasta de saída: 2º argumento, ou a pasta atual.
            string outDir = args.Length > 1 ? args[1] : Directory.GetCurrentDirectory();
            Directory.CreateDirectory(outDir);

            Console.WriteLine($"Lendo XML de: {xmlPath}");
            var nfse = NfseXmlParser.ParseFromFile(xmlPath);

            Console.WriteLine($"Nota lida: Nº {nfse.InfNFSe.NNFSe}, Chave: {nfse.InfNFSe.ChaveAcesso}");
            Console.WriteLine($"Tomador: {nfse.InfNFSe.Dps.InfDps.Toma.XNome}");
            Console.WriteLine($"Valor Serviço: {nfse.InfNFSe.Dps.InfDps.Valores.VServ}");

            var dadosMun = new DadosMunicipio
            {
                Nome = "PREFEITURA DE ITAJAÍ",
                Secretaria = "Secretaria Municipal da Fazenda",
                Telefone = "(47)3241-7400",
                Email = "plantaofiscal@itajai.sc.gov.br"
            };

            var gen = new DanfsePdfGenerator();

            // Gera nas duas versões para comparação (produção = sem marca d'água).
            string outV2 = Path.Combine(outDir, "DANFSe_v2_Gerado.pdf");
            gen.GeneratePdfFile(nfse, outV2, dadosMun, isHomologacao: false, versao: DanfseVersao.V2_0);
            Console.WriteLine($"PDF v2.0 gerado: {outV2} ({new FileInfo(outV2).Length} bytes)");

            string outV1 = Path.Combine(outDir, "DANFSe_v1_Gerado.pdf");
            gen.GeneratePdfFile(nfse, outV1, dadosMun, isHomologacao: false, versao: DanfseVersao.V1_0);
            Console.WriteLine($"PDF v1.0 gerado: {outV1} ({new FileInfo(outV1).Length} bytes)");
        }

        /// <summary>Localiza o XML de exemplo versionado em /samples subindo a partir do binário.</summary>
        private static string LocalizarSample()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                string candidate = Path.Combine(dir.FullName, "samples", "NFSe_exemplo_v2.xml");
                if (File.Exists(candidate)) return candidate;
                dir = dir.Parent;
            }
            // Fallback: caminho relativo simples.
            return Path.Combine("samples", "NFSe_exemplo_v2.xml");
        }
    }
}
