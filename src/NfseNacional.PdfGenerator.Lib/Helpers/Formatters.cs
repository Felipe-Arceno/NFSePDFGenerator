using System;
using System.Globalization;
using System.Linq;

namespace NfseNacional.PdfGenerator.Lib.Helpers
{
    public static class CnpjCpfFormatter
    {
        public static string FormatCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj)) return "-";
            var clean = new string(cnpj.ToCharArray().Where(char.IsDigit).ToArray());
            if (clean.Length == 14)
                return $"{clean.Substring(0, 2)}.{clean.Substring(2, 3)}.{clean.Substring(5, 3)}/{clean.Substring(8, 4)}-{clean.Substring(12, 2)}";
            return cnpj;
        }

        public static string FormatCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return "-";
            var clean = new string(cpf.ToCharArray().Where(char.IsDigit).ToArray());
            if (clean.Length == 11)
                return $"{clean.Substring(0, 3)}.{clean.Substring(3, 3)}.{clean.Substring(6, 3)}-{clean.Substring(9, 2)}";
            return cpf;
        }

        public static string FormatCnpjCpf(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "-";
            var clean = new string(value.ToCharArray().Where(char.IsDigit).ToArray());
            if (clean.Length == 14) return FormatCnpj(clean);
            if (clean.Length == 11) return FormatCpf(clean);
            return value;
        }
    }

    public static class CepFormatter
    {
        public static string FormatCep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep)) return "-";
            var clean = new string(cep.ToCharArray().Where(char.IsDigit).ToArray());
            if (clean.Length == 8)
                return $"{clean.Substring(0, 5)}-{clean.Substring(5, 3)}";
            return cep;
        }
    }

    public static class EnderecoFormatter
    {
        public static string FormatEndereco(string xlgr, string nro, string xcpl, string xbairro)
        {
            var parts = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrWhiteSpace(xlgr)) parts.Add(xlgr.Trim());
            if (!string.IsNullOrWhiteSpace(nro)) parts.Add(nro.Trim());
            if (!string.IsNullOrWhiteSpace(xcpl)) parts.Add(xcpl.Trim());
            if (!string.IsNullOrWhiteSpace(xbairro)) parts.Add(xbairro.Trim());
            return parts.Count > 0 ? string.Join(", ", parts) : "-";
        }
    }

    public static class ValueFormatter
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");

        public static string FormatCurrency(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "-";
            if (decimal.TryParse(value.Replace(".", ","), NumberStyles.Any, PtBr, out decimal val) ||
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
            {
                return $"R$ {val.ToString("N2", PtBr)}";
            }
            return value;
        }

        public static string FormatPercentage(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "-";
            if (decimal.TryParse(value.Replace(".", ","), NumberStyles.Any, PtBr, out decimal val) ||
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
            {
                return $"{val.ToString("N2", PtBr)} %";
            }
            return value;
        }

        public static string FormatOrDash(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        public static string FormatDate(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr)) return "-";
            if (DateTime.TryParse(dateStr, out DateTime dt))
            {
                return dt.ToString("dd/MM/yyyy");
            }
            return dateStr;
        }

        public static string FormatDateTime(string dateTimeStr)
        {
            if (string.IsNullOrWhiteSpace(dateTimeStr)) return "-";
            if (DateTime.TryParse(dateTimeStr, out DateTime dt))
            {
                return dt.ToString("dd/MM/yyyy HH:mm:ss");
            }
            return dateTimeStr;
        }
    }

    public static class NfseDescriptionHelper
    {
        public static string GetSimplesNacionalDescricao(string opSimpNac)
        {
            switch (opSimpNac)
            {
                case "1": return "Não optante";
                case "2": return "Optante - Microempreendedor Individual (MEI)";
                case "3": return "Optante - Microempresa ou Empresa de Pequeno Porte (ME/EPP)";
                default: return "-";
            }
        }

        public static string GetRegApTribSNDescricao(string regApTribSN)
        {
            switch (regApTribSN)
            {
                case "1": return "Operação normal pelo Simples Nacional";
                case "2": return "Tributação em separado por valor fixo";
                case "3": return "Regime de apuração dos tributos federais e municipal pela NFS-e conforme\nrespectivas legislações federal e municipal de cada tributo";
                case "4": return "Tributação em separado com recolhimento pelo tomador";
                default: return "-";
            }
        }

        public static string GetRegEspTribDescricao(string regEspTrib)
        {
            switch (regEspTrib)
            {
                case "0": return "Nenhum";
                case "1": return "Ato Cooperado";
                case "2": return "Estimativa";
                case "3": return "Microempresa Municipal";
                case "4": return "Notário ou Registrador";
                case "5": return "Profissional Autônomo";
                case "6": return "Sociedade de Profissionais";
                default: return "Nenhum";
            }
        }

        public static string GetTribISSQNDescricao(string tribISSQN)
        {
            switch (tribISSQN)
            {
                case "1": return "Operação Tributável";
                case "2": return "Imunidade";
                case "3": return "Exportação";
                case "4": return "Não Incidência";
                default: return "-";
            }
        }

        public static string GetTpRetISSQNDescricao(string tpRetISSQN)
        {
            switch (tpRetISSQN)
            {
                case "1": return "Não Retido";
                case "2": return "Retido pelo Tomador";
                case "3": return "Retido pelo Intermediário";
                default: return "-";
            }
        }

        public static string GetTpSuspDescricao(string tpSusp)
        {
            switch (tpSusp)
            {
                case "0": return "Não";
                case "1": return "Decisão Judicial";
                case "2": return "Processo Administrativo";
                default: return "Não";
            }
        }

        public static string TruncateWithEllipsis(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (text.Length <= maxLength) return text;
            int cutPos = Math.Max(0, maxLength - 3);
            return text.Substring(0, cutPos).TrimEnd() + "...";
        }

        public static string FormatCodTribNacional(string cTribNac, string xTribNac)
        {
            string result;
            if (string.IsNullOrWhiteSpace(cTribNac))
            {
                result = ValueFormatter.FormatOrDash(xTribNac);
            }
            else
            {
                string formatted = cTribNac;
                if (cTribNac.Length == 6)
                {
                    formatted = $"{cTribNac.Substring(0, 2)}.{cTribNac.Substring(2, 2)}.{cTribNac.Substring(4, 2)}";
                }
                if (!string.IsNullOrWhiteSpace(xTribNac))
                {
                    result = $"{formatted} - {xTribNac}";
                }
                else
                {
                    result = formatted;
                }
            }
            return TruncateWithEllipsis(result, 75);
        }

        public static string FormatCodTribMunicipal(string cTribMun, string xTribMun, string xTribNac)
        {
            if (string.IsNullOrWhiteSpace(cTribMun)) return "-";
            string desc = !string.IsNullOrWhiteSpace(xTribMun) ? xTribMun.Trim() : (!string.IsNullOrWhiteSpace(xTribNac) ? xTribNac.Trim() : string.Empty);
            string formatted = cTribMun.Trim();
            string result = !string.IsNullOrWhiteSpace(desc) ? $"{formatted} - {desc}" : formatted;
            return TruncateWithEllipsis(result, 75);
        }
    }
}
