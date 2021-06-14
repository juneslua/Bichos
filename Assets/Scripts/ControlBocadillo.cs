using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBocadillo : MonoBehaviour
{
    public float Duracion { get; set; }

    private CanvasGroup _canGroup;
    private InterpolacionFloat _interFloat;

    private float _tiempoCreacion;

    void Awake()
    {
        _canGroup = GetComponent<CanvasGroup>();

        _interFloat = new InterpolacionFloat(GetInstanceID(), 0.5f);
        _interFloat.IniciarInterpolacion(1f);
        _interFloat.EnBlancoAlcanzado += Interpolacion_EnBlancoAlcanzado;

        _tiempoCreacion = Time.time;
    }

    void Update()
    {
        if (_interFloat.Blanco.HasValue)
        {
            _interFloat.Tick(Time.deltaTime);
            _canGroup.alpha = _interFloat.ValorActual;
        }
    }

    void OnDisable()
    {
        _interFloat.EnBlancoAlcanzado -= Interpolacion_EnBlancoAlcanzado;
    }

    private void Interpolacion_EnBlancoAlcanzado(int id, float blanco)
    {
        if (GetInstanceID() == id)
        {
            if (blanco == 0f)
                Destroy(gameObject);
            else
                StartCoroutine("EsperarDuracion");
        }
    }

    private IEnumerator EsperarDuracion()
    {
        yield return new WaitForSeconds(Duracion - (Time.time - _tiempoCreacion));
        _interFloat.IniciarInterpolacion(0f);
    }

}
