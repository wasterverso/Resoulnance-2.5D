using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beela_Skill : MonoBehaviour
{
    /*
    [Header("Referencias")]
    //[SerializeField] NetworkObject netObj;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;
    [SerializeField] SkillControlePrefab skillControle;

    [Header("Controles")]
    [SerializeField] int idCarta = 4;
    public float speed = 5f;

    private Transform target;

    void Update()
    {
        if (target != null && IsServer)
        {
            // Mover em direção ao alvo
            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * speed;

            // Flip na direção do movimento
            if (direction.x > 0)
            {
                sprite.flipX = false;
            }
            else if (direction.x < 0)
            {
                sprite.flipX = true;
            }
        }
    }

    public void ExecutarSkill()
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue((ulong)skillControle.idTargetAlvo, out var objetoAssociado))
        {
            target = objetoAssociado.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer)
        {
            if (other.transform == target)
            {
                rb.velocity = Vector2.zero;

                IReceberDano playerReceber = other.GetComponent<IReceberDano>();
                playerReceber.ReceberDano((int)skillControle.valorAtributo_1, 1, skillControle.idPlayer);

                other.GetComponent<HitsController>().InstanciarHitsCartas_Rpc(idCarta, new Vector3(0,0,0));

                if (other.GetComponent<StatusPlayer>() != null)
                {
                    other.GetComponent<StatusPlayer>().ReceberStun_Rpc(skillControle.valorAtributo_2);
                }

                Destroy(gameObject);
            }
        }
    }

    public void ExecutarSkillLocal()
    {

    }
    */
}
