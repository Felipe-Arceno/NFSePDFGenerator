using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NfseNacional.PdfGenerator.Lib.Models;

namespace NfseNacional.PdfGenerator.Lib.Parsers
{
    public static class NfseXmlParser
    {
        public static NfseRetorno ParseFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo XML não encontrado.", filePath);

            string xml = File.ReadAllText(filePath);
            return Parse(xml);
        }

        public static NfseRetorno Parse(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("Conteúdo XML vazio.", nameof(xml));

            var doc = XDocument.Parse(xml);
            var root = doc.Root;
            if (root == null)
                throw new InvalidOperationException("XML inválido: elemento raiz ausente.");

            var retorno = new NfseRetorno
            {
                Versao = root.Attribute("versao")?.Value
            };

            // Tenta achar infNFSe independente de namespace ou com namespace sped
            var infNfseElem = FindElement(root, "infNFSe") ?? root;

            var inf = retorno.InfNFSe;
            inf.Id = infNfseElem.Attribute("Id")?.Value;
            if (!string.IsNullOrEmpty(inf.Id) && inf.Id.StartsWith("NFS"))
            {
                inf.ChaveAcesso = inf.Id.Substring(3);
            }
            else
            {
                inf.ChaveAcesso = inf.Id;
            }

            inf.XLocEmi = GetValue(infNfseElem, "xLocEmi");
            inf.XLocPrestacao = GetValue(infNfseElem, "xLocPrestacao");
            inf.NNFSe = GetValue(infNfseElem, "nNFSe");
            inf.CLocIncid = GetValue(infNfseElem, "cLocIncid");
            inf.XLocIncid = GetValue(infNfseElem, "xLocIncid");
            inf.XTribNac = GetValue(infNfseElem, "xTribNac");
            inf.XTribMun = GetValue(infNfseElem, "xTribMun") ?? GetValue(infNfseElem, "xDescTribMun");
            inf.XNBS = GetValue(infNfseElem, "xNBS");
            inf.VerAplic = GetValue(infNfseElem, "verAplic");
            inf.AmbGer = GetValue(infNfseElem, "ambGer");
            inf.TpEmis = GetValue(infNfseElem, "tpEmis");
            inf.ProcEmi = GetValue(infNfseElem, "procEmi");
            inf.CStat = GetValue(infNfseElem, "cStat");
            inf.DhProc = GetValue(infNfseElem, "dhProc");
            inf.NDFSe = GetValue(infNfseElem, "nDFSe");

            // Emit
            var emitElem = FindElement(infNfseElem, "emit");
            if (emitElem != null)
            {
                inf.Emit.Cnpj = GetValue(emitElem, "CNPJ");
                inf.Emit.Cpf = GetValue(emitElem, "CPF");
                inf.Emit.Im = GetValue(emitElem, "IM");
                inf.Emit.XNome = GetValue(emitElem, "xNome");
                inf.Emit.Fone = GetValue(emitElem, "fone");
                inf.Emit.Email = GetValue(emitElem, "email");

                var enderElem = FindElement(emitElem, "enderNac") ?? FindElement(emitElem, "end");
                if (enderElem != null)
                {
                    inf.Emit.EnderNac.XLgr = GetValue(enderElem, "xLgr");
                    inf.Emit.EnderNac.Nro = GetValue(enderElem, "nro");
                    inf.Emit.EnderNac.XCpl = GetValue(enderElem, "xCpl");
                    inf.Emit.EnderNac.XBairro = GetValue(enderElem, "xBairro");
                    inf.Emit.EnderNac.CMun = GetValue(enderElem, "cMun");
                    inf.Emit.EnderNac.Uf = GetValue(enderElem, "UF");
                    inf.Emit.EnderNac.Cep = GetValue(enderElem, "CEP");
                }
            }

            // Valores NFSe
            var valNfseElem = FindElement(infNfseElem, "valores");
            if (valNfseElem != null)
            {
                inf.Valores.VBC = GetValue(valNfseElem, "vBC");
                inf.Valores.PAliqAplic = GetValue(valNfseElem, "pAliqAplic");
                inf.Valores.VISSQN = GetValue(valNfseElem, "vISSQN");
                inf.Valores.VLiq = GetValue(valNfseElem, "vLiq");
                inf.Valores.VISSRet = GetValue(valNfseElem, "vISSRet");
                inf.Valores.VTotalRet = GetValue(valNfseElem, "vTotalRet");
            }

            // IBSCBS NFSe
            var ibsNfseElem = FindElement(infNfseElem, "IBSCBS");
            if (ibsNfseElem != null)
            {
                inf.IbsCbs.CLocalidadeIncid = GetValue(ibsNfseElem, "cLocalidadeIncid");
                inf.IbsCbs.XLocalidadeIncid = GetValue(ibsNfseElem, "xLocalidadeIncid");

                var ibsValElem = FindElement(ibsNfseElem, "valores");
                if (ibsValElem != null)
                {
                    inf.IbsCbs.Valores.VBC = GetValue(ibsValElem, "vBC");
                    inf.IbsCbs.Valores.VCalcReeRepRes = GetValue(ibsValElem, "vCalcReeRepRes");

                    var ufElem = FindElement(ibsValElem, "uf");
                    if (ufElem != null)
                    {
                        inf.IbsCbs.Valores.Uf.PIBSUF = GetValue(ufElem, "pIBSUF");
                        inf.IbsCbs.Valores.Uf.PAliqEfetUF = GetValue(ufElem, "pAliqEfetUF");
                    }

                    var munElem = FindElement(ibsValElem, "mun");
                    if (munElem != null)
                    {
                        inf.IbsCbs.Valores.Mun.PIBSMun = GetValue(munElem, "pIBSMun");
                        inf.IbsCbs.Valores.Mun.PAliqEfetMun = GetValue(munElem, "pAliqEfetMun");
                    }

                    var fedElem = FindElement(ibsValElem, "fed");
                    if (fedElem != null)
                    {
                        inf.IbsCbs.Valores.Fed.PCBS = GetValue(fedElem, "pCBS");
                        inf.IbsCbs.Valores.Fed.PAliqEfetCBS = GetValue(fedElem, "pAliqEfetCBS");
                    }
                }

                var totIbsElem = FindElement(ibsNfseElem, "totCIBS");
                if (totIbsElem != null)
                {
                    inf.IbsCbs.TotCIBS.VTotNF = GetValue(totIbsElem, "vTotNF");
                    var gIbsElem = FindElement(totIbsElem, "gIBS");
                    if (gIbsElem != null)
                    {
                        inf.IbsCbs.TotCIBS.GIBS.VIBSTot = GetValue(gIbsElem, "vIBSTot");
                        var ufTot = FindElement(gIbsElem, "gIBSUFTot");
                        if (ufTot != null) inf.IbsCbs.TotCIBS.GIBS.VIBSUF = GetValue(ufTot, "vIBSUF");
                        var munTot = FindElement(gIbsElem, "gIBSMunTot");
                        if (munTot != null) inf.IbsCbs.TotCIBS.GIBS.VIBSMun = GetValue(munTot, "vIBSMun");
                    }

                    var gCbsElem = FindElement(totIbsElem, "gCBS");
                    if (gCbsElem != null)
                    {
                        inf.IbsCbs.TotCIBS.GCBS.VCBS = GetValue(gCbsElem, "vCBS");
                    }
                }
            }

            // DPS / infDPS
            var dpsElem = FindElement(infNfseElem, "DPS");
            var infDpsElem = dpsElem != null ? FindElement(dpsElem, "infDPS") : FindElement(infNfseElem, "infDPS");
            if (infDpsElem != null)
            {
                var dps = inf.Dps.InfDps;
                dps.Id = infDpsElem.Attribute("Id")?.Value;
                dps.TpAmb = GetValue(infDpsElem, "tpAmb");
                dps.DhEmi = GetValue(infDpsElem, "dhEmi");
                dps.VerAplic = GetValue(infDpsElem, "verAplic");
                dps.Serie = GetValue(infDpsElem, "serie");
                dps.NDPS = GetValue(infDpsElem, "nDPS");
                dps.DCompet = GetValue(infDpsElem, "dCompet");
                dps.TpEmit = GetValue(infDpsElem, "tpEmit");
                dps.CLocEmi = GetValue(infDpsElem, "cLocEmi");

                var substElem = FindElement(infDpsElem, "subst");
                if (substElem != null)
                {
                    dps.Subst.ChSubstda = GetValue(substElem, "chSubstda");
                    dps.Subst.CMotivo = GetValue(substElem, "cMotivo");
                }

                // Prestador
                var prestElem = FindElement(infDpsElem, "prest");
                if (prestElem != null)
                {
                    dps.Prest.Cnpj = GetValue(prestElem, "CNPJ");
                    dps.Prest.Cpf = GetValue(prestElem, "CPF");
                    dps.Prest.Im = GetValue(prestElem, "IM");

                    var regElem = FindElement(prestElem, "regTrib");
                    if (regElem != null)
                    {
                        dps.Prest.RegTrib.OpSimpNac = GetValue(regElem, "opSimpNac");
                        dps.Prest.RegTrib.RegApTribSN = GetValue(regElem, "regApTribSN");
                        dps.Prest.RegTrib.RegEspTrib = GetValue(regElem, "regEspTrib");
                    }
                }

                // Tomador
                var tomaElem = FindElement(infDpsElem, "toma");
                if (tomaElem != null)
                {
                    dps.Toma.Cnpj = GetValue(tomaElem, "CNPJ");
                    dps.Toma.Cpf = GetValue(tomaElem, "CPF");
                    dps.Toma.Im = GetValue(tomaElem, "IM");
                    dps.Toma.XNome = GetValue(tomaElem, "xNome");
                    dps.Toma.Fone = GetValue(tomaElem, "fone");
                    dps.Toma.Email = GetValue(tomaElem, "email");

                    var endElem = FindElement(tomaElem, "end");
                    if (endElem != null)
                    {
                        var endNacElem = FindElement(endElem, "endNac");
                        dps.Toma.End.XLgr = GetValue(endElem, "xLgr") ?? (endNacElem != null ? GetValue(endNacElem, "xLgr") : null);
                        dps.Toma.End.Nro = GetValue(endElem, "nro") ?? (endNacElem != null ? GetValue(endNacElem, "nro") : null);
                        dps.Toma.End.XCpl = GetValue(endElem, "xCpl") ?? (endNacElem != null ? GetValue(endNacElem, "xCpl") : null);
                        dps.Toma.End.XBairro = GetValue(endElem, "xBairro") ?? (endNacElem != null ? GetValue(endNacElem, "xBairro") : null);

                        if (endNacElem != null)
                        {
                            dps.Toma.End.CMun = GetValue(endNacElem, "cMun") ?? GetValue(endElem, "cMun");
                            dps.Toma.End.Cep = GetValue(endNacElem, "CEP") ?? GetValue(endElem, "CEP");
                        }
                        else
                        {
                            dps.Toma.End.CMun = GetValue(endElem, "cMun");
                            dps.Toma.End.Cep = GetValue(endElem, "CEP");
                        }
                    }
                }

                // Intermediário
                var intermElem = FindElement(infDpsElem, "interm");
                if (intermElem != null)
                {
                    dps.Interm.Cnpj = GetValue(intermElem, "CNPJ");
                    dps.Interm.Cpf = GetValue(intermElem, "CPF");
                    dps.Interm.Im = GetValue(intermElem, "IM");
                    dps.Interm.XNome = GetValue(intermElem, "xNome");
                }

                // Serviço
                var servElem = FindElement(infDpsElem, "serv");
                if (servElem != null)
                {
                    var locElem = FindElement(servElem, "locPrest");
                    if (locElem != null)
                    {
                        dps.Serv.CLocPrestacao = GetValue(locElem, "cLocPrestacao");
                    }

                    var cServElem = FindElement(servElem, "cServ");
                    if (cServElem != null)
                    {
                        dps.Serv.CTribNac = GetValue(cServElem, "cTribNac");
                        dps.Serv.CTribMun = GetValue(cServElem, "cTribMun");
                        dps.Serv.XTribMun = GetValue(cServElem, "xTribMun") ?? GetValue(cServElem, "xDescTribMun");
                        dps.Serv.XDescServ = GetValue(cServElem, "xDescServ");
                        dps.Serv.CNBS = GetValue(cServElem, "cNBS");
                    }
                }

                // Valores DPS
                var valDpsElem = FindElement(infDpsElem, "valores");
                if (valDpsElem != null)
                {
                    var vServElem = FindElement(valDpsElem, "vServPrest");
                    if (vServElem != null)
                    {
                        dps.Valores.VServ = GetValue(vServElem, "vServ");
                    }

                    var vDescElem = FindElement(valDpsElem, "vDescCondIncond");
                    if (vDescElem != null)
                    {
                        dps.Valores.VDescIncond = GetValue(vDescElem, "vDescIncond");
                        dps.Valores.VDescCond = GetValue(vDescElem, "vDescCond");
                    }

                    var vDedElem = FindElement(valDpsElem, "vDedRed");
                    if (vDedElem != null)
                    {
                        dps.Valores.VDed = GetValue(vDedElem, "vDed");
                    }

                    var tribElem = FindElement(valDpsElem, "trib");
                    if (tribElem != null)
                    {
                        var tMun = FindElement(tribElem, "tribMun");
                        if (tMun != null)
                        {
                            dps.Valores.Trib.TribMun.TribISSQN = GetValue(tMun, "tribISSQN");
                            dps.Valores.Trib.TribMun.CPaisResult = GetValue(tMun, "cPaisResult");
                            dps.Valores.Trib.TribMun.TpImunidade = GetValue(tMun, "tpImunidade");
                            dps.Valores.Trib.TribMun.TpRetISSQN = GetValue(tMun, "tpRetISSQN");
                            dps.Valores.Trib.TribMun.NProcesso = GetValue(tMun, "nProcesso");
                            dps.Valores.Trib.TribMun.PAliq = GetValue(tMun, "pAliq");
                            dps.Valores.Trib.TribMun.CBenMun = GetValue(tMun, "cBenMun");
                            dps.Valores.Trib.TribMun.TpSusp = GetValue(tMun, "tpSusp");

                            var bmElem = FindElement(tMun, "BM");
                            if (bmElem != null)
                            {
                                dps.Valores.Trib.TribMun.BM.NBM = GetValue(bmElem, "nBM");
                                dps.Valores.Trib.TribMun.BM.VRedBCBM = GetValue(bmElem, "vRedBCBM");
                                dps.Valores.Trib.TribMun.BM.PRedBCBM = GetValue(bmElem, "pRedBCBM");
                            }
                        }

                        var tFed = FindElement(tribElem, "tribFed");
                        if (tFed != null)
                        {
                            dps.Valores.Trib.TribFed.VRetIRRF = GetValue(tFed, "vRetIRRF");
                            dps.Valores.Trib.TribFed.VRetCP = GetValue(tFed, "vRetCP");
                            dps.Valores.Trib.TribFed.VRetCSLL = GetValue(tFed, "vRetCSLL");
                            dps.Valores.Trib.TribFed.XDescRetCSLL = GetValue(tFed, "xDescRetCSLL");

                            var pisElem = FindElement(tFed, "piscofins");
                            if (pisElem != null)
                            {
                                dps.Valores.Trib.TribFed.PisCofins.Cst = GetValue(pisElem, "CST");
                                dps.Valores.Trib.TribFed.PisCofins.VBCPisCofins = GetValue(pisElem, "vBCPisCofins");
                                dps.Valores.Trib.TribFed.PisCofins.PPis = GetValue(pisElem, "pPIS");
                                dps.Valores.Trib.TribFed.PisCofins.PCofins = GetValue(pisElem, "pCOFINS");
                                dps.Valores.Trib.TribFed.PisCofins.VPis = GetValue(pisElem, "vPIS");
                                dps.Valores.Trib.TribFed.PisCofins.VCofins = GetValue(pisElem, "vCOFINS");
                            }
                        }

                        var totTribElem = FindElement(tribElem, "totTrib");
                        if (totTribElem != null)
                        {
                            var pTotElem = FindElement(totTribElem, "pTotTrib");
                            if (pTotElem != null)
                            {
                                dps.Valores.Trib.TotTrib.PTotTribFed = GetValue(pTotElem, "pTotTribFed");
                                dps.Valores.Trib.TotTrib.PTotTribEst = GetValue(pTotElem, "pTotTribEst");
                                dps.Valores.Trib.TotTrib.PTotTribMun = GetValue(pTotElem, "pTotTribMun");
                            }
                        }
                    }
                }

                // IBSCBS DPS
                var ibsDpsElem = FindElement(infDpsElem, "IBSCBS");
                if (ibsDpsElem != null)
                {
                    dps.IbsCbs.FinNFSe = GetValue(ibsDpsElem, "finNFSe");
                    dps.IbsCbs.CIndOp = GetValue(ibsDpsElem, "cIndOp");
                    dps.IbsCbs.IndDest = GetValue(ibsDpsElem, "indDest");

                    var tribElem = FindElement(ibsDpsElem, "valores") != null 
                        ? FindElement(FindElement(ibsDpsElem, "valores"), "trib") 
                        : null;
                    if (tribElem != null)
                    {
                        var gIbsCbs = FindElement(tribElem, "gIBSCBS");
                        if (gIbsCbs != null)
                        {
                            dps.IbsCbs.Cst = GetValue(gIbsCbs, "CST");
                            dps.IbsCbs.CClassTrib = GetValue(gIbsCbs, "cClassTrib");
                        }
                    }
                }
            }

            return retorno;
        }

        private static XElement FindElement(XElement parent, string localName)
        {
            if (parent == null) return null;
            return parent.Elements().FirstOrDefault(e => e.Name.LocalName == localName);
        }

        private static string GetValue(XElement parent, string localName)
        {
            var elem = FindElement(parent, localName);
            return elem?.Value;
        }
    }
}
