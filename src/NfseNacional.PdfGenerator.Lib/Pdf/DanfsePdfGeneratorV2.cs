using System;
using PdfSharp.Drawing;
using NfseNacional.PdfGenerator.Lib.Models;
using NfseNacional.PdfGenerator.Lib.Helpers;

namespace NfseNacional.PdfGenerator.Lib.Pdf
{
    /// <summary>
    /// Renderizador do layout DANFSe v2.0 (padrão nacional atual).
    /// Implementado como partial da <see cref="DanfsePdfGenerator"/> para reutilizar
    /// a inicialização de fontes/pen e conviver com o layout v1.0 (RenderPageV1).
    /// </summary>
    public partial class DanfsePdfGenerator
    {
        // Geometria (A4 595 x 842 pt)
        // Margem lateral de ~18pt (≈6,3mm) para não ser cortado na impressão física.
        private const float V2Left = 18f;
        private const float V2Right = 577f;
        // Boundaries das 4 colunas (simétricas em torno do centro da página, 297.5)
        private static readonly float[] V2Cols = { 18f, 157.75f, 297.5f, 437.25f, 577f };

        private void RenderPageV2(XGraphics gfx, InfNFSe inf, DadosMunicipio mun, bool isHomologacao)
        {
            // ---- Atalhos de dados (todos null-safe) ----
            var dps = inf.Dps?.InfDps;
            var emit = inf.Emit;
            var emitEnd = emit?.EnderNac;
            var toma = dps?.Toma;
            var tomaEnd = toma?.End;
            var ibsNfse = inf.IbsCbs;
            var ibsVal = ibsNfse?.Valores;
            var ibsDps = dps?.IbsCbs;
            var valNfse = inf.Valores;
            var valDps = dps?.Valores;
            var tribMun = valDps?.Trib?.TribMun;
            var tribFed = valDps?.Trib?.TribFed;
            var totTrib = valDps?.Trib?.TotTrib;

            // ---- Fontes ----
            string fam = _fontValue.FontFamily.Name;
            var fLabel = new XFont(fam, 5.4f, XFontStyleEx.Bold);
            var fValue = new XFont(fam, 7f, XFontStyleEx.Regular);
            var fValueBold = new XFont(fam, 7f, XFontStyleEx.Bold);
            var fSection = new XFont(fam, 7f, XFontStyleEx.Bold);
            var fTitle = new XFont(fam, 11f, XFontStyleEx.Bold);
            var fSub = new XFont(fam, 7.5f, XFontStyleEx.Bold);
            var fSmall = new XFont(fam, 5.7f, XFontStyleEx.Regular);
            var fKey = new XFont(fam, 8f, XFontStyleEx.Regular);
            var fLogo = new XFont(fam, 20f, XFontStyleEx.Bold);
            var fLogoSub = new XFont(fam, 6f, XFontStyleEx.Regular);

            // ---- Cores / Pens ----
            var black = XBrushes.Black;
            var greyFill = new XSolidBrush(XColor.FromArgb(226, 226, 226));
            var brushGreen = new XSolidBrush(XColor.FromArgb(0, 140, 60));
            var brushBlue = new XSolidBrush(XColor.FromArgb(0, 80, 160));
            var brushGray = new XSolidBrush(XColor.FromArgb(110, 110, 110));
            var brushWarning = new XSolidBrush(XColor.FromArgb(200, 0, 0));
            var penThin = new XPen(XColor.FromArgb(120, 120, 120), 0.4f);
            var penBorder = new XPen(XColors.Black, 0.7f);

            const float pageCx = (V2Left + V2Right) / 2f;

            // ============ Funções locais de desenho ============
            string Fit(string s, XFont f, float maxW)
            {
                if (string.IsNullOrEmpty(s)) return s ?? string.Empty;
                s = s.Replace("\r", " ").Replace("\n", " ");
                if (gfx.MeasureString(s, f).Width <= maxW) return s;
                for (int len = s.Length - 1; len > 0; len--)
                {
                    string cand = s.Substring(0, len).TrimEnd() + "…";
                    if (gfx.MeasureString(cand, f).Width <= maxW) return cand;
                }
                return "…";
            }

            void HLine(float yy) => gfx.DrawLine(penThin, V2Left, yy, V2Right, yy);
            void HLineLeft(float yy) => gfx.DrawLine(penThin, V2Left, yy, V2Cols[3], yy);
            void VLine(float xx, float y1, float y2) => gfx.DrawLine(penThin, xx, y1, xx, y2);
            void Band(float yy, float h) => gfx.DrawRectangle(greyFill, V2Left, yy, V2Right - V2Left, h);

            void Field(float x, float w, float y, string label, string value, bool boldValue = false)
            {
                if (!string.IsNullOrEmpty(label))
                    gfx.DrawString(Fit(label, fLabel, w - 4), fLabel, black, x + 2, y + 5.6f);
                var vf = boldValue ? fValueBold : fValue;
                string v = ValueFormatter.FormatOrDash(value);
                gfx.DrawString(Fit(v, vf, w - 4), vf, black, x + 2, y + 12.8f);
            }

            void FieldCol(int i, float y, string label, string value, bool bold = false)
                => Field(V2Cols[i], V2Cols[i + 1] - V2Cols[i], y, label, value, bold);

            void FieldSpan(int i0, int i1, float y, string label, string value, bool bold = false)
                => Field(V2Cols[i0], V2Cols[i1 + 1] - V2Cols[i0], y, label, value, bold);

            // Campo com valor em múltiplas linhas (quebra por largura). Retorna a altura usada.
            float FieldMulti(float x, float w, float y, string label, string value, int maxLines)
            {
                const float step = 8.3f;
                if (!string.IsNullOrEmpty(label))
                    gfx.DrawString(Fit(label, fLabel, w - 4), fLabel, black, x + 2, y + 5.6f);

                string v = ValueFormatter.FormatOrDash(value).Replace("\r", " ").Replace("\n", " ");
                var words = v.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                float lineY = y + 12.8f;
                int lines = 0;
                string cur = string.Empty;

                for (int i = 0; i < words.Length; i++)
                {
                    string test = cur.Length == 0 ? words[i] : cur + " " + words[i];
                    if (gfx.MeasureString(test, fValue).Width <= w - 4)
                    {
                        cur = test;
                    }
                    else
                    {
                        if (lines == maxLines - 1)
                        {
                            // Última linha permitida: junta o restante e trunca com "…".
                            string rest = cur;
                            for (int j = i; j < words.Length; j++) rest += " " + words[j];
                            gfx.DrawString(Fit(rest, fValue, w - 4), fValue, black, x + 2, lineY);
                            lines++;
                            cur = string.Empty;
                            break;
                        }
                        gfx.DrawString(cur, fValue, black, x + 2, lineY);
                        lineY += step;
                        lines++;
                        cur = words[i];
                    }
                }
                if (cur.Length > 0 && lines < maxLines)
                {
                    gfx.DrawString(Fit(cur, fValue, w - 4), fValue, black, x + 2, lineY);
                    lines++;
                }
                if (lines == 0) lines = 1;
                return 12.8f + (lines - 1) * step + 4.5f;
            }

            void SectionTitle(float y, float h, string title)
                => gfx.DrawString(Fit(title, fSection, (V2Cols[1] - V2Left) - 4), fSection, black, V2Left + 2, y + h / 2f + 2.4f);

            void CenterString(string s, XFont f, XBrush b, float cx, float y)
            {
                double w = gfx.MeasureString(s, f).Width;
                gfx.DrawString(s, f, b, (float)(cx - w / 2f), y);
            }

            // ============ Borda externa (recuada para dar margem à impressão) ============
            gfx.DrawRectangle(penBorder, 13f, 6f, 569f, 830f);

            // ============ 1. Cabeçalho ============
            // Logo textual "NFSe"
            float logoX = V2Left - 2f;
            gfx.DrawString("NFS", fLogo, brushGreen, logoX, 28f);
            double nfsW = gfx.MeasureString("NFS", fLogo).Width;
            gfx.DrawString("e", fLogo, brushBlue, logoX + (float)nfsW + 1f, 28f);
            gfx.DrawString("Nota Fiscal de", fLogoSub, brushGray, logoX + (float)nfsW + 16f, 19f);
            gfx.DrawString("Serviço eletrônica", fLogoSub, brushGray, logoX + (float)nfsW + 16f, 27f);

            // Título central
            CenterString("DANFSe v2.0", fTitle, black, pageCx, 18f);
            CenterString("Documento Auxiliar da NFS-e", fSub, black, pageCx, 30f);
            if (isHomologacao)
                CenterString("NFS-e SEM VALIDADE JURÍDICA", fSub, brushWarning, pageCx, 41f);

            // Bloco direito
            string ufEmit = !string.IsNullOrWhiteSpace(emitEnd?.Uf) ? emitEnd.Uf : DanfseV2Helper.ResolveUf(emitEnd?.CMun ?? inf.CLocIncid);
            string municipioHdr = $"Município: {ValueFormatter.FormatOrDash(inf.XLocEmi)} - {ufEmit}";
            float hdrRx = V2Cols[3] + 3f;
            float hdrRw = V2Right - hdrRx - 1f;
            gfx.DrawString(Fit(municipioHdr, fSmall, hdrRw), fSmall, black, hdrRx, 16f);
            gfx.DrawString(Fit($"Ambiente Gerador: {ValueFormatter.FormatOrDash(inf.AmbGer)}", fSmall, hdrRw), fSmall, black, hdrRx, 25f);
            gfx.DrawString(Fit($"Tipo de Ambiente: {ValueFormatter.FormatOrDash(dps?.TpAmb ?? inf.TpEmis)}", fSmall, hdrRw), fSmall, black, hdrRx, 34f);

            HLine(46f);

            // ============ 2. Chave de Acesso + QR ============
            float yBlockTop = 46f;
            // Chave (região esquerda)
            gfx.DrawString("CHAVE DE ACESSO DA NFS-e", fLabel, black, V2Left + 2, yBlockTop + 6.5f);
            gfx.DrawString(Fit(ValueFormatter.FormatOrDash(inf.ChaveAcesso), fKey, (V2Cols[3] - V2Left) - 6), fKey, black, V2Left + 2, yBlockTop + 15f);
            HLineLeft(yBlockTop + 16f);

            // QR (região direita) + texto de verificação
            float qrSize = 44f;
            float qrX = V2Cols[3] + ((V2Cols[4] - V2Cols[3]) - qrSize) / 2f;
            QrCodeGenerator.RenderQrCodeVector(gfx, inf.ChaveAcesso, qrX, yBlockTop + 3f, qrSize);
            float verifW = V2Right - (V2Cols[3] + 3f) - 1f;
            gfx.DrawString(Fit("A autenticidade desta NFS-e pode ser verificada", fSmall, verifW), fSmall, black, V2Cols[3] + 3f, yBlockTop + 51f);
            gfx.DrawString(Fit("pela leitura deste código QR ou pela consulta da", fSmall, verifW), fSmall, black, V2Cols[3] + 3f, yBlockTop + 58f);
            gfx.DrawString(Fit("chave de acesso no portal nacional da NFS-e", fSmall, verifW), fSmall, black, V2Cols[3] + 3f, yBlockTop + 65f);

            // Linhas 1-3 (região esquerda, 3 colunas)
            float r1 = yBlockTop + 16f;   // 62
            float r2 = r1 + 17f;          // 79
            float r3 = r2 + 17f;          // 96
            float blockBottom = r3 + 17f; // 113

            FieldCol(0, r1, "NÚMERO DA NFS-e", inf.NNFSe);
            FieldCol(1, r1, "COMPETÊNCIA DA NFS-e", ValueFormatter.FormatDate(dps?.DCompet));
            FieldCol(2, r1, "DATA E HORA DA EMISSÃO DA NFS-e", ValueFormatter.FormatDateTime(inf.DhProc));
            HLineLeft(r2);

            FieldCol(0, r2, "NÚMERO DA DPS", dps?.NDPS);
            FieldCol(1, r2, "SÉRIE DA DPS", DanfseV2Helper.TrimLeadingZeros(dps?.Serie));
            FieldCol(2, r2, "DATA E HORA DA EMISSÃO DA DPS", ValueFormatter.FormatDateTime(dps?.DhEmi));
            HLineLeft(r3);

            FieldCol(0, r3, "EMITENTE DA NFS-e", DanfseV2Helper.GetEmitenteDescricao(dps?.TpEmit));
            FieldCol(1, r3, "SITUAÇÃO DA NFS-e", DanfseV2Helper.GetSituacaoDescricao(inf.CStat));
            FieldCol(2, r3, "FINALIDADE", DanfseV2Helper.GetFinalidadeDescricao(ibsDps?.FinNFSe));

            // Separadores verticais do bloco superior
            VLine(V2Cols[1], r1, blockBottom);
            VLine(V2Cols[2], r1, blockBottom);
            VLine(V2Cols[3], yBlockTop, blockBottom);
            HLine(blockBottom);

            float y = blockBottom; // cursor

            // ============ 3. PRESTADOR / FORNECEDOR ============
            const float bandH = 15f;
            const float rowH = 17f;

            Band(y, bandH);
            SectionTitle(y, bandH, "PRESTADOR / FORNECEDOR");
            FieldCol(1, y, "CNPJ / CPF / NIF", CnpjCpfFormatter.FormatCnpjCpf(emit?.Cnpj ?? emit?.Cpf));
            FieldCol(2, y, "Indicador Municipal (Inscrição)", ValueFormatter.FormatOrDash(emit?.Im));
            FieldCol(3, y, "Telefone", "-"); // espelha o portal nacional (não mapeia emit/fone)
            VLine(V2Cols[1], y, y + bandH); VLine(V2Cols[2], y, y + bandH); VLine(V2Cols[3], y, y + bandH);
            y += bandH; HLine(y);

            FieldSpan(0, 1, y, "Nome / Nome Empresarial", emit?.XNome);
            FieldCol(2, y, "Município / Sigla UF", DanfseV2Helper.MunicipioUf(emitEnd?.CMun ?? inf.CLocIncid));
            FieldCol(3, y, "Código IBGE / CEP", DanfseV2Helper.CodigoIbgeCep(emitEnd?.CMun ?? inf.CLocIncid, emitEnd?.Cep));
            VLine(V2Cols[2], y, y + rowH); VLine(V2Cols[3], y, y + rowH);
            y += rowH; HLine(y);

            FieldSpan(0, 1, y, "Endereço", EnderecoFormatter.FormatEndereco(emitEnd?.XLgr, emitEnd?.Nro, emitEnd?.XCpl, emitEnd?.XBairro));
            FieldSpan(2, 3, y, "E-mail", ValueFormatter.FormatOrDash(emit?.Email));
            VLine(V2Cols[2], y, y + rowH);
            y += rowH; HLine(y);

            FieldSpan(0, 1, y, "Simples Nacional na Data de Competência", NfseDescriptionHelper.GetSimplesNacionalDescricao(dps?.Prest?.RegTrib?.OpSimpNac));
            FieldSpan(2, 3, y, "Regime de Apuração Tributária pelo SN", NfseDescriptionHelper.GetRegApTribSNDescricao(dps?.Prest?.RegTrib?.RegApTribSN));
            VLine(V2Cols[2], y, y + rowH);
            y += rowH; HLine(y);

            // ============ 4. TOMADOR / ADQUIRENTE ============
            string tomaMunFallback = (!string.IsNullOrWhiteSpace(tomaEnd?.CMun) && tomaEnd.CMun == ibsNfse?.CLocalidadeIncid)
                ? ibsNfse?.XLocalidadeIncid : null;

            Band(y, bandH);
            SectionTitle(y, bandH, "TOMADOR / ADQUIRENTE");
            FieldCol(1, y, "CNPJ / CPF / NIF", CnpjCpfFormatter.FormatCnpjCpf(toma?.Cnpj ?? toma?.Cpf));
            FieldCol(2, y, "Indicador Municipal (Inscrição)", ValueFormatter.FormatOrDash(toma?.Im));
            FieldCol(3, y, "Telefone", ValueFormatter.FormatOrDash(toma?.Fone));
            VLine(V2Cols[1], y, y + bandH); VLine(V2Cols[2], y, y + bandH); VLine(V2Cols[3], y, y + bandH);
            y += bandH; HLine(y);

            FieldSpan(0, 1, y, "Nome / Nome Empresarial", toma?.XNome);
            FieldCol(2, y, "Município / Sigla UF", DanfseV2Helper.MunicipioUf(tomaEnd?.CMun, tomaMunFallback));
            FieldCol(3, y, "Código IBGE / CEP", DanfseV2Helper.CodigoIbgeCep(tomaEnd?.CMun, tomaEnd?.Cep));
            VLine(V2Cols[2], y, y + rowH); VLine(V2Cols[3], y, y + rowH);
            y += rowH; HLine(y);

            FieldSpan(0, 1, y, "Endereço", EnderecoFormatter.FormatEndereco(tomaEnd?.XLgr, tomaEnd?.Nro, tomaEnd?.XCpl, tomaEnd?.XBairro));
            FieldSpan(2, 3, y, "E-mail", ValueFormatter.FormatOrDash(toma?.Email));
            VLine(V2Cols[2], y, y + rowH);
            y += rowH; HLine(y);

            // ============ 5. Destinatário / Intermediário (não identificados) ============
            const float centerH = 13f;
            CenterString("DESTINATÁRIO DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e", fValueBold, black, pageCx, y + 9f);
            y += centerH; HLine(y);
            CenterString("INTERMEDIÁRIO DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e", fValueBold, black, pageCx, y + 9f);
            y += centerH; HLine(y);

            // ============ 6. SERVIÇO PRESTADO ============
            Band(y, bandH);
            SectionTitle(y, bandH, "SERVIÇO PRESTADO");
            FieldCol(1, y, "Código de Tributação Nacional/Municipal", DanfseV2Helper.CodTribNacionalMunicipal(dps?.Serv?.CTribNac, dps?.Serv?.CTribMun));
            FieldCol(2, y, "Código da NBS", DanfseV2Helper.FormatNbs(dps?.Serv?.CNBS ?? inf.XNBS));
            FieldCol(3, y, "Local da Prestação / Sigla UF / País", DanfseV2Helper.MunicipioUfPais(dps?.Serv?.CLocPrestacao ?? inf.CLocIncid));
            VLine(V2Cols[1], y, y + bandH); VLine(V2Cols[2], y, y + bandH); VLine(V2Cols[3], y, y + bandH);
            y += bandH; HLine(y);

            FieldSpan(0, 3, y, null, inf.XTribNac);
            y += rowH; HLine(y);

            // Descrição do Serviço em múltiplas linhas (espelha o portal nacional, que quebra a linha).
            float descH = FieldMulti(V2Left, V2Right - V2Left, y, "Descrição do Serviço", dps?.Serv?.XDescServ, 3);
            y += Math.Max(rowH, descH); HLine(y);

            // ============ 7. TRIBUTAÇÃO MUNICIPAL (ISSQN) ============
            Band(y, bandH);
            SectionTitle(y, bandH, "TRIBUTAÇÃO MUNICIPAL (ISSQN)");
            FieldCol(1, y, "Tipo de Tributação do ISSQN", NfseDescriptionHelper.GetTribISSQNDescricao(tribMun?.TribISSQN));
            FieldSpan(2, 3, y, "Município / Sigla UF / País de Incidência do ISSQN", DanfseV2Helper.MunicipioUfPais(inf.CLocIncid));
            VLine(V2Cols[1], y, y + bandH); VLine(V2Cols[2], y, y + bandH);
            y += bandH; HLine(y);

            FieldCol(0, y, "BC ISSQN", ValueFormatter.FormatCurrency(valNfse?.VBC));
            FieldCol(1, y, "Alíquota Aplicada", ValueFormatter.FormatPercentage(valNfse?.PAliqAplic));
            FieldCol(2, y, "Retenção do ISSQN", NfseDescriptionHelper.GetTpRetISSQNDescricao(tribMun?.TpRetISSQN));
            FieldCol(3, y, "ISSQN Apurado", ValueFormatter.FormatCurrency(valNfse?.VISSQN));
            VLine(V2Cols[1], y, y + rowH); VLine(V2Cols[2], y, y + rowH); VLine(V2Cols[3], y, y + rowH);
            y += rowH; HLine(y);

            // ============ 8. TRIBUTAÇÃO FEDERAL (EXCETO CBS) ============
            Band(y, bandH);
            SectionTitle(y, bandH, "TRIBUTAÇÃO FEDERAL (EXCETO CBS)");
            FieldCol(1, y, "IRRF", ValueFormatter.FormatCurrency(tribFed?.VRetIRRF));
            FieldCol(2, y, "Contribuição Previdenciária - Retida", ValueFormatter.FormatCurrency(tribFed?.VRetCP));
            FieldCol(3, y, "Contribuições Sociais - Retidas", ValueFormatter.FormatCurrency(tribFed?.VRetCSLL));
            VLine(V2Cols[1], y, y + bandH); VLine(V2Cols[2], y, y + bandH); VLine(V2Cols[3], y, y + bandH);
            y += bandH; HLine(y);

            FieldCol(0, y, "PIS - Débito Apuração Própria", ValueFormatter.FormatCurrency(tribFed?.PisCofins?.VPis));
            FieldCol(1, y, "COFINS - Débito Apuração Própria", ValueFormatter.FormatCurrency(tribFed?.PisCofins?.VCofins));
            FieldSpan(2, 3, y, "Descrição Contrib. Sociais - Retidas", ValueFormatter.FormatOrDash(tribFed?.XDescRetCSLL));
            VLine(V2Cols[1], y, y + rowH); VLine(V2Cols[2], y, y + rowH);
            y += rowH; HLine(y);

            // ============ 9. TRIBUTAÇÃO IBS/CBS ============
            string cstClass = $"{ValueFormatter.FormatOrDash(ibsDps?.Cst)} / {ValueFormatter.FormatOrDash(ibsDps?.CClassTrib)}";
            string incNome = DanfseV2Helper.ResolveNomeMunicipio(ibsNfse?.CLocalidadeIncid, ibsNfse?.XLocalidadeIncid);
            string incUf = DanfseV2Helper.ResolveUf(ibsNfse?.CLocalidadeIncid);
            string indicadorOp = $"{ValueFormatter.FormatOrDash(ibsDps?.CIndOp)} / {ValueFormatter.FormatOrDash(ibsNfse?.CLocalidadeIncid)} / {incNome} / {incUf}";

            Band(y, bandH);
            SectionTitle(y, bandH, "TRIBUTAÇÃO IBS/CBS");
            FieldCol(1, y, "CST / cClassTrib", cstClass);
            FieldSpan(2, 3, y, "Indicador de Operação / Código IBGE Incidência / Município Incidência / Sigla UF", indicadorOp);
            VLine(V2Cols[1], y, y + bandH); VLine(V2Cols[2], y, y + bandH);
            y += bandH; HLine(y);

            FieldCol(0, y, "Exclusões e Reduções da Base de Cálculo", DanfseV2Helper.DiferencaMoeda(valNfse?.VBC, ibsVal?.VBC));
            FieldCol(1, y, "Base de Cálculo Após Exclusões e Reduções", ValueFormatter.FormatCurrency(ibsVal?.VBC));
            FieldCol(2, y, "Red. Alíquota IBS / Red. Alíquota CBS", "- / - / -");
            FieldCol(3, y, "Alíquota - IBS UF / IBS Mun", $"{ValueFormatter.FormatPercentage(ibsVal?.Uf?.PIBSUF)} / {ValueFormatter.FormatPercentage(ibsVal?.Mun?.PIBSMun)}");
            VLine(V2Cols[1], y, y + rowH); VLine(V2Cols[2], y, y + rowH); VLine(V2Cols[3], y, y + rowH);
            y += rowH; HLine(y);

            FieldCol(0, y, "Alíq. Efetiva Municipal - IBS", ValueFormatter.FormatPercentage(ibsVal?.Mun?.PAliqEfetMun));
            FieldCol(1, y, "Valor Apurado Municipal - IBS", ValueFormatter.FormatCurrency(ibsNfse?.TotCIBS?.GIBS?.VIBSMun));
            FieldCol(2, y, "Alíq. Efetiva Estadual - IBS", ValueFormatter.FormatPercentage(ibsVal?.Uf?.PAliqEfetUF));
            FieldCol(3, y, "Valor Apurado Estadual - IBS", ValueFormatter.FormatCurrency(ibsNfse?.TotCIBS?.GIBS?.VIBSUF));
            VLine(V2Cols[1], y, y + rowH); VLine(V2Cols[2], y, y + rowH); VLine(V2Cols[3], y, y + rowH);
            y += rowH; HLine(y);

            FieldCol(0, y, "Valor Total Apurado - IBS", ValueFormatter.FormatCurrency(ibsNfse?.TotCIBS?.GIBS?.VIBSTot));
            FieldCol(1, y, "Alíquota - CBS", ValueFormatter.FormatPercentage(ibsVal?.Fed?.PCBS));
            FieldCol(2, y, "Alíquota Efetiva - CBS", ValueFormatter.FormatPercentage(ibsVal?.Fed?.PAliqEfetCBS));
            FieldCol(3, y, "Valor Total Apurado - CBS", ValueFormatter.FormatCurrency(ibsNfse?.TotCIBS?.GCBS?.VCBS));
            VLine(V2Cols[1], y, y + rowH); VLine(V2Cols[2], y, y + rowH); VLine(V2Cols[3], y, y + rowH);
            y += rowH; HLine(y);

            // ============ 10. VALOR TOTAL DA NFS-e ============
            string totRet = !string.IsNullOrWhiteSpace(valNfse?.VTotalRet)
                ? ValueFormatter.FormatCurrency(valNfse.VTotalRet)
                : DanfseV2Helper.SomaMoeda(valNfse?.VISSRet, tribFed?.VRetIRRF, tribFed?.VRetCP, tribFed?.VRetCSLL);
            string totIbsCbs = DanfseV2Helper.SomaMoeda(ibsNfse?.TotCIBS?.GIBS?.VIBSTot, ibsNfse?.TotCIBS?.GCBS?.VCBS);

            Band(y, bandH);
            SectionTitle(y, bandH, "VALOR TOTAL DA NFS-e");
            FieldCol(1, y, "VALOR DA OPERAÇÃO / SERVIÇO", ValueFormatter.FormatCurrency(valDps?.VServ));
            FieldCol(2, y, "Desconto Incondicionado", ValueFormatter.FormatCurrency(valDps?.VDescIncond));
            FieldCol(3, y, "Desconto Condicionado", ValueFormatter.FormatCurrency(valDps?.VDescCond));
            VLine(V2Cols[1], y, y + bandH); VLine(V2Cols[2], y, y + bandH); VLine(V2Cols[3], y, y + bandH);
            y += bandH; HLine(y);

            FieldCol(0, y, "Total das Retenções (ISSQN / Federais)", totRet);
            FieldCol(1, y, "VALOR LÍQUIDO DA NFS-e", ValueFormatter.FormatCurrency(valNfse?.VLiq), true);
            FieldCol(2, y, "Total do IBS/CBS", totIbsCbs);
            FieldCol(3, y, "VALOR LÍQUIDO DA NFS-e + IBS/CBS", ValueFormatter.FormatCurrency(ibsNfse?.TotCIBS?.VTotNF ?? valNfse?.VLiq), true);
            VLine(V2Cols[1], y, y + rowH); VLine(V2Cols[2], y, y + rowH); VLine(V2Cols[3], y, y + rowH);
            y += rowH; HLine(y);

            // ============ 11. INFORMAÇÕES COMPLEMENTARES ============
            Band(y, bandH);
            SectionTitle(y, bandH, "INFORMAÇÕES COMPLEMENTARES");
            y += bandH; HLine(y);

            string pf = ValueFormatter.FormatPercentage(totTrib?.PTotTribFed ?? "0.00");
            string pe = ValueFormatter.FormatPercentage(totTrib?.PTotTribEst ?? "0.00");
            string pm = ValueFormatter.FormatPercentage(totTrib?.PTotTribMun ?? "0.00");
            string infoCompl = $"Totais aproximados dos Tributos cfe. Lei n° 12.741/2012: Federais: {pf}; Estaduais: {pe}; Municipais: {pm};";
            gfx.DrawString(Fit(infoCompl, fValue, (V2Right - V2Left) - 8), fValue, black, V2Left + 3, y + 12f);
            y += 22f; HLine(y);

            // ============ 12. Rodapé (identificação / assinatura) ============
            float fy = 792f;
            float f1 = 205f, f2 = 398f;
            gfx.DrawRectangle(penThin, V2Left, fy, V2Right - V2Left, 26f);
            VLine(f1, fy, fy + 26f);
            VLine(f2, fy, fy + 26f);
            gfx.DrawString("DATA CIENTIFICAÇÃO:", fLabel, black, V2Left + 3, fy + 8f);
            gfx.DrawString("IDENTIFICAÇÃO E ASSINATURA", fLabel, black, f1 + 3, fy + 8f);
            gfx.DrawString("N° NFS-e / CHAVE NFS-e", fLabel, black, f2 + 3, fy + 8f);
            gfx.DrawString(Fit($"{ValueFormatter.FormatOrDash(inf.NNFSe)} / {ValueFormatter.FormatOrDash(inf.ChaveAcesso)}", fValue, (V2Right - f2) - 6), fValue, black, f2 + 3, fy + 17f);
        }
    }
}
