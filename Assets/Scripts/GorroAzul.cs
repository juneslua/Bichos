using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorroAzul : MonoBehaviour
{
    [SerializeField]
    private Animator _animacionFinal;

    private BoxCollider2D _col2D;
    private AudioSource _audio;
    private ControlJugador _jugador;

    private InterpolacionFloat _interFloat;

    void Start()
    {
        _col2D = GetComponent<BoxCollider2D>();
        _audio = Camera.main.GetComponent<AudioSource>();

        _jugador = ControlJugador.Jugador;
        _jugador.EnInteraccion += Jugador_EnInteraccion;

        _interFloat = new InterpolacionFloat(GetInstanceID(), 5f, _audio.volume);
        _interFloat.IniciarInterpolacion(0f);
        _interFloat.EnBlancoAlcanzado += Blanco_Alcanzado;
    }

    void Update()
    {
        if (_animacionFinal.gameObject.activeInHierarchy)
        {
            _interFloat.Tick(Time.deltaTime);
            _audio.volume = _interFloat.ValorActual;
        }
    }

    void OnDrawGizmos()
    {
        if (_col2D == null) _col2D = GetComponent<BoxCollider2D>();

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_col2D.bounds.center, _col2D.bounds.size);
    }

    private void OnDisable() => _jugador.EnInteraccion -= Jugador_EnInteraccion;

    private void Jugador_EnInteraccion(int id)
    {
        if (gameObject.GetInstanceID() == id)
        {
            _animacionFinal.gameObject.SetActive(true);
            _animacionFinal.SetBool("Iniciar", true);
        }
    }

    private void Blanco_Alcanzado(int id, float blanco)
    {
        if (GetInstanceID() == id) Destroy(gameObject);
    }


}
