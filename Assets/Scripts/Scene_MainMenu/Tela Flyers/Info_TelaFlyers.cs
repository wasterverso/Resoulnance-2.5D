using Resoulnance.Flyers;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaFlyers
{
    public class Info_TelaFlyers : MonoBehaviour
    {
        [Header("Controle De dados")]
        [SerializeField] Icons_Data iconsData;
        [SerializeField] Flyer_Data flyersData;

        [Header("Refs Script")]
        [SerializeField] InfoSkins_TelaFlyers skinScript;

        [Header("Flyer")]
        [SerializeField] Text nomeFlyer;
        [SerializeField] Text classeFlyer;
        [SerializeField] Image imgClasseFlyer;
        [SerializeField] Text passiva_txt;
        [SerializeField] Text suprema_txt;
        [SerializeField] Image suprema_icon;

        [Header("Velocidade")]
        [SerializeField] Image barraVeloc;
        [SerializeField] Text valorVeloc;

        [Header("Vida")]
        [SerializeField] Image barraVida;
        [SerializeField] Text valorVida;

        [Header("Escudo")]
        [SerializeField] Image barraEscudo;
        [SerializeField] Text valorEscudo;

        [Header("Afinidades")]
        [SerializeField] Image afini1;
        [SerializeField] Image afini2;
        [SerializeField] Image naoAfini1;
        [SerializeField] Image naoAfini2;

        Personagem_Data personagemEscolhido;

        public void AtualizarInfoPersonagem(Personagem_Data personagem)
        {
            personagemEscolhido = personagem;

            skinScript.PersonagemSelecionado(personagemEscolhido);

            nomeFlyer.text = personagem.nome;

            classeFlyer.text = personagem.classe.ToString();

            SpriteClass spriteClass = iconsData.spriteClasses.Find(sc => sc.nome == personagem.classe.ToString());
            imgClasseFlyer.sprite = spriteClass.sprite;

            passiva_txt.text = personagem.passivaDesc;
            suprema_txt.text = personagem.supremaDesc;
            suprema_icon.sprite = personagem.supremaIcon;

            if (personagemEscolhido.vida != 0)
            {
                valorVida.text = $"{personagem.vida}/100";
                float valorNormalizado = Mathf.Clamp01((float)personagem.vida / 100f);
                barraVida.fillAmount = valorNormalizado;
            }

            if (personagemEscolhido.escudo != 0)
            {
                valorEscudo.text = $"{personagem.escudo}/100";
                float valorNormalizado = Mathf.Clamp01((float)personagem.escudo / 100f);
                barraEscudo.fillAmount = valorNormalizado;
            }

            if (personagemEscolhido.velocidade != 0)
            {
                valorVeloc.text = $"{personagem.velocidade}/10";

                // Normalizar a velocidade entre 0 (para 4) e 1 (para 6)
                float valorNormalizado = (personagem.velocidade - 4) / (6 - 4);
                valorNormalizado = Mathf.Clamp01(valorNormalizado);
                barraVeloc.fillAmount = valorNormalizado;
            }

            afini1.sprite = iconsData.iconAfinidade.Find(icon => icon.Afinidade == personagem.Afinidade1).SpriteAfinidade;
            afini2.sprite = iconsData.iconAfinidade.Find(icon => icon.Afinidade == personagem.Afinidade2).SpriteAfinidade;
            naoAfini1.sprite = iconsData.iconAfinidade.Find(icon => icon.Afinidade == personagem.NaoAfinidade1).SpriteAfinidade;
            naoAfini2.sprite = iconsData.iconAfinidade.Find(icon => icon.Afinidade == personagem.NaoAfinidade2).SpriteAfinidade;
        }

        public void Personagem_Anterior()
        {
            int total = flyersData.personagens.Count;
            int idFlyer = personagemEscolhido.id;

            do
            {
                idFlyer--;
                if (idFlyer < 0)
                    idFlyer = total - 1;

            } while (!flyersData.personagens[idFlyer].Ativo);

            Personagem_Data newFlyer = flyersData.personagens[idFlyer];
            AtualizarInfoPersonagem(newFlyer);
        }

        public void Personagem_Proximo()
        {
            int total = flyersData.personagens.Count;
            int idFlyer = personagemEscolhido.id;

            do
            {
                idFlyer++;
                if (idFlyer >= total)
                    idFlyer = 0;

            } while (!flyersData.personagens[idFlyer].Ativo);

            Personagem_Data newFlyer = flyersData.personagens[idFlyer];
            AtualizarInfoPersonagem(newFlyer);
        }
    }
}

