using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NfseNacional.PdfGenerator.Lib.Helpers
{
    /// <summary>
    /// Helpers específicos do layout DANFSe v2.0 (padrão nacional).
    /// Contém formatações e descrições que só aparecem no novo layout,
    /// deixando o helper original (<see cref="NfseDescriptionHelper"/>) intacto
    /// para manter compatibilidade com o layout v1.0.
    /// </summary>
    public static class DanfseV2Helper
    {
        /// <summary>
        /// Mapa das duas primeiras posições do código IBGE do município → sigla da UF.
        /// Permite obter a UF de qualquer município, mesmo que ele não esteja na
        /// tabela de municípios cadastrada.
        /// </summary>
        private static readonly Dictionary<string, string> UfPorPrefixoIbge = new Dictionary<string, string>
        {
            {"11","RO"},{"12","AC"},{"13","AM"},{"14","RR"},{"15","PA"},{"16","AP"},{"17","TO"},
            {"21","MA"},{"22","PI"},{"23","CE"},{"24","RN"},{"25","PB"},{"26","PE"},{"27","AL"},{"28","SE"},{"29","BA"},
            {"31","MG"},{"32","ES"},{"33","RJ"},{"35","SP"},
            {"41","PR"},{"42","SC"},{"43","RS"},
            {"50","MS"},{"51","MT"},{"52","GO"},{"53","DF"}
        };

        /// <summary>Obtém a sigla da UF a partir do código IBGE (2 primeiros dígitos).</summary>
        public static string UfFromCodigoIbge(string codigoIbge)
        {
            if (string.IsNullOrWhiteSpace(codigoIbge)) return "-";
            string digits = new string(codigoIbge.Where(char.IsDigit).ToArray());
            if (digits.Length < 2) return "-";
            return UfPorPrefixoIbge.TryGetValue(digits.Substring(0, 2), out var uf) ? uf : "-";
        }

        /// <summary>
        /// Resolve a sigla da UF: usa a tabela cadastral e, se não encontrar,
        /// deriva do prefixo do código IBGE.
        /// </summary>
        public static string ResolveUf(string codigoIbge)
        {
            var uf = MunicipioHelper.GetUF(codigoIbge);
            if (string.IsNullOrWhiteSpace(uf) || uf == "-")
                uf = UfFromCodigoIbge(codigoIbge);
            return string.IsNullOrWhiteSpace(uf) ? "-" : uf;
        }

        /// <summary>
        /// Resolve o nome do município. Usa a tabela cadastral; se não encontrar,
        /// tenta um nome de fallback (ex.: xLocalidadeIncid do próprio XML); por fim,
        /// devolve o próprio código.
        /// </summary>
        public static string ResolveNomeMunicipio(string codigoIbge, string fallbackNome = null)
        {
            if (string.IsNullOrWhiteSpace(codigoIbge)) return "-";
            string nome = MunicipioHelper.GetMunicipioNomeOnly(codigoIbge);
            bool encontrado = nome != codigoIbge.Trim();
            if (!encontrado && !string.IsNullOrWhiteSpace(fallbackNome))
                return fallbackNome.Trim();
            return nome;
        }

        /// <summary>"Nome / UF" (ex.: "Itajaí / SC").</summary>
        public static string MunicipioUf(string codigoIbge, string fallbackNome = null)
        {
            return $"{ResolveNomeMunicipio(codigoIbge, fallbackNome)} / {ResolveUf(codigoIbge)}";
        }

        /// <summary>"Nome / UF / País" (ex.: "Itajaí / SC / -").</summary>
        public static string MunicipioUfPais(string codigoIbge, string fallbackNome = null)
        {
            return $"{ResolveNomeMunicipio(codigoIbge, fallbackNome)} / {ResolveUf(codigoIbge)} / -";
        }

        /// <summary>
        /// Formata o código IBGE no padrão exibido no DANFSe v2.0 (ex.: 4208203 → "42.08203").
        /// </summary>
        public static string FormatCodigoIbge(string codigoIbge)
        {
            if (string.IsNullOrWhiteSpace(codigoIbge)) return "-";
            string d = new string(codigoIbge.Where(char.IsDigit).ToArray());
            if (d.Length >= 3)
                return $"{d.Substring(0, 2)}.{d.Substring(2)}";
            return codigoIbge;
        }

        /// <summary>
        /// Formata o CEP no padrão exibido no DANFSe v2.0 (ex.: 88307390 → "88.307-390").
        /// </summary>
        public static string FormatCepV2(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep)) return "-";
            string d = new string(cep.Where(char.IsDigit).ToArray());
            if (d.Length == 8)
                return $"{d.Substring(0, 2)}.{d.Substring(2, 3)}-{d.Substring(5, 3)}";
            return cep;
        }

        /// <summary>"Código IBGE / CEP" já formatados.</summary>
        public static string CodigoIbgeCep(string codigoIbge, string cep)
        {
            return $"{FormatCodigoIbge(codigoIbge)} / {FormatCepV2(cep)}";
        }

        /// <summary>Descrição do emitente da NFS-e a partir de tpEmit.</summary>
        public static string GetEmitenteDescricao(string tpEmit)
        {
            switch (tpEmit)
            {
                case "1": return "Prestador";
                case "2": return "Tomador";
                case "3": return "Intermediário";
                default: return "Prestador";
            }
        }

        /// <summary>Situação da NFS-e a partir de cStat.</summary>
        public static string GetSituacaoDescricao(string cStat)
        {
            switch (cStat)
            {
                case "100": return "NFS-e Gerada";
                case "101": return "NFS-e Cancelada";
                case "102": return "NFS-e Substituída";
                default: return string.IsNullOrWhiteSpace(cStat) ? "NFS-e Gerada" : $"NFS-e ({cStat})";
            }
        }

        /// <summary>Finalidade da NFS-e a partir de finNFSe.</summary>
        public static string GetFinalidadeDescricao(string finNFSe)
        {
            switch (finNFSe)
            {
                case "0": return "NFS-e regular";
                case "1": return "NFS-e complementar";
                case "2": return "NFS-e extemporânea";
                case "3": return "NFS-e de substituição";
                default: return "NFS-e regular";
            }
        }

        /// <summary>
        /// Formata "cTribNac / cTribMun" no padrão do v2.0 (ex.: "01.07.01 / -").
        /// </summary>
        public static string CodTribNacionalMunicipal(string cTribNac, string cTribMun)
        {
            string nac = FormatCodTrib(cTribNac);
            string mun = string.IsNullOrWhiteSpace(cTribMun) ? "-" : cTribMun.Trim();
            return $"{nac} / {mun}";
        }

        /// <summary>Formata um código de tributação de 6 dígitos como "NN.NN.NN".</summary>
        public static string FormatCodTrib(string cod)
        {
            if (string.IsNullOrWhiteSpace(cod)) return "-";
            string d = cod.Trim();
            if (d.Length == 6 && d.All(char.IsDigit))
                return $"{d.Substring(0, 2)}.{d.Substring(2, 2)}.{d.Substring(4, 2)}";
            return d;
        }

        /// <summary>
        /// Formata o código NBS no padrão v2.0 (ex.: 115080000 → "1.1508.00.00").
        /// </summary>
        public static string FormatNbs(string cNBS)
        {
            if (string.IsNullOrWhiteSpace(cNBS)) return "-";
            string d = new string(cNBS.Where(char.IsDigit).ToArray());
            if (d.Length == 9)
                return $"{d.Substring(0, 1)}.{d.Substring(1, 4)}.{d.Substring(5, 2)}.{d.Substring(7, 2)}";
            return cNBS;
        }

        /// <summary>Remove zeros à esquerda de uma string numérica (ex.: "05000" → "5000").</summary>
        public static string TrimLeadingZeros(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "-";
            string trimmed = value.Trim().TrimStart('0');
            return string.IsNullOrEmpty(trimmed) ? "0" : trimmed;
        }

        /// <summary>
        /// Soma valores monetários (string, cultura invariante). Retorna "-" se todos vazios.
        /// </summary>
        public static string SomaMoeda(params string[] valores)
        {
            decimal total = 0m;
            bool has = false;
            foreach (var v in valores)
            {
                if (string.IsNullOrWhiteSpace(v)) continue;
                if (decimal.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                {
                    total += d;
                    has = true;
                }
            }
            if (!has) return "-";
            return ValueFormatter.FormatCurrency(total.ToString("F2", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Diferença monetária a − b (ex.: exclusões da base de cálculo). Nunca negativa.
        /// Retorna "R$ 0,00" quando não há diferença e "-" quando faltam ambos os operandos.
        /// </summary>
        public static string DiferencaMoeda(string a, string b)
        {
            bool hasA = decimal.TryParse(a, NumberStyles.Any, CultureInfo.InvariantCulture, out var da);
            bool hasB = decimal.TryParse(b, NumberStyles.Any, CultureInfo.InvariantCulture, out var db);
            if (!hasA && !hasB) return "-";
            decimal diff = (hasA ? da : 0m) - (hasB ? db : 0m);
            if (diff < 0) diff = 0m;
            return ValueFormatter.FormatCurrency(diff.ToString("F2", CultureInfo.InvariantCulture));
        }
    }
}
