using Resoulnance.Cartas;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Resoulnance.Telas.TelaCartas
{
    public class CodiceCartasConfig : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] InfoCartaPainel infoCartaPainel;

        [Header("UI")]
        [SerializeField] GameObject cartaPrefab;
        [SerializeField] Transform gridInstance;

        public void InstanciarCartasCodice()
        {
            foreach (Transform t in gridInstance)
            {
                Destroy(t.gameObject);
            }

            ListaCartas cartasList = ListaCartas.Instance;

            var cartasPorTipo = cartasList.cartasData.cartas
                .GroupBy(carta => carta.tipo)
                .OrderBy(grupo => grupo.Key);

            foreach (var grupo in cartasPorTipo)
            {
                foreach (Carta carta in grupo)
                {
                    GameObject cartaObj = Instantiate(cartaPrefab, gridInstance);
                    CartaCodicePrefab cartaScript = cartaObj.GetComponent<CartaCodicePrefab>();

                    if (cartaScript != null)
                    {
                        bool temCarta = cartasList.minhasCartas.Exists(c => c.id == carta.id);
                        cartaScript.ConfigurarCarta(carta, temCarta, this);
                    }
                }
            }
        }

        public void MostrarInfoCarta(Carta carta)
        {
            infoCartaPainel.AtribuirInfoCarta(carta, false, false);
        }
    }

}
