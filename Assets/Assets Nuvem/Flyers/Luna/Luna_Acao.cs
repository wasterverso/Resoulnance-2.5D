using PurrNet.Prediction;
using Resoulnance.Scene_Arena;
using Resoulnance.Scene_Arena.Player;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Luna_Acao : PredictedIdentity<Luna_Acao.State>, IAttack
{
    public struct State : IPredictedData<State>
    {
        public bool ativouSuprema;
        public float tempoSuprema;
        public bool estaNaSuprema;

        public void Dispose() { }
    }

    [Header("Referencias")]
    [SerializeField] PlayerReferences playerReferences;

    [Header("Config")]
    [SerializeField] float _tempoSuprema = 5;
    [SerializeField] float _mudancaVelocidade = 1.5f;

    [Header("Projetil")]
    [SerializeField] PredictedRigidbody _projetilPrefab;
    [SerializeField] Transform _projetilSpawnTransform;

    [Header("Suprema")]
    [SerializeField] GameObject hudPlyer_obj;
    [SerializeField] SpriteRenderer spriteAnim_Renderer;
    [SerializeField] SpriteRenderer armaAnim_Renderer;
    [SerializeField] Material spriteLit_mat;
    [SerializeField] Material spriteSombra_mat;
    [SerializeField] GameObject supremaAnim_Obj;

    int attackCount = 0;

    protected override State GetInitialState()
    {
        return new State()
        {
            ativouSuprema = false,
            tempoSuprema = 0,
            estaNaSuprema = false,
        };
    }

    public void ExecutarAttack(bool arrastou, Vector2 angle, GameObject target)
    {
        PredictedObjectID alvoIdObj = default;

        if (!arrastou)
        {
            angle = Vector2.zero;

            if (predictionManager.hierarchy.TryGetId(target, out PredictedObjectID alvoId))
            {
                alvoIdObj = alvoId;
            }
        }

        var createObject = predictionManager.hierarchy.Create(_projetilPrefab.gameObject, _projetilSpawnTransform.position, Quaternion.identity);
        if (!createObject.HasValue) return;

        if (!createObject.Value.TryGetComponent(predictionManager, out Luna_Projetil lunaProjetil)) return;

        Team meuTime = playerReferences.currentState.team;
        
        lunaProjetil.SetInfoConfig(angle, alvoIdObj, meuTime, owner.Value.id.value);

        //Ativar Passiva
        attackCount++;

        if (attackCount == 3)
        {
            attackCount = 0;
            playerReferences.playerMovement.MudarVelocidadeTemporariamente(5, 0.2f);
        }
    }

    public void ExecutarSuprema(bool arrastar, Vector2 angulo)
    {
        currentState.ativouSuprema = true;
    }

    public void PlayerAtacou()
    {
        if(currentState.estaNaSuprema)
        {
            currentState.tempoSuprema = 0;
        }    
    }

    public void PlayerUsouSkill()
    {
        if (currentState.estaNaSuprema)
        {
            currentState.tempoSuprema = 0;
        }
    }

    protected override void Simulate(ref State state, float delta)
    {
        if (state.ativouSuprema)
        {
            state.ativouSuprema = false;
            state.estaNaSuprema = true;
            state.tempoSuprema = _tempoSuprema;
            SupremaOn(true);
            playerReferences.playerMovement.MudarVelocidadeTemporariamente(_mudancaVelocidade, _tempoSuprema);
        }

        if (state.estaNaSuprema)
        {
            state.tempoSuprema -= delta;
            if (state.tempoSuprema <= 0)
            {
                state.estaNaSuprema = false;
                SupremaOn(false);
            }
        }
    }

    void SupremaOn(bool ativa)
    {
        Team timeAtual = ArenaReferences.Instance.team;

        Color cor = new Color(1f, 1f, 1f, 1f);

        if (ativa)
        {
            supremaAnim_Obj.SetActive(true);

            spriteAnim_Renderer.material = spriteLit_mat;
            if (timeAtual == playerReferences.currentState.team)
            {
                cor.a = 0.75f;
                spriteAnim_Renderer.color = cor;
                armaAnim_Renderer.color = cor;
            }
            else
            {
                hudPlyer_obj.SetActive(false);
                cor.a = 0f;
                spriteAnim_Renderer.color = cor;
                armaAnim_Renderer.color = cor;
            }            
        }
        else
        {
            spriteAnim_Renderer.material = spriteSombra_mat;
            hudPlyer_obj.SetActive(true);
            cor.a = 1f;
            spriteAnim_Renderer.color = cor;
            armaAnim_Renderer.color = cor;
        }
    }
}
