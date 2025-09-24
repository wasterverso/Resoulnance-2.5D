using UnityEngine;

[System.Serializable]
public class StatusAlvo
{
    public ulong idPlayer;
    public bool podeReceberDano;
    public bool podeSerAlvo;
    public Team team;
    public PlayerState playerState;
    public int vidaMax;
    public int vidaAtual;
    public int escudoMax;
    public int escudoAtual;
}
