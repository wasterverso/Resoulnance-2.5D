using PurrNet.Prediction;
using Resoulnance.Scene_Arena.Player;
using System;
using UnityEngine;

public class Luna_Projetil : PredictedIdentity<Luna_Projetil.State>
{
    public struct State : IPredictedData<State>
    {
        public Vector2 direction;
        public float tempoParaDestruir;

        public bool temAlvo;
        public PredictedObjectID alvoId;

        public void Dispose() { }
    }

    [Header("Refs")]
    [SerializeField] PredictedRigidbody _rb;
    [SerializeField] Transform _visualsTransform;

    [Header("Configs")]
    [SerializeField] float _velocidade = 5f;
    [SerializeField] float _tempoDestruir = 2f;

    Team meuTime;
    ulong meuId;
    GameObject alvoObj;

    private void OnEnable()
    {
        _rb.onTriggerEnter += OnUnityTriggerEnter;
    }

    private void OnDisable()
    {
        _rb.onTriggerEnter -= OnUnityTriggerEnter;
    }

    protected override State GetInitialState()
    {
        return new State
        {
            tempoParaDestruir = _tempoDestruir,
        };
    }

    public void SetInfoConfig(Vector2 direction, PredictedObjectID alvoId, Team team, ulong playerId)
    {
        meuTime = team;
        meuId = playerId;

        currentState.temAlvo = (direction == Vector2.zero);

        if (currentState.temAlvo)
        {
            currentState.alvoId = alvoId;
            currentState.tempoParaDestruir = 5;
        }
        else
        {
            currentState.direction = direction;
        }
    }

    protected override void Simulate(ref State state, float delta)
    {
        if (!state.temAlvo)
        {
            Vector3 newDirection = new Vector3(state.direction.x, 0, state.direction.y);
            _rb.linearVelocity = newDirection * _velocidade;
        }
        else
        {
            if (alvoObj == null)
            {
                alvoObj = predictionManager.hierarchy.GetGameObject(state.alvoId);
            }

            if (alvoObj != null)
            {
                Vector3 dir3 = (alvoObj.transform.position - transform.position);
                dir3.y = 0; // mantém o Y fixo
                dir3.Normalize();

                state.direction = new Vector2(dir3.x, dir3.z);

                Vector3 newDirection = new Vector3(state.direction.x, 0, state.direction.y);
                _rb.linearVelocity = newDirection * _velocidade;
            }
        }

        state.tempoParaDestruir -= delta;

        if (state.tempoParaDestruir < 0)
        {
            predictionManager.hierarchy.Delete(gameObject);
        }
    }

    protected override void UpdateView(State viewState, State? verified)
    {
        if (viewState.direction != Vector2.zero)
        {
            float angulo = Mathf.Atan2(viewState.direction.x, -viewState.direction.y) * Mathf.Rad2Deg;
            Quaternion rotacao = Quaternion.Euler(45f, 0f, angulo);
            _visualsTransform.localRotation = rotacao;
        }
    }

    void OnUnityTriggerEnter(GameObject other)
    {
        other.TryGetComponent(out IGuardiao guardiaoScript);
        other.TryGetComponent(out IReceberDano receberDanoScript);

        if (receberDanoScript != null)
        {
            StatusAlvo status = receberDanoScript.ReceberStatusAlvo();
            if (status.team != meuTime && status.podeReceberDano)
            {
                receberDanoScript.ReceberDano(10, 1, meuId);
                predictionManager.hierarchy.Delete(gameObject);
            }
        }

        if (guardiaoScript != null)
        {
            Team teamGuardiao = guardiaoScript.TeamGuardiao();
            if (teamGuardiao != meuTime)
            {
                guardiaoScript.ReceberDano(1);
                predictionManager.hierarchy.Delete(gameObject);
            }
        }
    }
}
