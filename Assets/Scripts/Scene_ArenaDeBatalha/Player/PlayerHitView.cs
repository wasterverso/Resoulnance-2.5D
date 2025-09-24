using PurrNet;
using UnityEngine;

public class PlayerHitView : MonoBehaviour
{
    public void SpawnNotification(int dano, int tipo, Vector3 myPosition)
    {
        Vector3 posicaoAleatoria = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.4f, 0.8f), 0);
        GameObject bala = PoolManager.Instance.Spawn("Damage", myPosition + posicaoAleatoria, Quaternion.identity);
        DamageNotification scriptDamage = bala.GetComponent<DamageNotification>();
        scriptDamage.StartarScript(dano, tipo);
    }
}

public struct HitEventData
{
    public int dano;
    public int tipo;
    public Vector3 position;

    public HitEventData(int dano, int tipo, Vector3 position)
    {
        this.dano = dano;
        this.tipo = tipo;
        this.position = position;
    }
}