using PurrNet.Prediction;
using Resoulnance.Scene_Arena;
using Resoulnance.Scene_Arena.Player;
using Resoulnance.Scene_Arena.UI;
using UnityEngine;

public class PlayerDash : PredictedIdentity<PlayerDash.State>
{
    [Header("Refs Scripts")]
    [SerializeField] PlayerController playerController;

    [Header("Controles")]
    [SerializeField] float tempoRecarga = 4f;
    private int maxQuantidadeCargas = 6;

    CargasDashUI dashUI;

    protected override void LateAwake()
    {
        dashUI = ArenaReferences.Instance.dashUI;
    }

    protected override State GetInitialState()
    {
        return new State()
        {
            cargasAtual = maxQuantidadeCargas
        };
    }

    protected override void Simulate(ref State state, float delta)
    {
        if (state.cargasAtual < maxQuantidadeCargas)
        {
            state.tempoCarregamentoAtual += delta;

            if (state.tempoCarregamentoAtual >= tempoRecarga)
            {
                state.cargasAtual++;
                state.tempoCarregamentoAtual = 0f; // reinicia a contagem

                if (isOwner)
                {
                    dashUI.RecuperarCarga(state.cargasAtual);
                }
            }
        }
    }

    public void ChamouDash()
    {
        currentState.cargasAtual--;

        if (currentState.cargasAtual < 0)
        {
            currentState.cargasAtual = 0;
        }

        playerController.SetPlayerState(PlayerState.Dash);

        if (isOwner)
        {
            dashUI.ChamarDash(currentState.cargasAtual);
        }
    }

    public struct State : IPredictedData<State>
    {
        public int cargasAtual;

        public float tempoCarregamentoAtual;

        public void Dispose() { }
    }
}
