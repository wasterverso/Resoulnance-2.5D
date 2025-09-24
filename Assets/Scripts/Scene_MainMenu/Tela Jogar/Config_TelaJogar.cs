using PlayFlow;
using Resoulnance.Player;
using Resoulnance.Telas.TelaLobby;
using Resoulnance.Telas.TelaMainMenu;
using UnityEngine;

namespace Resoulnance.Telas.TelaJogar
{
    public class Config_TelaJogar : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] Start_LobbyController startLobbyController;

        [Header("UI")]
        [SerializeField] GameObject lobby_Painel;
        [SerializeField] GameObject calculadora_Painel;


        public void CriarSala()
        {
            CriarLobby(TiposDeSalas.Normal1v1, 2);
        }

        public void CriarSalaPersonalizada()
        {
            CriarLobby(TiposDeSalas.Personalizada, 8);
        }

        async void CriarLobby(TiposDeSalas tipoSala, int quantPlayers)
        {
            if (ListaDecks.Instance.meusDecksUniversais[0].cartas.Count != 3)
            {
                NotificarPainel.Instance.AtivarErro(true, 1, null);
                return;
            }

            NotificarPainel.Instance.AtivarCarregandoPainel(true);

            string nick = PlayerConfigData.Instance.Nickname;
            var lobbyManager = PlayFlowLobbyManagerV2.Instance.GetComponentInChildren<LobbyManager>();
            string codigo = await lobbyManager.CriarLobby(tipoSala, $"lobby_{nick}", 2, true);

            if (codigo == "InLobby" || codigo == "error")
            {
                NotificarPainel.Instance.AtivarCarregandoPainel(false);
            }
            else
            {
                Debug.Log(codigo);
                startLobbyController.StartSala(codigo);
            }
        }

        public async void EntrarNoLobbyPorCodigo(string codigo)
        {
            if (ListaDecks.Instance.meusDecksUniversais[0].cartas.Count != 3)
            {
                NotificarPainel.Instance.AtivarErro(true, 1, null);
                return;
            }

            NotificarPainel.Instance.AtivarCarregandoPainel(true);

            var lobbyManager = PlayFlowLobbyManagerV2.Instance.GetComponentInChildren<LobbyManager>();
            string result = await lobbyManager.EntrarNoLobbyPeloCodigo(codigo);

            if (result == "InLobby" || result == "error")
            {
                NotificarPainel.Instance.AtivarCarregandoPainel(false);
            }
            else if (result == "notFound")
            {
                NotificarPainel.Instance.AtivarErro(true, 4);
            }
            else
            {
                calculadora_Painel.SetActive(false);
                startLobbyController.StartSala(codigo);
            }
        }
    }
}

