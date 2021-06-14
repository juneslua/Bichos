using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class Puerta : MonoBehaviour
{
    public static bool EnTP { get; set; }

    private enum TipoPuerta {Exterior, Interior}

    private static Camera _camPrincipal;
    private ControlJugador _jugador;
    private AudioSource _audio;

    [SerializeField]
    private Puerta _destino;
    [SerializeField]
    private TipoPuerta _tipoPuerta = 0;
    
    public AudioClip piedraClip;
    public AudioClip hojaClip;

    public AudioMixerGroup mixerGroup;

    void Start()
    {
        _camPrincipal = Camera.main;

        _jugador = ControlJugador.Jugador;
        _jugador.EnInteraccion += Jugador_EnInteraccion;

        _audio = gameObject.AddComponent<AudioSource>();
        _audio.outputAudioMixerGroup = mixerGroup;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        BoxCollider2D col2D = GetComponent<BoxCollider2D>();
        Vector2 origen = (Vector2)transform.position + col2D.offset;
        Gizmos.DrawWireCube(origen, col2D.bounds.size);
    }

    void OnDisable()
    {
        _jugador.EnInteraccion -= Jugador_EnInteraccion;
    }

    private void Jugador_EnInteraccion(int id)
    {
        if (gameObject.GetInstanceID() == id) {
            Teletransportar();
        }
    }

    private void Teletransportar()
    {
        EnTP = true;
        _jugador.transform.position = _destino.transform.position + Vector3.up * 0.01f;

        if (_destino._tipoPuerta == TipoPuerta.Exterior){
            _camPrincipal.clearFlags = CameraClearFlags.Skybox;
            PlayClip(hojaClip);
        }
        else if (_destino._tipoPuerta == TipoPuerta.Interior){
            PlayClip(piedraClip);
            _camPrincipal.clearFlags = CameraClearFlags.SolidColor;
        }
    }

    public void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        _audio.PlayOneShot(clip, 0.5f);
    }

}
