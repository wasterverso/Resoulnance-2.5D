using PurrNet.Prediction;
using Resoulnance.Scene_Arena.Player;
using System.Collections;
using UnityEngine;

public class Fanbuu_Skill : PredictedIdentity<Fanbuu_Skill.State>, ISkill
{
    [SerializeField] SkillControlePrefab skillControle;
    [SerializeField] float tempoImune = 3;

    PlayerReferences playerReferences;
    SpriteRenderer spriteAnim;

    protected override State GetInitialState()
    {
        return new State()
        {
            iniciarContagem = false
        };
    }

    public void ExecutarSkill()
    {
        playerReferences = skillControle.playerReferences;

        if (playerReferences == null)
        {
            Debug.Log("PlayerReferences nao encontrado");
            return;
        }

        spriteAnim = playerReferences.playerAnimation.Get_SpriteAnim();
        Color cor = spriteAnim.color;
        cor.a = 0.5f;
        spriteAnim.color = cor;

        playerReferences.playerVida.Set_PodeReceberDano(false);

        currentState.tempoPassado = tempoImune;
        currentState.iniciarContagem = true;
    }

    protected override void Simulate(ref State state, float delta)
    {
        if (!state.iniciarContagem) return;

        if (state.tempoPassado > 0)
        {
            state.tempoPassado -= delta;
            if (state.tempoPassado <= 0)
            {
                VoltarReceberDano();
            }
        }
    }

    void VoltarReceberDano()
    {
        Color cor = spriteAnim.color;
        cor.a = 1;
        spriteAnim.color = cor;

        playerReferences.playerVida.Set_PodeReceberDano(true);
        hierarchy.Delete(this.gameObject);
    }

    public struct State : IPredictedData<State>
    {
        public bool iniciarContagem;
        public float tempoPassado;

        public void Dispose() { }
    }

    public void ExecutarSkillLocal() { }
}
