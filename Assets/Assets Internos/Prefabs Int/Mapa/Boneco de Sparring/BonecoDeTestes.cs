using PurrNet;
using PurrNet.Prediction;
using Resoulnance.Scene_Arena;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class BonecoDeTestes : PredictedIdentity<BonecoDeTestes.State>, IReceberDano
{
    [Header("Refs")]
    PlayerHitView playerHitView;

    [Header("UI")]
    [SerializeField] Image _barraDeVida;
    [SerializeField] Image _barraDeEscudo;

    [Header("Config")]
    [SerializeField] Team _team;
    [SerializeField] PlayerState _playerState;
    [SerializeField] float _tempoRecuperarStatus;
    [SerializeField] int _valorRecuperar;
    [SerializeField] ulong _idPlayer;

    [Header("Status")]
    [SerializeField] int _vidaMax = 100;
    [SerializeField] int _escudoMax = 100;

    [SerializeField] float _tempoRecuperarEscudo = 4;
    [SerializeField] float _tempoRegenEscudo_AposDano = 6;

    bool podeReceberDano = true;
    bool podeSerAlvo = true;

    protected override void LateAwake()
    {
        playerHitView = ArenaReferences.Instance.playerHitView;
    }

    protected override State GetInitialState()
    {
        return new State()
        {
            vidaAtual = _vidaMax,
            escudoAtual = _escudoMax,
        };
    }

    public void ReceberDano(int damage, int tipo, ulong idRemetente)
    {
        //if (!isServer || !podeReceberDano) return;

        //idUltimoHit = idRemetente;

        var escudo = currentState.escudoAtual;
        var vida = currentState.vidaAtual;

        switch (tipo)
        {
            case 1: // Dano normal
                if (escudo > 0)
                {
                    if (damage >= escudo)
                    {
                        vida -= (damage - escudo); // Zerar o escudo e subtrair o restante do dano da vida
                        escudo = 0;
                    }
                    else
                    {
                        escudo -= damage;
                    }
                }
                else
                {
                    vida -= damage;
                }
                break;

            case 2: // Dano real
                vida -= damage;
                break;
        }

        if (vida <= 0)
        {
            vida = 0;

            //playerController.EstaVivo_Rpc(false);
        }

        currentState.delayRegenTimer = _tempoRegenEscudo_AposDano;

        currentState.escudoAtual = escudo;
        currentState.vidaAtual = vida;

        if (isServer)
            playerHitView.SpawnNotification(damage, tipo, transform.position);
    }


    protected override void Simulate(ref State state, float delta)
    {
        // Se tomou dano recentemente, conta o delay antes de regenerar
        if (state.delayRegenTimer > 0f)
        {
            state.delayRegenTimer -= delta;
            return; // ainda em cooldown, não regenera
        }

        if (state.escudoAtual < _escudoMax)
        {
            state.escudoRegenTimer += delta;

            if (state.escudoRegenTimer >= _tempoRecuperarEscudo)
            {
                state.escudoAtual += 20;
                state.vidaAtual += 20;
                state.escudoRegenTimer = 0f; // resetar timer
            }
        }
        else
        {
            state.escudoRegenTimer = 0f; // escudo cheio → timer não acumula
        }
    }

    protected override void UpdateView(State viewState, State? verified)
    {
        if (_barraDeVida && !Mathf.Approximately(_barraDeVida.fillAmount, (float)viewState.vidaAtual / _vidaMax))
        {
            _barraDeVida.fillAmount = (float)viewState.vidaAtual / _vidaMax;
        }

        if (_barraDeEscudo && !Mathf.Approximately(_barraDeEscudo.fillAmount, (float)viewState.escudoAtual / _escudoMax))
        {
            _barraDeEscudo.fillAmount = (float)viewState.escudoAtual / _escudoMax;
        }

        if (verified.HasValue && verified.Value.vidaAtual <= 0)
        {
            //Player esta morto
        }
    }


    public struct State : IPredictedData<State>
    {
        public int vidaAtual;
        public int escudoAtual;

        public float escudoRegenTimer; // controla regeneração a cada 4s
        public float delayRegenTimer;  // controla espera após levar dano

        public void Dispose() { }
    }

    public StatusAlvo ReceberStatusAlvo()
    {
        StatusAlvo status = new StatusAlvo();
        status.idPlayer = _idPlayer;
        status.podeReceberDano = podeReceberDano;
        status.podeSerAlvo = podeSerAlvo;
        status.team = _team;
        status.playerState = _playerState;
        status.vidaMax = _vidaMax;
        status.vidaAtual = currentState.vidaAtual;
        status.escudoMax = _escudoMax;
        status.escudoAtual = currentState.escudoAtual;
        return status;
    }

    public void CurarVidaOuEscudo(int valorEscudo, int valorVida)
    {
        throw new System.NotImplementedException();
    }
}
