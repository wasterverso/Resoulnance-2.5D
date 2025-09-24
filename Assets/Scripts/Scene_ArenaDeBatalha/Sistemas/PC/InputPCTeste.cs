using Resoulnance.Scene_Arena;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputPCTeste : MonoBehaviour
{
    public static InputPCTeste Instance;

    public int _horizontal;
    public int _vertical;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE //Rodar só no PC e Editor
        _horizontal = 0;
        _vertical = 0;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) _horizontal = -1;
            if (Keyboard.current.dKey.isPressed) _horizontal = 1;
            if (Keyboard.current.wKey.isPressed) _vertical = 1;
            if (Keyboard.current.sKey.isPressed) _vertical = -1;

            if (Keyboard.current.spaceKey.isPressed) Atacar();
        }
#endif
    }

    void Atacar()
    {
        ArenaReferences.Instance.playerReferences.playerAtk.AtkBasico(false, Vector2.zero);
    }
}

