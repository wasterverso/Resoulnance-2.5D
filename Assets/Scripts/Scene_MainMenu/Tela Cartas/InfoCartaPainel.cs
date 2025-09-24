using Resoulnance.Cartas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaCartas
{
    public class InfoCartaPainel : MonoBehaviour
    {
        [SerializeField] GameObject prefabIconsCaratas;

        [Header("Controle De dados")]
        [SerializeField] Icons_Data iconsData;

        [Header("Referencias")]
        [SerializeField] GameObject painelInfoCarta;
        [SerializeField] Text nomeCarta;
        [SerializeField] Text modoCarta;
        [SerializeField] CartaUIPrefab_TelaCartas cartaPrefab;
        [SerializeField] Image tipoCarta_img;
        [SerializeField] Text tipoCarta_txt;
        [SerializeField] Text recarga_txt;
        [SerializeField] Transform gridTransform;
        [SerializeField] Text acao_txt;
        [SerializeField] GameObject adicionar_btn;
        [SerializeField] GameObject remover_btn;

        [Header("Audio")]
        [SerializeField] AudioClip abrirPainel_Audio;

        public void AtribuirInfoCarta(Carta carta, bool adicionarCarta, bool mostrarBotao)
        {
            foreach (Transform itens in gridTransform)
            {
                Destroy(itens.gameObject);
            }

            cartaPrefab.ReceberCartaUI(carta);

            nomeCarta.text = carta.NomeCarta;
            modoCarta.text = carta.cartaAtiva.ToString();

            IconAfinidade tipo = iconsData.iconAfinidade.Find(sc => sc.Afinidade == carta.tipo);
            tipoCarta_img.sprite = tipo.SpriteAfinidade;
            tipoCarta_txt.text = tipo.Afinidade.ToString();

            recarga_txt.text = $"{carta.tempoRecarga}s";
            acao_txt.text = carta.acao;

            if (mostrarBotao && carta.podeColocarDeck)
            {
                adicionar_btn.SetActive(adicionarCarta);
                remover_btn.SetActive(!adicionarCarta);
            }
            else
            {
                adicionar_btn.SetActive(false);
                remover_btn.SetActive(false);
            }

            if (carta.efeitosCarta.Alvo != TipoAlvoFinal.Nenhum)
            {
                GameObject efeitoObj = Instantiate(prefabIconsCaratas, gridTransform);
                EfeitoIcons_Prefabs efeitoScript = efeitoObj.GetComponent<EfeitoIcons_Prefabs>();
                if (efeitoScript != null)
                {
                    EfeitosCarta_Icons icon = iconsData.iconsEfeitos_Cartas.Find(sc => sc.nomeIcon == "Alvo");
                    efeitoScript.icon_img.sprite = icon.sprite_;
                    efeitoScript.nomeTipo_txt.text = icon.nomeIcon;
                    efeitoScript.valorTipo_txt.text = carta.efeitosCarta.Alvo.ToString();
                }
            }

            if (carta.efeitosCarta.dano != 0)
            {
                GameObject efeitoObj = Instantiate(prefabIconsCaratas, gridTransform);
                EfeitoIcons_Prefabs efeitoScript = efeitoObj.GetComponent<EfeitoIcons_Prefabs>();
                if (efeitoScript != null)
                {
                    EfeitosCarta_Icons icon = iconsData.iconsEfeitos_Cartas.Find(sc => sc.nomeIcon == "Dano");
                    efeitoScript.icon_img.sprite = icon.sprite_;

                    efeitoScript.nomeTipo_txt.text = icon.nomeIcon;
                    efeitoScript.valorTipo_txt.text = carta.efeitosCarta.dano.ToString();
                }
            }

            if (carta.efeitosCarta.danoReal != 0)
            {
                GameObject efeitoObj = Instantiate(prefabIconsCaratas, gridTransform);
                EfeitoIcons_Prefabs efeitoScript = efeitoObj.GetComponent<EfeitoIcons_Prefabs>();
                if (efeitoScript != null)
                {
                    EfeitosCarta_Icons icon = iconsData.iconsEfeitos_Cartas.Find(sc => sc.nomeIcon == "Dano Real");
                    efeitoScript.icon_img.sprite = icon.sprite_;

                    efeitoScript.nomeTipo_txt.text = icon.nomeIcon;
                    efeitoScript.valorTipo_txt.text = carta.efeitosCarta.danoReal.ToString();
                }
            }

            if (carta.efeitosCarta.vida != 0)
            {
                GameObject efeitoObj = Instantiate(prefabIconsCaratas, gridTransform);
                EfeitoIcons_Prefabs efeitoScript = efeitoObj.GetComponent<EfeitoIcons_Prefabs>();
                if (efeitoScript != null)
                {
                    EfeitosCarta_Icons icon = iconsData.iconsEfeitos_Cartas.Find(sc => sc.nomeIcon == "Vida");
                    efeitoScript.icon_img.sprite = icon.sprite_;

                    efeitoScript.nomeTipo_txt.text = icon.nomeIcon;
                    efeitoScript.valorTipo_txt.text = carta.efeitosCarta.vida.ToString();
                }
            }

            if (carta.efeitosCarta.escudo != 0)
            {
                GameObject efeitoObj = Instantiate(prefabIconsCaratas, gridTransform);
                EfeitoIcons_Prefabs efeitoScript = efeitoObj.GetComponent<EfeitoIcons_Prefabs>();
                if (efeitoScript != null)
                {
                    EfeitosCarta_Icons icon = iconsData.iconsEfeitos_Cartas.Find(sc => sc.nomeIcon == "Escudo");
                    efeitoScript.icon_img.sprite = icon.sprite_;

                    efeitoScript.nomeTipo_txt.text = icon.nomeIcon;
                    efeitoScript.valorTipo_txt.text = carta.efeitosCarta.escudo.ToString();
                }
            }

            if (carta.efeitosCarta.velocidade != 0)
            {
                GameObject efeitoObj = Instantiate(prefabIconsCaratas, gridTransform);
                EfeitoIcons_Prefabs efeitoScript = efeitoObj.GetComponent<EfeitoIcons_Prefabs>();
                if (efeitoScript != null)
                {
                    EfeitosCarta_Icons icon = iconsData.iconsEfeitos_Cartas.Find(sc => sc.nomeIcon == "Velocidade");
                    efeitoScript.icon_img.sprite = icon.sprite_;

                    efeitoScript.nomeTipo_txt.text = icon.nomeIcon;
                    efeitoScript.valorTipo_txt.text = carta.efeitosCarta.velocidade.ToString();
                }
            }

            if (carta.efeitosCarta.controle != 0)
            {
                GameObject efeitoObj = Instantiate(prefabIconsCaratas, gridTransform);
                EfeitoIcons_Prefabs efeitoScript = efeitoObj.GetComponent<EfeitoIcons_Prefabs>();
                if (efeitoScript != null)
                {
                    EfeitosCarta_Icons icon = iconsData.iconsEfeitos_Cartas.Find(sc => sc.nomeIcon == "Controle");
                    efeitoScript.icon_img.sprite = icon.sprite_;

                    efeitoScript.nomeTipo_txt.text = "Atordoar";
                    efeitoScript.valorTipo_txt.text = carta.efeitosCarta.controle.ToString() + " s";
                }
            }

            if (carta.efeitosCarta.imunidade != 0)
            {
                GameObject efeitoObj = Instantiate(prefabIconsCaratas, gridTransform);
                EfeitoIcons_Prefabs efeitoScript = efeitoObj.GetComponent<EfeitoIcons_Prefabs>();
                if (efeitoScript != null)
                {
                    EfeitosCarta_Icons icon = iconsData.iconsEfeitos_Cartas.Find(sc => sc.nomeIcon == "Imunidade");
                    efeitoScript.icon_img.sprite = icon.sprite_;

                    efeitoScript.nomeTipo_txt.text = icon.nomeIcon;
                    efeitoScript.valorTipo_txt.text = carta.efeitosCarta.imunidade.ToString() + " s";
                }
            }

            painelInfoCarta.SetActive(true);
        }
    }
}

