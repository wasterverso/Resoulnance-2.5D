using PurrNet;
using Resoulnance.Flyers;
using Resoulnance.Scene_Preparation.Visuals;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Analytics.IAnalytic;

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

        public void IniciarAtualizacoes()
        {
            List<Jogador> selectedTeam = ListTeamController.Instance.JogadoresConfig
            .Where(j => j.team == ListTeamController.Instance.meuTime).ToList();

            int maxCount = Mathf.Min(selectedTeam.Count, playersAliados.Length);

            for (int i = 0; i < maxCount; i++)
            {
                playersAliados[i].idAuth = selectedTeam[i].authId;
                playersAliados[i].nome_txt.text = selectedTeam[i].nickname;
                playersAliados[i].obj.SetActive(true);
            }

            TelaDeCarregamento.Instance.FazerFadeOut();
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
        public void AtualizarInfoNoServer(
            string idAuthPlayer, string nick, PlayerID idPlayer, int idFlyer, int idSkin, int c1, int c2, int c3, int idItem)
        {
            if (!isServer) return;

            var listTeamController = ListTeamController.Instance;
            var jogadorExistente = listTeamController.JogadoresConfig.FirstOrDefault(j => j.authId == idAuthPlayer);

            if (jogadorExistente == null)
            {
                jogadorExistente = new Jogador { authId = idAuthPlayer };
                listTeamController.JogadoresConfig.Add(jogadorExistente);
            }

            jogadorExistente.playerID = idPlayer;
            jogadorExistente.nickname = nick;
            jogadorExistente.idFlyer = idFlyer;
            jogadorExistente.idSkin = idSkin;
            jogadorExistente.idCarta1 = c1;
            jogadorExistente.idCarta2 = c2;
            jogadorExistente.idCarta3 = c3;
            jogadorExistente.idItemAtivavel = idItem;

            AtualizarPlayerNosClientes(idAuthPlayer, nick, idPlayer, idFlyer, idSkin, c1, c2, c3, idItem);
        }

        [ObserversRpc(runLocally: true)]
        void AtualizarPlayerNosClientes(
            string idAuthPlayer, string nick, PlayerID idPlayer, int idFlyer, int idSkin, int c1, int c2, int c3, int idItem)
        {

            var listTeamController = ListTeamController.Instance;
            var jogadorExistente = listTeamController.JogadoresConfig.FirstOrDefault(j => j.authId == idAuthPlayer);

            if (jogadorExistente == null)
            {
                jogadorExistente = new Jogador { authId = idAuthPlayer };
                listTeamController.JogadoresConfig.Add(jogadorExistente);
            }

            jogadorExistente.playerID = idPlayer;
            jogadorExistente.nickname = nick;
            jogadorExistente.idFlyer = idFlyer;
            jogadorExistente.idSkin = idSkin;
            jogadorExistente.idCarta1 = c1;
            jogadorExistente.idCarta2 = c2;
            jogadorExistente.idCarta3 = c3;
            jogadorExistente.idItemAtivavel = idItem;
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

