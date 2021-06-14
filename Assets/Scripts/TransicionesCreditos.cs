using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicionesCreditos : MonoBehaviour
{
    public void IrAlJuego() => SceneManager.LoadScene("Nivel 1");

    public void IrACreditos() => SceneManager.LoadScene("Creditos");

    public void VolverAlMenu() => SceneManager.LoadScene(0);
}
