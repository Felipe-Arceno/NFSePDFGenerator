using System;

namespace NfseNacional.PdfGenerator.Lib.Models
{
    public class HistoricoItem
    {
        public string Guid { get; set; }
        public DateTime DataHora { get; set; }
        public string NumeroNota { get; set; }
        public string ChaveAcesso { get; set; }
        public string TomadorNome { get; set; }
        public string MunicipioNome { get; set; }
        public string CodigoIbge { get; set; }
        public string CaminhoPasta { get; set; }
        public string ArquivoXml { get; set; }
        public string ArquivoPdf { get; set; }
        public DadosMunicipio DadosMunicipio { get; set; }
    }
}
