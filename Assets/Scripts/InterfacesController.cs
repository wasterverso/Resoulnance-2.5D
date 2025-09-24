using Resoulnance.Scene_Arena.Player;
using UnityEngine;

public interface IReceberDano
{
    public void ReceberDano(int damage, int tipo, ulong idRemetente);

    public void CurarVidaOuEscudo(int valorEscudo, int valorVida);

    public StatusAlvo ReceberStatusAlvo();
}

public interface IAttack
{
    void ExecutarAttack(bool arrastou, Vector2 angle, GameObject alvo);

    void ExecutarSuprema(bool arrastar, Vector2 angulo);

    public void PlayerAtacou();

    public void PlayerUsouSkill();
}

public interface ISkill
{
    void ExecutarSkill();
}

public interface IGuardiao
{
    public void ReceberDano(int dano);
    public Team TeamGuardiao();
}