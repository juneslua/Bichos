using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InterpolacionFloat
{

    public event Action<int, float> EnBlancoAlcanzado;

    public int ID { get; }
    public float Duracion { get; set; } 
    public float ValorActual { get; private set; }
    public float? Blanco { get; private set; }


    private Queue<Action> _EnCallback = new Queue<Action>();

    private float _valorInicial = 0f, _tiempo = 0f;

    public InterpolacionFloat(int id) : this (id, 0f, 0f) { }

    public InterpolacionFloat(int id, float duracion) : this (id, duracion, 0f) { }

    public InterpolacionFloat(int id, float duracion, float valorActual)
    {
        ID = id;
        Duracion = duracion;
        ValorActual = valorActual;
    }


    public static float Lerp(float inicial, float final, float porcentaje)
    {
        float delta = final - inicial;
        porcentaje = Mathf.Clamp(porcentaje, 0f, 1f);

        return inicial  + delta * porcentaje;
    }


    public void Tick(float deltaTiempo)
    {
        if (_tiempo >= Duracion || !Blanco.HasValue) return;

        _tiempo += deltaTiempo;
        float porcentaje = _tiempo / Duracion;
        ValorActual = Lerp(_valorInicial, Blanco.Value, porcentaje);

        if (_tiempo >= Duracion)
        {
            EnBlancoAlcanzado?.Invoke(ID, Blanco.Value);

            if (_EnCallback.Count > 0f)
            {
                _EnCallback.Dequeue()?.Invoke();
            }
            else
                Blanco = null;
        }
    }

    public void IniciarInterpolacion(float blanco) => CambiarA(blanco);

    private void CambiarA(float b)
    {
        _valorInicial = ValorActual;
        Blanco = b;

        _tiempo = 0f;
    }
}
