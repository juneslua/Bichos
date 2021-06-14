using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteraccionSonora : MonoBehaviour
{
    private const float RANGO_VOLUMEN = 0.2f, RANGO_FRECUENCIA = 0.5f;

    private AudioSource _audio;
    private BoxCollider2D _col2D;
    private ControlJugador _jugador;

    private float volIni = 0f, frecIni = 0f;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _col2D = GetComponent<BoxCollider2D>();

        volIni = _audio.volume;
        frecIni = _audio.pitch;
    }

    void Start()
    {
        _jugador = ControlJugador.Jugador;
        _jugador.EnInteraccion += Jugador_EnInteraccion;
    }

    void Update() => _col2D.enabled = !_audio.isPlaying;

    void OnDrawGizmos()
    {
        if (_col2D == null) _col2D = GetComponent<BoxCollider2D>();

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_col2D.bounds.center, _col2D.bounds.size);
    }

    void OnDisable() => _jugador.EnInteraccion -= Jugador_EnInteraccion;

    void Jugador_EnInteraccion(int id)
    {
        if (gameObject.GetInstanceID() == id)
        {
            _audio.volume = RangoAzar(volIni, RANGO_VOLUMEN);
            _audio.pitch = RangoAzar(frecIni, RANGO_FRECUENCIA);
            _audio.Play();

            float RangoAzar(float actual, float rango) => (actual - rango / 2f) + Random.value * rango;
        }
    }
}
