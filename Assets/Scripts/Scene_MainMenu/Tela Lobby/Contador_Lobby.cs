using System;
using UnityEngine;
using UnityEngine.UI;

public class Contador_Lobby : MonoBehaviour
{
    [SerializeField] private Text relogioTxt;
    [SerializeField] GameObject contador_painel;

    private float tempo = 0f;
    private bool rodando = false;

    void Update()
    {
        if (!rodando) return;

        tempo += Time.deltaTime;

        TimeSpan t = TimeSpan.FromSeconds(tempo);
        relogioTxt.text = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
    }

    public void Iniciar()
    {
        tempo = 0f;
        rodando = true;
        contador_painel.SetActive(true);
    }

    public void Parar()
    {
        rodando = false;
        contador_painel.SetActive(false);
    }

    public void Continuar()
    {
        rodando = true;
    }

    public void Resetar()
    {
        tempo = 0f;
        relogioTxt.text = "00:00";
    }
}
