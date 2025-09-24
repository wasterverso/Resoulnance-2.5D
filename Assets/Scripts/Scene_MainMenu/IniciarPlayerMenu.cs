using PurrNet.Utils;
using Resoulnance.Cartas;
using Resoulnance.Flyers;
using Resoulnance.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IniciarPlayerMenu : MonoBehaviour
{
    [Header("Listas")]
    ListaCartas listCartas;
    ListaDecks listDecks;
    ListTeamController listTeamController;
    ListaFlyers listFlyers;

    [Header("Controle")]
    [SerializeField] int idCarta1;
    [SerializeField] int idCarta2;
    [SerializeField] int idCarta3;

    void Start()
    {
        //Adicionar cartas
        listCartas = ListaCartas.Instance;
        listDecks = ListaDecks.Instance;
        listTeamController = ListTeamController.Instance;
        listFlyers = ListaFlyers.Instance;

        Cartas_Data cartasData = listCartas.cartasData;

        Carta card1 = cartasData.cartas.Find(c => c.id == idCarta1);
        Carta card2 = cartasData.cartas.Find(c => c.id == idCarta2);
        Carta card3 = cartasData.cartas.Find(c => c.id == idCarta3);

        listCartas.minhasCartas.Add(card1);
        listCartas.minhasCartas.Add(card2);
        listCartas.minhasCartas.Add(card3);

        listDecks.meusDecksUniversais[0].nomeDeck = "Deck Teste";
        listDecks.meusDecksUniversais[0].cartas.Add(card1);
        listDecks.meusDecksUniversais[0].cartas.Add(card2);
        listDecks.meusDecksUniversais[0].cartas.Add(card3);
        listDecks.meusDecksUniversais[0].itemAtivavel = listDecks.itensData.itensAtivaveis[0];

        listDecks.meusDecksUniversais[1].nomeDeck = "Deck Teste 2";
        listDecks.meusDecksUniversais[1].cartas.Add(card2);
        listDecks.meusDecksUniversais[1].cartas.Add(card3);
        listDecks.meusDecksUniversais[1].cartas.Add(card1);
        listDecks.meusDecksUniversais[1].itemAtivavel = listDecks.itensData.itensAtivaveis[0];

        //Adicionar Flyers
        listFlyers.meusPersonagens.Add(listFlyers.flyerData.personagens[0]);
        listFlyers.meusPersonagens.Add(listFlyers.flyerData.personagens[1]);

        listFlyers.flyerEscolhido = listFlyers.flyerData.personagens[1];
    }

    public void TestarGameplay()
    {        
        listTeamController.tipoSalaAtual = TiposDeSalas.Treinamento;

        listTeamController.JogadoresConfig.Clear();

        Jogador playerConfig = new Jogador();
        playerConfig.team = Team.Blue;
        playerConfig.idCarta1 = listDecks.meusDecksUniversais[0].cartas[0].id;
        playerConfig.idCarta2 = listDecks.meusDecksUniversais[0].cartas[1].id;
        playerConfig.idCarta3 = listDecks.meusDecksUniversais[0].cartas[2].id;

        listTeamController.JogadoresConfig.Add(playerConfig);

        SceneManager.LoadScene("ArenaDeBatalha", LoadSceneMode.Single);
    }

    public void IniciarTreinamento()
    {
        PlayerConfigData playerConfigData = PlayerConfigData.Instance;

        listTeamController.networkMode = NetworkMode.Host;
        listTeamController.meuTime = Team.Blue;
        listTeamController.tipoSalaAtual = TiposDeSalas.Treinamento;

        listTeamController.JogadoresConfig.Clear();

        Jogador jogador = new Jogador();
        jogador.nickname = playerConfigData.Nickname;
        jogador.authId = playerConfigData.idAuth;
        jogador.team = Team.Blue;

        listTeamController.JogadoresConfig.Add(jogador);

        TelaDeCarregamento.Instance.CarregarCena("Preparacao", false);
        //SceneManager.LoadScene("Preparacao", LoadSceneMode.Single);
    }

    public void IniciarTesteServer()
    {
        if (ApplicationContext.isClone)
        {
            listTeamController.networkMode = NetworkMode.Cliente;
            Debug.Log("Sou clone"); 
        }
        else
        {
            listTeamController.networkMode = NetworkMode.Server;
            Destroy(ListTeamController.Instance.gameObject);
            Debug.Log("Sou Server");
        }

        SceneManager.LoadScene("Preparacao", LoadSceneMode.Single);
    }

}
