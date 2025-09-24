using System;
using UnityEngine;

namespace Resoulnance.Scene_Arena.Config
{
    public class ArenaConfig : MonoBehaviour
    {
        public static ArenaConfig Instance { get; private set; }

        public ProcurarAlvo alvo = ProcurarAlvo.MaisPerto;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
    }
}


