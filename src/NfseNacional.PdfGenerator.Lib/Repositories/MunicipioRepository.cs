using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NfseNacional.PdfGenerator.Lib.Models;

namespace NfseNacional.PdfGenerator.Lib.Repositories
{
    /// <summary>
    /// Repositório para gerenciamento de cadastros de municípios.
    /// Persiste os dados em arquivo JSON e gerencia imagens de brasão.
    /// </summary>
    public class MunicipioRepository
    {
        private readonly string _basePath;
        private readonly string _jsonFilePath;
        private readonly string _brasoesPath;
        private static readonly object _fileLock = new object();

        /// <summary>
        /// Inicializa o repositório com o caminho base para armazenamento de dados.
        /// </summary>
        /// <param name="basePath">Caminho raiz onde os dados serão armazenados.</param>
        /// <exception cref="ArgumentNullException">Quando basePath é nulo ou vazio.</exception>
        public MunicipioRepository(string basePath)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                throw new ArgumentNullException(nameof(basePath), "O caminho base não pode ser nulo ou vazio.");
            }

            _basePath = basePath;
            _jsonFilePath = Path.Combine(_basePath, "municipios.json");
            _brasoesPath = Path.Combine(_basePath, "brasoes");

            EnsureDirectoriesExist();
        }

        /// <summary>
        /// Retorna todos os municípios cadastrados.
        /// </summary>
        /// <returns>Lista de municípios. Retorna lista vazia se o arquivo não existir.</returns>
        public List<MunicipioCadastro> GetAll()
        {
            lock (_fileLock)
            {
                return ReadMunicipiosFromFile();
            }
        }

        /// <summary>
        /// Busca um município pelo código IBGE.
        /// </summary>
        /// <param name="codigoIbge">Código IBGE do município.</param>
        /// <returns>O município encontrado ou null se não existir.</returns>
        public MunicipioCadastro GetByCodigo(string codigoIbge)
        {
            if (string.IsNullOrWhiteSpace(codigoIbge))
            {
                return null;
            }

            lock (_fileLock)
            {
                List<MunicipioCadastro> municipios = ReadMunicipiosFromFile();
                return municipios.FirstOrDefault(m =>
                    string.Equals(m.CodigoIbge, codigoIbge, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Adiciona ou atualiza um município (upsert pelo CodigoIbge).
        /// </summary>
        /// <param name="municipio">Dados do município a salvar.</param>
        /// <exception cref="ArgumentNullException">Quando municipio é nulo.</exception>
        /// <exception cref="ArgumentException">Quando CodigoIbge é nulo ou vazio.</exception>
        public void Save(MunicipioCadastro municipio)
        {
            if (municipio == null)
            {
                throw new ArgumentNullException(nameof(municipio));
            }

            if (string.IsNullOrWhiteSpace(municipio.CodigoIbge))
            {
                throw new ArgumentException("O CodigoIbge do município não pode ser nulo ou vazio.", nameof(municipio));
            }

            lock (_fileLock)
            {
                List<MunicipioCadastro> municipios = ReadMunicipiosFromFile();

                int index = municipios.FindIndex(m =>
                    string.Equals(m.CodigoIbge, municipio.CodigoIbge, StringComparison.OrdinalIgnoreCase));

                if (index >= 0)
                {
                    municipios[index] = municipio;
                }
                else
                {
                    municipios.Add(municipio);
                }

                WriteMunicipiosToFile(municipios);
            }
        }

        /// <summary>
        /// Remove um município pelo código IBGE e exclui o arquivo de brasão, se existir.
        /// </summary>
        /// <param name="codigoIbge">Código IBGE do município a remover.</param>
        public void Delete(string codigoIbge)
        {
            if (string.IsNullOrWhiteSpace(codigoIbge))
            {
                return;
            }

            lock (_fileLock)
            {
                List<MunicipioCadastro> municipios = ReadMunicipiosFromFile();

                int removed = municipios.RemoveAll(m =>
                    string.Equals(m.CodigoIbge, codigoIbge, StringComparison.OrdinalIgnoreCase));

                if (removed > 0)
                {
                    WriteMunicipiosToFile(municipios);
                }
            }

            // Remove o brasão fora do lock do JSON, pois é operação independente
            string brasaoPath = GetBrasaoPath(codigoIbge);
            if (File.Exists(brasaoPath))
            {
                File.Delete(brasaoPath);
            }
        }

        /// <summary>
        /// Salva a imagem do brasão do município.
        /// </summary>
        /// <param name="codigoIbge">Código IBGE do município.</param>
        /// <param name="imageBytes">Bytes da imagem PNG do brasão.</param>
        /// <exception cref="ArgumentNullException">Quando codigoIbge ou imageBytes são nulos.</exception>
        public void SaveBrasao(string codigoIbge, byte[] imageBytes)
        {
            if (string.IsNullOrWhiteSpace(codigoIbge))
            {
                throw new ArgumentNullException(nameof(codigoIbge), "O código IBGE não pode ser nulo ou vazio.");
            }

            if (imageBytes == null)
            {
                throw new ArgumentNullException(nameof(imageBytes));
            }

            EnsureDirectoriesExist();

            string brasaoPath = GetBrasaoPath(codigoIbge);
            File.WriteAllBytes(brasaoPath, imageBytes);
        }

        /// <summary>
        /// Lê os bytes da imagem do brasão do município.
        /// </summary>
        /// <param name="codigoIbge">Código IBGE do município.</param>
        /// <returns>Bytes da imagem ou null se o arquivo não existir.</returns>
        public byte[] GetBrasaoBytes(string codigoIbge)
        {
            if (string.IsNullOrWhiteSpace(codigoIbge))
            {
                return null;
            }

            string brasaoPath = GetBrasaoPath(codigoIbge);

            if (!File.Exists(brasaoPath))
            {
                return null;
            }

            return File.ReadAllBytes(brasaoPath);
        }

        /// <summary>
        /// Retorna o caminho completo para o arquivo de brasão do município.
        /// </summary>
        /// <param name="codigoIbge">Código IBGE do município.</param>
        /// <returns>Caminho completo para o arquivo PNG do brasão.</returns>
        public string GetBrasaoPath(string codigoIbge)
        {
            return Path.Combine(_brasoesPath, codigoIbge + ".png");
        }

        /// <summary>
        /// Converte um MunicipioCadastro para DadosMunicipio, carregando os bytes do brasão automaticamente.
        /// </summary>
        /// <param name="codigoIbge">Código IBGE do município.</param>
        /// <returns>Instância de DadosMunicipio preenchida ou null se o município não for encontrado.</returns>
        public DadosMunicipio ToDadosMunicipio(string codigoIbge)
        {
            MunicipioCadastro municipio = GetByCodigo(codigoIbge);

            if (municipio == null)
            {
                return null;
            }

            string brasaoPath = GetBrasaoPath(codigoIbge);
            byte[] brasaoBytes = GetBrasaoBytes(codigoIbge);

            DadosMunicipio dados = new DadosMunicipio
            {
                Nome = municipio.Nome,
                Secretaria = municipio.Secretaria,
                Telefone = municipio.Telefone,
                Email = municipio.Email,
                CaminhoBrasao = File.Exists(brasaoPath) ? brasaoPath : null,
                BrasaoBytes = brasaoBytes,
                CaminhoLogoNfse = municipio.CaminhoLogoNfse
            };

            return dados;
        }

        #region Private Methods

        private void EnsureDirectoriesExist()
        {
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }

            if (!Directory.Exists(_brasoesPath))
            {
                Directory.CreateDirectory(_brasoesPath);
            }
        }

        private List<MunicipioCadastro> ReadMunicipiosFromFile()
        {
            if (!File.Exists(_jsonFilePath))
            {
                return new List<MunicipioCadastro>();
            }

            string json = File.ReadAllText(_jsonFilePath, Encoding.UTF8);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<MunicipioCadastro>();
            }

            List<MunicipioCadastro> municipios = JsonConvert.DeserializeObject<List<MunicipioCadastro>>(json);

            return municipios ?? new List<MunicipioCadastro>();
        }

        private void WriteMunicipiosToFile(List<MunicipioCadastro> municipios)
        {
            string json = JsonConvert.SerializeObject(municipios, Formatting.Indented);
            File.WriteAllText(_jsonFilePath, json, Encoding.UTF8);
        }

        #endregion
    }
}
