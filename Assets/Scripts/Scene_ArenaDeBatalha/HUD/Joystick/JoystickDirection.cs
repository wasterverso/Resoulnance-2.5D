using UnityEngine;
using UnityEngine.UI;

public class JoystickDirection : MonoBehaviour
{
    [SerializeField] Image setaDirection;
    [SerializeField] Image fundoJoystick;
    [SerializeField] Image handleJoystick;

    [SerializeField] float opacidadeOff;

    private Vector3 posicaoInicialHandle;

    void Start()
    {
        setaDirection.gameObject.SetActive(false);

        fundoJoystick.color = new Color(fundoJoystick.color.r, fundoJoystick.color.g, fundoJoystick.color.b, opacidadeOff);
        handleJoystick.color = new Color(handleJoystick.color.r, handleJoystick.color.g, handleJoystick.color.b, opacidadeOff);

        posicaoInicialHandle = handleJoystick.transform.position;
    }

    void Update()
    {
        if (handleJoystick.transform.position != posicaoInicialHandle)
        {
            handleJoystick.color = new Color(handleJoystick.color.r, handleJoystick.color.g, handleJoystick.color.b, 1);
            fundoJoystick.color = new Color(fundoJoystick.color.r, fundoJoystick.color.g, fundoJoystick.color.b, 1);

            setaDirection.gameObject.SetActive(true);

            Vector3 dragDirection = handleJoystick.transform.position - posicaoInicialHandle;
            float angle = Mathf.Atan2(dragDirection.y, dragDirection.x) * Mathf.Rad2Deg;
            setaDirection.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            handleJoystick.color = new Color(handleJoystick.color.r, handleJoystick.color.g, handleJoystick.color.b, opacidadeOff);
            fundoJoystick.color = new Color(fundoJoystick.color.r, fundoJoystick.color.g, fundoJoystick.color.b, opacidadeOff);

            setaDirection.gameObject.SetActive(false);
        }
    }

}
