using PurrNet;
using PurrNet.Prediction;
using System.Collections;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Arena.Player
{
    public class PlayerVida : PredictedIdentity<PlayerVida.State>, IReceberDano
    {
        [Header("Ref Scripts")]
        [SerializeField] PlayerReferences playerReferences;
        PlayerController playerController;

        [Header("UI")]
        [SerializeField] Image _barraDeVida;
        [SerializeField] Image _barraDeVidaFundo;
        [SerializeField] Image _barraDeEscudo;

        [Header("Vida")]
        [SerializeField] int _vidaMax = 100;

        [Header("Barras de Vida")]
        [SerializeField] Sprite _aliadoLifeBar;
        [SerializeField] Sprite _adversarioLifeBar;

        [Header("Escudo")]
        [SerializeField] int _escudoMax = 100;
        [SerializeField] float _tempoRecuperarEscudo = 4;
        [SerializeField] float _tempoRegenEscudo_AposDano = 6;

        public PredictedEvent<HitEventData> OnReceivedDamage;

        private void Awake()
        {
            playerController = playerReferences.playerController;
        }

        protected override void LateAwake()
        {
            OnReceivedDamage = new PredictedEvent<HitEventData>(predictionManager, this);
            OnReceivedDamage.AddListener(data =>
            {
                ChamaParticulasVisuais(data.dano, data.tipo, data.position);
            });
        }

        protected override State GetInitialState()
        {
            return new State()
            {
                vidaAtual = _vidaMax,
                escudoAtual = _escudoMax,
                podeReceberDano = true,
                podeSerAlvo = true,
            };
        }

        public void ReceberDano(int damage, int tipo, ulong idRemetente)
        {
            if (!currentState.podeSerAlvo || !currentState.podeReceberDano || currentState.vidaAtual == 0) return;

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

                PlayerMorreu();
            }

            currentState.delayRegenTimer = _tempoRegenEscudo_AposDano;

            currentState.escudoAtual = escudo;
            currentState.vidaAtual = vida;

            var hitData = new HitEventData(damage, tipo, transform.position);
            OnReceivedDamage.Invoke(hitData);
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
                    state.escudoAtual += 10;
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
                StartCoroutine(FundoBarraDeVida());
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

        void PlayerMorreu()
        {
            playerController.SetPlayerState(PlayerState.Dead);
            playerController.EstaVivo(false);

            currentState.podeSerAlvo = false;
            currentState.podeReceberDano = false;
        }

        public void Ressucitou()
        {
            currentState.vidaAtual = _vidaMax;
            currentState.escudoAtual = _escudoMax;

            currentState.podeSerAlvo = true;
            currentState.podeReceberDano = true;
        }

        public void Set_PodeSerAlvo(bool pode)
        {
            currentState.podeSerAlvo = pode;
        }

        public void Set_PodeReceberDano(bool pode)
        {
            currentState.podeReceberDano = pode;
        }

        void ChamaParticulasVisuais(int dano, int tipo, Vector3 newposition)
        {
            ArenaReferences.Instance.playerHitView.SpawnNotification(dano, tipo, newposition);
        }

        public struct State : IPredictedData<State>
        {
            public int escudoAtual;
            public int vidaAtual;

            public float escudoRegenTimer; // controla regeneração a cada 4s
            public float delayRegenTimer;  // controla espera após levar dano

            public bool podeReceberDano;
            public bool podeSerAlvo;

            public void Dispose() { }
        }

        public StatusAlvo ReceberStatusAlvo()
        {
            StatusAlvo status = new StatusAlvo();
            status.idPlayer = playerReferences.currentState.playerId;
            status.podeReceberDano = currentState.podeReceberDano;
            status.podeSerAlvo = currentState.podeSerAlvo;
            status.team = playerReferences.currentState.team;
            status.playerState = playerController.currentState.playerState;
            status.vidaMax = _vidaMax;
            status.vidaAtual = currentState.vidaAtual;
            status.escudoMax = _escudoMax;
            status.escudoAtual = currentState.escudoAtual;
            return status;
        }

        IEnumerator FundoBarraDeVida()
        {
            float duracao = 0.5f;
            float tempoPassado = 0f;
            float valorInicial = _barraDeVidaFundo.fillAmount;
            float valorFinal = _barraDeVida.fillAmount;

            while (tempoPassado < duracao)
            {
                tempoPassado += Time.deltaTime;
                _barraDeVidaFundo.fillAmount = Mathf.Lerp(valorInicial, valorFinal, tempoPassado / duracao);
                yield return null;
            }

            _barraDeVidaFundo.fillAmount = valorFinal;
        }

        public void CurarVidaOuEscudo(int valorEscudo, int valorVida)
        {
            if (valorEscudo > 0)
            {
                currentState.escudoAtual += valorEscudo;
                if (currentState.escudoAtual > _escudoMax)
                {
                    currentState.escudoAtual = _escudoMax;
                }

                var hitData = new HitEventData(valorEscudo, 5, transform.position);
                OnReceivedDamage.Invoke(hitData);
            }
            if (valorVida > 0)
            {
                currentState.vidaAtual += valorVida;
                if (currentState.vidaAtual > _vidaMax)
                {
                    currentState.vidaAtual = _vidaMax;
                }

                var hitData = new HitEventData(valorVida, 4, transform.position);
                OnReceivedDamage.Invoke(hitData);
            }
        }

        public void AtualizarTipoBarraDeVida(Team meuTeam)
        {
            if (isOwner) return;

            Team time = ArenaReferences.Instance.team;
            if (time == meuTeam)
            {
                _barraDeVida.sprite = _aliadoLifeBar;
            }

            if (time != meuTeam)
            {
                _barraDeVida.sprite = _adversarioLifeBar;
            }
        }
    }
}

