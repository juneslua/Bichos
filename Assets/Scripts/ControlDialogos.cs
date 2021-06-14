using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Mathf;

public class ControlDialogos : MonoBehaviour
{

    [Serializable]
    public class Conversacion
    {
        public enum Efectos { Ninguno, Enfocar, Congelar, EnfocarYCongelar }

        public string nombre;
        public Efectos efectos;
        public List<Dialogo> dialogos;
    }

    [Serializable]
    public class Dialogo
    {
        public Texture2D dialogo;
        public float angulo, duracion;
        public Vector2 offset;
        public float escala;
        public bool dialogoJugador, noEsperar, autoOrientar;
    }
 
    public bool EnDialogo { get; private set; }

    [SerializeField]
    private EmisorDialogos _emisor;
    [SerializeField]
    private Texture2D _estilo;
    [SerializeField]
    private List<Conversacion> _conversaciones;
    [SerializeField]
    private RectTransform _icono;

    private RawImage _iconoImg;
    private BoxCollider2D _col2D;

    private Dictionary<string, Conversacion> _diccionarioConversaciones;
    private Queue<Action> _dialogos_EnCallback = new Queue<Action>();
    private List<Dialogo> _proximosDialogos = new List<Dialogo>();
    private float _tiempoInicio = 0f;

    void Awake()
    {
        _icono = transform.Find("Icono")?.GetComponent<RectTransform>();
        _col2D = GetComponent<BoxCollider2D>();

        if(_icono != null)
        {
            _iconoImg = _icono.GetComponentInChildren<RawImage>();
            _icono.gameObject.SetActive(false);
        }

        _diccionarioConversaciones = new Dictionary<string, Conversacion>();
        _conversaciones.ForEach(c => _diccionarioConversaciones.Add(c.nombre, c));
    }

    void Start()
    {
        ControlJugador.Jugador.EnInteraccion += Jugador_EnInteraccion;
    }

    void Update()
    {
        if (_proximosDialogos.Any())
        {
            float dur = _proximosDialogos[0].duracion;
            if (dur < Time.time - _tiempoInicio || _proximosDialogos[0].noEsperar)
            {
                if (_dialogos_EnCallback.Any())
                    _dialogos_EnCallback.Dequeue()?.Invoke();

                _tiempoInicio += _proximosDialogos[0].duracion;
                _proximosDialogos.RemoveAt(0);

                if (_proximosDialogos.Count == 0f)
                {
                    EnDialogo = false;
                    _col2D.enabled = !EnDialogo;
                }
            }
        }
    }

    void OnValidate()
    {
        if (_emisor != null) _emisor.EstiloBocadillo = _estilo;
    }

    void OnTriggerStay2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.tag == "Player")
        {
            float angulo = OrientarDialogo(_emisor);

            _icono.gameObject.SetActive(!EnDialogo);
            UbicarDialogo(_emisor, angulo, _icono, _iconoImg);
        }
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.tag == "Player") _icono.gameObject.SetActive(false);
    }

    void OnDrawGizmos()
    {
        BoxCollider2D col2D = GetComponent<BoxCollider2D>();

        Gizmos.color = new Color(0.9f, 0.5f, 0f);
        Vector2 origen = (Vector2)transform.position + col2D.offset;
        Gizmos.DrawWireCube(origen, col2D.bounds.size);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            float angulo = OrientarDialogo(_emisor);
            Gizmos.DrawWireSphere((Vector3)_emisor.CentroEmisor + new Vector3(Cos(angulo), Sin(angulo)) * _emisor.Radio, 0.1f);
        }
    }

    void OnDisable()
    {    
        ControlJugador.Jugador.EnInteraccion -= Jugador_EnInteraccion;
    }

    public static float OrientarDialogo(EmisorDialogos emisor)
    {
        float angulo = 0f;

        Vector2 distancia = (Vector2)ControlJugador.Jugador.transform.position  - emisor.CentroEmisor;
        float mag = 60f * Clamp(distancia.magnitude/3.5f, 0f, 1f);

        if (distancia.x > 0f)
            angulo = 70f - mag;  
        else
            angulo = 110f + mag;

        return angulo * Deg2Rad;
    }

    public static void UbicarDialogo(EmisorDialogos emisor, float angulo, RectTransform rect, RawImage imgRaw)
    {
        Vector2 posLocal = new Vector2(Cos(angulo), Sin(angulo)) * emisor.Radio;
        Vector2 pivot = new Vector2(posLocal.x <= 0 ? 1 : 0, posLocal.y <= 0 ? 1 : 0);

        Rect uvR = imgRaw.uvRect;
        uvR.x = pivot.x;
        uvR.y = pivot.y;
        uvR.width = pivot.x == 1f ? -1 : 1;
        uvR.height = pivot.y == 1f ? -1 : 1;
        imgRaw.uvRect = uvR;

        rect.pivot = pivot;
        rect.position = emisor.CentroEmisor + posLocal;
    }

    public static void UbicarDialogo(EmisorDialogos emisor, float angulo, RectTransform rect, RawImage imgRaw, out Vector2 pivot)
    {
        Vector2 posLocal = new Vector2(Cos(angulo), Sin(angulo)) * emisor.Radio;
        pivot = new Vector2(posLocal.x <= 0 ? 1 : 0, posLocal.y <= 0 ? 1 : 0);

        Rect uvR = imgRaw.uvRect;
        uvR.x = pivot.x;
        uvR.y = pivot.y;
        uvR.width = pivot.x == 1f ? -1 : 1;
        uvR.height = pivot.y == 1f ? -1 : 1;
        imgRaw.uvRect = uvR;

        rect.pivot = pivot;
        rect.position = emisor.CentroEmisor + posLocal;
    }

    private void Jugador_EnInteraccion(int id)
    {
        if (id == gameObject.GetInstanceID())
        {
            if (!EnDialogo)
                IniciarConversacion("Prueba");

            _col2D.enabled = !EnDialogo;
        }
    }

    private void IniciarConversacion(string nombre)
    {
        _dialogos_EnCallback.Clear();
        if(_diccionarioConversaciones.TryGetValue(nombre, out var conversacion))
        {
            EmisorDialogos emiNPC = _emisor, emiJug = ControlJugador.Jugador.Emisor;
            foreach(Dialogo d in conversacion.dialogos)
            {
                EmisorDialogos emiActual = d.dialogoJugador ? emiJug : emiNPC;

                if (emiActual == null) continue;

                _dialogos_EnCallback.Enqueue(() => emiActual.Hablar(d.dialogo, d.angulo, d.offset, d.escala, d.duracion));
                _proximosDialogos.Add(d);
            }

            _dialogos_EnCallback.Dequeue()?.Invoke();
            _tiempoInicio = Time.time;

            EnDialogo = true;
        }
    }

}
