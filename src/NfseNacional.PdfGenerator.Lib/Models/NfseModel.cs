using System;
using System.Collections.Generic;

namespace NfseNacional.PdfGenerator.Lib.Models
{
    public class DadosMunicipio
    {
        public string Nome { get; set; } = "MUNICÍPIO DE ITAJAÍ";
        public string Secretaria { get; set; } = "SECRETARIA MUNICIPAL DA FAZENDA";
        public string Telefone { get; set; } = "(47)3241-7400";
        public string Email { get; set; } = "plantaofiscal@itajai.sc.gov.br";
        public string CaminhoBrasao { get; set; }
        public byte[] BrasaoBytes { get; set; }
        public string CaminhoLogoNfse { get; set; }
    }

    public class NfseRetorno
    {
        public string Versao { get; set; }
        public InfNFSe InfNFSe { get; set; } = new InfNFSe();
    }

    public class InfNFSe
    {
        public string Id { get; set; }
        public string ChaveAcesso { get; set; }
        public string XLocEmi { get; set; }
        public string XLocPrestacao { get; set; }
        public string NNFSe { get; set; }
        public string CLocIncid { get; set; }
        public string XLocIncid { get; set; }
        public string XTribNac { get; set; }
        public string XTribMun { get; set; }
        public string XNBS { get; set; }
        public string VerAplic { get; set; }
        public string AmbGer { get; set; }
        public string TpEmis { get; set; }
        public string ProcEmi { get; set; }
        public string CStat { get; set; }
        public string DhProc { get; set; }
        public string NDFSe { get; set; }
        public Emitente Emit { get; set; } = new Emitente();
        public ValoresNFSe Valores { get; set; } = new ValoresNFSe();
        public IbsCbsNfse IbsCbs { get; set; } = new IbsCbsNfse();
        public Dps Dps { get; set; } = new Dps();
    }

    public class Emitente
    {
        public string Cnpj { get; set; }
        public string Cpf { get; set; }
        public string Im { get; set; }
        public string XNome { get; set; }
        public EnderNac EnderNac { get; set; } = new EnderNac();
        public string Fone { get; set; }
        public string Email { get; set; }
    }

    public class EnderNac
    {
        public string XLgr { get; set; }
        public string Nro { get; set; }
        public string XCpl { get; set; }
        public string XBairro { get; set; }
        public string CMun { get; set; }
        public string Uf { get; set; }
        public string Cep { get; set; }
    }

    public class ValoresNFSe
    {
        public string VBC { get; set; }
        public string PAliqAplic { get; set; }
        public string VISSQN { get; set; }
        public string VLiq { get; set; }
        public string VISSRet { get; set; }
        /// <summary>
        /// Tag: NFSe/infNFSe/valores/vTotalRet
        /// Valor total das retenções de tributos da NFS-e.
        /// Fórmula: vTotalRet = Σ(vRetCP + vRetIRRF + vRetCSLL + ISSQN*)
        /// *ISSQN somente é somado quando retido (tpRetISSQN = 2 ou 3).
        /// </summary>
        public string VTotalRet { get; set; }
    }

    public class IbsCbsNfse
    {
        public string CLocalidadeIncid { get; set; }
        public string XLocalidadeIncid { get; set; }
        public IbsCbsValores Valores { get; set; } = new IbsCbsValores();
        public TotCibs TotCIBS { get; set; } = new TotCibs();
    }

    public class IbsCbsValores
    {
        public string VBC { get; set; }
        public string VCalcReeRepRes { get; set; }
        public IbsCbsUf Uf { get; set; } = new IbsCbsUf();
        public IbsCbsMun Mun { get; set; } = new IbsCbsMun();
        public IbsCbsFed Fed { get; set; } = new IbsCbsFed();
    }

    public class IbsCbsUf
    {
        public string PIBSUF { get; set; }
        public string PAliqEfetUF { get; set; }
    }

    public class IbsCbsMun
    {
        public string PIBSMun { get; set; }
        public string PAliqEfetMun { get; set; }
    }

    public class IbsCbsFed
    {
        public string PCBS { get; set; }
        public string PAliqEfetCBS { get; set; }
    }

    public class TotCibs
    {
        public string VTotNF { get; set; }
        public GIbs GIBS { get; set; } = new GIbs();
        public GCbs GCBS { get; set; } = new GCbs();
    }

    public class GIbs
    {
        public string VIBSTot { get; set; }
        public string VIBSUF { get; set; }
        public string VIBSMun { get; set; }
    }

    public class GCbs
    {
        public string VCBS { get; set; }
    }

    public class Dps
    {
        public string Versao { get; set; }
        public InfDps InfDps { get; set; } = new InfDps();
    }

    public class InfDps
    {
        public string Id { get; set; }
        public string TpAmb { get; set; }
        public string DhEmi { get; set; }
        public string VerAplic { get; set; }
        public string Serie { get; set; }
        public string NDPS { get; set; }
        public string DCompet { get; set; }
        public string TpEmit { get; set; }
        public string CLocEmi { get; set; }
        public Substituicao Subst { get; set; } = new Substituicao();
        public Prestador Prest { get; set; } = new Prestador();
        public Tomador Toma { get; set; } = new Tomador();
        public Intermediario Interm { get; set; } = new Intermediario();
        public Servico Serv { get; set; } = new Servico();
        public ValoresDps Valores { get; set; } = new ValoresDps();
        public IbsCbsDps IbsCbs { get; set; } = new IbsCbsDps();
    }

    public class Substituicao
    {
        public string ChSubstda { get; set; }
        public string CMotivo { get; set; }
    }

    public class Prestador
    {
        public string Cnpj { get; set; }
        public string Cpf { get; set; }
        public string Im { get; set; }
        public RegTrib RegTrib { get; set; } = new RegTrib();
    }

    public class RegTrib
    {
        public string OpSimpNac { get; set; }
        public string RegApTribSN { get; set; }
        public string RegEspTrib { get; set; }
    }

    public class Tomador
    {
        public string Cnpj { get; set; }
        public string Cpf { get; set; }
        public string Im { get; set; }
        public string XNome { get; set; }
        public EnderecoTomador End { get; set; } = new EnderecoTomador();
        public string Fone { get; set; }
        public string Email { get; set; }
    }

    public class EnderecoTomador
    {
        public string CMun { get; set; }
        public string Cep { get; set; }
        public string XLgr { get; set; }
        public string Nro { get; set; }
        public string XCpl { get; set; }
        public string XBairro { get; set; }
    }

    public class Intermediario
    {
        public string Cnpj { get; set; }
        public string Cpf { get; set; }
        public string Im { get; set; }
        public string XNome { get; set; }
    }

    public class Servico
    {
        public string CLocPrestacao { get; set; }
        public string CTribNac { get; set; }
        public string CTribMun { get; set; }
        public string XTribMun { get; set; }
        public string XDescServ { get; set; }
        public string CNBS { get; set; }
    }

    public class ValoresDps
    {
        public string VServ { get; set; }
        public string VDescIncond { get; set; }
        public string VDescCond { get; set; }
        public string VDed { get; set; }
        public Tributos Trib { get; set; } = new Tributos();
    }

    public class Tributos
    {
        public TribMun TribMun { get; set; } = new TribMun();
        public TribFed TribFed { get; set; } = new TribFed();
        public TotTrib TotTrib { get; set; } = new TotTrib();
    }

    public class TribMun
    {
        public string TribISSQN { get; set; }
        public string CPaisResult { get; set; }
        public string TpImunidade { get; set; }
        public string TpRetISSQN { get; set; }
        public string NProcesso { get; set; }
        public BeneficioMunicipal BM { get; set; } = new BeneficioMunicipal();
        public string PAliq { get; set; }
        public string CBenMun { get; set; }
        public string TpSusp { get; set; }
    }

    public class BeneficioMunicipal
    {
        public string NBM { get; set; }
        public string VRedBCBM { get; set; }
        public string PRedBCBM { get; set; }
    }

    public class TribFed
    {
        public string VRetIRRF { get; set; }
        public string VRetCP { get; set; }
        public string VRetCSLL { get; set; }
        public string XDescRetCSLL { get; set; }
        public PisCofins PisCofins { get; set; } = new PisCofins();
    }

    public class PisCofins
    {
        public string Cst { get; set; }
        public string VBCPisCofins { get; set; }
        public string PPis { get; set; }
        public string PCofins { get; set; }
        public string VPis { get; set; }
        public string VCofins { get; set; }
    }

    public class TotTrib
    {
        public string PTotTribFed { get; set; }
        public string PTotTribEst { get; set; }
        public string PTotTribMun { get; set; }
    }

    public class IbsCbsDps
    {
        public string FinNFSe { get; set; }
        public string CIndOp { get; set; }
        public string IndDest { get; set; }
        public string Cst { get; set; }
        public string CClassTrib { get; set; }
    }
}
