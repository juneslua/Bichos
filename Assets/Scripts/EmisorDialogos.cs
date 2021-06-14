using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class EmisorDialogos : MonoBehaviour
{
    public Vector2 CentroEmisor { get => (Vector2)transform.position + _posicionEmisor; }
    public float Radio { get => _radioEmisor; }
    public Texture2D EstiloBocadillo { get => _estiloBocadillo; set => _estiloBocadillo = value; }

    public bool Jugador;

    [SerializeField, ControladorBool("Jugador")]
    private Texture2D _estiloBocadillo;

    [Header("Posicion Bocadillos")]
    [SerializeField]
    private Vector2 _posicionEmisor = new Vector2();
    [SerializeField, Range(0f, 2f)]
    private float _radioEmisor = 0.5f;

    [Header("Sonido"), SerializeField]
    private AudioClip _voz;
    [SerializeField]
    private AudioMixerGroup _master;
    
    private GameObject _bocadilloBase;
    private Vector2 _ultimaPosicion;

    private AudioSource _audio;

    void Awake()
    {
        _bocadilloBase = Resources.Load<GameObject>("Bocadillo");
        _ultimaPosicion = transform.position;

        _audio = gameObject.AddComponent<AudioSource>();
        _audio.outputAudioMixerGroup = _master;
    }

    void LateUpdate()
    {
        Vector2 deltaPosicion = (Vector2)transform.position - _ultimaPosicion;
        deltaPosicion.Normalize();

        bool? movDer = deltaPosicion.x != 0 ? deltaPosicion.x > 0 : (bool?) null;
        if (movDer != null)
        {
            if ((bool)movDer) _posicionEmisor.x = Mathf.Abs(_posicionEmisor.x);
            else if (_posicionEmisor.x > 0f) _posicionEmisor.x = -_posicionEmisor.x;
        }

        _ultimaPosicion = transform.position;
    }

    void OnDrawGizmos()
    {
        BoxCollider2D col2D = GetComponent<BoxCollider2D>();

        Vector2 origen = transform.position;

        Gizmos.color = new Color(0.8f, 0.3f, 0f);
        origen = (Vector2)transform.position + _posicionEmisor;
        Gizmos.DrawWireSphere(origen, _radioEmisor);
    }

    public void Hablar(Texture2D dialogo, float angulo, Vector2 offset, float escala, float duracion)
    {
        GameObject bocadillo = Instantiate(_bocadilloBase, transform.position, Quaternion.identity, transform);
        bocadillo.GetComponent<ControlBocadillo>().Duracion = duracion;

        RectTransform rectBoc = (RectTransform)bocadillo.transform;
        RectTransform rectFondo = (RectTransform)rectBoc.Find("Fondo"), rectDiag = (RectTransform)rectBoc.Find("Dialogo");
        RawImage imgFondo = rectFondo.GetComponent<RawImage>(), imgDiag = rectDiag.GetComponent<RawImage>();

        // Ubicar BOCADILLO - Estilo
        Vector2 bSize = new Vector2(_estiloBocadillo.width, _estiloBocadillo.height);
        bSize /= bSize.x > bSize.y ? bSize.x : bSize.y;

        rectFondo.sizeDelta = bSize;
        imgFondo.texture = _estiloBocadillo;

        ControlDialogos.UbicarDialogo(this, angulo * Mathf.Deg2Rad, rectBoc, imgFondo, out var pivot);

        // Ubicar DIALOGO - Imagen
        Vector2 dSize = new Vector2(dialogo.width, dialogo.height);
        Vector2 dAncla = new Vector2(pivot.x, -(pivot.y - 1f));
        dSize /= dSize.x > dSize.y ? dSize.x : dSize.y;

        imgDiag.texture = dialogo;
        rectDiag.sizeDelta = dSize * escala;
        rectDiag.anchorMin = dAncla;
        rectDiag.anchorMax = dAncla;
        if (pivot.x == 1f) offset.x *= -1;
        if (pivot.y == 0f) offset.y *= -1;
        rectDiag.anchoredPosition = offset;

        // SONIDO - Reproducir voz
        if (_voz != null)
            _audio.PlayOneShot(_voz, 0.5f);
    } 
}
