# CLAUDE.md — NfseNacional.PdfGenerator

Contexto e guia de manutenção do projeto para futuras sessões (com Claude ou não).
Este arquivo resume a arquitetura, as decisões de design e — em detalhe — a
**adição do layout DANFSe v2.0** feita em agosto/2026.

---

## 1. O que é o projeto

Biblioteca .NET + app Windows Forms que:

1. Lê o **XML de retorno** da NFS-e do **Sistema Nacional (Sefin Nacional / DPS)**.
2. Gera o **DANFSe** (Documento Auxiliar da NFS-e) em **PDF** usando `PDFsharp` + `QRCoder`.

Projeto localizado em `C:\Projects\Github\NFSePDFGenerator` (mudou de pasta; caso
scripts antigos apontem para outro caminho, atualizar).

### Estrutura

```
NFSePDFGenerator/
├── CLAUDE.md                        # este arquivo
├── README.md
├── NfseNacional.PdfGenerator.sln
├── samples/
│   └── NFSe_exemplo_v2.xml          # XML de exemplo (base do PDF de referência v2.0)
├── 4208...0572.pdf                  # PDF de REFERÊNCIA v2.0 (gerado pelo portal nacional)
└── src/
    ├── NfseNacional.PdfGenerator.Lib/        # Core — netstandard2.0 (empacotável em NuGet)
    │   ├── Models/
    │   │   ├── NfseModel.cs                   # DTOs do XML (NfseRetorno, InfNFSe, Dps, IBSCBS, ...)
    │   │   └── DanfseVersao.cs                # [NOVO] enum de versão do layout
    │   ├── Parsers/NfseXmlParser.cs           # XDocumentT -> modelo (namespace-agnóstico)
    │   ├── Helpers/
    │   │   ├── Formatters.cs                  # CNPJ/CPF, CEP, endereço, moeda, %, descrições v1
    │   │   ├── DanfseV2Helper.cs              # [NOVO] formatações/descrições específicas do v2.0
    │   │   └── MunicipioHelper.cs             # tabela IBGE->(nome,UF) + GetUF/GetMunicipioNomeOnly
    │   └── Pdf/
    │       ├── DanfseLayout.cs                # constantes de layout (usadas pelo v1.0)
    │       ├── DanfsePdfGenerator.cs          # entrada pública + RenderPageV1 (layout antigo)
    │       ├── DanfsePdfGeneratorV2.cs        # [NOVO] partial class — RenderPageV2 (layout novo)
    │       ├── QrCodeGenerator.cs             # QR em vetor / PNG
    │       └── WindowsFontResolver.cs         # resolve fontes do Windows p/ PDFsharp
    ├── NfseNacional.PdfGenerator.WinForms/    # App desktop — net8.0-windows
    │   ├── FormMain.cs / FormMain.Designer.cs # UI (tabs: Gerar PDF, Municípios, Histórico, Config)
    │   └── ...
    └── TestRunner/                            # Console de teste — net8.0
        └── Program.cs
```

---

## 2. Fluxo de geração (visão geral)

`XML (string/arquivo)` → `NfseXmlParser.Parse` → `NfseRetorno` (modelo) →
`DanfsePdfGenerator.GeneratePdfFile/Bytes(nfse, dadosMun, isHomologacao, versao)` →
`RenderPageV1` **ou** `RenderPageV2` conforme a versão → bytes do PDF.

- `isHomologacao == true` imprime a marca **"NFS-e SEM VALIDADE JURÍDICA"**.
- `dadosMun` (nome/secretaria/telefone/brasão) é usado **principalmente no v1.0**.
  O layout **v2.0 não exibe dados do município** no cabeçalho (segue o padrão nacional,
  que mostra `Município / Ambiente Gerador / Tipo de Ambiente`).

---

## 3. Seleção de versão do DANFSe (v1.0 x v2.0) — decisão principal

Requisito: suportar a **nova versão 2.0** do PDF **sem perder a 1.0**, podendo
**selecionar a versão desejada**, sendo **2.0 o default**.

Como foi implementado, de forma retrocompatível:

- Novo enum `NfseNacional.PdfGenerator.Lib.Models.DanfseVersao { V1_0, V2_0 }`.
- `DanfsePdfGenerator` virou **`partial class`**. O layout antigo foi **renomeado**
  de `RenderPage` para **`RenderPageV1`** (corpo inalibrado). O novo layout está em
  **`RenderPageV2`** (arquivo `DanfsePdfGeneratorV2.cs`).
- As APIs públicas ganharam um parâmetro **opcional no fim**, default `V2_0`:

  ```csharp
  byte[] GeneratePdfBytes(NfseRetorno nfse, DadosMunicipio dadosMun = null,
                          bool isHomologacao = true,
                          DanfseVersao versao = DanfseVersao.V2_0);

  void   GeneratePdfFile (NfseRetorno nfse, string outputFilePath,
                          DadosMunicipio dadosMun = null, bool isHomologacao = true,
                          DanfseVersao versao = DanfseVersao.V2_0);
  ```

  Por ser parâmetro opcional **no final**, chamadas antigas continuam compilando;
  a única mudança de comportamento é que o **default agora é v2.0** (exatamente o pedido).

### App WinForms

- Novo combo **`cmbVersao`** na aba "Gerar PDF" (rótulo "Versão:"), itens:
  - índice 0 → `DANFSe 2.0 (Padrão Nacional)` **(default, selecionado em `FormMain`)**
  - índice 1 → `DANFSe 1.0 (Layout anterior)`
- `btnGeneratePdf_Click` monta `versao = cmbVersao.SelectedIndex == 1 ? V1_0 : V2_0` e
  repassa para `GeneratePdfFile`.

### TestRunner

`src/TestRunner/Program.cs` agora aceita `args[0]=xml` e `args[1]=pasta_saida`
(default: `samples/NFSe_exemplo_v2.xml` e a pasta atual) e gera **os dois PDFs**
(`DANFSe_v2_Gerado.pdf` e `DANFSe_v1_Gerado.pdf`) para comparação.

---

## 4. Layout DANFSe v2.0 — como o `RenderPageV2` foi construído

O renderizador reproduz o PDF de referência (`4208...0572.pdf`) numa grade de
**4 colunas** (`V2Cols = {8, 152.75, 297.5, 442.25, 587}` pt, A4 595×842) com:

- Faixas cinza de título de seção (`Band` + `SectionTitle`).
- Células label/valor (`Field`/`FieldCol`/`FieldSpan`) com truncamento por largura (`Fit`).
- Separadores horizontais/verticais finos (`HLine`/`VLine`).

Ordem das seções (de cima para baixo):

1. **Cabeçalho**: logo textual "NFSe" · título "DANFSe v2.0 / Documento Auxiliar da NFS-e"
   (+ "NFS-e SEM VALIDADE JURÍDICA" se homologação) · bloco direito
   `Município: <xLocEmi> - <UF> / Ambiente Gerador: <ambGer> / Tipo de Ambiente: <tpAmb>`.
2. **Chave de Acesso** + **QR Code** (à direita) + texto de verificação.
3. **3 linhas** de dados: Número/Competência/Emissão NFS-e · Número/Série/Emissão DPS ·
   Emitente/Situação/Finalidade.
4. **PRESTADOR / FORNECEDOR** e **TOMADOR / ADQUIRENTE** (CNPJ, Inscrição, Telefone, Nome,
   Município/UF, Código IBGE/CEP, Endereço, E-mail; prestador também Simples Nacional / Regime).
5. **DESTINATÁRIO** e **INTERMEDIÁRIO** "não identificados" (linhas centralizadas).
6. **SERVIÇO PRESTADO** (Cód. Trib. Nac./Mun., NBS, Local da Prestação, descrição nacional, Descrição do Serviço).
7. **TRIBUTAÇÃO MUNICIPAL (ISSQN)**.
8. **TRIBUTAÇÃO FEDERAL (EXCETO CBS)**.
9. **TRIBUTAÇÃO IBS/CBS** (3 linhas de valores).
10. **VALOR TOTAL DA NFS-e**.
11. **INFORMAÇÕES COMPLEMENTARES** (frase da Lei 12.741/2012).
12. **Rodapé**: Data Cientificação / Identificação e Assinatura / N° NFS-e / Chave.

### Mapeamento de campos e valores derivados (validados contra o PDF de referência)

| Campo no PDF v2.0 | Origem no XML / cálculo | Valor no exemplo |
|---|---|---|
| Município (cabeçalho) | `xLocEmi` + UF do emitente | Itajaí - SC |
| Ambiente Gerador / Tipo de Ambiente | `ambGer` / `tpAmb` | 2 / 1 |
| Série da DPS | `serie` sem zeros à esquerda | 05000 → **5000** |
| Situação | `cStat` (100 → "NFS-e Gerada") | NFS-e Gerada |
| Finalidade | `IBSCBS/finNFSe` (0 → regular) | NFS-e regular |
| Código IBGE (formatado) | `NN.resto` | 4208203 → **42.08203** |
| CEP (formatado v2) | `NN.NNN-NNN` | 88307390 → **88.307-390** |
| Cód. Trib. Nac./Mun. | `cTribNac` (NN.NN.NN) `/` `cTribMun` | 010701 → **01.07.01** / - |
| Código da NBS | `cNBS` (`N.NNNN.NN.NN`) | 115080000 → **1.1508.00.00** |
| UF (quando não há na tag) | 2 primeiros dígitos do IBGE → UF | 2303709 → **CE** |
| Exclusões e Reduções da BC | `valores/vBC` − `IBSCBS/valores/vBC` | 3253,20 − 3188,14 = **65,06** |
| BC após exclusões | `IBSCBS/valores/vBC` | **3.188,14** |
| Total do IBS/CBS | `vIBSTot` + `vCBS` | 3,19 + 28,69 = **31,88** |
| Valor Líquido + IBS/CBS | `totCIBS/vTotNF` | **3.253,20** |
| Indicador de Operação | `DPS/IBSCBS/cIndOp` `/` `IBSCBS/cLocalidadeIncid` `/` nome `/` UF | 100301 / 2303709 / Caucaia / CE |

Todos os valores acima foram **conferidos numericamente** e batem com o PDF de referência.

### Helpers novos — `DanfseV2Helper`

`UfFromCodigoIbge`, `ResolveUf`, `ResolveNomeMunicipio`, `MunicipioUf`, `MunicipioUfPais`,
`FormatCodigoIbge`, `FormatCepV2`, `CodigoIbgeCep`, `GetEmitenteDescricao`,
`GetSituacaoDescricao`, `GetFinalidadeDescricao`, `CodTribNacionalMunicipal`,
`FormatCodTrib`, `FormatNbs`, `TrimLeadingZeros`, `SomaMoeda`, `DiferencaMoeda`.

> Mantidos **separados** do `NfseDescriptionHelper`/`Formatters` (usados pelo v1.0) para
> **não alterar** o comportamento do layout antigo.

---

## 5. Decisões e pontos de atenção (deliberados)

- **Telefone do prestador**: **espelha o portal nacional** → o campo mostra `"-"`
  (o portal não mapeia `emit/fone`). Decisão confirmada na auditoria de ago/2026.
  Se um dia quiser exibir o telefone real, troque `"-"` por
  `ValueFormatter.FormatOrDash(emit?.Fone)` no bloco PRESTADOR de `RenderPageV2`.
- **Descrição do Serviço multilinha**: a linha "Descrição do Serviço" usa `FieldMulti`
  (até 3 linhas, quebra por largura), espelhando o portal, que quebra descrições longas
  (ex.: nota 91, com texto de liminar/ZFM). A altura da seção é dinâmica. Os demais
  campos continuam em linha única com truncamento "…" (`Fit`).
- **Red. Alíquota IBS / CBS**: exibida como `- / - / -` (três traços), igual ao portal.
- **Tabela de municípios limitada**: `MunicipioHelper._municipios` cobre SC e alguns
  outros. Para municípios fora da tabela (ex.: Caucaia/CE), a **UF** é derivada do prefixo
  do código IBGE e o **nome** cai em fallback (`xLocalidadeIncid` do XML, quando bate com o
  código; senão o próprio código). Para cobertura nacional completa, **expandir** a tabela.
- **Margem lateral (impressão)**: o v2.0 usa margem de **~18pt (≈6,3mm)** de cada lado
  (`V2Left=18`, `V2Right=577`, colunas simétricas em torno de 297.5, borda recuada
  para `13,6,569,830`). Isso evita que impressoras físicas cortem o início da primeira
  letra. Para ajustar, altere `V2Left`/`V2Right` e o `V2Cols` em `DanfsePdfGeneratorV2.cs`.
- **Página única**: o conteúdo do v2.0 termina ~y=534 pt e o rodapé fica em y=792 pt,
  cabendo folgado em 1 página A4. Se surgirem descrições muito longas, elas são truncadas
  com "…" (função `Fit`) — ajuste larguras/линhas se precisar de multilinha.
- **Fontes**: reaproveita o `WindowsFontResolver` (Segoe WP / Arial). Em ambiente sem
  as fontes do Windows, o PDFsharp precisará de um `IFontResolver` alternativo.

---

## 6. Como compilar, rodar e validar

> ⚠️ **Não foi possível compilar/rodar no ambiente onde as mudanças foram feitas**
> (sem .NET SDK e sem acesso ao nuget.org). O código foi revisado estaticamente e os
> **valores derivados foram validados numericamente** contra o PDF de referência.
> **Recomenda-se buildar localmente** antes de usar.

```bash
# build
dotnet build NfseNacional.PdfGenerator.sln -c Release

# app WinForms (Windows)
dotnet run --project src/NfseNacional.PdfGenerator.WinForms/NfseNacional.PdfGenerator.WinForms.csproj

# teste rápido (gera DANFSe_v2_Gerado.pdf e DANFSe_v1_Gerado.pdf)
dotnet run --project src/TestRunner/TestRunner.csproj -- samples/NFSe_exemplo_v2.xml .

# pacote NuGet da lib
dotnet pack src/NfseNacional.PdfGenerator.Lib/NfseNacional.PdfGenerator.Lib.csproj -c Release -o ./nupkg
```

**Checklist de validação sugerido após o build:**
1. Rodar o TestRunner e abrir `DANFSe_v2_Gerado.pdf`; comparar lado a lado com
   `4208...0572.pdf` (PDF de referência).
2. Conferir número da nota (102), chave, CNPJs, valores (ISSQN 65,06; IBS 3,19; CBS 28,69;
   Total IBS/CBS 31,88; líquido 3.253,20) e o QR Code.
3. No app WinForms, alternar `Versão` entre 2.0 e 1.0 e confirmar que ambos geram.

---

## 7. Arquivos alterados/criados nesta rodada

**Criados**
- `src/NfseNacional.PdfGenerator.Lib/Models/DanfseVersao.cs`
- `src/NfseNacional.PdfGenerator.Lib/Helpers/DanfseV2Helper.cs`
- `src/NfseNacional.PdfGenerator.Lib/Pdf/DanfsePdfGeneratorV2.cs`
- `samples/NFSe_exemplo_v2.xml`
- `CLAUDE.md`

**Alterados**
- `src/NfseNacional.PdfGenerator.Lib/Pdf/DanfsePdfGenerator.cs`
  (`partial class`; `RenderPage`→`RenderPageV1`; parâmetro `versao` + dispatch)
- `src/NfseNacional.PdfGenerator.WinForms/FormMain.cs` (default + repasse da versão)
- `src/NfseNacional.PdfGenerator.WinForms/FormMain.Designer.cs` (`lblVersao` + `cmbVersao`)
- `src/TestRunner/Program.cs` (usa sample e gera as duas versões)

---

## 8. Auditorias realizadas

- **Nota 102** (`samples/NFSe_exemplo_v2.xml` / `...020572.pdf`): base do layout; todos os
  valores conferidos.
- **Nota 91** (`nfse 91.xml` / `...715788.pdf`, produção, tomador Manaus/AM): auditada em
  ago/2026. Dados 100% corretos (inclusive Manaus/AM, fora da tabela, resolvido via
  `xLocalidadeIncid` + prefixo IBGE). Diferenças encontradas e **já corrigidas**:
  (1) descrição longa agora multilinha; (2) telefone espelha o portal (`-`);
  (3) Red. Alíquota agora `- / - / -`.

## 9. Próximos passos possíveis

- Expandir `MunicipioHelper` para cobertura nacional de nomes de municípios.
- Persistir a **última versão escolhida** nas configurações do app.
