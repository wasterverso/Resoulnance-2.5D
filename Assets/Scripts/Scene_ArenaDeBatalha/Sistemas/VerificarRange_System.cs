using UnityEngine;

public static class VerificarRange_System
{
    public static GameObject GetAlvoAutomatico(ProcurarAlvo tipoAlvo, Transform posicao, float range, Team timeAtual)
    {
        Collider[] hitEnemies = Physics.OverlapSphere(posicao.position, range);

        Collider targetEnemy = null;
        float minDistance = float.MaxValue;
        float minVida = float.MaxValue;
        float minEscudo = float.MaxValue;

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.gameObject.TryGetComponent<IReceberDano>(out IReceberDano receberDanoScript))
            {
                StatusAlvo status = receberDanoScript.ReceberStatusAlvo();

                if (status.team != timeAtual && status.playerState != PlayerState.Dead && status.podeSerAlvo)
                {
                    float distance = Vector3.Distance(posicao.position, enemy.transform.position);

                    float vida = status.vidaAtual;
                    float escudo = status.escudoAtual;

                    switch (tipoAlvo)
                    {
                        case ProcurarAlvo.MaisPerto:
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                targetEnemy = enemy;
                            }
                            break;
                        case ProcurarAlvo.MenorVida:
                            if (vida < minVida)
                            {
                                minVida = vida;
                                targetEnemy = enemy;
                            }
                            break;
                        case ProcurarAlvo.MenorEscudo:
                            if (escudo < minEscudo)
                            {
                                minEscudo = escudo;
                                targetEnemy = enemy;
                            }
                            break;
                    }
                }
            }
        }

        return targetEnemy != null ? targetEnemy.gameObject : null;
    }

    public static GameObject GetAlvoGuardiao(Transform posicao, float range, Team timeAtual)
    {
        Collider[] hitEnemies = Physics.OverlapSphere(posicao.position, range);

        Collider targetEnemy = null;

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.gameObject.TryGetComponent<IGuardiao>(out IGuardiao guardiaoScript))
            {
                Team team = guardiaoScript.TeamGuardiao();
                if (team != timeAtual)
                {
                    targetEnemy = enemy;
                    break;
                }
            }
        }

        return targetEnemy != null ? targetEnemy.gameObject : null;
    }
}
