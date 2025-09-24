using PurrNet;
using Resoulnance.Scene_Arena.Player;
using System;
using UnityEngine;

public class ProjetilFollowTarget : NetworkBehaviour
{
    [Header("Config")]
    [SerializeField] int dano = 10;
    [SerializeField] float velocidade = 5f;
    [SerializeField] float tempoDestruir = 0.5f;

    [SerializeField] Transform AnimTrans;

    bool iniciou = false;
    Transform target;
    ulong idTarget = default;
    bool perseguir = false;
    Team team;
    Vector3 direcao;
    float tempoRestante;

    Vector3 alvoXZ = Vector3.zero;
    Quaternion rotacao = Quaternion.identity;

    public void AtribuirDados(GameObject novoTarget, ulong idPlayerTarget, Team teamProjetil, float angle)
    {
        idTarget = idPlayerTarget;
        team = teamProjetil;

        if (novoTarget != null)
        {
            perseguir = true;
            target = novoTarget.transform;
        }
        else
        {
            direcao = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
            tempoRestante = tempoDestruir;
        }

        iniciou = true;
    }

    void Update()
    {
        if (!iniciou) return;

        if (target != null && perseguir)
        {
            alvoXZ.Set(target.position.x, transform.position.y, target.position.z);
            direcao = (alvoXZ - transform.position);
            float sqrMag = direcao.sqrMagnitude;

            if (sqrMag > 0.0001f)
            {
                direcao.Normalize();
                transform.position += direcao * velocidade * Time.deltaTime;

                // cálculo manual do ângulo (mesmo do else)
                float angulo = Mathf.Atan2(-direcao.x, direcao.z) * Mathf.Rad2Deg;
                rotacao = Quaternion.Euler(45f, 0f, angulo + 90f);
                AnimTrans.rotation = rotacao;
            }
        }
        else if (!perseguir && direcao.sqrMagnitude > 0.0001f)
        {
            transform.position += direcao * velocidade * Time.deltaTime;

            float angulo = Mathf.Atan2(-direcao.x, direcao.z) * Mathf.Rad2Deg;
            rotacao = Quaternion.Euler(45f, 0f, angulo + 90f);
            AnimTrans.rotation = rotacao;

            tempoRestante -= Time.deltaTime;
            if (tempoRestante <= 0f)
                Destruir();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!iniciou) return;

        if (other.TryGetComponent<IReceberDano>(out IReceberDano receberDano))
        {
            var status = receberDano.ReceberStatusAlvo();
            if (status == null) return;

            bool acertouAlvoPerseguido = perseguir && status.idPlayer == idTarget;

            bool acertouInimigo = !perseguir && status.team != team;

            if (acertouAlvoPerseguido || acertouInimigo)
            {
                receberDano.ReceberDano(dano, 1, 500);
                Destruir();
            }
        }
    }

    void Destruir()
    {
        Destroy(this.gameObject);
    }
}
