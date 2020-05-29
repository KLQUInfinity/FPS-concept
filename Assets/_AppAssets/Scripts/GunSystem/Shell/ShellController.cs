using UnityEngine;
using System.Collections;

public class ShellController : MonoBehaviour
{

    [SerializeField] private float deathTime;

    void OnEnable()
    {
        EnableShade();
        Invoke("HideShell", deathTime);
    }

    void EnableShade()
    {
        GetComponentInChildren<Animator>().enabled = true;
    }

    void HideShell()
    {
        GetComponentInChildren<Animator>().enabled = false;
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}
