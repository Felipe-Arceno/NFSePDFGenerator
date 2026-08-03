using System;

namespace NfseNacional.PdfGenerator.Lib.Pdf
{
    /// <summary>
    /// Constantes de layout para o PDF da DANFSe.
    /// Todas as posições são em pontos (pt) para página A4 (595 x 842).
    /// Baseado na análise pixel-a-pixel dos PDFs de homologação gerados pela SEFIN Nacional.
    /// </summary>
    public static class DanfseLayout
    {
        // Página A4
        public const float PageWidth = 595f;
        public const float PageHeight = 842f;

        // Margens
        public const float MarginLeft = 5f;
        public const float MarginTop = 5f;
        public const float MarginRight = 5f;
        public const float MarginBottom = 5f;

        // Área útil
        public const float ContentLeft = 10.77f;
        public const float ContentRight = 577.70f;
        public const float ContentWidth = ContentRight - ContentLeft;

        // Borda externa
        public const float BorderX = 5f;
        public const float BorderY = 5f;
        public const float BorderWidth = 585f;
        public const float BorderHeight = 832f;

        // Colunas (4 colunas equidistantes)
        public const float Col1X = 14f;
        public const float Col2X = 156f;
        public const float Col3X = 298f;
        public const float Col4X = 439f;

        // Larguras das colunas
        public const float ColWidth = 141.7f; // Largura padrão de cada coluna
        public const float Col12Width = 283.4f; // Duas colunas juntas (Nome / Endereço)
        public const float Col34Width = 283.4f;

        // Tamanhos de fonte
        public const float FontSizeTitle = 9f;         // "DANFSe v1.0"
        public const float FontSizeLabel = 7f;         // Labels dos campos
        public const float FontSizeValue = 8f;         // Valores dos campos
        public const float FontSizeSectionTitle = 8f;   // Títulos de seção ("EMITENTE DA NFS-e")
        public const float FontSizeMunicipioHeader = 8f; // Nome município no header
        public const float FontSizeMunicipioDetail = 6f; // Detalhes município no header
        public const float FontSizeQrText = 6f;         // Texto junto ao QR Code

        // Espaçamentos verticais
        public const float LabelValueSpacing = 9.5f;    // Distância entre label e valor
        public const float RowHeight = 24f;              // Altura de uma linha (label + valor)
        public const float SectionTitleHeight = 12f;     // Altura do título de seção
        public const float SectionSpacing = 2f;          // Espaço entre seções
        public const float LineSpacing = 10f;            // Espaço entre linhas de texto

        // Header
        public const float HeaderLogoX = 14.17f;
        public const float HeaderLogoY = 13.97f;
        public const float HeaderLogoWidth = 113.39f;
        public const float HeaderLogoHeight = 22.47f;

        public const float HeaderTitleX = 225f;
        public const float HeaderTitleY = 9f;
        public const float HeaderSubtitleY = 20f;
        public const float HeaderWarningY = 32f;

        public const float HeaderMunicipioX = 439f;
        public const float HeaderMunicipioNameY = 9f;
        public const float HeaderMunicipioSecretariaY = 19f;
        public const float HeaderMunicipioTelefoneY = 27f;
        public const float HeaderMunicipioEmailY = 35f;

        // Brasão do município
        public const float HeaderBrasaoX = 402.57f;
        public const float HeaderBrasaoY = 8.5f;
        public const float HeaderBrasaoSize = 30f;

        // Linha separadora do header
        public const float HeaderLineY = 44.67f;

        // Chave de Acesso
        public const float ChaveAcessoLabelY = 49f;
        public const float ChaveAcessoValueY = 58f;

        // QR Code
        public const float QrCodeX = 484.33f;
        public const float QrCodeY = 49.17f;
        public const float QrCodeSize = 45f;
        public const float QrTextX = 439f;
        public const float QrTextY = 97f;
        public const float QrTextLineSpacing = 8f;

        // Dados da NFS-e (Número, Competência, Data/Hora)
        public const float DadosNfseRow1Y = 73f;
        public const float DadosNfseRow2Y = 98f;
        public const float DadosNfseLineY = 122f;

        // Seção Emitente
        public const float EmitenteTitleY = 122f;
        public const float EmitenteRow1Y = 122f;   // CNPJ, IM, Telefone
        public const float EmitenteRow2Y = 148f;   // Nome, Email
        public const float EmitenteRow3Y = 172f;   // Endereço, Município, CEP
        public const float EmitenteRow4Y = 196f;   // Simples Nacional, Regime Apuração
        public const float EmitenteEndLineY = 231.31f;

        // Seção Tomador
        public const float TomadorTitleY = 231.31f;
        public const float TomadorRow1Y = 232f;    // CNPJ, IM, Telefone
        public const float TomadorRow2Y = 256f;    // Nome, Email
        public const float TomadorRow3Y = 280f;    // Endereço, Município, CEP
        public const float TomadorEndLineY = 304.42f;

        // Seção Intermediário
        public const float IntermediarioY = 304.42f;

        // Seção Serviço Prestado
        public const float ServicoTitleY = 315.56f;
        public const float ServicoRow1Y = 316f;    // Cód. Trib. Nacional, Cód. Trib. Municipal, Local Prestação, País
        public const float ServicoRow2Y = 350f;    // Descrição

        // Separador após Serviço
        public const float ServicoEndLineY = 392.66f;

        // Seção Tributação Municipal
        public const float TribMunTitleY = 393f;

        // Seção Tributação Federal
        // Y positions calculadas dinamicamente

        // Seção Valor Total
        // Y positions calculadas dinamicamente

        // Totais Aproximados
        // Y positions calculadas dinamicamente

        // Informações Complementares
        // Y positions calculadas dinamicamente

        // Largura das linhas separadoras
        public const float SeparatorWidth = 0.5f;

        // Cor vermelha para "SEM VALIDADE JURÍDICA"
        public const int WarningColorR = 255;
        public const int WarningColorG = 0;
        public const int WarningColorB = 0;

        // Nomes das fontes
        public const string FontFamily = "Segoe WP";
        public const string FontFamilyFallback = "Arial";

        // URL base para QR Code
        public const string QrCodeBaseUrl = "https://www.nfse.gov.br/ConsultaPublica?chave=";
    }
}
