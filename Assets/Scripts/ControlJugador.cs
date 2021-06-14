using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Input;
using static UnityEngine.Mathf;

public struct Controles
{
    public Vector2 flechas;
    public bool salto;

    public void SetSalto(bool estado) => salto = estado;
}

public class ControlJugador : MonoBehaviour
{
    public event Action<int> EnInteraccion;
    public event Action<bool, Vector3> EnMovimiento;

    public static ControlJugador Jugador { get; private set; }

    public bool EnSuelo { get; private set; }
    public int FramesEnAire { get => _framesEnAire; }
    public Vector2 Velocidad { get => _rb2D.velocity; }
    public Controles Control { get => _control; set => _control = value; }
    public EmisorDialogos Emisor { get; private set; }

    [Header("Movimiento")]
    [SerializeField, Range(0f, 5f)]
    private float _alturaSalto = 1.5f;
    [SerializeField, Range(0f, 25f)]
    private float _velocidadMax = 5f, _acelercionMax = 5f, _aceleracionAereaMax = 2f;
    [SerializeField, Range(0f, 90f)]
    private float _maximaPendiente = 60f;
    [SerializeField, Range(0f, 25f)]
    private float _velocidadLanzamiento = 4.6f;

    [Header("Interaccion"), SerializeField]
    private GameObject _botonInteraccion = null;

    // Inputs
    private Controles _control;

    // Camara
    private Camera _camPrincipal;
    private Vector3 _prevPosition;

    // Movimiento
    private Rigidbody2D _rb2D;
    private SpriteRenderer _spriteRnd;
    private Vector2 _momento, _normalSuelo;
    private float _minYNormal;
    private int _framesEnAire = 0, _framesEnSalto = 0;

    // Interaccion
    private bool EnRangoInteraccion
    {
        get => _enRangoInteraccion;
        set
        {
            if (value != _enRangoInteraccion)
                _botonInteraccion.SetActive(value);
            _enRangoInteraccion = value;
        }
    }
    private bool _enRangoInteraccion = false;
    private GameObject _objetoEnRango;
    

    //  ------- UNITY - Metodos ------------------------------------------------
    void Awake()
    {
        if (Jugador == null) Jugador = this;
        else Destroy(gameObject);

        _camPrincipal = Camera.main;
        _prevPosition = transform.position;

        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRnd = GetComponentInChildren<SpriteRenderer>();

        Emisor = GetComponent<EmisorDialogos>();

        OnValidate();
    }

    void OnValidate()
    {
        _minYNormal = Cos(_maximaPendiente * Deg2Rad);
    }

    void FixedUpdate()
    {
        ActualizarFisicas();

        AjustarVelocidad();
        if (_control.salto)
        {
            Saltar();
        }

        _rb2D.velocity = _momento;

        EnSuelo = false;
        _normalSuelo = Vector2.zero;
    }

    void Update()
    {
        Vector2 inputAxis = new Vector2(GetAxis("Horizontal"), 0f);
        _control.flechas = Vector2.ClampMagnitude(inputAxis, 1f) * _velocidadMax;

        if (EnRangoInteraccion && GetButtonDown("Interact"))
            EnInteraccion?.Invoke(_objetoEnRango.GetInstanceID());
    }

    void LateUpdate()
    {
        bool? movDer = _control.flechas.x != 0 ? _control.flechas.x > 0 : (bool?) null;
        if (movDer != null) _spriteRnd.flipX = !(bool)movDer;

        Vector2 delta = transform.position - _prevPosition;

        // Movimiento camara
        _camPrincipal.transform.Translate(delta, Space.World);

        // Parallax
        float mag = delta.magnitude;
        if (mag > 0.001f) EnMovimiento?.Invoke(Puerta.EnTP, delta);

        _prevPosition = transform.position;
        Puerta.EnTP = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        BuscarSuelo(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        BuscarSuelo(collision);    
    }

    void OnTriggerStay2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.layer == LayerMask.NameToLayer("Interactuable"))
        {
           EnRangoInteraccion = true;
            _objetoEnRango = collider2D.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        EnRangoInteraccion = false;
        _objetoEnRango = null;
    }


    //  ------- METODOS ------------------------------------------------
    private void ActualizarFisicas()
    {
        _momento = Velocidad;
        _framesEnAire++;
        _framesEnSalto++;

        if (EnSuelo || Snap())
        {
            _framesEnAire = 0;
            _normalSuelo.Normalize();
        }
        else
            _normalSuelo = Vector2.up;
    }

    private void AjustarVelocidad()
    {
        Vector2 localXaxis = Proyectar(Vector2.right).normalized;
        float localXVel = Vector2.Dot(_momento, localXaxis);

        float aceleracion = EnSuelo ? _acelercionMax : _aceleracionAereaMax * Time.fixedDeltaTime;
        float nXVel = MoveTowards(localXVel, _control.flechas.x, aceleracion);

        _momento += localXaxis * (nXVel - localXVel);
    }

    private void Saltar()
    {
        if (EnSuelo)
        {
            _framesEnSalto = 0;

            float velY = Sqrt(-2 * Physics2D.gravity.y * _alturaSalto);
            float dot = Vector2.Dot(_momento, _normalSuelo);
            if (dot > 0f)
                velY = Max(velY - dot, 0f);

            _momento += _normalSuelo * velY;
        }

        _control.salto = false;
    }

    private void BuscarSuelo(Collision2D cols)
    {
        for (int i = 0; i < cols.contactCount; i++)
        {
            Vector2 normal = cols.contacts[i].normal;
            if (normal.y >= _minYNormal)
            {
                _normalSuelo += normal;
                EnSuelo = true;
            }
        }
    }

    private bool Snap()
    {
        bool noSnap = _framesEnAire > 1 || _framesEnSalto <= 2;
        
        float mag = _momento.magnitude;
        noSnap |= _velocidadLanzamiento < mag;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f);
        noSnap |= !hit;
        noSnap |= hit.normal.y < _minYNormal;

        if (noSnap) return false;

        _normalSuelo = hit.normal;
        float dot = Vector2.Dot(_momento, _normalSuelo);
        if (dot > 0f)
            _momento = (_momento - _normalSuelo * dot).normalized * mag;

        Debug.DrawRay(transform.position, _momento, Color.green);

        return true;
    }

    private Vector2 Proyectar(Vector2 vector) => vector - _normalSuelo * Vector2.Dot(vector, _normalSuelo);
}
