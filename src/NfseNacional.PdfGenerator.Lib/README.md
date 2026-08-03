# NfseNacional.PdfGenerator

Biblioteca .NET compatível com **.NET Standard 2.0**, **.NET Framework 4.6.1+**, **.NET Core 2.0+** e **.NET 5/6/7/8** (Aplicações Console, Web API, ASP.NET Core MVC, Windows Forms, WPF, etc.) para leitura de XML da NFS-e Nacional e geração de PDF (DANFSe).

## Autor
**Felipe Arceno**

## Funcionalidades
- **Parse Completo do XML**: Leitura do padrão nacional da Sefin Nacional / Receita Federal (DPS e NFS-e).
- **Geração de DANFSe em PDF**: Renderização em alta qualidade utilizando `PDFsharp` e `QRCoder`.
- **Cabeçalho Inteligente**: Quebra dinâmica de linhas para secretarias com nomes longos e reposicionamento automático de telefone e e-mail.
- **Repositório de Municípios e Brasões**: Gerenciamento integrado para logos de prefeituras por código IBGE.
- **Histórico de Auditoria**: Estrutura de armazenamento automática em pastas particionadas por data (`ANO/MES/DIA/GUID/`).

## Como Instalar

Via CLI do .NET:
```bash
dotnet add package NfseNacional.PdfGenerator
```

Via Console do Gerenciador de Pacotes do Visual Studio:
```powershell
Install-Package NfseNacional.PdfGenerator
```

## Exemplo Rápido de Uso

```csharp
using System.IO;
using NfseNacional.PdfGenerator.Lib.Models;
using NfseNacional.PdfGenerator.Lib.Parsers;
using NfseNacional.PdfGenerator.Lib.Pdf;

// 1. Ler o XML retornado pelo Sistema Nacional NFS-e
string xmlConteudo = File.ReadAllText("retorno.xml");
NfseRetorno nfse = NfseXmlParser.Parse(xmlConteudo);

// 2. Informar os dados do município emissor
var dadosMun = new DadosMunicipio
{
    Nome = "Prefeitura Municipal de Manaus",
    Secretaria = "Secretaria Municipal de Finanças, Planejamento e Tecnologia da Informação - SEMEF",
    Telefone = "(92) 3300-0000",
    Email = "atendimento.semef@manaus.am.gov.br",
    CaminhoBrasao = @"C:\brasoes\1302603.png"
};

// 3. Gerar o PDF (DANFSe)
var generator = new DanfsePdfGenerator();
bool isHomologacao = false; // true para ambiente de homologação (exibe marca d'água)

generator.GeneratePdfFile(nfse, "DANFSe_NotaFiscal.pdf", dadosMun, isHomologacao);
```

## Licença
MIT
