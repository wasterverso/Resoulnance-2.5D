using UnityEngine;
using PurrNet.Prediction;
using UnityEngine.UI;

public class SlimeDeVida : PredictedIdentity<SlimeDeVida.State>
{
    [Header("Refs")]
    [SerializeField] PredictedRigidbody _rb;
    [SerializeField] GameObject skimAnim;
    [SerializeField] Image contadorBar;
    [SerializeField] GameObject canvasBar;

    [Header("Controle")]
    [SerializeField] bool ativarSlime = true;
    [SerializeField] int _tempoRenascer = 15;
    [SerializeField] int _valorCura = 100;

    protected override void LateAwake()
    {
        if (!ativarSlime)
        {
            gameObject.SetActive(false);
        }
    }

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
        return new State()
        {
            podePegarSlime = true,
            esperarRenascer = false,
            tempoParaRenascer = 0,
        };
    }

    protected override void Simulate(ref State state, float delta)
    {
        if (!ativarSlime)
            return;

        if (state.esperarRenascer)
        {
            if (state.tempoParaRenascer > 0f)
            {
                state.tempoParaRenascer -= delta;
            }
            else
            {                
                IniciarRespawn();
            }
        }
    }

    protected override void UpdateView(State viewState, State? verified)
    {
        if (viewState.esperarRenascer)
        {
            contadorBar.fillAmount = 1f - (viewState.tempoParaRenascer / _tempoRenascer);
        }
    }

    void OnUnityTriggerEnter(GameObject other)
    {
        if (!currentState.podePegarSlime) return;

        if (other.layer != 3) return;

        if (!other.TryGetComponent(out IReceberDano receberDanoScript)) return;

        StatusAlvo status = receberDanoScript.ReceberStatusAlvo();

        if (status.vidaAtual == status.vidaMax) return;

        receberDanoScript.CurarVidaOuEscudo(0, _valorCura);

        PegouSlime();
        //predictionManager.hierarchy.Delete(gameObject);
    }

    public void PegouSlime()
    {
        skimAnim.SetActive(false);

        canvasBar.SetActive(true);
        contadorBar.fillAmount = 1;

        currentState.tempoParaRenascer = _tempoRenascer;

        currentState.esperarRenascer = true;
        currentState.podePegarSlime = false;
    }

    public void IniciarRespawn()
    {
        skimAnim.SetActive(true);
        canvasBar.SetActive(false);

        currentState.esperarRenascer = false;
        currentState.podePegarSlime = true;
    }

    public struct State : IPredictedData<State>
    {
        public bool podePegarSlime;

        public bool esperarRenascer;
        public float tempoParaRenascer;

        public void Dispose() { }
    }
}
