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
            string xmlPath = @"C:\Users\Admin\Desktop\XML_RETORNO_NACIONAL.xml";
            string outputPath = @"C:\Users\Admin\.gemini\antigravity\scratch\DANFSe_Teste_Gerado.pdf";

            Console.WriteLine($"Lendo XML de: {xmlPath}");
            var nfse = NfseXmlParser.ParseFromFile(xmlPath);

            Console.WriteLine($"Nota lida: Nº {nfse.InfNFSe.NNFSe}, Chave: {nfse.InfNFSe.ChaveAcesso}");
            Console.WriteLine($"Tomador: {nfse.InfNFSe.Dps.InfDps.Toma.XNome}");
            Console.WriteLine($"Valor Serviço: {nfse.InfNFSe.Dps.InfDps.Valores.VServ}");

            var gen = new DanfsePdfGenerator();
            gen.GeneratePdfFile(nfse, outputPath, new DadosMunicipio
            {
                Nome = "PREFEITURA DE MANAUS",
                Secretaria = "Secretaria Municipal de Finanças, Planejamento e Tecnologia da Informação - SEMEF",
                Telefone = "(92)3215-3424",
                Email = "nota.monitoramento@manaus.am.gov.br"
            });

            var fi = new FileInfo(outputPath);
            Console.WriteLine($"PDF gerado com sucesso em: {outputPath} (Tamanho: {fi.Length} bytes)");
        }
    }
}
