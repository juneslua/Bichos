using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControlAnimacionJugador : MonoBehaviour
{

    private const float RANGO_VOLUMEN = 0.1f, RANGO_FRECUENCIA = 0.6f;

    [Header("Pasos")]
    [SerializeField, Range(0f, 1f)]
    private float volumenPasos;
    [SerializeField, Range(-3f, 3f)]
    private float frecuenciaPasos;
    
    private ControlJugador _jugador;

    private Animator _animator;
    private AudioSource _audio;
    private List<AudioSource> _pasosSource = new List<AudioSource>();

    void Awake()
    {
        _animator = GetComponent<Animator>();

        _audio = GetComponent<AudioSource>();
        for (int i = 0; i < 4; i++)
        {
            Debug.Log("Why");
            _pasosSource.Add(gameObject.AddComponent<AudioSource>());
            _pasosSource[i].playOnAwake = false;
            _pasosSource[i].loop = false;
            _pasosSource[i].outputAudioMixerGroup = _audio.outputAudioMixerGroup;
        }
    }

    void Start() => _jugador = GetComponentInParent<ControlJugador>();

    void Update()
    {
        if (_jugador.EnSuelo)
            _animator.SetBool("EnSalto", _animator.GetBool("EnSalto") || Input.GetButtonDown("Jump"));

        _animator.SetBool("EnSuelo", _jugador.EnSuelo);
    }

    void LateUpdate()
    {
        float velX = Mathf.Abs(_jugador.Control.flechas.x);
        _animator.SetFloat("Velocidad", velX);
        _animator.SetInteger("FramesEnAire", _jugador.FramesEnAire);
    }

    private void Saltar()
    {
        Controles c = _jugador.Control;
        c.salto = true;
        _jugador.Control = c;
        _animator.SetBool("EnSalto", false);
    }

    private void ReproducirAudio(AudioClip audio)
    {
        _audio.PlayOneShot(audio);
    }

    private void ReporoducirStep(AudioClip clip)
    {
        AudioSource aSr = _pasosSource[0];

        aSr.volume = RangoAzar(volumenPasos, RANGO_VOLUMEN);
        aSr.pitch = RangoAzar(frecuenciaPasos, RANGO_FRECUENCIA);
        aSr.clip = clip;
        aSr.Play();

        _pasosSource.RemoveAt(0);
        _pasosSource.Add(aSr);

        float RangoAzar(float actual, float rango) => (actual - rango / 2f) + Random.value * rango; 
    }
}
