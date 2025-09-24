using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using PurrNet;
using Resoulnance.AvatarCustomization;

public class ListTeamController : MonoBehaviour
{
    public static ListTeamController Instance { get; private set; }

    [Header("Sala")]
    public TiposDeSalas tipoSalaAtual;
    public string address;
    public int port;

    [Header("Network")]
    public NetworkMode networkMode;

    [Header("Network")]
    public bool criouLobby;

    [Header("Jogador")]
    public Team meuTime;
    public int meuId;

    [Header("Lista de jogadores")]
    public List<Jogador> JogadoresConfig = new List<Jogador>();

    [Header("Gameplay")]
    public int QuatidadePlayers;

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

    public void LimparListaJogadores()
    {
        QuatidadePlayers = 0;
        JogadoresConfig.Clear();
    }
}

[System.Serializable]
public class Jogador
{
    public string nickname;
    public string authId;
    public PlayerID playerID;
    public Team team;
    public int idKdaBackground;
    public int idHudBackground;
    public int idCarta1, idCarta2, idCarta3;
    public int idItemAtivavel;
    public int idFlyer;
    public int idSkin;
    public int idEfeitoAbate;
    public int idObject_PlayerPrefab;
    public AvatarCustom avatarCustom;
}
