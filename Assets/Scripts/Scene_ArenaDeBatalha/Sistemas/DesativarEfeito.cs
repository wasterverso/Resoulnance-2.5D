using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesativarEfeito : MonoBehaviour
{
    [Header("Desativar")]
    [SerializeField] bool desativar = false;
    [SerializeField] float tempoDesativar;

    [Header("Destruir")]
    [SerializeField] bool destruir = false;
    [SerializeField] float tempoDestruir;

    private void OnEnable()
    {
        if (desativar)
        {
            Invoke("Desativar", tempoDesativar);
        }
        if (destruir)
        {
            Invoke("Destruir", tempoDestruir);
        }
    }

    void Desativar()
    {
        this.gameObject.SetActive(false);
    }

    void Destruir()
    {
        Destroy(this.gameObject);
    }
}
