using PurrNet.Prediction;
using UnityEngine;

namespace Resoulnance.Scene_Arena.Guardiao
{
    public class ProjetilGuardiao : PredictedIdentity<ProjetilGuardiao.State>
    {
        public struct State : IPredictedData<State>
        {
            public float tempoDestruir;

            public void Dispose() { }
        }

        [Header("Refs")]
        [SerializeField] PredictedRigidbody _rb;
        [SerializeField] Transform _visualsTransform;

        [Header("Configs")]
        [SerializeField] float _velocidade = 5f;
        [SerializeField] float tempoDestruir = 10;

        Team meuTime;
        GameObject alvo;

        private void OnEnable()
        {
            _rb.onTriggerEnter += OnUnityTriggerEnter;
        }

        private void OnDisable()
        {
            _rb.onTriggerEnter -= OnUnityTriggerEnter;
        }

        public void SetInfoConfig(GameObject alvoObj, Team team)
        {
            alvo = alvoObj;
            meuTime = team;
        }

        protected override State GetInitialState()
        {
            return new State
            {
                tempoDestruir = tempoDestruir
            };
        }

        protected override void Simulate(ref State state, float delta)
        {
            if (alvo != null)
            {
                Vector3 direcao = (alvo.transform.position - transform.position).normalized;

                _rb.linearVelocity = direcao * _velocidade;
            }

            state.tempoDestruir -= delta;
            if (state.tempoDestruir <= 0)
                predictionManager.hierarchy.Delete(gameObject);
        }

        void OnUnityTriggerEnter(GameObject other)
        {
            if (other != alvo) return;

            other.TryGetComponent(out IReceberDano receberDanoScript);

            if (receberDanoScript != null)
            {
                StatusAlvo status = receberDanoScript.ReceberStatusAlvo();
                if (status.team != meuTime && status.podeReceberDano)
                {
                    receberDanoScript.ReceberDano(15, 1, 0);
                    predictionManager.hierarchy.Delete(gameObject);
                }
            }
        }
    }
}

