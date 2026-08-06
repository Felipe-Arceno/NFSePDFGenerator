namespace NfseNacional.PdfGenerator.Lib.Models
{
    /// <summary>
    /// Versão do layout do DANFSe (Documento Auxiliar da NFS-e) a ser gerado.
    /// <para>
    /// O layout <see cref="V2_0"/> é o padrão nacional atual (Sefin Nacional) e é o
    /// valor <b>default</b> em toda a biblioteca. O layout <see cref="V1_0"/> é mantido
    /// para compatibilidade com o formato anterior.
    /// </para>
    /// </summary>
    public enum DanfseVersao
    {
        /// <summary>Layout anterior (primeira versão do DANFSe gerado por esta biblioteca).</summary>
        V1_0 = 1,

        /// <summary>Layout atual do padrão nacional (default).</summary>
        V2_0 = 2
    }
}
