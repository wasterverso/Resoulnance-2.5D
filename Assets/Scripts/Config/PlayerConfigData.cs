using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;
using UnityEngine;

namespace Resoulnance.Player
{
    public class PlayerConfigData : MonoBehaviour
    {
        public static PlayerConfigData Instance { get; private set; }

        [Header("Sobre Mim")]
        public string Nickname;
        public string NicknameAuth;
        public string idAuth;

        [Header("Ranking")]
        public ElosRanking eloAtual;
        public ElosRanking eloMaisAlto;

        [Header("Historico")]
        public int quantidadePartidas = 0;
        public float taxaVitoria = 0;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public DadosPublicosPlayer GetDadosPlayer()
        {
            DadosPublicosPlayer efeitoIDs = new DadosPublicosPlayer();
            efeitoIDs.nickAuth = NicknameAuth;
            efeitoIDs.id = idAuth;
            efeitoIDs.idElo = eloAtual.ToString();
            //efeitoIDs.idIconPerfil = ListEfeitos.Instance.iconPerfilEscolhido?.id ?? 0;
            //efeitoIDs.idMoldura = ListEfeitos.Instance.molduraEscolhido?.id ?? 0;
            //efeitoIDs.idFundoKda = ListEfeitos.Instance.fundoKdaEscolhido?.id ?? 0;
            efeitoIDs.quantPartidas = quantidadePartidas;
            efeitoIDs.taxaVitoria = taxaVitoria;

            return efeitoIDs;
        }

        public async void SalvarInfosPlayer()
        {
            DadosPublicosPlayer efeitoIDs = GetDadosPlayer();

            string efeitoIDsJson = JsonUtility.ToJson(efeitoIDs);
            var playerData = new Dictionary<string, object> { { "PlayerInfoData", efeitoIDsJson } };

            try
            {
                await CloudSaveService.Instance.Data.Player.SaveAsync(playerData, new SaveOptions(new PublicWriteAccessClassOptions()));
                Debug.Log($"Efeitos publicos salvos: {string.Join(',', playerData)}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao salvar efeitos publicos: {e.Message}");
            }
        }

        public async Task CarregarInfoPlayer()
        {
            try
            {
                var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "PlayerInfoData" }, new LoadOptions(new PublicReadAccessClassOptions()));

                if (playerData.ContainsKey("PlayerInfoData"))
                {
                    string efeitoIDsJson = playerData["PlayerInfoData"].Value.GetAsString();

                    DadosPublicosPlayer efeitoIDs = JsonUtility.FromJson<DadosPublicosPlayer>(efeitoIDsJson);

                    eloAtual = Enum.TryParse(efeitoIDs.idElo, out ElosRanking novoElo) ? novoElo : ElosRanking.E;
                    quantidadePartidas = efeitoIDs.quantPartidas;
                    taxaVitoria = efeitoIDs.taxaVitoria;
                }
                else
                {
                    Debug.LogWarning("Nenhum dado de jogador encontrado.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao carregar dados do jogador: {e.Message}");
            }
        }


        public async Task<DadosPublicosPlayer> ReceberDadosDoJogador(string id)
        {
            DadosPublicosPlayer dadosJogador = new DadosPublicosPlayer();

            try
            {
                var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(
                    new HashSet<string> { "PlayerInfoData" },
                    new LoadOptions(new PublicReadAccessClassOptions(id))
                );

                if (playerData.ContainsKey("PlayerInfoData"))
                {
                    string efeitoIDsJson = playerData["PlayerInfoData"].Value.GetAs<string>();
                    dadosJogador = JsonUtility.FromJson<DadosPublicosPlayer>(efeitoIDsJson);
                }
                else
                {
                    Debug.LogWarning($"Dados do jogador com ID {id} não encontrados.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Erro ao carregar os dados do jogador: {ex.Message}");
            }
            return dadosJogador;
        }
    }

}


[System.Serializable]
public class DadosPublicosPlayer
{
    public string nickAuth;
    public string id;
    public string idElo;
    public int idIconPerfil;
    public int idMoldura;
    public int idFundoKda;
    public int quantPartidas;
    public float taxaVitoria;
}