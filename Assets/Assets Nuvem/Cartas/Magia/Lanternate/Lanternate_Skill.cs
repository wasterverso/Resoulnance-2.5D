using PurrNet.Prediction;
using Resoulnance.Scene_Arena.Player;
using UnityEngine;

public class Lanternate_Skill : PredictedIdentity<Lanternate_Skill.State>, ISkill
{
    public struct State : IPredictedData<State>
    {
        public Vector2 direction;
        public float tempoParaDestruir;
        public float tempoDanoContinuo;

        public void Dispose() { }
    }

    [Header("Referencias")]
    [SerializeField] PredictedRigidbody _rb;
    [SerializeField] SkillControlePrefab skillControle;
    [SerializeField] Transform _visualsTransform;

    [Header("Controles")]
    [SerializeField] float _velocidade = 5f;
    [SerializeField] float _tempoDestruir = 10;
    [SerializeField] float tempoContinuo = 1f;

    [Header("Gizmos")]
    [SerializeField] bool mostrarGizmo = false;
    [SerializeField] float range;

    Team meuTime;
    ulong meuId;

    protected override State GetInitialState()
    {
        return new State
        {
            tempoParaDestruir = _tempoDestruir,
            tempoDanoContinuo = tempoContinuo,
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

        state.tempoDanoContinuo -= delta;
        if (state.tempoDanoContinuo <= 0)
        {
            state.tempoDanoContinuo = tempoContinuo;
            VerificarInRange();
        }

        state.tempoParaDestruir -= delta;
        if (state.tempoParaDestruir < 0)
        {
            predictionManager.hierarchy.Delete(gameObject);
        }
    }

    void VerificarInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        if (hits.Length == 0) return;

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IReceberDano receberDanoScript)) continue;

            StatusAlvo status = receberDanoScript.ReceberStatusAlvo();
            if (status.team != meuTime && status.podeReceberDano)
            {
                receberDanoScript.ReceberDano((int)skillControle.valorAtributo_1, 1, meuId);
            }

            if (!hit.TryGetComponent(out PlayerMovement playerMovement)) continue;

            playerMovement.MudarVelocidadeTemporariamente(skillControle.valorAtributo_2, 2);
        }
    }

    private void OnDrawGizmos()
    {
        if (mostrarGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
