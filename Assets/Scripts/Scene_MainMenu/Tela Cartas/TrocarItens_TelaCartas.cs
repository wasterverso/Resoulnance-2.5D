using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaCartas
{
    public class TrocarItens_TelaCartas : MonoBehaviour
    {
        [SerializeField] DeckPainel_TelaCartas deckTelaCartas;
        [SerializeField] StartTelaCartas startTelaCartas;
        [SerializeField] GameObject itensPainel;
        [SerializeField] Transform gridInstance;
        [SerializeField] GameObject itemPrefab;
        [SerializeField] Sprite itemFundoSelecionado;
        [SerializeField] Sprite itemFundoPadrao;
        [SerializeField] Text descricaoItem;

        ItemAtivavel itemSelecioinado;
        List<GameObject> buttonsItens = new List<GameObject>();

        public void AbrirPainelItens(ItemAtivavel itemRecebido)
        {
            itemSelecioinado = itemRecebido;

            itensPainel.SetActive(true);
            var listItens = ListaDecks.Instance.itensData.itensAtivaveis;

            buttonsItens.Clear();
            foreach (Transform it in gridInstance)
            {
                Destroy(it.gameObject);
            }

            GameObject btnEscolhido = null;
            foreach (var item in listItens)
            {
                GameObject itemObj = Instantiate(itemPrefab, gridInstance);
                buttonsItens.Add(itemObj);

                Button itemButton = itemObj.GetComponent<Button>();
                itemButton.onClick.AddListener(() => SelecionarItem(item, itemObj));

                itemObj.GetComponentInChildren<Text>().text = item.nomeItem;

                Image[] images = itemObj.GetComponentsInChildren<Image>();
                if (images.Length > 1)
                {
                    images[1].sprite = item.iconSprite;
                }

                if (item == itemRecebido)
                {
                    btnEscolhido = itemObj;
                }
            }

            SelecionarItem(itemRecebido, btnEscolhido);
        }

        public void SelecionarItem(ItemAtivavel item, GameObject botaoSelecionado)
        {
            itemSelecioinado = item;

            foreach (GameObject botoes in buttonsItens)
            {
                if (botoes == botaoSelecionado)
                {
                    botoes.GetComponentInChildren<Image>().sprite = itemFundoSelecionado;
                }
                else
                {
                    botoes.GetComponentInChildren<Image>().sprite = itemFundoPadrao;
                }
            }

            descricaoItem.text = item.descricao;
        }

        public void FinalizarSelecionar()
        {
            itensPainel.SetActive(false);
            deckTelaCartas.decksTemporarios[startTelaCartas.deckAtual].itemAtivavel = itemSelecioinado;
            deckTelaCartas.AtualizarImagensDoDeck();
        }

    }

}
