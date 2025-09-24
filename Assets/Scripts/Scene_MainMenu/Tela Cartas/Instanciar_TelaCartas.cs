using Resoulnance.Cartas;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaCartas
{
    public class Instanciar_TelaCartas : MonoBehaviour
    {
        [Header("Refs Scripts")]
        [SerializeField] DeckPainel_TelaCartas deckTelaCartas;

        [Header("Controle De dados")]
        [SerializeField] Cartas_Data cartasData;

        [Header("Refs")]
        [SerializeField] GameObject cartaPrefab;
        [SerializeField] Transform gridLayoutGroup;

        [Header("Filtros")]
        [SerializeField] Dropdown tipo_drop;
        [SerializeField] Dropdown ativo_drop;

        void Awake()
        {
            tipo_drop.options.Clear();
            tipo_drop.options.Add(new Dropdown.OptionData("Tudo"));
            foreach (Tipagem tipo in System.Enum.GetValues(typeof(Tipagem)))
            {
                if (tipo != Tipagem.Nenhum)
                {
                    tipo_drop.options.Add(new Dropdown.OptionData(tipo.ToString()));
                }
            }
            tipo_drop.captionText.text = tipo_drop.options[0].text;
            tipo_drop.onValueChanged.AddListener(delegate { FiltrarCartas(); });

            ativo_drop.options.Clear();
            ativo_drop.options.Add(new Dropdown.OptionData("Tudo"));
            foreach (CartaAtivaPassiva tipo in System.Enum.GetValues(typeof(CartaAtivaPassiva)))
            {
                ativo_drop.options.Add(new Dropdown.OptionData(tipo.ToString()));
            }
            ativo_drop.captionText.text = ativo_drop.options[0].text;
            ativo_drop.onValueChanged.AddListener(delegate { FiltrarCartas(); });
        }

        public void InstanciarCartas()
        {
            foreach (Transform child in gridLayoutGroup)
            {
                Destroy(child.gameObject);
            }

            foreach (Carta carta in ListaCartas.Instance.minhasCartas)
            {
                GameObject cartaObj = Instantiate(cartaPrefab, gridLayoutGroup);

                CartaUIPrefab_TelaCartas cartaScript = cartaObj.GetComponent<CartaUIPrefab_TelaCartas>();
                if (cartaScript != null)
                {
                    cartaScript.ReceberCartaUI(carta, deckTelaCartas);
                }
            }
        }

        public void FiltrarCartas()
        {
            string tipoSelecionadoTexto = tipo_drop.options[tipo_drop.value].text;
            string atividadeSelecionadoTexto = ativo_drop.options[ativo_drop.value].text;

            Tipagem tipoSelecionado = tipoSelecionadoTexto == "Tudo"
                ? Tipagem.Nenhum : (Tipagem)System.Enum.Parse(typeof(Tipagem), tipoSelecionadoTexto);

            CartaAtivaPassiva atividadeSelecionado = atividadeSelecionadoTexto == "Tudo" ?
                (CartaAtivaPassiva)(-1) : (CartaAtivaPassiva)System.Enum.Parse(typeof(CartaAtivaPassiva), atividadeSelecionadoTexto);

            foreach (Transform cartaObj in gridLayoutGroup)
            {
                CartaUIPrefab_TelaCartas cartaArrastar = cartaObj.GetComponent<CartaUIPrefab_TelaCartas>();
                if (cartaArrastar != null)
                {
                    bool tipoMatch = tipoSelecionado == Tipagem.Nenhum || cartaArrastar.GetCarta().tipo == tipoSelecionado;
                    bool atividadeMatch = atividadeSelecionado == (CartaAtivaPassiva)(-1) || cartaArrastar.GetCarta().cartaAtiva == atividadeSelecionado;

                    cartaObj.gameObject.SetActive(tipoMatch && atividadeMatch);
                }
            }
        }
    }
}

