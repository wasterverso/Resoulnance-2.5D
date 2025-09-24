using Resoulnance.Cartas;
using Resoulnance.Flyers;
using Resoulnance.Scene_Arena;
using UnityEngine;

namespace Resoulnance.Scene_Arena.HUD
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] Skill_Btn skill1;
        [SerializeField] Skill_Btn skill2;
        [SerializeField] Skill_Btn skill3;
        [SerializeField] Suprema_Btn suprema_btn;
        [SerializeField] ItemAtivavel_Btn itemAtivavel_btn;

        public void AtribuirCartas(int idCard1, int idCard2, int idCard3, int idItem)
        {
            Cartas_Data cartasData = ArenaReferences.Instance.cartasData;

            Carta card1 = cartasData.cartas.Find(c => c.id == idCard1);
            Carta card2 = cartasData.cartas.Find(c => c.id == idCard2);
            Carta card3 = cartasData.cartas.Find(c => c.id == idCard3);

            skill1.ReceberCarta(card1);
            skill2.ReceberCarta(card2);
            skill3.ReceberCarta(card3);

            itemAtivavel_btn.ReceberItem(idItem);
        }

        public void AtribuirSupremaFlyer(Personagem_Data flyerData)
        {
            suprema_btn.ReceberFlyer(flyerData);
        }
    }

}
