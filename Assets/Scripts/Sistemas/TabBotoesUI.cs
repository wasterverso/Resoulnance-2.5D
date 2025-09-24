using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TabUI
{
    public Button botao;
    public GameObject painel;
}

public class TabBotoesUI : MonoBehaviour
{
    [Header("Opcoes")]
    [SerializeField] bool resetarAoAtivar = false;

    [Header("Botoes")]
    public List<TabUI> botoesList = new List<TabUI>();

    [Header("Cor Texto")]
    [SerializeField] Color colorSelecionada = Color.black;
    [SerializeField] Color colorpadrao = Color.black;

    [Header("Sprite Botao")]
    [SerializeField] Sprite padrao_img;
    [SerializeField] Sprite selecionado_img;

    void Awake()
    {
        foreach (TabUI btn in botoesList)
        {
            btn.botao.onClick.AddListener(() => OnButtonClick(btn.botao));
            OnButtonClick(botoesList[0].botao);
        }
    }

    private void OnEnable()
    {
        if (resetarAoAtivar)
        {
            OnButtonClick(botoesList[0].botao);
        }
    }

    public void OnButtonClick(Button clickedButton)
    {
        foreach (TabUI botao in botoesList)
        {
            var image = botao.botao.GetComponent<Image>();
            var texto = botao.botao.GetComponentInChildren<Text>();

            if (botao.botao == clickedButton)
            {
                if (botao.painel != null) botao.painel.SetActive(true);

                image.sprite = selecionado_img;
                texto.color = colorSelecionada;
            }
            else
            {
                if (botao.painel != null) botao.painel.SetActive(false);

                image.sprite = padrao_img;
                texto.color = colorpadrao;
            }
        }
    }

}
