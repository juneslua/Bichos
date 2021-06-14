using System;
using UnityEngine;
using System.Collections;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public class ControladorBoolAttribute : PropertyAttribute
{
    public string Campo { get; }
    public bool Valor { get; }
    
    /// <summary>
    /// Se mostrara en el editor si el <em>campo</em> boleano es igual  <strong>valor</strong> (Default: true).
    /// </summary>
    /// <param name="campo">Bool con el cual se realizara la comparacion.</param>
    public ControladorBoolAttribute(string campo, bool valor = true)
    {
        this.Campo = campo;
        this.Valor = valor;
    }
}
