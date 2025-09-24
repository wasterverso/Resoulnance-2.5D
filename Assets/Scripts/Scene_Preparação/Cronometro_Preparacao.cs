using PurrNet;
using Resoulnance.Scene_Preparation.Visuals;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Preparation.Controles
{
    public class Cronometro_Preparacao : NetworkBehaviour
    {
        private enum Fase { Nenhuma, Flyers, DecksSkins, Final }

        [Header("Refs Script")]
        [SerializeField] PreparacaoController prepController;
        [SerializeField] PreparacaoVisuals prepVisuals;

        [Header("Ref UI")]
        [SerializeField] Text timer_txt;
        [SerializeField] Text titulosTempos_txt;

        [Header("Configuração")]
        [SerializeField] int tempoEscolhaFlyers = 20;
        [SerializeField] int tempoEscolhaDecksSkins = 15;
        [SerializeField] int tempoFinal = 3;

        SyncVar<int> tempo = new();

        Fase faseAtual = Fase.Nenhuma;

        private void Awake()
        {
            tempo.onChanged += MostrarTempo;
            titulosTempos_txt.text = "Escolha seu Flyer";
        }

        private void MostrarTempo(int valor)
        {
            timer_txt.text = $"{valor:00}";
        }

        public void IniciarCronometro()
        {
            if (!isServer) return;

            faseAtual = Fase.Flyers;
            StartCoroutine(ContagemTempo(tempoEscolhaFlyers));
        }

        IEnumerator ContagemTempo(int tempoTotal)
        {
            tempo.value = tempoTotal;

            int tempoCorrido = tempoTotal;

            while (tempoCorrido > 0)
            {
                yield return new WaitForSeconds(1f);

                tempoCorrido -= 1;
                tempo.value = tempoCorrido;
            }

            tempo.value = 0;

            OnTimerEnded();
        }

        private void OnTimerEnded()
        {
            if (!isServer) return; // Esta lógica roda APENAS no servidor.

            StopAllCoroutines();

            switch (faseAtual)
            {
                case Fase.Flyers:
                    faseAtual = Fase.DecksSkins;

                    StartCoroutine(ContagemTempo(tempoEscolhaDecksSkins));

                    EncerrarFaseFlyers();
                    break;

                case Fase.DecksSkins:
                    faseAtual = Fase.Final;

                    StartCoroutine(ContagemTempo(tempoFinal));

                    EncerrarFaseDecksSkins();


                    break;

                case Fase.Final:
                    faseAtual = Fase.Nenhuma;

                    EncerrarFaseFinal();

                    networkManager.sceneModule.LoadSceneAsync("ArenaDeBatalha");
                    break;
            }
        }

        [ObserversRpc(runLocally: true)]
        private void EncerrarFaseFlyers()
        {
            titulosTempos_txt.text = "Personalização";
            prepVisuals.ConfirmarButton();
            Debug.Log("Fim da fase de escolha de Flyers.");
        }

        [ObserversRpc(runLocally: true)]
        private void EncerrarFaseDecksSkins()
        {
            titulosTempos_txt.text = "Iniciando Jogo";
            prepVisuals.ChamarFinal();
            Debug.Log("Fim da fase 2: escolha de Decks/Skins.");
        }

        [ObserversRpc(runLocally: true)]
        private void EncerrarFaseFinal()
        {
            titulosTempos_txt.text = "Abrindo campo de batalha";
            Debug.Log("Fim da fase 3: preparação concluída.");
        }

        protected override void OnDestroy()
        {
            tempo.onChanged -= MostrarTempo;
        }
    }
}
