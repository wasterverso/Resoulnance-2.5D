using PurrNet;
using Resoulnance.Flyers;
using Resoulnance.Player;
using Resoulnance.Scene_Preparation.Visuals;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Preparation.Controles
{
    public class PreparacaoController : NetworkBehaviour
    {
        [Header("Refs Script")]
        [SerializeField] PreparacaoVisuals prepVisuals;

        [Header("Outros Players")]
        [SerializeField] PlayerPickBan[] playersAliados = new PlayerPickBan[4];

        private void Awake()
        {
            foreach (var ali in playersAliados)
            {
                ali.obj.SetActive(false);
            }
        }

        public async void IniciarAtualizacoes()
        {
            List<Jogador> selectedTeam = await ReceberJogadores_ServerRpc();

            Team meuTime = selectedTeam.Find(t => t.authId == PlayerConfigData.Instance.idAuth).team;
            ListTeamController.Instance.meuTime = meuTime;

            var timeSelecionado = selectedTeam.Where(j => j.team == meuTime).ToList();
            int maxCount = Mathf.Min(timeSelecionado.Count, playersAliados.Length);

            for (int i = 0; i < maxCount; i++)
            {
                playersAliados[i].idAuth = timeSelecionado[i].authId;
                playersAliados[i].nome_txt.text = timeSelecionado[i].nickname;
                playersAliados[i].obj.SetActive(true);
            }

            TelaDeCarregamento.Instance.CarregamentoAchouPartida(false);
        }

        [ServerRpc(requireOwnership: false)]
        async Task<List<Jogador>> ReceberJogadores_ServerRpc()
        {
            await Task.Yield(); //Esperar o proximo frame

            return ListTeamController.Instance.JogadoresConfig;
        }

        [ObserversRpc(runLocally: true, requireServer: false)]
        public void SelecionouPersonagem(int idFlyer, string idAuthPlayer)
        {
            foreach (var aliObj in playersAliados)
            {
                if (aliObj.idAuth == idAuthPlayer)
                {
                    aliObj.obj.GetComponent<Image>().sprite =
                        ListaFlyers.Instance.flyerData.personagens.Find(c => c.id == idFlyer).quadradoSprite;
                }
            }
        }

        [ServerRpc(requireOwnership: false)]
        public void AtualizarInfoNoServer(Jogador jogador)
        {
            if (!isServer) return;

            var listTeamController = ListTeamController.Instance;
            Jogador jogadorExistente = listTeamController.JogadoresConfig.FirstOrDefault(j => j.authId == jogador.authId);

            jogadorExistente.nickname = jogador.nickname;
            jogadorExistente.playerID = jogador.playerID;
            //jogadorExistente.team = 
            //jogadorExistente.idKdaBackground = 
            //jogadorExistente.idHudBackground = 
            jogadorExistente.idCarta1 = jogador.idCarta1;
            jogadorExistente.idCarta2 = jogador.idCarta2;
            jogadorExistente.idCarta3 = jogador.idCarta3;
            jogadorExistente.idItemAtivavel = jogador.idItemAtivavel;
            jogadorExistente.idFlyer = jogador.idFlyer;
            jogadorExistente.idSkin = jogador.idSkin;
            //jogadorExistente.idEfeitoAbate =

            AtualizarPlayerNosClientes(jogadorExistente);
        }

        [ObserversRpc(runLocally: true)]
        void AtualizarPlayerNosClientes(Jogador jogador)
        {
            var listTeamController = ListTeamController.Instance;
            var jogadorExistente = listTeamController.JogadoresConfig.FirstOrDefault(j => j.authId == jogador.authId);

            if (jogadorExistente == null)
            {
                jogadorExistente = new Jogador();
                jogadorExistente.CopyFrom(jogador);
                listTeamController.JogadoresConfig.Add(jogadorExistente);
            }

            //jogadorExistente = jogador;
        }

        [ObserversRpc(requireServer: false, runLocally: true)]
        public void DebugServidor(string textDebug)
        {
            Debug.Log($"[Debug Server] {textDebug}");
        }

        [System.Serializable]
        class PlayerPickBan
        {
            public GameObject obj;
            public Text nome_txt;
            public string idAuth;
        }
    }
}

