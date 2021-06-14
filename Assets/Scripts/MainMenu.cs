using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Animator _TransicionInicial;

    public void play(){
        _TransicionInicial.gameObject.SetActive(true);
        _TransicionInicial.SetBool("Iniciar", true);
    }
    public void quit() {
        Application.Quit();
    }
}
