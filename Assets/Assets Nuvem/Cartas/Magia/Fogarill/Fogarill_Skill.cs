using PurrNet;
using PurrNet.Prediction;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class Fogarill_Skill : PredictedIdentity<Fogarill_Skill.State>, ISkill
{
    [Header("Referencias")]
    [SerializeField] PredictedRigidbody _rb;
    [SerializeField] SkillControlePrefab skillControle;
    [SerializeField] Transform _visualsTransform;

    [Header("Controles")]
    [SerializeField] float _velocidade = 5f;
    [SerializeField] float _tempoDestruir = 10;
    [SerializeField] int _damage = 25;

    Team meuTime;
    ulong meuId;

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
            tempoParaDestruir = _tempoDestruir
        };
    }

    public void ExecutarSkill()
    {
        currentState.direction = skillControle.direction;
        meuTime = skillControle.team;
        meuId = skillControle.playerId;
    }

    protected override void Simulate(ref State state, float delta)
    {
        if (state.direction != Vector2.zero)
        {
            Vector3 newDirection = new Vector3(state.direction.x, 0, state.direction.y);
            _rb.linearVelocity = newDirection * _velocidade;
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

    private void OnUnityTriggerEnter(GameObject other)
    {
        if (!other.TryGetComponent(out IReceberDano receberDanoScript)) return;

        StatusAlvo status = receberDanoScript.ReceberStatusAlvo();
        if (status.team != meuTime && status.podeReceberDano)
        {
            receberDanoScript.ReceberDano(_damage, 1, meuId);
            predictionManager.hierarchy.Delete(gameObject);
        }
    }

    public struct State : IPredictedData<State>
    {
        public Vector2 direction;
        public float tempoParaDestruir;

        public void Dispose() { }
    }
}
