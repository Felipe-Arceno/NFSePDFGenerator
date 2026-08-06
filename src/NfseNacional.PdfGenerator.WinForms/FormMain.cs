using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using NfseNacional.PdfGenerator.Lib.Models;
using NfseNacional.PdfGenerator.Lib.Parsers;
using NfseNacional.PdfGenerator.Lib.Pdf;
using NfseNacional.PdfGenerator.Lib.Repositories;

namespace NfseNacional.PdfGenerator.WinForms
{
    public partial class FormMain : Form
    {
        private MunicipioRepository _repository;
        private AppConfigRepository _configRepo;
        private AppConfig _config;
        private HistoricoRepository _histRepo;
        private string _cadBrasaoFilePath; // path temporário do brasão selecionado no cadastro

        public FormMain()
        {
            InitializeComponent();
            cmbAmbiente.SelectedIndex = 0; // Homologação por padrão
            cmbVersao.SelectedIndex = 0;   // DANFSe 2.0 (padrão nacional) por padrão

            // Inicializa repositórios na pasta do executável
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dados_municipios");
            _repository = new MunicipioRepository(dataPath);
            _configRepo = new AppConfigRepository(dataPath);
            _config = _configRepo.Load();
            _histRepo = new HistoricoRepository();

            txtConfigBasePath.Text = _config.HistoricoBasePath;

            LoadMunicipioComboBox();
            LoadMunicipioGrid();
            LoadHistoricoGrid();
        }

        // =============================================================
        // TAB 1 - GERAR PDF
        // =============================================================

        private void LoadMunicipioComboBox()
        {
            cmbMunSelector.Items.Clear();
            cmbMunSelector.Items.Add("(Nenhum - preencher manualmente)");

            List<MunicipioCadastro> municipios = _repository.GetAll()
                .OrderBy(m => m.Nome)
                .ToList();

            foreach (var mun in municipios)
            {
                cmbMunSelector.Items.Add($"{mun.CodigoIbge} - {mun.Nome}");
            }

            cmbMunSelector.SelectedIndex = 0;
        }

        private void cmbMunSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMunSelector.SelectedIndex <= 0)
            {
                return;
            }

            string selectedText = cmbMunSelector.SelectedItem.ToString();
            string codigoIbge = selectedText.Split('-')[0].Trim();

            MunicipioCadastro mun = _repository.GetByCodigo(codigoIbge);
            if (mun != null)
            {
                txtMunNome.Text = mun.Nome ?? "";
                txtMunSec.Text = mun.Secretaria ?? "";
                txtMunFone.Text = mun.Telefone ?? "";
                txtMunEmail.Text = mun.Email ?? "";

                string brasaoPath = _repository.GetBrasaoPath(codigoIbge);
                txtMunBrasao.Text = File.Exists(brasaoPath) ? brasaoPath : "";

                lblStatus.Text = $"Dados do município {mun.Nome} carregados.";
            }
        }

        private void btnCarregarMun_Click(object sender, EventArgs e)
        {
            LoadMunicipioComboBox();
            lblStatus.Text = "Lista de municípios atualizada.";
        }

        private void btnLoadXml_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtXml.Text = File.ReadAllText(openFileDialog.FileName);
                    lblStatus.Text = $"XML carregado: {Path.GetFileName(openFileDialog.FileName)}";

                    // Tenta auto-selecionar o município pelo cLocEmi do XML
                    TryAutoSelectMunicipio();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar arquivo XML:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnIndentXml_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtXml.Text))
            {
                MessageBox.Show("Cole ou carregue o conteúdo XML no campo abaixo antes de formatar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var doc = XDocument.Parse(txtXml.Text);
                txtXml.Text = doc.ToString();
                lblStatus.Text = "XML identado com sucesso.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"O conteúdo atual não é um XML válido para identação:\n{ex.Message}", "Erro de Formatação XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TryAutoSelectMunicipio()
        {
            try
            {
                NfseRetorno nfse = NfseXmlParser.Parse(txtXml.Text);
                string cLocEmi = nfse?.InfNFSe?.Dps?.InfDps?.CLocEmi;

                if (!string.IsNullOrWhiteSpace(cLocEmi))
                {
                    // Busca o item no combo que começa com o código IBGE
                    for (int i = 1; i < cmbMunSelector.Items.Count; i++)
                    {
                        string item = cmbMunSelector.Items[i].ToString();
                        if (item.StartsWith(cLocEmi))
                        {
                            cmbMunSelector.SelectedIndex = i;
                            lblStatus.Text = $"XML carregado. Município {cLocEmi} auto-detectado.";
                            return;
                        }
                    }

                    lblStatus.Text = $"XML carregado. Município IBGE {cLocEmi} não cadastrado.";
                }
            }
            catch
            {
                // Ignora erros de parse na auto-seleção
            }
        }

        private void btnSelectBrasao_Click(object sender, EventArgs e)
        {
            if (openImageDialog.ShowDialog() == DialogResult.OK)
            {
                txtMunBrasao.Text = openImageDialog.FileName;
            }
        }

        private void btnGeneratePdf_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtXml.Text))
            {
                MessageBox.Show("Por favor, informe ou carregue o conteúdo XML antes de gerar o PDF.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                lblStatus.Text = "Processando XML...";
                Application.DoEvents();

                // 1. Faz o parse do XML
                NfseRetorno nfse = NfseXmlParser.Parse(txtXml.Text);

                if (nfse == null || nfse.InfNFSe == null)
                {
                    throw new InvalidOperationException("Não foi possível ler as informações da nota (infNFSe) no XML fornecido.");
                }

                // 2. Coleta os dados do município (do formulário preenchido)
                var dadosMun = new DadosMunicipio
                {
                    Nome = txtMunNome.Text.Trim(),
                    Secretaria = txtMunSec.Text.Trim(),
                    Telefone = txtMunFone.Text.Trim(),
                    Email = txtMunEmail.Text.Trim(),
                    CaminhoBrasao = txtMunBrasao.Text.Trim()
                };

                // Sugere o nome do arquivo PDF baseado no número da nota ou chave
                string suggestedName = $"DANFSe_{nfse.InfNFSe.NNFSe ?? "Nota"}.pdf";
                if (!string.IsNullOrEmpty(nfse.InfNFSe.ChaveAcesso))
                {
                    suggestedName = $"{nfse.InfNFSe.ChaveAcesso}.pdf";
                }

                saveFileDialog.FileName = suggestedName;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lblStatus.Text = "Gerando PDF...";
                    Application.DoEvents();

                    // 3. Gera o PDF
                    bool isHomologacao = cmbAmbiente.SelectedIndex == 0; // 0 = Homologação, 1 = Produção
                    DanfseVersao versao = cmbVersao.SelectedIndex == 1 ? DanfseVersao.V1_0 : DanfseVersao.V2_0; // 0 = v2.0 (padrão), 1 = v1.0
                    var generator = new DanfsePdfGenerator();
                    generator.GeneratePdfFile(nfse, saveFileDialog.FileName, dadosMun, isHomologacao, versao);

                    // 4. Salva no Histórico na estrutura PATH_BASE/AAAA/MM/DD/GUID/
                    try
                    {
                        _histRepo.Save(_config.HistoricoBasePath, txtXml.Text, saveFileDialog.FileName, nfse, dadosMun);
                        LoadHistoricoGrid();
                    }
                    catch (Exception exHist)
                    {
                        Console.WriteLine($"Erro ao arquivar histórico: {exHist.Message}");
                    }

                    lblStatus.Text = $"PDF gerado com sucesso em: {saveFileDialog.FileName}";

                    if (MessageBox.Show("PDF gerado com sucesso! Deseja abrir o arquivo agora?", "Sucesso", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = saveFileDialog.FileName,
                            UseShellExecute = true
                        };
                        Process.Start(psi);
                    }
                }
                else
                {
                    lblStatus.Text = "Geração cancelada.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Erro ao gerar PDF.";
                MessageBox.Show($"Ocorreu um erro na geração do PDF:\n{ex.Message}\n\nDetalhes:\n{ex.StackTrace}", "Erro na Geração", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================================
        // TAB 2 - CADASTRO DE MUNICÍPIOS
        // =============================================================

        private void LoadMunicipioGrid()
        {
            List<MunicipioCadastro> municipios = _repository.GetAll()
                .OrderBy(m => m.CodigoIbge)
                .ToList();

            dgvMunicipios.DataSource = null;
            dgvMunicipios.DataSource = new System.ComponentModel.BindingList<MunicipioCadastro>(municipios);
            FormatGridColumns();
        }

        private void dgvMunicipios_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            FormatGridColumns();
        }

        private void FormatGridColumns()
        {
            if (dgvMunicipios.Columns == null) return;

            var colIbge = dgvMunicipios.Columns["CodigoIbge"];
            if (colIbge != null)
            {
                colIbge.HeaderText = "Código IBGE";
                colIbge.Width = 90;
                colIbge.FillWeight = 15;
            }

            var colNome = dgvMunicipios.Columns["Nome"];
            if (colNome != null)
            {
                colNome.HeaderText = "Nome";
                colNome.FillWeight = 35;
            }

            var colSec = dgvMunicipios.Columns["Secretaria"];
            if (colSec != null)
            {
                colSec.HeaderText = "Secretaria";
                colSec.FillWeight = 25;
            }

            var colFone = dgvMunicipios.Columns["Telefone"];
            if (colFone != null)
            {
                colFone.HeaderText = "Telefone";
                colFone.FillWeight = 12;
            }

            var colEmail = dgvMunicipios.Columns["Email"];
            if (colEmail != null)
            {
                colEmail.HeaderText = "E-mail";
                colEmail.FillWeight = 13;
            }

            var colLogo = dgvMunicipios.Columns["CaminhoLogoNfse"];
            if (colLogo != null)
            {
                colLogo.Visible = false;
            }
        }

        private void dgvMunicipios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMunicipios.SelectedRows.Count == 0) return;

            var row = dgvMunicipios.SelectedRows[0];
            string codigoIbge = row.Cells["CodigoIbge"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(codigoIbge)) return;

            MunicipioCadastro mun = _repository.GetByCodigo(codigoIbge);
            if (mun != null)
            {
                txtCadCodIbge.Text = mun.CodigoIbge;
                txtCadCodIbge.ReadOnly = true;
                txtCadNome.Text = mun.Nome ?? "";
                txtCadSec.Text = mun.Secretaria ?? "";
                txtCadFone.Text = mun.Telefone ?? "";
                txtCadEmail.Text = mun.Email ?? "";
                _cadBrasaoFilePath = null;

                string brasaoPath = _repository.GetBrasaoPath(codigoIbge);
                if (File.Exists(brasaoPath))
                {
                    txtCadBrasao.Text = brasaoPath;
                    try
                    {
                        using (var stream = new FileStream(brasaoPath, FileMode.Open, FileAccess.Read))
                        {
                            picBrasaoPreview.Image = System.Drawing.Image.FromStream(stream);
                        }
                    }
                    catch
                    {
                        picBrasaoPreview.Image = null;
                    }
                }
                else
                {
                    txtCadBrasao.Text = "";
                    picBrasaoPreview.Image = null;
                }
            }
        }

        private void btnCadNovo_Click(object sender, EventArgs e)
        {
            ClearCadastroForm();
            txtCadCodIbge.ReadOnly = false;
            txtCadCodIbge.Focus();
            lblStatus.Text = "Novo município. Preencha os dados e clique em Salvar.";
        }

        private void ClearCadastroForm()
        {
            txtCadCodIbge.Text = "";
            txtCadCodIbge.ReadOnly = false;
            txtCadNome.Text = "";
            txtCadSec.Text = "";
            txtCadFone.Text = "";
            txtCadEmail.Text = "";
            txtCadBrasao.Text = "";
            _cadBrasaoFilePath = null;
            picBrasaoPreview.Image = null;
        }

        private void btnCadSelectBrasao_Click(object sender, EventArgs e)
        {
            if (openImageDialogCad.ShowDialog() == DialogResult.OK)
            {
                _cadBrasaoFilePath = openImageDialogCad.FileName;
                txtCadBrasao.Text = Path.GetFileName(_cadBrasaoFilePath);

                try
                {
                    using (var stream = new FileStream(_cadBrasaoFilePath, FileMode.Open, FileAccess.Read))
                    {
                        picBrasaoPreview.Image = System.Drawing.Image.FromStream(stream);
                    }
                }
                catch
                {
                    picBrasaoPreview.Image = null;
                }
            }
        }

        private void btnCadSalvar_Click(object sender, EventArgs e)
        {
            string codigoIbge = txtCadCodIbge.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigoIbge))
            {
                MessageBox.Show("O Código IBGE é obrigatório.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCadCodIbge.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCadNome.Text))
            {
                MessageBox.Show("O Nome do município é obrigatório.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCadNome.Focus();
                return;
            }

            try
            {
                var municipio = new MunicipioCadastro
                {
                    CodigoIbge = codigoIbge,
                    Nome = txtCadNome.Text.Trim(),
                    Secretaria = txtCadSec.Text.Trim(),
                    Telefone = txtCadFone.Text.Trim(),
                    Email = txtCadEmail.Text.Trim()
                };

                _repository.Save(municipio);

                if (!string.IsNullOrEmpty(_cadBrasaoFilePath) && File.Exists(_cadBrasaoFilePath))
                {
                    byte[] brasaoBytes = File.ReadAllBytes(_cadBrasaoFilePath);
                    _repository.SaveBrasao(codigoIbge, brasaoBytes);
                    _cadBrasaoFilePath = null;
                }

                lblStatus.Text = $"Município {municipio.Nome} ({codigoIbge}) salvo com sucesso.";

                LoadMunicipioGrid();
                LoadMunicipioComboBox();

                foreach (DataGridViewRow row in dgvMunicipios.Rows)
                {
                    if (row.Cells["CodigoIbge"].Value?.ToString() == codigoIbge)
                    {
                        row.Selected = true;
                        dgvMunicipios.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }

                txtCadCodIbge.ReadOnly = true;
                MessageBox.Show($"Município salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar município:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCadExcluir_Click(object sender, EventArgs e)
        {
            string codigoIbge = txtCadCodIbge.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigoIbge))
            {
                MessageBox.Show("Selecione um município na lista para excluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Tem certeza que deseja excluir o município {codigoIbge}?\nEsta ação não pode ser desfeita.",
                "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    _repository.Delete(codigoIbge);

                    ClearCadastroForm();
                    LoadMunicipioGrid();
                    LoadMunicipioComboBox();

                    lblStatus.Text = $"Município {codigoIbge} excluído com sucesso.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir município:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =============================================================
        // TAB 3 - HISTÓRICO DE EMISSÕES
        // =============================================================

        private void LoadHistoricoGrid()
        {
            List<HistoricoItem> itens = _histRepo.GetAll(_config?.HistoricoBasePath);

            dgvHistorico.DataSource = null;
            dgvHistorico.DataSource = new System.ComponentModel.BindingList<HistoricoItem>(itens);
            FormatHistoricoColumns();
        }

        private void dgvHistorico_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            FormatHistoricoColumns();
        }

        private void FormatHistoricoColumns()
        {
            if (dgvHistorico.Columns == null) return;

            var colData = dgvHistorico.Columns["DataHora"];
            if (colData != null)
            {
                colData.HeaderText = "Data/Hora";
                colData.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                colData.FillWeight = 18;
            }

            var colNum = dgvHistorico.Columns["NumeroNota"];
            if (colNum != null)
            {
                colNum.HeaderText = "Nº Nota";
                colNum.FillWeight = 12;
            }

            var colChave = dgvHistorico.Columns["ChaveAcesso"];
            if (colChave != null)
            {
                colChave.HeaderText = "Chave de Acesso";
                colChave.FillWeight = 30;
            }

            var colToma = dgvHistorico.Columns["TomadorNome"];
            if (colToma != null)
            {
                colToma.HeaderText = "Tomador do Serviço";
                colToma.FillWeight = 25;
            }

            var colMun = dgvHistorico.Columns["MunicipioNome"];
            if (colMun != null)
            {
                colMun.HeaderText = "Município";
                colMun.FillWeight = 15;
            }

            string[] ocultar = { "Guid", "CodigoIbge", "CaminhoPasta", "ArquivoXml", "ArquivoPdf", "DadosMunicipio" };
            foreach (var colName in ocultar)
            {
                var col = dgvHistorico.Columns[colName];
                if (col != null) col.Visible = false;
            }
        }

        private HistoricoItem GetSelectedHistoricoItem()
        {
            if (dgvHistorico.SelectedRows.Count == 0) return null;
            return dgvHistorico.SelectedRows[0].DataBoundItem as HistoricoItem;
        }

        private void btnHistAtualizar_Click(object sender, EventArgs e)
        {
            LoadHistoricoGrid();
            lblStatus.Text = "Lista de histórico atualizada.";
        }

        private void btnHistAbrirPdf_Click(object sender, EventArgs e)
        {
            var item = GetSelectedHistoricoItem();
            if (item == null)
            {
                MessageBox.Show("Selecione um item no histórico para abrir o PDF.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(item.ArquivoPdf))
            {
                MessageBox.Show($"Arquivo PDF não encontrado em:\n{item.ArquivoPdf}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = item.ArquivoPdf,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir arquivo PDF:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHistAbrirXml_Click(object sender, EventArgs e)
        {
            var item = GetSelectedHistoricoItem();
            if (item == null)
            {
                MessageBox.Show("Selecione um item no histórico para abrir o arquivo XML.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(item.ArquivoXml))
            {
                MessageBox.Show($"Arquivo XML não encontrado em:\n{item.ArquivoXml}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = item.ArquivoXml,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir arquivo XML:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHistVerXml_Click(object sender, EventArgs e)
        {
            var item = GetSelectedHistoricoItem();
            if (item == null)
            {
                MessageBox.Show("Selecione um item no histórico para recarregar o XML.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(item.ArquivoXml))
            {
                MessageBox.Show($"Arquivo XML não encontrado em:\n{item.ArquivoXml}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                txtXml.Text = File.ReadAllText(item.ArquivoXml);
                lblStatus.Text = $"XML recarregado do histórico: Nota Nº {item.NumeroNota}";

                TryAutoSelectMunicipio();

                tabControl.SelectedTab = tabGerarPdf;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao recarregar XML:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHistExcluir_Click(object sender, EventArgs e)
        {
            var item = GetSelectedHistoricoItem();
            if (item == null)
            {
                MessageBox.Show("Selecione um item na lista para excluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Deseja excluir o item de histórico da nota {item.NumeroNota} ({item.DataHora:dd/MM/yyyy HH:mm})?\nA pasta em disco será removida.",
                "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _histRepo.Delete(item);
                    LoadHistoricoGrid();
                    lblStatus.Text = "Item de histórico removido com sucesso.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir item do histórico:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =============================================================
        // TAB 4 - CONFIGURAÇÕES
        // =============================================================

        private void btnConfigBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = "Selecione a pasta base para armazenar o histórico de emissões (PATH_BASE)";
            if (Directory.Exists(txtConfigBasePath.Text))
            {
                folderBrowserDialog.SelectedPath = txtConfigBasePath.Text;
            }

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtConfigBasePath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnConfigSalvar_Click(object sender, EventArgs e)
        {
            string newPath = txtConfigBasePath.Text.Trim();
            if (string.IsNullOrWhiteSpace(newPath))
            {
                MessageBox.Show("O caminho base do histórico não pode ficar em branco.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                _config.HistoricoBasePath = newPath;
                _configRepo.Save(_config);

                LoadHistoricoGrid();
                lblStatus.Text = "Configurações salvas com sucesso.";
                MessageBox.Show("Configurações salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configurações:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
