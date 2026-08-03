# NfseNacional.PdfGenerator

Biblioteca .NET e aplicação Windows Forms para leitura de XML de retorno do **Sistema Nacional NFS-e (Sefin Nacional / DPS)** e geração de PDF (**DANFSe**).

## 🚀 Funcionalidades
- **Parser de XML Nacional**: Leitura completa dos nós de DPS, NFS-e, tributação municipal/federal e IBS/CBS.
- **Geração de PDF (DANFSe)**: Renderização com alta qualidade e fidelidade visual usando `PDFsharp` e `QRCoder`.
- **Cabeçalho Dinâmico**: Quebra de linha inteligente para secretarias extensas e alinhamento automático de telefone e e-mail.
- **Aplicação WinForms**:
  - Aba de Geração de PDF com função **Identar XML (Pretty Print)**.
  - Cadastro e gerenciamento de Municípios com brasões por código IBGE.
  - Histórico de Emissões estruturado em `ANO/MES/DIA/GUID/`.
- **Pronto para NuGet**: Configurado para empacotamento da biblioteca `.Lib`.

## 📁 Estrutura do Repositório
```text
NFSePDFGenerator/
├── src/
│   ├── NfseNacional.PdfGenerator.Lib/       # Biblioteca Core (.NET Standard 2.0)
│   └── NfseNacional.PdfGenerator.WinForms/  # Aplicação Desktop de Teste/Uso (.NET 8-windows)
├── nupkg/                                  # Pacote NuGet (.nupkg)
└── NfseNacional.PdfGenerator.sln           # Arquivo de Solução
```

## 🛠️ Como Compilar e Rodar

### Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) ou superior.

### Compilar a Solução
```bash
dotnet build NfseNacional.PdfGenerator.sln -c Release
```

### Executar a aplicação WinForms
```bash
dotnet run --project src/NfseNacional.PdfGenerator.WinForms/NfseNacional.PdfGenerator.WinForms.csproj
```

### Gerar Pacote NuGet
```bash
dotnet pack src/NfseNacional.PdfGenerator.Lib/NfseNacional.PdfGenerator.Lib.csproj -c Release -o ./nupkg
```

## 👤 Autor
- **Felipe Arceno**

## 📄 Licença
[MIT](LICENSE)
