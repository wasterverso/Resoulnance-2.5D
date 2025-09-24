using PurrNet;
using Resoulnance.Scene_Arena.Player;
using UnityEngine;

public class SkillControlePrefab : NetworkBehaviour
{
    [Header("Referencias Internas")]    
    ISkill skillScript;

    [Header("Referencias Publicas")]
    public PlayerReferences playerReferences {  get; private set; }
    public float valorAtributo_1 = 0;
    public float valorAtributo_2 = 0;
    public Vector2 direction;
    public Team team;
    public ulong playerId;

    public void SetInfoSkill(PlayerReferences playerRefs, float valor1, float valor2, 
        Vector2 direcao = default, Team meuTime = Team.Nenhum, ulong idPlayer = 0)
    {
        skillScript = GetComponent<ISkill>();   

        playerReferences = playerRefs;
        valorAtributo_1 = valor1;
        valorAtributo_2 = valor2;
        direction = direcao;
        team = meuTime;
        playerId = idPlayer;

        if (skillScript != null)
        {
            skillScript.ExecutarSkill();
        }
        else
        {
            Debug.LogError($"[SkillControle: {gameObject.name}] Iskill nao encontrado");
        }
    }
}
