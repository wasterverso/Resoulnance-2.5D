using PurrNet.Prediction;
using Resoulnance.Scene_Arena.Player;
using UnityEngine;

public class Brabuleta_Skill : StatelessPredictedIdentity, ISkill
{
    [SerializeField] SkillControlePrefab skillControle;

    public void ExecutarSkill()
    {
        PlayerVida vidaScript = skillControle.playerReferences.playerVida;

        vidaScript.CurarVidaOuEscudo((int)skillControle.valorAtributo_1, (int)skillControle.valorAtributo_2);

        predictionManager.hierarchy.Delete(gameObject);
    }
}
