namespace NfseNacional.PdfGenerator.Lib.Models
{
    /// <summary>
    /// Representa o cadastro de um município para emissão de NFS-e.
    /// </summary>
    public class MunicipioCadastro
    {
        /// <summary>
        /// Código IBGE do município (chave primária). Ex: "4208203".
        /// </summary>
        public string CodigoIbge { get; set; }

        /// <summary>
        /// Nome do município/prefeitura. Ex: "Prefeitura Municipal de Itajaí".
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Nome da secretaria responsável. Ex: "SECRETARIA MUNICIPAL DA FAZENDA".
        /// </summary>
        public string Secretaria { get; set; }

        /// <summary>
        /// Telefone de contato do município.
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// E-mail de contato do município.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Caminho opcional para a imagem do logo da NFS-e.
        /// </summary>
        public string CaminhoLogoNfse { get; set; }
    }
}
