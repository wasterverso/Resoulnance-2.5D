using PurrNet;
using PurrNet.Packing;
using Resoulnance.AvatarCustomization;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

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
    public ulong meuId;

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
public class Jogador : IPackedAuto
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

    public void Write(BitPacker packer)
    {
        Packer<string>.Write(packer, nickname);
        Packer<string>.Write(packer, authId);
        Packer<PlayerID>.Write(packer, playerID);
        Packer<Team>.Write(packer, team);
        Packer<int>.Write(packer, idKdaBackground);
        Packer<int>.Write(packer, idHudBackground);
        Packer<int>.Write(packer, idCarta1);
        Packer<int>.Write(packer, idCarta2);
        Packer<int>.Write(packer, idCarta3);
        Packer<int>.Write(packer, idItemAtivavel);
        Packer<int>.Write(packer, idFlyer);
        Packer<int>.Write(packer, idSkin);
        Packer<int>.Write(packer, idEfeitoAbate);
    }

    public void Read(BitPacker packer)
    {
        Packer<string>.Read(packer, ref nickname);
        Packer<string>.Read(packer, ref authId);
        Packer<PlayerID>.Read(packer, ref playerID);
        Packer<Team>.Read(packer, ref team);
        Packer<int>.Read(packer, ref idKdaBackground);
        Packer<int>.Read(packer, ref idHudBackground);
        Packer<int>.Read(packer, ref idCarta1);
        Packer<int>.Read(packer, ref idCarta2);
        Packer<int>.Read(packer, ref idCarta3);
        Packer<int>.Read(packer, ref idItemAtivavel);
        Packer<int>.Read(packer, ref idFlyer);
        Packer<int>.Read(packer, ref idSkin);
        Packer<int>.Read(packer, ref idEfeitoAbate);
    }

    public void CopyFrom(Jogador other)
    {
        nickname = other.nickname;
        authId = other.authId;
        playerID = other.playerID;
        team = other.team;
        idKdaBackground = other.idKdaBackground;
        idHudBackground = other.idHudBackground;
        idCarta1 = other.idCarta1;
        idCarta2 = other.idCarta2;
        idCarta3 = other.idCarta3;
        idItemAtivavel = other.idItemAtivavel;
        idFlyer = other.idFlyer;
        idSkin = other.idSkin;
        idEfeitoAbate = other.idEfeitoAbate;
    }
}

