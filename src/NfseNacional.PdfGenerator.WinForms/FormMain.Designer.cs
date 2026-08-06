namespace NfseNacional.PdfGenerator.WinForms
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tabControl = new TabControl();
            tabGerarPdf = new TabPage();
            grpMun = new GroupBox();
            lblMunSelector = new Label();
            cmbMunSelector = new ComboBox();
            btnCarregarMun = new Button();
            lblMunNome = new Label();
            txtMunNome = new TextBox();
            lblMunSec = new Label();
            txtMunSec = new TextBox();
            lblMunFone = new Label();
            txtMunFone = new TextBox();
            lblMunEmail = new Label();
            txtMunEmail = new TextBox();
            lblMunBrasao = new Label();
            txtMunBrasao = new TextBox();
            btnSelectBrasao = new Button();
            lblXml = new Label();
            lblVersao = new Label();
            cmbVersao = new ComboBox();
            lblAmbiente = new Label();
            cmbAmbiente = new ComboBox();
            btnLoadXml = new Button();
            btnIndentXml = new Button();
            btnGeneratePdf = new Button();
            txtXml = new TextBox();
            tabCadastroMun = new TabPage();
            splitCadastro = new SplitContainer();
            dgvMunicipios = new DataGridView();
            grpCadForm = new GroupBox();
            lblCadCodIbge = new Label();
            txtCadCodIbge = new TextBox();
            lblCadNome = new Label();
            txtCadNome = new TextBox();
            lblCadSec = new Label();
            txtCadSec = new TextBox();
            lblCadFone = new Label();
            txtCadFone = new TextBox();
            lblCadEmail = new Label();
            txtCadEmail = new TextBox();
            lblCadBrasao = new Label();
            txtCadBrasao = new TextBox();
            btnCadSelectBrasao = new Button();
            picBrasaoPreview = new PictureBox();
            btnCadSalvar = new Button();
            btnCadNovo = new Button();
            btnCadExcluir = new Button();
            tabHistorico = new TabPage();
            dgvHistorico = new DataGridView();
            panelHistActions = new Panel();
            btnHistAtualizar = new Button();
            btnHistAbrirPdf = new Button();
            btnHistAbrirXml = new Button();
            btnHistVerXml = new Button();
            btnHistExcluir = new Button();
            tabConfiguracoes = new TabPage();
            grpConfig = new GroupBox();
            lblConfigPath = new Label();
            txtConfigBasePath = new TextBox();
            btnConfigBrowse = new Button();
            btnConfigSalvar = new Button();
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            openFileDialog = new OpenFileDialog();
            openImageDialog = new OpenFileDialog();
            openImageDialogCad = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            folderBrowserDialog = new FolderBrowserDialog();
            tabControl.SuspendLayout();
            tabGerarPdf.SuspendLayout();
            grpMun.SuspendLayout();
            tabCadastroMun.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitCadastro).BeginInit();
            splitCadastro.Panel1.SuspendLayout();
            splitCadastro.Panel2.SuspendLayout();
            splitCadastro.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMunicipios).BeginInit();
            grpCadForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picBrasaoPreview).BeginInit();
            tabHistorico.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistorico).BeginInit();
            panelHistActions.SuspendLayout();
            tabConfiguracoes.SuspendLayout();
            grpConfig.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Controls.Add(tabGerarPdf);
            tabControl.Controls.Add(tabCadastroMun);
            tabControl.Controls.Add(tabHistorico);
            tabControl.Controls.Add(tabConfiguracoes);
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1045, 817);
            tabControl.TabIndex = 0;
            // 
            // tabGerarPdf
            // 
            tabGerarPdf.Controls.Add(grpMun);
            tabGerarPdf.Controls.Add(lblXml);
            tabGerarPdf.Controls.Add(lblVersao);
            tabGerarPdf.Controls.Add(cmbVersao);
            tabGerarPdf.Controls.Add(lblAmbiente);
            tabGerarPdf.Controls.Add(cmbAmbiente);
            tabGerarPdf.Controls.Add(btnLoadXml);
            tabGerarPdf.Controls.Add(btnIndentXml);
            tabGerarPdf.Controls.Add(btnGeneratePdf);
            tabGerarPdf.Controls.Add(txtXml);
            tabGerarPdf.Location = new Point(4, 24);
            tabGerarPdf.Name = "tabGerarPdf";
            tabGerarPdf.Padding = new Padding(3);
            tabGerarPdf.Size = new Size(1037, 789);
            tabGerarPdf.TabIndex = 0;
            tabGerarPdf.Text = "  Gerar PDF DANFSe  ";
            tabGerarPdf.UseVisualStyleBackColor = true;
            // 
            // grpMun
            // 
            grpMun.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpMun.Controls.Add(lblMunSelector);
            grpMun.Controls.Add(cmbMunSelector);
            grpMun.Controls.Add(btnCarregarMun);
            grpMun.Controls.Add(lblMunNome);
            grpMun.Controls.Add(txtMunNome);
            grpMun.Controls.Add(lblMunSec);
            grpMun.Controls.Add(txtMunSec);
            grpMun.Controls.Add(lblMunFone);
            grpMun.Controls.Add(txtMunFone);
            grpMun.Controls.Add(lblMunEmail);
            grpMun.Controls.Add(txtMunEmail);
            grpMun.Controls.Add(lblMunBrasao);
            grpMun.Controls.Add(txtMunBrasao);
            grpMun.Controls.Add(btnSelectBrasao);
            grpMun.Location = new Point(8, 8);
            grpMun.Name = "grpMun";
            grpMun.Size = new Size(1021, 155);
            grpMun.TabIndex = 0;
            grpMun.TabStop = false;
            grpMun.Text = " Dados da Prefeitura Emitente (Cabeçalho do DANFSe) ";
            // 
            // lblMunSelector
            // 
            lblMunSelector.AutoSize = true;
            lblMunSelector.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMunSelector.Location = new Point(15, 27);
            lblMunSelector.Name = "lblMunSelector";
            lblMunSelector.Size = new Size(128, 15);
            lblMunSelector.TabIndex = 0;
            lblMunSelector.Text = "Município Cadastrado:";
            // 
            // cmbMunSelector
            // 
            cmbMunSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMunSelector.FormattingEnabled = true;
            cmbMunSelector.Location = new Point(155, 24);
            cmbMunSelector.Name = "cmbMunSelector";
            cmbMunSelector.Size = new Size(575, 23);
            cmbMunSelector.TabIndex = 1;
            cmbMunSelector.SelectedIndexChanged += cmbMunSelector_SelectedIndexChanged;
            // 
            // btnCarregarMun
            // 
            btnCarregarMun.Location = new Point(740, 23);
            btnCarregarMun.Name = "btnCarregarMun";
            btnCarregarMun.Size = new Size(100, 25);
            btnCarregarMun.TabIndex = 2;
            btnCarregarMun.Text = "Atualizar Lista";
            btnCarregarMun.UseVisualStyleBackColor = true;
            btnCarregarMun.Click += btnCarregarMun_Click;
            // 
            // lblMunNome
            // 
            lblMunNome.AutoSize = true;
            lblMunNome.Location = new Point(15, 63);
            lblMunNome.Name = "lblMunNome";
            lblMunNome.Size = new Size(67, 15);
            lblMunNome.TabIndex = 3;
            lblMunNome.Text = "Nome Pref:";
            // 
            // txtMunNome
            // 
            txtMunNome.Location = new Point(100, 60);
            txtMunNome.Name = "txtMunNome";
            txtMunNome.Size = new Size(320, 23);
            txtMunNome.TabIndex = 4;
            // 
            // lblMunSec
            // 
            lblMunSec.AutoSize = true;
            lblMunSec.Location = new Point(435, 63);
            lblMunSec.Name = "lblMunSec";
            lblMunSec.Size = new Size(61, 15);
            lblMunSec.TabIndex = 5;
            lblMunSec.Text = "Secretaria:";
            // 
            // txtMunSec
            // 
            txtMunSec.Location = new Point(505, 60);
            txtMunSec.Name = "txtMunSec";
            txtMunSec.Size = new Size(335, 23);
            txtMunSec.TabIndex = 6;
            // 
            // lblMunFone
            // 
            lblMunFone.AutoSize = true;
            lblMunFone.Location = new Point(15, 93);
            lblMunFone.Name = "lblMunFone";
            lblMunFone.Size = new Size(55, 15);
            lblMunFone.TabIndex = 7;
            lblMunFone.Text = "Telefone:";
            // 
            // txtMunFone
            // 
            txtMunFone.Location = new Point(100, 90);
            txtMunFone.Name = "txtMunFone";
            txtMunFone.Size = new Size(320, 23);
            txtMunFone.TabIndex = 8;
            // 
            // lblMunEmail
            // 
            lblMunEmail.AutoSize = true;
            lblMunEmail.Location = new Point(435, 93);
            lblMunEmail.Name = "lblMunEmail";
            lblMunEmail.Size = new Size(44, 15);
            lblMunEmail.TabIndex = 9;
            lblMunEmail.Text = "E-mail:";
            // 
            // txtMunEmail
            // 
            txtMunEmail.Location = new Point(505, 90);
            txtMunEmail.Name = "txtMunEmail";
            txtMunEmail.Size = new Size(335, 23);
            txtMunEmail.TabIndex = 10;
            // 
            // lblMunBrasao
            // 
            lblMunBrasao.AutoSize = true;
            lblMunBrasao.Location = new Point(15, 123);
            lblMunBrasao.Name = "lblMunBrasao";
            lblMunBrasao.Size = new Size(77, 15);
            lblMunBrasao.TabIndex = 11;
            lblMunBrasao.Text = "Brasão (Img):";
            // 
            // txtMunBrasao
            // 
            txtMunBrasao.Location = new Point(100, 120);
            txtMunBrasao.Name = "txtMunBrasao";
            txtMunBrasao.ReadOnly = true;
            txtMunBrasao.Size = new Size(630, 23);
            txtMunBrasao.TabIndex = 12;
            // 
            // btnSelectBrasao
            // 
            btnSelectBrasao.Location = new Point(740, 119);
            btnSelectBrasao.Name = "btnSelectBrasao";
            btnSelectBrasao.Size = new Size(100, 25);
            btnSelectBrasao.TabIndex = 13;
            btnSelectBrasao.Text = "Buscar Img...";
            btnSelectBrasao.UseVisualStyleBackColor = true;
            btnSelectBrasao.Click += btnSelectBrasao_Click;
            // 
            // lblXml
            // 
            lblXml.AutoSize = true;
            lblXml.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            lblXml.Location = new Point(8, 175);
            lblXml.Name = "lblXml";
            lblXml.Size = new Size(202, 17);
            lblXml.TabIndex = 1;
            lblXml.Text = "Conteúdo XML (NFSe Retorno):";
            //
            // lblVersao
            //
            lblVersao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblVersao.AutoSize = true;
            lblVersao.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblVersao.Location = new Point(216, 176);
            lblVersao.Name = "lblVersao";
            lblVersao.Size = new Size(50, 15);
            lblVersao.TabIndex = 7;
            lblVersao.Text = "Versão:";
            //
            // cmbVersao
            //
            cmbVersao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmbVersao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVersao.FormattingEnabled = true;
            cmbVersao.Items.AddRange(new object[] { "DANFSe 2.0 (Padrão Nacional)", "DANFSe 1.0 (Layout anterior)" });
            cmbVersao.Location = new Point(268, 173);
            cmbVersao.Name = "cmbVersao";
            cmbVersao.Size = new Size(112, 23);
            cmbVersao.TabIndex = 8;
            //
            // lblAmbiente
            //
            lblAmbiente.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblAmbiente.AutoSize = true;
            lblAmbiente.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblAmbiente.Location = new Point(386, 176);
            lblAmbiente.Name = "lblAmbiente";
            lblAmbiente.Size = new Size(65, 15);
            lblAmbiente.TabIndex = 2;
            lblAmbiente.Text = "Ambiente:";
            // 
            // cmbAmbiente
            // 
            cmbAmbiente.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmbAmbiente.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAmbiente.FormattingEnabled = true;
            cmbAmbiente.Items.AddRange(new object[] { "Homologação (Com marca d'água)", "Produção (Sem marca d'água)" });
            cmbAmbiente.Location = new Point(456, 173);
            cmbAmbiente.Name = "cmbAmbiente";
            cmbAmbiente.Size = new Size(195, 23);
            cmbAmbiente.TabIndex = 3;
            // 
            // btnLoadXml
            // 
            btnLoadXml.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLoadXml.Location = new Point(775, 171);
            btnLoadXml.Name = "btnLoadXml";
            btnLoadXml.Size = new Size(122, 26);
            btnLoadXml.TabIndex = 5;
            btnLoadXml.Text = "Carregar Arquivo...";
            btnLoadXml.UseVisualStyleBackColor = true;
            btnLoadXml.Click += btnLoadXml_Click;
            // 
            // btnIndentXml
            // 
            btnIndentXml.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnIndentXml.Location = new Point(659, 171);
            btnIndentXml.Name = "btnIndentXml";
            btnIndentXml.Size = new Size(110, 26);
            btnIndentXml.TabIndex = 4;
            btnIndentXml.Text = "Identar XML";
            btnIndentXml.UseVisualStyleBackColor = true;
            btnIndentXml.Click += btnIndentXml_Click;
            // 
            // btnGeneratePdf
            // 
            btnGeneratePdf.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGeneratePdf.BackColor = Color.FromArgb(0, 120, 215);
            btnGeneratePdf.FlatStyle = FlatStyle.Flat;
            btnGeneratePdf.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnGeneratePdf.ForeColor = Color.White;
            btnGeneratePdf.Location = new Point(903, 171);
            btnGeneratePdf.Name = "btnGeneratePdf";
            btnGeneratePdf.Size = new Size(126, 26);
            btnGeneratePdf.TabIndex = 6;
            btnGeneratePdf.Text = "GERAR PDF";
            btnGeneratePdf.UseVisualStyleBackColor = false;
            btnGeneratePdf.Click += btnGeneratePdf_Click;
            // 
            // txtXml
            // 
            txtXml.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtXml.Font = new Font("Consolas", 9.75F);
            txtXml.Location = new Point(8, 205);
            txtXml.Multiline = true;
            txtXml.Name = "txtXml";
            txtXml.ScrollBars = ScrollBars.Both;
            txtXml.Size = new Size(1021, 574);
            txtXml.TabIndex = 6;
            // 
            // tabCadastroMun
            // 
            tabCadastroMun.Controls.Add(splitCadastro);
            tabCadastroMun.Location = new Point(4, 24);
            tabCadastroMun.Name = "tabCadastroMun";
            tabCadastroMun.Padding = new Padding(3);
            tabCadastroMun.Size = new Size(1037, 789);
            tabCadastroMun.TabIndex = 1;
            tabCadastroMun.Text = "  Cadastro de Municípios  ";
            tabCadastroMun.UseVisualStyleBackColor = true;
            // 
            // splitCadastro
            // 
            splitCadastro.Dock = DockStyle.Fill;
            splitCadastro.FixedPanel = FixedPanel.Panel2;
            splitCadastro.Location = new Point(3, 3);
            splitCadastro.Name = "splitCadastro";
            // 
            // splitCadastro.Panel1
            // 
            splitCadastro.Panel1.Controls.Add(dgvMunicipios);
            // 
            // splitCadastro.Panel2
            // 
            splitCadastro.Panel2.Controls.Add(grpCadForm);
            splitCadastro.Size = new Size(1031, 783);
            splitCadastro.SplitterDistance = 681;
            splitCadastro.TabIndex = 0;
            // 
            // dgvMunicipios
            // 
            dgvMunicipios.AllowUserToAddRows = false;
            dgvMunicipios.AllowUserToDeleteRows = false;
            dgvMunicipios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMunicipios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMunicipios.Dock = DockStyle.Fill;
            dgvMunicipios.Location = new Point(0, 0);
            dgvMunicipios.MultiSelect = false;
            dgvMunicipios.Name = "dgvMunicipios";
            dgvMunicipios.ReadOnly = true;
            dgvMunicipios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMunicipios.Size = new Size(681, 783);
            dgvMunicipios.TabIndex = 0;
            dgvMunicipios.DataBindingComplete += dgvMunicipios_DataBindingComplete;
            dgvMunicipios.SelectionChanged += dgvMunicipios_SelectionChanged;
            // 
            // grpCadForm
            // 
            grpCadForm.Controls.Add(lblCadCodIbge);
            grpCadForm.Controls.Add(txtCadCodIbge);
            grpCadForm.Controls.Add(lblCadNome);
            grpCadForm.Controls.Add(txtCadNome);
            grpCadForm.Controls.Add(lblCadSec);
            grpCadForm.Controls.Add(txtCadSec);
            grpCadForm.Controls.Add(lblCadFone);
            grpCadForm.Controls.Add(txtCadFone);
            grpCadForm.Controls.Add(lblCadEmail);
            grpCadForm.Controls.Add(txtCadEmail);
            grpCadForm.Controls.Add(lblCadBrasao);
            grpCadForm.Controls.Add(txtCadBrasao);
            grpCadForm.Controls.Add(btnCadSelectBrasao);
            grpCadForm.Controls.Add(picBrasaoPreview);
            grpCadForm.Controls.Add(btnCadSalvar);
            grpCadForm.Controls.Add(btnCadNovo);
            grpCadForm.Controls.Add(btnCadExcluir);
            grpCadForm.Dock = DockStyle.Fill;
            grpCadForm.Location = new Point(0, 0);
            grpCadForm.Name = "grpCadForm";
            grpCadForm.Size = new Size(346, 783);
            grpCadForm.TabIndex = 0;
            grpCadForm.TabStop = false;
            grpCadForm.Text = " Dados do Município ";
            // 
            // lblCadCodIbge
            // 
            lblCadCodIbge.AutoSize = true;
            lblCadCodIbge.Location = new Point(10, 25);
            lblCadCodIbge.Name = "lblCadCodIbge";
            lblCadCodIbge.Size = new Size(120, 15);
            lblCadCodIbge.TabIndex = 0;
            lblCadCodIbge.Text = "Código IBGE (Chave):";
            // 
            // txtCadCodIbge
            // 
            txtCadCodIbge.Location = new Point(10, 43);
            txtCadCodIbge.Name = "txtCadCodIbge";
            txtCadCodIbge.Size = new Size(320, 23);
            txtCadCodIbge.TabIndex = 1;
            // 
            // lblCadNome
            // 
            lblCadNome.AutoSize = true;
            lblCadNome.Location = new Point(10, 75);
            lblCadNome.Name = "lblCadNome";
            lblCadNome.Size = new Size(113, 15);
            lblCadNome.TabIndex = 2;
            lblCadNome.Text = "Nome da Prefeitura:";
            // 
            // txtCadNome
            // 
            txtCadNome.Location = new Point(10, 93);
            txtCadNome.Name = "txtCadNome";
            txtCadNome.Size = new Size(320, 23);
            txtCadNome.TabIndex = 3;
            // 
            // lblCadSec
            // 
            lblCadSec.AutoSize = true;
            lblCadSec.Location = new Point(10, 125);
            lblCadSec.Name = "lblCadSec";
            lblCadSec.Size = new Size(61, 15);
            lblCadSec.TabIndex = 4;
            lblCadSec.Text = "Secretaria:";
            // 
            // txtCadSec
            // 
            txtCadSec.Location = new Point(10, 143);
            txtCadSec.Name = "txtCadSec";
            txtCadSec.Size = new Size(320, 23);
            txtCadSec.TabIndex = 5;
            // 
            // lblCadFone
            // 
            lblCadFone.AutoSize = true;
            lblCadFone.Location = new Point(10, 175);
            lblCadFone.Name = "lblCadFone";
            lblCadFone.Size = new Size(55, 15);
            lblCadFone.TabIndex = 6;
            lblCadFone.Text = "Telefone:";
            // 
            // txtCadFone
            // 
            txtCadFone.Location = new Point(10, 193);
            txtCadFone.Name = "txtCadFone";
            txtCadFone.Size = new Size(320, 23);
            txtCadFone.TabIndex = 7;
            // 
            // lblCadEmail
            // 
            lblCadEmail.AutoSize = true;
            lblCadEmail.Location = new Point(10, 225);
            lblCadEmail.Name = "lblCadEmail";
            lblCadEmail.Size = new Size(44, 15);
            lblCadEmail.TabIndex = 8;
            lblCadEmail.Text = "E-mail:";
            // 
            // txtCadEmail
            // 
            txtCadEmail.Location = new Point(10, 243);
            txtCadEmail.Name = "txtCadEmail";
            txtCadEmail.Size = new Size(320, 23);
            txtCadEmail.TabIndex = 9;
            // 
            // lblCadBrasao
            // 
            lblCadBrasao.AutoSize = true;
            lblCadBrasao.Location = new Point(10, 275);
            lblCadBrasao.Name = "lblCadBrasao";
            lblCadBrasao.Size = new Size(131, 15);
            lblCadBrasao.TabIndex = 10;
            lblCadBrasao.Text = "Brasão (Opcional PNG):";
            // 
            // txtCadBrasao
            // 
            txtCadBrasao.Location = new Point(10, 293);
            txtCadBrasao.Name = "txtCadBrasao";
            txtCadBrasao.ReadOnly = true;
            txtCadBrasao.Size = new Size(235, 23);
            txtCadBrasao.TabIndex = 11;
            // 
            // btnCadSelectBrasao
            // 
            btnCadSelectBrasao.Location = new Point(250, 292);
            btnCadSelectBrasao.Name = "btnCadSelectBrasao";
            btnCadSelectBrasao.Size = new Size(80, 25);
            btnCadSelectBrasao.TabIndex = 12;
            btnCadSelectBrasao.Text = "Buscar...";
            btnCadSelectBrasao.UseVisualStyleBackColor = true;
            btnCadSelectBrasao.Click += btnCadSelectBrasao_Click;
            // 
            // picBrasaoPreview
            // 
            picBrasaoPreview.BorderStyle = BorderStyle.FixedSingle;
            picBrasaoPreview.Location = new Point(10, 360);
            picBrasaoPreview.Name = "picBrasaoPreview";
            picBrasaoPreview.Size = new Size(120, 120);
            picBrasaoPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picBrasaoPreview.TabIndex = 13;
            picBrasaoPreview.TabStop = false;
            // 
            // btnCadSalvar
            // 
            btnCadSalvar.BackColor = Color.FromArgb(0, 120, 215);
            btnCadSalvar.FlatStyle = FlatStyle.Flat;
            btnCadSalvar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCadSalvar.ForeColor = Color.White;
            btnCadSalvar.Location = new Point(10, 323);
            btnCadSalvar.Name = "btnCadSalvar";
            btnCadSalvar.Size = new Size(95, 30);
            btnCadSalvar.TabIndex = 14;
            btnCadSalvar.Text = "Salvar";
            btnCadSalvar.UseVisualStyleBackColor = false;
            btnCadSalvar.Click += btnCadSalvar_Click;
            // 
            // btnCadNovo
            // 
            btnCadNovo.Location = new Point(115, 323);
            btnCadNovo.Name = "btnCadNovo";
            btnCadNovo.Size = new Size(95, 30);
            btnCadNovo.TabIndex = 15;
            btnCadNovo.Text = "Novo";
            btnCadNovo.UseVisualStyleBackColor = true;
            btnCadNovo.Click += btnCadNovo_Click;
            // 
            // btnCadExcluir
            // 
            btnCadExcluir.BackColor = Color.FromArgb(200, 50, 50);
            btnCadExcluir.FlatStyle = FlatStyle.Flat;
            btnCadExcluir.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCadExcluir.ForeColor = Color.White;
            btnCadExcluir.Location = new Point(220, 323);
            btnCadExcluir.Name = "btnCadExcluir";
            btnCadExcluir.Size = new Size(110, 30);
            btnCadExcluir.TabIndex = 16;
            btnCadExcluir.Text = "Excluir";
            btnCadExcluir.UseVisualStyleBackColor = false;
            btnCadExcluir.Click += btnCadExcluir_Click;
            // 
            // tabHistorico
            // 
            tabHistorico.Controls.Add(dgvHistorico);
            tabHistorico.Controls.Add(panelHistActions);
            tabHistorico.Location = new Point(4, 24);
            tabHistorico.Name = "tabHistorico";
            tabHistorico.Padding = new Padding(3);
            tabHistorico.Size = new Size(1037, 789);
            tabHistorico.TabIndex = 2;
            tabHistorico.Text = "  Histórico de Emissões  ";
            tabHistorico.UseVisualStyleBackColor = true;
            // 
            // dgvHistorico
            // 
            dgvHistorico.AllowUserToAddRows = false;
            dgvHistorico.AllowUserToDeleteRows = false;
            dgvHistorico.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistorico.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistorico.Dock = DockStyle.Fill;
            dgvHistorico.Location = new Point(3, 3);
            dgvHistorico.MultiSelect = false;
            dgvHistorico.Name = "dgvHistorico";
            dgvHistorico.ReadOnly = true;
            dgvHistorico.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistorico.Size = new Size(1031, 733);
            dgvHistorico.TabIndex = 0;
            dgvHistorico.DataBindingComplete += dgvHistorico_DataBindingComplete;
            // 
            // panelHistActions
            // 
            panelHistActions.Controls.Add(btnHistAtualizar);
            panelHistActions.Controls.Add(btnHistAbrirPdf);
            panelHistActions.Controls.Add(btnHistAbrirXml);
            panelHistActions.Controls.Add(btnHistVerXml);
            panelHistActions.Controls.Add(btnHistExcluir);
            panelHistActions.Dock = DockStyle.Bottom;
            panelHistActions.Location = new Point(3, 736);
            panelHistActions.Name = "panelHistActions";
            panelHistActions.Size = new Size(1031, 50);
            panelHistActions.TabIndex = 1;
            // 
            // btnHistAtualizar
            // 
            btnHistAtualizar.Location = new Point(10, 10);
            btnHistAtualizar.Name = "btnHistAtualizar";
            btnHistAtualizar.Size = new Size(115, 30);
            btnHistAtualizar.TabIndex = 0;
            btnHistAtualizar.Text = "Atualizar Lista";
            btnHistAtualizar.UseVisualStyleBackColor = true;
            btnHistAtualizar.Click += btnHistAtualizar_Click;
            // 
            // btnHistAbrirPdf
            // 
            btnHistAbrirPdf.BackColor = Color.FromArgb(0, 120, 215);
            btnHistAbrirPdf.FlatStyle = FlatStyle.Flat;
            btnHistAbrirPdf.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnHistAbrirPdf.ForeColor = Color.White;
            btnHistAbrirPdf.Location = new Point(135, 10);
            btnHistAbrirPdf.Name = "btnHistAbrirPdf";
            btnHistAbrirPdf.Size = new Size(120, 30);
            btnHistAbrirPdf.TabIndex = 1;
            btnHistAbrirPdf.Text = "Abrir PDF";
            btnHistAbrirPdf.UseVisualStyleBackColor = false;
            btnHistAbrirPdf.Click += btnHistAbrirPdf_Click;
            // 
            // btnHistAbrirXml
            // 
            btnHistAbrirXml.BackColor = Color.FromArgb(40, 160, 90);
            btnHistAbrirXml.FlatStyle = FlatStyle.Flat;
            btnHistAbrirXml.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnHistAbrirXml.ForeColor = Color.White;
            btnHistAbrirXml.Location = new Point(265, 10);
            btnHistAbrirXml.Name = "btnHistAbrirXml";
            btnHistAbrirXml.Size = new Size(120, 30);
            btnHistAbrirXml.TabIndex = 2;
            btnHistAbrirXml.Text = "Abrir XML";
            btnHistAbrirXml.UseVisualStyleBackColor = false;
            btnHistAbrirXml.Click += btnHistAbrirXml_Click;
            // 
            // btnHistVerXml
            // 
            btnHistVerXml.Location = new Point(395, 10);
            btnHistVerXml.Name = "btnHistVerXml";
            btnHistVerXml.Size = new Size(190, 30);
            btnHistVerXml.TabIndex = 3;
            btnHistVerXml.Text = "Recarregar na Aba Gerar PDF";
            btnHistVerXml.UseVisualStyleBackColor = true;
            btnHistVerXml.Click += btnHistVerXml_Click;
            // 
            // btnHistExcluir
            // 
            btnHistExcluir.BackColor = Color.FromArgb(200, 50, 50);
            btnHistExcluir.FlatStyle = FlatStyle.Flat;
            btnHistExcluir.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnHistExcluir.ForeColor = Color.White;
            btnHistExcluir.Location = new Point(595, 10);
            btnHistExcluir.Name = "btnHistExcluir";
            btnHistExcluir.Size = new Size(120, 30);
            btnHistExcluir.TabIndex = 4;
            btnHistExcluir.Text = "Excluir Item";
            btnHistExcluir.UseVisualStyleBackColor = false;
            btnHistExcluir.Click += btnHistExcluir_Click;
            // 
            // tabConfiguracoes
            // 
            tabConfiguracoes.Controls.Add(grpConfig);
            tabConfiguracoes.Location = new Point(4, 24);
            tabConfiguracoes.Name = "tabConfiguracoes";
            tabConfiguracoes.Padding = new Padding(15);
            tabConfiguracoes.Size = new Size(1037, 789);
            tabConfiguracoes.TabIndex = 3;
            tabConfiguracoes.Text = "  Configurações  ";
            tabConfiguracoes.UseVisualStyleBackColor = true;
            // 
            // grpConfig
            // 
            grpConfig.Controls.Add(lblConfigPath);
            grpConfig.Controls.Add(txtConfigBasePath);
            grpConfig.Controls.Add(btnConfigBrowse);
            grpConfig.Controls.Add(btnConfigSalvar);
            grpConfig.Dock = DockStyle.Top;
            grpConfig.Location = new Point(15, 15);
            grpConfig.Name = "grpConfig";
            grpConfig.Size = new Size(1007, 130);
            grpConfig.TabIndex = 0;
            grpConfig.TabStop = false;
            grpConfig.Text = " Diretório Base de Armazenamento do Histórico ";
            // 
            // lblConfigPath
            // 
            lblConfigPath.AutoSize = true;
            lblConfigPath.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblConfigPath.Location = new Point(20, 35);
            lblConfigPath.Name = "lblConfigPath";
            lblConfigPath.Size = new Size(284, 15);
            lblConfigPath.TabIndex = 0;
            lblConfigPath.Text = "Caminho Base (PATH_BASE/AAAA/MM/DD/GUID):";
            // 
            // txtConfigBasePath
            // 
            txtConfigBasePath.Location = new Point(20, 58);
            txtConfigBasePath.Name = "txtConfigBasePath";
            txtConfigBasePath.Size = new Size(680, 23);
            txtConfigBasePath.TabIndex = 1;
            // 
            // btnConfigBrowse
            // 
            btnConfigBrowse.Location = new Point(710, 57);
            btnConfigBrowse.Name = "btnConfigBrowse";
            btnConfigBrowse.Size = new Size(110, 25);
            btnConfigBrowse.TabIndex = 2;
            btnConfigBrowse.Text = "Procurar Pasta...";
            btnConfigBrowse.UseVisualStyleBackColor = true;
            btnConfigBrowse.Click += btnConfigBrowse_Click;
            // 
            // btnConfigSalvar
            // 
            btnConfigSalvar.BackColor = Color.FromArgb(0, 120, 215);
            btnConfigSalvar.FlatStyle = FlatStyle.Flat;
            btnConfigSalvar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnConfigSalvar.ForeColor = Color.White;
            btnConfigSalvar.Location = new Point(20, 90);
            btnConfigSalvar.Name = "btnConfigSalvar";
            btnConfigSalvar.Size = new Size(160, 28);
            btnConfigSalvar.TabIndex = 3;
            btnConfigSalvar.Text = "Salvar Configurações";
            btnConfigSalvar.UseVisualStyleBackColor = false;
            btnConfigSalvar.Click += btnConfigSalvar_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusStrip.Location = new Point(0, 817);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1045, 22);
            statusStrip.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(94, 17);
            lblStatus.Text = "Pronto para uso.";
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "Arquivos XML (*.xml)|*.xml|Todos os arquivos (*.*)|*.*";
            openFileDialog.Title = "Selecione o arquivo XML de Retorno NFSe";
            // 
            // openImageDialog
            // 
            openImageDialog.Filter = "Imagens (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Todos os arquivos (*.*)|*.*";
            openImageDialog.Title = "Selecione a Imagem do Brasão do Município";
            // 
            // openImageDialogCad
            // 
            openImageDialogCad.Filter = "Imagens (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Todos os arquivos (*.*)|*.*";
            openImageDialogCad.Title = "Selecione a Imagem do Brasão do Município";
            // 
            // saveFileDialog
            // 
            saveFileDialog.Filter = "Arquivos PDF (*.pdf)|*.pdf";
            saveFileDialog.Title = "Salvar DANFSe PDF";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1045, 839);
            Controls.Add(tabControl);
            Controls.Add(statusStrip);
            MinimumSize = new Size(750, 500);
            Name = "FormMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Gerador de PDF DANFSe Nacional";
            tabControl.ResumeLayout(false);
            tabGerarPdf.ResumeLayout(false);
            tabGerarPdf.PerformLayout();
            grpMun.ResumeLayout(false);
            grpMun.PerformLayout();
            tabCadastroMun.ResumeLayout(false);
            splitCadastro.Panel1.ResumeLayout(false);
            splitCadastro.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitCadastro).EndInit();
            splitCadastro.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMunicipios).EndInit();
            grpCadForm.ResumeLayout(false);
            grpCadForm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picBrasaoPreview).EndInit();
            tabHistorico.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvHistorico).EndInit();
            panelHistActions.ResumeLayout(false);
            tabConfiguracoes.ResumeLayout(false);
            grpConfig.ResumeLayout(false);
            grpConfig.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        // === Tab Control ===
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabGerarPdf;
        private System.Windows.Forms.TabPage tabCadastroMun;
        private System.Windows.Forms.TabPage tabHistorico;
        private System.Windows.Forms.TabPage tabConfiguracoes;

        // === Tab 1: Gerar PDF ===
        private System.Windows.Forms.GroupBox grpMun;
        private System.Windows.Forms.Label lblMunSelector;
        private System.Windows.Forms.ComboBox cmbMunSelector;
        private System.Windows.Forms.Button btnCarregarMun;
        private System.Windows.Forms.TextBox txtMunNome;
        private System.Windows.Forms.Label lblMunNome;
        private System.Windows.Forms.TextBox txtMunSec;
        private System.Windows.Forms.Label lblMunSec;
        private System.Windows.Forms.TextBox txtMunFone;
        private System.Windows.Forms.Label lblMunFone;
        private System.Windows.Forms.TextBox txtMunEmail;
        private System.Windows.Forms.Label lblMunEmail;
        private System.Windows.Forms.TextBox txtMunBrasao;
        private System.Windows.Forms.Label lblMunBrasao;
        private System.Windows.Forms.Button btnSelectBrasao;
        private System.Windows.Forms.TextBox txtXml;
        private System.Windows.Forms.Label lblXml;
        private System.Windows.Forms.Label lblVersao;
        private System.Windows.Forms.ComboBox cmbVersao;
        private System.Windows.Forms.Label lblAmbiente;
        private System.Windows.Forms.ComboBox cmbAmbiente;
        private System.Windows.Forms.Button btnLoadXml;
        private System.Windows.Forms.Button btnIndentXml;
        private System.Windows.Forms.Button btnGeneratePdf;

        // === Tab 2: Cadastro de Municípios ===
        private System.Windows.Forms.SplitContainer splitCadastro;
        private System.Windows.Forms.DataGridView dgvMunicipios;
        private System.Windows.Forms.GroupBox grpCadForm;
        private System.Windows.Forms.TextBox txtCadCodIbge;
        private System.Windows.Forms.Label lblCadCodIbge;
        private System.Windows.Forms.TextBox txtCadNome;
        private System.Windows.Forms.Label lblCadNome;
        private System.Windows.Forms.TextBox txtCadSec;
        private System.Windows.Forms.Label lblCadSec;
        private System.Windows.Forms.TextBox txtCadFone;
        private System.Windows.Forms.Label lblCadFone;
        private System.Windows.Forms.TextBox txtCadEmail;
        private System.Windows.Forms.Label lblCadEmail;
        private System.Windows.Forms.TextBox txtCadBrasao;
        private System.Windows.Forms.Label lblCadBrasao;
        private System.Windows.Forms.Button btnCadSelectBrasao;
        private System.Windows.Forms.PictureBox picBrasaoPreview;
        private System.Windows.Forms.Button btnCadSalvar;
        private System.Windows.Forms.Button btnCadNovo;
        private System.Windows.Forms.Button btnCadExcluir;

        // === Tab 3: Histórico ===
        private System.Windows.Forms.DataGridView dgvHistorico;
        private System.Windows.Forms.Panel panelHistActions;
        private System.Windows.Forms.Button btnHistAtualizar;
        private System.Windows.Forms.Button btnHistAbrirPdf;
        private System.Windows.Forms.Button btnHistAbrirXml;
        private System.Windows.Forms.Button btnHistVerXml;
        private System.Windows.Forms.Button btnHistExcluir;

        // === Tab 4: Configurações ===
        private System.Windows.Forms.GroupBox grpConfig;
        private System.Windows.Forms.Label lblConfigPath;
        private System.Windows.Forms.TextBox txtConfigBasePath;
        private System.Windows.Forms.Button btnConfigBrowse;
        private System.Windows.Forms.Button btnConfigSalvar;

        // === Shared ===
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.OpenFileDialog openImageDialog;
        private System.Windows.Forms.OpenFileDialog openImageDialogCad;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}
