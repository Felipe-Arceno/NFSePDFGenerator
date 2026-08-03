using System;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using NfseNacional.PdfGenerator.Lib.Models;
using NfseNacional.PdfGenerator.Lib.Helpers;

namespace NfseNacional.PdfGenerator.Lib.Pdf
{
    public class DanfsePdfGenerator
    {
        private XFont _fontTitleBold;
        private XFont _fontLabelBold;
        private XFont _fontValue;
        private XFont _fontSectionBold;
        private XFont _fontWarningBold;
        private XFont _fontHeaderSmall;
        private XPen _penBorder;

        public DanfsePdfGenerator()
        {
            WindowsFontResolver.EnsureInitialized();
            InitializeFonts();
            _penBorder = new XPen(XColors.Black, DanfseLayout.SeparatorWidth);
        }

        private void InitializeFonts()
        {
            string fontName = DanfseLayout.FontFamily;
            try
            {
                _fontTitleBold = new XFont(fontName, DanfseLayout.FontSizeTitle, XFontStyleEx.Bold);
                _fontLabelBold = new XFont(fontName, DanfseLayout.FontSizeLabel, XFontStyleEx.Bold);
                _fontValue = new XFont(fontName, DanfseLayout.FontSizeValue, XFontStyleEx.Regular);
                _fontSectionBold = new XFont(fontName, DanfseLayout.FontSizeSectionTitle, XFontStyleEx.Bold);
                _fontWarningBold = new XFont(fontName, DanfseLayout.FontSizeTitle, XFontStyleEx.Bold);
                _fontHeaderSmall = new XFont(fontName, DanfseLayout.FontSizeMunicipioDetail, XFontStyleEx.Regular);
            }
            catch
            {
                fontName = DanfseLayout.FontFamilyFallback;
                _fontTitleBold = new XFont(fontName, DanfseLayout.FontSizeTitle, XFontStyleEx.Bold);
                _fontLabelBold = new XFont(fontName, DanfseLayout.FontSizeLabel, XFontStyleEx.Bold);
                _fontValue = new XFont(fontName, DanfseLayout.FontSizeValue, XFontStyleEx.Regular);
                _fontSectionBold = new XFont(fontName, DanfseLayout.FontSizeSectionTitle, XFontStyleEx.Bold);
                _fontWarningBold = new XFont(fontName, DanfseLayout.FontSizeTitle, XFontStyleEx.Bold);
                _fontHeaderSmall = new XFont(fontName, DanfseLayout.FontSizeMunicipioDetail, XFontStyleEx.Regular);
            }
        }

        public byte[] GeneratePdfBytes(NfseRetorno nfse, DadosMunicipio dadosMun = null, bool isHomologacao = true)
        {
            if (nfse == null || nfse.InfNFSe == null)
                throw new ArgumentNullException(nameof(nfse));

            if (dadosMun == null)
                dadosMun = new DadosMunicipio();

            using (var doc = new PdfDocument())
            {
                doc.Info.Title = $"DANFSe - NFS-e {nfse.InfNFSe.NNFSe}";
                doc.Info.Subject = "Documento Auxiliar da NFS-e";

                var page = doc.AddPage();
                page.Width = XUnit.FromPoint(DanfseLayout.PageWidth);
                page.Height = XUnit.FromPoint(DanfseLayout.PageHeight);

                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    RenderPage(gfx, nfse.InfNFSe, dadosMun, isHomologacao);
                }

                using (var ms = new MemoryStream())
                {
                    doc.Save(ms, false);
                    return ms.ToArray();
                }
            }
        }

        public void GeneratePdfFile(NfseRetorno nfse, string outputFilePath, DadosMunicipio dadosMun = null, bool isHomologacao = true)
        {
            byte[] bytes = GeneratePdfBytes(nfse, dadosMun, isHomologacao);
            File.WriteAllBytes(outputFilePath, bytes);
        }

        private void RenderPage(XGraphics gfx, InfNFSe inf, DadosMunicipio mun, bool isHomologacao)
        {
            var dps = inf.Dps?.InfDps;

            // 1. Borda Externa Principal
            gfx.DrawRectangle(_penBorder, DanfseLayout.BorderX, DanfseLayout.BorderY, DanfseLayout.BorderWidth, DanfseLayout.BorderHeight);

            // 2. Cabeçalho (Header)
            // Logo NFSe à esquerda
            if (!string.IsNullOrEmpty(mun.CaminhoLogoNfse) && File.Exists(mun.CaminhoLogoNfse))
            {
                try {
                    using (var xImg = XImage.FromFile(mun.CaminhoLogoNfse))
                    {
                        gfx.DrawImage(xImg, DanfseLayout.HeaderLogoX, DanfseLayout.HeaderLogoY, DanfseLayout.HeaderLogoWidth, DanfseLayout.HeaderLogoHeight);
                    }
                } catch {}
            }
            else
            {
                var fontNfs = new XFont(_fontTitleBold.FontFamily.Name, 22f, XFontStyleEx.Bold);
                var brushGreen = new XSolidBrush(XColor.FromArgb(0, 140, 60));
                var brushBlue = new XSolidBrush(XColor.FromArgb(0, 80, 160));
                gfx.DrawString("NFS", fontNfs, brushGreen, 14f, 26f);
                gfx.DrawString("e", fontNfs, brushBlue, 58f, 26f);
                
                var fontSubLogo = new XFont(_fontValue.FontFamily.Name, 6.5f, XFontStyleEx.Regular);
                var brushGray = new XSolidBrush(XColor.FromArgb(100, 100, 100));
                gfx.DrawString("Nota Fiscal de", fontSubLogo, brushGray, 73f, 18f);
                gfx.DrawString("Serviço eletrônica", fontSubLogo, brushGray, 73f, 26f);
            }

            // Títulos Centrais
            gfx.DrawString("DANFSe v1.0", _fontTitleBold, XBrushes.Black, DanfseLayout.HeaderTitleX, 9f + 8);
            gfx.DrawString("Documento Auxiliar da NFS-e", _fontTitleBold, XBrushes.Black, DanfseLayout.HeaderTitleX, 20f + 8);
            
            if (isHomologacao)
            {
                var brushWarning = new XSolidBrush(XColor.FromArgb(DanfseLayout.WarningColorR, DanfseLayout.WarningColorG, DanfseLayout.WarningColorB));
                gfx.DrawString("NFS-e SEM VALIDADE JURÍDICA", _fontTitleBold, brushWarning, DanfseLayout.HeaderTitleX, 32f + 8);
            }

            // Brasão do Município
            if (mun.BrasaoBytes != null && mun.BrasaoBytes.Length > 0)
            {
                try {
                    using (var msImg = new MemoryStream(mun.BrasaoBytes))
                    using (var xImg = XImage.FromStream(msImg))
                    {
                        gfx.DrawImage(xImg, DanfseLayout.HeaderBrasaoX, DanfseLayout.HeaderBrasaoY, DanfseLayout.HeaderBrasaoSize, DanfseLayout.HeaderBrasaoSize);
                    }
                } catch {}
            }
            else if (!string.IsNullOrEmpty(mun.CaminhoBrasao) && File.Exists(mun.CaminhoBrasao))
            {
                try {
                    using (var xImg = XImage.FromFile(mun.CaminhoBrasao))
                    {
                        gfx.DrawImage(xImg, DanfseLayout.HeaderBrasaoX, DanfseLayout.HeaderBrasaoY, DanfseLayout.HeaderBrasaoSize, DanfseLayout.HeaderBrasaoSize);
                    }
                } catch {}
            }

            // Dados do Município
            var fontMunBold = new XFont(_fontValue.FontFamily.Name, 8f, XFontStyleEx.Bold);
            float maxMunWidth = DanfseLayout.BorderX + DanfseLayout.BorderWidth - DanfseLayout.HeaderMunicipioX - 4f;

            var headerLines = new System.Collections.Generic.List<Tuple<string, XFont, bool>>();
            if (!string.IsNullOrWhiteSpace(mun.Nome))
            {
                foreach (var l in WrapTextToLines(gfx, mun.Nome, fontMunBold, maxMunWidth))
                    headerLines.Add(Tuple.Create(l, fontMunBold, true));
            }
            if (!string.IsNullOrWhiteSpace(mun.Secretaria))
            {
                foreach (var l in WrapTextToLines(gfx, mun.Secretaria, _fontHeaderSmall, maxMunWidth))
                    headerLines.Add(Tuple.Create(l, _fontHeaderSmall, false));
            }
            if (!string.IsNullOrWhiteSpace(mun.Telefone))
            {
                foreach (var l in WrapTextToLines(gfx, mun.Telefone, _fontHeaderSmall, maxMunWidth))
                    headerLines.Add(Tuple.Create(l, _fontHeaderSmall, false));
            }
            if (!string.IsNullOrWhiteSpace(mun.Email))
            {
                foreach (var l in WrapTextToLines(gfx, mun.Email, _fontHeaderSmall, maxMunWidth))
                    headerLines.Add(Tuple.Create(l, _fontHeaderSmall, false));
            }

            int count = headerLines.Count;
            float yMun = 14f;
            float stepNome = 8.0f;
            float stepSmall = 7.2f;

            if (count > 4)
            {
                yMun = 13.5f;
                stepSmall = Math.Min(6.8f, (42.5f - 13.5f - stepNome) / Math.Max(1, count - 2));
            }
            else if (count < 4)
            {
                yMun = 16f;
                stepSmall = 7.5f;
            }

            for (int i = 0; i < count; i++)
            {
                var item = headerLines[i];
                gfx.DrawString(item.Item1, item.Item2, XBrushes.Black, DanfseLayout.HeaderMunicipioX, yMun);
                if (i < count - 1)
                {
                    yMun += item.Item3 ? stepNome : stepSmall;
                }
            }

            // Linha separadora do cabeçalho
            DrawHorizontalLine(gfx, 45f);

            // 3. Chave de Acesso & QR Code
            DrawField(gfx, "Chave de Acesso da NFS-e", inf.ChaveAcesso, DanfseLayout.Col1X, 48f);

            // QR Code em vetor
            QrCodeGenerator.RenderQrCodeVector(gfx, inf.ChaveAcesso, DanfseLayout.QrCodeX, 49f, 45f);

            // Texto ao lado do QR Code
            gfx.DrawString("A autenticidade desta NFS-e pode ser verificada", _fontHeaderSmall, XBrushes.Black, DanfseLayout.QrTextX, 97f);
            gfx.DrawString("pela leitura deste código QR ou pela consulta da", _fontHeaderSmall, XBrushes.Black, DanfseLayout.QrTextX, 97f + 8);
            gfx.DrawString("chave de acesso no portal nacional da NFS-e", _fontHeaderSmall, XBrushes.Black, DanfseLayout.QrTextX, 97f + 16);

            // 4. Dados da Nota (Linhas 1 e 2)
            DrawField(gfx, "Número da NFS-e", inf.NNFSe, DanfseLayout.Col1X, 73f);
            DrawField(gfx, "Competência da NFS-e", ValueFormatter.FormatDate(dps?.DCompet), DanfseLayout.Col2X, 73f);
            DrawField(gfx, "Data e Hora da emissão da NFS-e", ValueFormatter.FormatDateTime(inf.DhProc), DanfseLayout.Col3X, 73f);

            DrawField(gfx, "Número da DPS", dps?.NDPS, DanfseLayout.Col1X, 97f);
            DrawField(gfx, "Série da DPS", dps?.Serie, DanfseLayout.Col2X, 97f);
            DrawField(gfx, "Data e Hora da emissão da DPS", ValueFormatter.FormatDateTime(dps?.DhEmi), DanfseLayout.Col3X, 97f);

            DrawHorizontalLine(gfx, 120f);

            // 5. Emitente da NFS-e / Prestador
            DrawSectionHeader(gfx, "EMITENTE DA NFS-e", "Prestador do Serviço", 121f);
            DrawField(gfx, "CNPJ / CPF / NIF", CnpjCpfFormatter.FormatCnpjCpf(inf.Emit?.Cnpj ?? inf.Emit?.Cpf), DanfseLayout.Col2X, 121f);
            DrawField(gfx, "Inscrição Municipal", ValueFormatter.FormatOrDash(inf.Emit?.Im), DanfseLayout.Col3X, 121f);
            DrawField(gfx, "Telefone", ValueFormatter.FormatOrDash(inf.Emit?.Fone), DanfseLayout.Col4X, 121f);

            DrawField(gfx, "Nome / Nome Empresarial", inf.Emit?.XNome, DanfseLayout.Col1X, 143f);
            DrawField(gfx, "E-mail", ValueFormatter.FormatOrDash(inf.Emit?.Email), DanfseLayout.Col3X, 143f);

            string enderecoEmit = EnderecoFormatter.FormatEndereco(inf.Emit?.EnderNac?.XLgr, inf.Emit?.EnderNac?.Nro, inf.Emit?.EnderNac?.XCpl, inf.Emit?.EnderNac?.XBairro);
            float endEmitBottom = DrawFieldMultiline(gfx, "Endereço", enderecoEmit, DanfseLayout.Col1X, 165f, 275f);
            DrawField(gfx, "Município", MunicipioHelper.GetMunicipioNome(inf.Emit?.EnderNac?.CMun ?? inf.CLocIncid), DanfseLayout.Col3X, 165f);
            DrawField(gfx, "CEP", CepFormatter.FormatCep(inf.Emit?.EnderNac?.Cep), DanfseLayout.Col4X, 165f);

            float yEmitRow4 = Math.Max(187f, endEmitBottom + 6f);
            DrawField(gfx, "Simples Nacional na Data de Competência", NfseDescriptionHelper.GetSimplesNacionalDescricao(dps?.Prest?.RegTrib?.OpSimpNac), DanfseLayout.Col1X, yEmitRow4);
            float regBottom = DrawFieldMultiline(gfx, "Regime de Apuração Tributária pelo SN", NfseDescriptionHelper.GetRegApTribSNDescricao(dps?.Prest?.RegTrib?.RegApTribSN), DanfseLayout.Col3X, yEmitRow4, 275f);

            float yCur = Math.Max(yEmitRow4 + 24f, regBottom + 8f);
            DrawHorizontalLine(gfx, yCur);

            // 6. Tomador do Serviço
            DrawSectionHeader(gfx, "TOMADOR DO SERVIÇO", null, yCur + 1f);
            float yToma1 = yCur + 1f;
            DrawField(gfx, "CNPJ / CPF / NIF", CnpjCpfFormatter.FormatCnpjCpf(dps?.Toma?.Cnpj ?? dps?.Toma?.Cpf), DanfseLayout.Col2X, yToma1);
            DrawField(gfx, "Inscrição Municipal", ValueFormatter.FormatOrDash(dps?.Toma?.Im), DanfseLayout.Col3X, yToma1);
            DrawField(gfx, "Telefone", ValueFormatter.FormatOrDash(dps?.Toma?.Fone), DanfseLayout.Col4X, yToma1);

            float yToma2 = yToma1 + 22f;
            DrawField(gfx, "Nome / Nome Empresarial", dps?.Toma?.XNome, DanfseLayout.Col1X, yToma2);
            DrawField(gfx, "E-mail", ValueFormatter.FormatOrDash(dps?.Toma?.Email), DanfseLayout.Col3X, yToma2);

            float yToma3 = yToma2 + 22f;
            string enderecoToma = EnderecoFormatter.FormatEndereco(dps?.Toma?.End?.XLgr, dps?.Toma?.End?.Nro, dps?.Toma?.End?.XCpl, dps?.Toma?.End?.XBairro);
            float endTomaBottom = DrawFieldMultiline(gfx, "Endereço", enderecoToma, DanfseLayout.Col1X, yToma3, 275f);
            DrawField(gfx, "Município", MunicipioHelper.GetMunicipioNome(dps?.Toma?.End?.CMun), DanfseLayout.Col3X, yToma3);
            DrawField(gfx, "CEP", CepFormatter.FormatCep(dps?.Toma?.End?.Cep), DanfseLayout.Col4X, yToma3);

            yCur = Math.Max(yToma3 + 23f, endTomaBottom + 6f);
            DrawHorizontalLine(gfx, yCur);

            // 7. Intermediário
            DrawCenteredSectionHeader(gfx, "INTERMEDIÁRIO DO SERVIÇO NÃO IDENTIFICADO NA NFS-e", yCur);
            yCur += 14f;
            DrawHorizontalLine(gfx, yCur);

            // 8. Serviço Prestado
            DrawSectionHeader(gfx, "SERVIÇO PRESTADO", null, yCur + 1f);
            float yServ1 = yCur + 13f;
            float servCodNacBottom = DrawFieldMultiline(gfx, "Código de Tributação Nacional", NfseDescriptionHelper.FormatCodTribNacional(dps?.Serv?.CTribNac, inf.XTribNac), DanfseLayout.Col1X, yServ1, 135f);
            float servCodMunBottom = DrawFieldMultiline(gfx, "Código de Tributação Municipal", NfseDescriptionHelper.FormatCodTribMunicipal(dps?.Serv?.CTribMun, dps?.Serv?.XTribMun ?? inf.XTribMun, inf.XTribNac), DanfseLayout.Col2X, yServ1, 135f);
            DrawField(gfx, "Local da Prestação", MunicipioHelper.GetMunicipioNome(dps?.Serv?.CLocPrestacao ?? inf.XLocPrestacao), DanfseLayout.Col3X, yServ1);
            DrawField(gfx, "País da Prestação", "-", DanfseLayout.Col4X, yServ1);

            float yServ2 = Math.Max(yServ1 + 24f, Math.Max(servCodNacBottom, servCodMunBottom) + 6f);
            float descBottom = DrawFieldMultiline(gfx, "Descrição do Serviço", NfseDescriptionHelper.TruncateWithEllipsis(dps?.Serv?.XDescServ, 800), DanfseLayout.Col1X, yServ2, 560f);

            yCur = Math.Max(yServ2 + 24f, descBottom + 8f);
            DrawHorizontalLine(gfx, yCur);

            // 9. Tributação Municipal
            DrawSectionHeader(gfx, "TRIBUTAÇÃO MUNICIPAL", null, yCur + 1f);
            
            float yTrib1 = yCur + 13f;
            DrawField(gfx, "Tributação do ISSQN", NfseDescriptionHelper.GetTribISSQNDescricao(dps?.Valores?.Trib?.TribMun?.TribISSQN), DanfseLayout.Col1X, yTrib1);
            DrawField(gfx, "País Resultado da Prestação do Serviço", "-", DanfseLayout.Col2X, yTrib1);
            DrawField(gfx, "Município de Incidência do ISSQN", MunicipioHelper.GetMunicipioNome(inf.CLocIncid), DanfseLayout.Col3X, yTrib1);
            DrawField(gfx, "Regime Especial de Tributação", NfseDescriptionHelper.GetRegEspTribDescricao(dps?.Prest?.RegTrib?.RegEspTrib), DanfseLayout.Col4X, yTrib1);

            float yTrib2 = yTrib1 + 22f;
            DrawField(gfx, "Tipo de Imunidade", "-", DanfseLayout.Col1X, yTrib2);
            DrawField(gfx, "Suspensão da Exigibilidade do ISSQN", NfseDescriptionHelper.GetTpSuspDescricao(dps?.Valores?.Trib?.TribMun?.TpSusp), DanfseLayout.Col2X, yTrib2);
            string nBmVal = dps?.Valores?.Trib?.TribMun?.BM?.NBM ?? dps?.Valores?.Trib?.TribMun?.CBenMun;
            DrawField(gfx, "Benefício Municipal", ValueFormatter.FormatOrDash(nBmVal), DanfseLayout.Col4X, yTrib2);

            float yTrib3 = yTrib2 + 22f;
            DrawField(gfx, "Valor do Serviço", ValueFormatter.FormatCurrency(dps?.Valores?.VServ), DanfseLayout.Col1X, yTrib3);
            DrawField(gfx, "Desconto Incondicionado", ValueFormatter.FormatCurrency(dps?.Valores?.VDescIncond), DanfseLayout.Col2X, yTrib3);
            DrawField(gfx, "Total Deduções/Reduções", ValueFormatter.FormatCurrency(dps?.Valores?.VDed), DanfseLayout.Col3X, yTrib3);
            
            string calculoBm = "-";
            if (dps?.Valores?.Trib?.TribMun?.BM != null)
            {
                var bm = dps.Valores.Trib.TribMun.BM;
                if (!string.IsNullOrWhiteSpace(bm.VRedBCBM))
                    calculoBm = ValueFormatter.FormatCurrency(bm.VRedBCBM);
                else if (!string.IsNullOrWhiteSpace(bm.PRedBCBM))
                    calculoBm = ValueFormatter.FormatPercentage(bm.PRedBCBM);
            }
            DrawField(gfx, "Cálculo do BM", calculoBm, DanfseLayout.Col4X, yTrib3);

            float yTrib4 = yTrib3 + 22f;
            DrawField(gfx, "BC ISSQN", ValueFormatter.FormatCurrency(inf.Valores?.VBC), DanfseLayout.Col1X, yTrib4);
            DrawField(gfx, "Alíquota Aplicada", ValueFormatter.FormatPercentage(inf.Valores?.PAliqAplic), DanfseLayout.Col2X, yTrib4);
            DrawField(gfx, "Retenção do ISSQN", NfseDescriptionHelper.GetTpRetISSQNDescricao(dps?.Valores?.Trib?.TribMun?.TpRetISSQN), DanfseLayout.Col3X, yTrib4);
            DrawField(gfx, "ISSQN Apurado", ValueFormatter.FormatCurrency(inf.Valores?.VISSQN), DanfseLayout.Col4X, yTrib4);

            yCur = yTrib4 + 23f;
            DrawHorizontalLine(gfx, yCur);

            // 10. Tributação Federal
            DrawSectionHeader(gfx, "TRIBUTAÇÃO FEDERAL", null, yCur + 1f);
            float yFed1 = yCur + 13f;
            DrawField(gfx, "IRRF", ValueFormatter.FormatCurrency(dps?.Valores?.Trib?.TribFed?.VRetIRRF), DanfseLayout.Col1X, yFed1);
            DrawField(gfx, "Contribuição Previdenciária - Retida", ValueFormatter.FormatCurrency(dps?.Valores?.Trib?.TribFed?.VRetCP), DanfseLayout.Col2X, yFed1);
            DrawField(gfx, "Contribuições Sociais - Retidas", ValueFormatter.FormatCurrency(dps?.Valores?.Trib?.TribFed?.VRetCSLL), DanfseLayout.Col3X, yFed1);
            DrawField(gfx, "Descrição Contrib. Sociais - Retidas", ValueFormatter.FormatOrDash(dps?.Valores?.Trib?.TribFed?.XDescRetCSLL), DanfseLayout.Col4X, yFed1);

            float yFed2 = yFed1 + 22f;
            DrawField(gfx, "PIS - Débito Apuração Própria", ValueFormatter.FormatCurrency(dps?.Valores?.Trib?.TribFed?.PisCofins?.VPis), DanfseLayout.Col1X, yFed2);
            DrawField(gfx, "COFINS - Débito Apuração Própria", ValueFormatter.FormatCurrency(dps?.Valores?.Trib?.TribFed?.PisCofins?.VCofins), DanfseLayout.Col2X, yFed2);

            yCur = yFed2 + 23f;
            DrawHorizontalLine(gfx, yCur);

            // 11. Valor Total da NFS-e
            DrawSectionHeader(gfx, "VALOR TOTAL DA NFS-E", null, yCur + 1f);
            float yTot1 = yCur + 13f;
            DrawField(gfx, "Valor do Serviço", ValueFormatter.FormatCurrency(dps?.Valores?.VServ), DanfseLayout.Col1X, yTot1);
            DrawField(gfx, "Desconto Condicionado", ValueFormatter.FormatCurrency(dps?.Valores?.VDescCond), DanfseLayout.Col2X, yTot1);
            DrawField(gfx, "Desconto Incondicionado", ValueFormatter.FormatCurrency(dps?.Valores?.VDescIncond), DanfseLayout.Col3X, yTot1);
            DrawField(gfx, "ISSQN Retido", ValueFormatter.FormatCurrency(inf.Valores?.VISSRet), DanfseLayout.Col4X, yTot1);

            float yTot2 = yTot1 + 22f;

            // TODO: Verificar se "Total das Retenções Federais" corresponde exatamente à soma
            // de vRetIRRF + vRetCP + vRetCSLL (tribFed) ou se deve usar outra composição.
            // A tag vTotalRet (infNFSe/valores/vTotalRet) inclui ISSQN retido na soma e
            // portanto NÃO é exclusivamente federal.
            // Fórmula da planilha Sefin: vTotalRet = Σ(vRetCP + vRetIRRF + vRetCSLL + ISSQN*)
            string totalRetFed = CalcularTotalRetencoesFederais(dps);
            DrawField(gfx, "Total das Retenções Federais", totalRetFed, DanfseLayout.Col1X, yTot2);

            // TODO: Verificar se "PIS/COFINS - Débito Apur. Própria" é a soma de vPis + vCofins
            // ou se deve exibir de outra forma. Mapeando conforme campos da tribFed/piscofins.
            string pisCofinsDebito = CalcularPisCofinsTotalDebito(dps);
            DrawField(gfx, "PIS/COFINS - Débito Apur. Própria", pisCofinsDebito, DanfseLayout.Col2X, yTot2);
            DrawFieldBoldValue(gfx, "Valor Líquido da NFS-e", ValueFormatter.FormatCurrency(inf.Valores?.VLiq), DanfseLayout.Col4X, yTot2);

            yCur = yTot2 + 23f;
            DrawHorizontalLine(gfx, yCur);

            // 12. Totais Aproximados
            DrawSectionHeader(gfx, "TOTAIS APROXIMADOS DOS TRIBUTOS", null, yCur + 1f);
            float yAprox = yCur + 13f;
            DrawField(gfx, "Federais", ValueFormatter.FormatPercentage(dps?.Valores?.Trib?.TotTrib?.PTotTribFed ?? "0.00"), 92f, yAprox);
            DrawField(gfx, "Estaduais", ValueFormatter.FormatPercentage(dps?.Valores?.Trib?.TotTrib?.PTotTribEst ?? "0.00"), 279f, yAprox);
            DrawField(gfx, "Municipais", ValueFormatter.FormatPercentage(dps?.Valores?.Trib?.TotTrib?.PTotTribMun ?? "0.00"), 465f, yAprox);

            yCur = yAprox + 23f;
            DrawHorizontalLine(gfx, yCur);

            // 13. Informações Complementares
            DrawSectionHeader(gfx, "INFORMAÇÕES COMPLEMENTARES", null, yCur + 1f);
            string nbs = dps?.Serv?.CNBS ?? inf.XNBS;
            gfx.DrawString("NBS:", _fontLabelBold, XBrushes.Black, DanfseLayout.Col1X, yCur + 13f + 6);
            gfx.DrawString(ValueFormatter.FormatOrDash(nbs), _fontValue, XBrushes.Black, DanfseLayout.Col1X + 22f, yCur + 13f + 6);
        }

        private void DrawSectionHeader(XGraphics gfx, string title, string subtitle, float y)
        {
            gfx.DrawString(title, _fontSectionBold, XBrushes.Black, DanfseLayout.Col1X, y + 8);
            if (!string.IsNullOrEmpty(subtitle))
            {
                gfx.DrawString(subtitle, _fontValue, XBrushes.Black, DanfseLayout.Col1X, y + 19);
            }
        }

        private void DrawCenteredSectionHeader(XGraphics gfx, string title, float y)
        {
            var size = gfx.MeasureString(title, _fontSectionBold);
            float x = DanfseLayout.ContentLeft + (DanfseLayout.ContentWidth - (float)size.Width) / 2f;
            gfx.DrawString(title, _fontSectionBold, XBrushes.Black, x, y + 10f);
        }

        private void DrawField(XGraphics gfx, string label, string value, float x, float y)
        {
            gfx.DrawString(label, _fontLabelBold, XBrushes.Black, x, y + 6);
            gfx.DrawString(ValueFormatter.FormatOrDash(value), _fontValue, XBrushes.Black, x, y + 15.5f);
        }

        private void DrawFieldBoldValue(XGraphics gfx, string label, string value, float x, float y)
        {
            gfx.DrawString(label, _fontLabelBold, XBrushes.Black, x, y + 6);
            var fontValBold = new XFont(_fontValue.FontFamily.Name, _fontValue.Size, XFontStyleEx.Bold);
            gfx.DrawString(ValueFormatter.FormatOrDash(value), fontValBold, XBrushes.Black, x, y + 15.5f);
        }

        private float DrawFieldMultiline(XGraphics gfx, string label, string value, float x, float y, float maxWidth = 135f)
        {
            gfx.DrawString(label, _fontLabelBold, XBrushes.Black, x, y + 6);
            string val = ValueFormatter.FormatOrDash(value);
            
            string[] words = val.Replace("\r", "").Replace("\n", " ").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string currentLine = "";
            float currentY = y + 15.5f;

            foreach (var word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                if (gfx.MeasureString(testLine, _fontValue).Width <= maxWidth)
                {
                    currentLine = testLine;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        gfx.DrawString(currentLine, _fontValue, XBrushes.Black, x, currentY);
                        currentY += 9.5f;
                    }
                    currentLine = word;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                gfx.DrawString(currentLine, _fontValue, XBrushes.Black, x, currentY);
            }

            return currentY;
        }

        private System.Collections.Generic.List<string> WrapTextToLines(XGraphics gfx, string text, XFont font, float maxWidth)
        {
            var lines = new System.Collections.Generic.List<string>();
            if (string.IsNullOrWhiteSpace(text)) return lines;

            var paragraphs = text.Replace("\r", "").Split('\n');
            foreach (var p in paragraphs)
            {
                string[] words = p.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length == 0) continue;

                string currentLine = "";
                foreach (var word in words)
                {
                    string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                    if (gfx.MeasureString(testLine, font).Width <= maxWidth)
                    {
                        currentLine = testLine;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(currentLine))
                        {
                            lines.Add(currentLine);
                        }
                        currentLine = word;
                    }
                }
                if (!string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                }
            }
            return lines;
        }

        private void DrawHorizontalLine(XGraphics gfx, float y)
        {
            gfx.DrawLine(_penBorder, DanfseLayout.ContentLeft, y, DanfseLayout.ContentRight, y);
        }

        /// <summary>
        /// Calcula o Total das Retenções Federais somando vRetIRRF + vRetCP + vRetCSLL.
        /// Retorna "-" se todos os valores estiverem vazios ou zerados.
        /// </summary>
        private string CalcularTotalRetencoesFederais(InfDps dps)
        {
            var tribFed = dps?.Valores?.Trib?.TribFed;
            if (tribFed == null) return "-";

            decimal total = 0m;
            bool hasValue = false;

            hasValue |= TryAddDecimal(tribFed.VRetIRRF, ref total);
            hasValue |= TryAddDecimal(tribFed.VRetCP, ref total);
            hasValue |= TryAddDecimal(tribFed.VRetCSLL, ref total);

            if (!hasValue) return "-";
            return ValueFormatter.FormatCurrency(total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Calcula o PIS/COFINS - Débito Apuração Própria somando vPis + vCofins.
        /// Retorna "-" se todos os valores estiverem vazios ou zerados.
        /// </summary>
        private string CalcularPisCofinsTotalDebito(InfDps dps)
        {
            var pisCofins = dps?.Valores?.Trib?.TribFed?.PisCofins;
            if (pisCofins == null) return "-";

            decimal total = 0m;
            bool hasValue = false;

            hasValue |= TryAddDecimal(pisCofins.VPis, ref total);
            hasValue |= TryAddDecimal(pisCofins.VCofins, ref total);

            if (!hasValue) return "-";
            return ValueFormatter.FormatCurrency(total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Tenta converter o valor string em decimal e somar ao acumulador.
        /// Retorna true se o valor foi parseado com sucesso.
        /// </summary>
        private bool TryAddDecimal(string value, ref decimal accumulator)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (decimal.TryParse(value, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal parsed))
            {
                accumulator += parsed;
                return true;
            }
            return false;
        }
    }
}
