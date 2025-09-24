using PurrNet;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlacarController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] ListCristaisController cristaisController;

    [Header("Barras")]
    [SerializeField] Image blueBar;
    [SerializeField] Text placarBlue_txt;
    [SerializeField] Image redBar;
    [SerializeField] Text placarRed_txt;

    private int _cristaisBlue = 0;
    private int _cristaisRed = 0;

    private void Awake()
    {
        blueBar.fillAmount = 0;
        redBar.fillAmount = 0;
    }

    private void OnEnable()
    {
        cristaisController.OnListChanged += AtualizarContagem;
    }

    void AtualizarContagem(CristalInfo change)
    {
        int totalCristaisBlue = 0;
        int totalCristaisRed = 0;

        foreach (var cristal in cristaisController.currentState.cristaisList)
        {
            switch (cristal.team)
            {
                case Team.Blue:
                    totalCristaisBlue++;
                    break;
                case Team.Red:
                    totalCristaisRed++;
                    break;
            }
        }

        if (totalCristaisBlue != _cristaisBlue)
        {
            AtualizarAzul(totalCristaisBlue);
        }

        if (totalCristaisRed != _cristaisRed)
        {
            AtualizarVermelho(totalCristaisRed);
        }
    }

    void AtualizarAzul(int totalCristaisBlue)
    {
        placarBlue_txt.text = $"{totalCristaisBlue}";
        blueBar.fillAmount = Mathf.Clamp01((float)totalCristaisBlue / 32f);
        _cristaisBlue = totalCristaisBlue;
    }

    void AtualizarVermelho(int totalCristaisRed)
    {
        placarRed_txt.text = $"{totalCristaisRed}";
        redBar.fillAmount = Mathf.Clamp01((float)totalCristaisRed / 32f);
        _cristaisRed = totalCristaisRed;
    }

    private void OnDisable()
    {
        cristaisController.OnListChanged -= AtualizarContagem;
    }
}
