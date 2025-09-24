using Resoulnance.AvatarCustomization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaAvatarCustom
{
    public enum AvatarTipoItem { Nenhum, Acessorio, Cabelo, Rosto, Corpo, Roupa, Pes }

    public class Painel_TelaAvatarCustom : MonoBehaviour
    {
        [Header("Refs Script")]
        [SerializeField] AtribuirDados_AvatarCustom attdadosAvatar;

        [Header("Refs UI")]
        [SerializeField] Transform gridItens;

        [Header("GameObjects")]
        [SerializeField] GameObject itemPrefab;

        [Header("Botoes")]
        [SerializeField] Button sexo_btn;
        [SerializeField] Button acessorio_btn;
        [SerializeField] Button cabelo_btn;
        [SerializeField] Button rosto_btn;
        [SerializeField] Button roupa_btn;
        [SerializeField] Button pes_btn;

        [Header("Botoes Colors")]
        [SerializeField] GameObject coresPainel;
        [SerializeField] List<Button> colorList = new List<Button>();

        [Header("Controle de dados")]
        [SerializeField] AvatarCustom_Data customData;

        AvatarTipoItem tipoItem = AvatarTipoItem.Nenhum;
        AvatarCustom newPlayer;

        void Start()
        {
            sexo_btn.onClick.AddListener(() => AtribuirListas(customData.corpo));
            acessorio_btn.onClick.AddListener(() => AtribuirListas(customData.acessorios));
            cabelo_btn.onClick.AddListener(() => AtribuirListas(customData.cabelos));
            rosto_btn.onClick.AddListener(() => AtribuirListas(customData.rostos));
            roupa_btn.onClick.AddListener(() => AtribuirListas(customData.roupas));
            pes_btn.onClick.AddListener(() => AtribuirListas(customData.pes));

            newPlayer = ListAvatarCustom.Instance.NovoAvatarCustom();

            AtribuirListas(customData.corpo);
            AtribuirCores();
        }

        void AtribuirListas(List<Item_AvatarCustom> lista)
        {
            foreach (Transform item in gridItens)
            {
                Destroy(item.gameObject);
            }

            if (lista == customData.rostos) tipoItem = AvatarTipoItem.Rosto;
            else if (lista == customData.cabelos) tipoItem = AvatarTipoItem.Cabelo;
            else if (lista == customData.corpo) tipoItem = AvatarTipoItem.Corpo;
            else if (lista == customData.acessorios) tipoItem = AvatarTipoItem.Acessorio;
            else if (lista == customData.roupas) tipoItem = AvatarTipoItem.Roupa;
            else if (lista == customData.pes) tipoItem = AvatarTipoItem.Pes;

            foreach (Item_AvatarCustom cust in lista)
            {
                GameObject obj = Instantiate(itemPrefab, gridItens);
                Item_Prefab itemScript = obj.GetComponent<Item_Prefab>();
                if (itemScript != null)
                {
                    itemScript.ReceberItem(cust, tipoItem, AtribuirItemSelecionado);
                }
            }

            if (lista == customData.cabelos || lista == customData.pes || lista == customData.corpo)
            {
                coresPainel.SetActive(true);
            }
            else
            {
                coresPainel.SetActive(false);
            }
        }

        void AtribuirCores()
        {
            // Definindo as cores
            Color[] cores = new Color[]
            {
            Color.white,
            Color.black,
            Color.red,
            new Color(1.0f, 0.5f, 0.0f), // Laranja
            Color.yellow,
            Color.green,
            new Color(0.5f, 0.75f, 1.0f), // Azul claro
            new Color(0.0f, 0.0f, 0.5f), // Azul escuro
            new Color(0.5f, 0.0f, 0.5f), // Roxo
            new Color(1.0f, 0.0f, 1.0f), // Rosa           
            Color.gray,
            new Color(0.5f, 0.25f, 0.0f) // Marrom            
            };

            // Atribuindo as cores aos botões
            for (int i = 0; i < colorList.Count && i < cores.Length; i++)
            {
                Image buttonImage = colorList[i].GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = cores[i];
                }

                int posicaoCor = i;
                colorList[i].onClick.AddListener(() => MudarCorSelecionado(cores[posicaoCor]));
            }
        }

        public void MudarCorSelecionado(Color cor)
        {
            if (tipoItem == AvatarTipoItem.Cabelo)
            {
                newPlayer.cabelos.cor = cor;
            }
            else if (tipoItem == AvatarTipoItem.Pes)
            {
                newPlayer.pes.cor = cor;
            }
            else if (tipoItem == AvatarTipoItem.Corpo)
            {
                newPlayer.corpo.cor = cor;
            }

            attdadosAvatar.AtribuirDados(newPlayer);
        }

        public void AtribuirItemSelecionado(Item_AvatarCustom item, AvatarTipoItem tipo)
        {
            tipoItem = tipo;

            if (tipo == AvatarTipoItem.Rosto)
            {
                newPlayer.rostos = item;
            }
            else if (tipo == AvatarTipoItem.Cabelo)
            {
                newPlayer.cabelos = item;
            }
            else if (tipo == AvatarTipoItem.Acessorio)
            {
                newPlayer.acessorios = item;
            }
            else if (tipo == AvatarTipoItem.Roupa)
            {
                newPlayer.roupas = item;
            }
            else if (tipo == AvatarTipoItem.Pes)
            {
                newPlayer.pes = item;
            }
            else if (tipo == AvatarTipoItem.Corpo)
            {
                newPlayer.corpo = item;
            }

            attdadosAvatar.AtribuirDados(newPlayer);
        }
        public void SalvarAvatar()
        {
            ListAvatarCustom.Instance.SalvarAvatarCustom(newPlayer, true);
        }
    }
}

