using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    [Range(-1f, 1f)]
    public float effect;
    public bool grupo;

    private Camera cam;
    private SpriteRenderer spriteRnd;
    private List<SpriteRenderer> spritesRnds = new List<SpriteRenderer>();

    void Start()
    {
        cam = Camera.main;
        if (!grupo)
            spriteRnd = GetComponent<SpriteRenderer>();
        else
            GetComponentsInChildren<SpriteRenderer>(spritesRnds);

        ControlJugador.Jugador.EnMovimiento += Jugador_EnMovimiento;
    }

    private void OnValidate()
    {
        int orden = Mathf.RoundToInt(effect * 100f);
        orden = -(orden > 0f ? orden : orden - 1);

        if (!grupo)
        {
            if (!spriteRnd) spriteRnd = GetComponent<SpriteRenderer>();
            spriteRnd.sortingOrder = orden;
        }
        else
        {
            if (!spritesRnds.Any()) GetComponentsInChildren<SpriteRenderer>(spritesRnds);
            foreach (var sr in spritesRnds) sr.sortingOrder = orden;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (!cam) cam = Camera.main;

        Bounds sB;
        if (!grupo)
        {
            if (!spriteRnd) spriteRnd = GetComponent<SpriteRenderer>();
            sB = spriteRnd.bounds;
        }
        else
        {
            if (!spritesRnds.Any()) GetComponentsInChildren<SpriteRenderer>(spritesRnds);
            sB = SumarLimites(spritesRnds);
        }

        if (EnVista(sB, out var cB))
            Gizmos.color = Color.red;

        Gizmos.DrawWireCube(sB.center, sB.size);
        Gizmos.DrawWireCube(cB.center, cB.size);
    }

    void OnDisable() => ControlJugador.Jugador.EnMovimiento -= Jugador_EnMovimiento;

    private Bounds SumarLimites(List<SpriteRenderer> sprites)
    {
        Bounds sB = sprites[0].bounds;
        for (int i = 1; i < spritesRnds.Count; i++)
            sB.Encapsulate(spritesRnds[i].bounds);

        return sB;
    }

    private void Jugador_EnMovimiento(bool enTP, Vector3 delta)
    {
        if (!enTP) CalcularParallax(delta);  
    }
    
    private void CalcularParallax(Vector3 delta)
    {
        Bounds limites = !grupo ? spriteRnd.bounds : SumarLimites(spritesRnds);

        if (!EnVista(limites)) return;

        Vector3 dist = delta * effect;
        transform.position += dist;
    }

    private bool EnVista(Bounds spriteBound)
    {
        Bounds camBounds = new Bounds();
        camBounds.SetMinMax(cam.ViewportToWorldPoint(Vector2.zero), cam.ViewportToWorldPoint(Vector2.one));
        camBounds.size += Vector3.forward * 100f;

        return spriteBound.Intersects(camBounds);
    }

    private bool EnVista(Bounds spriteBound, out Bounds camBounds)
    {
        camBounds = new Bounds();
        camBounds.SetMinMax(cam.ViewportToWorldPoint(Vector2.zero), cam.ViewportToWorldPoint(Vector2.one));
        camBounds.size += Vector3.forward * 100f;

        return spriteBound.Intersects(camBounds);
    }
}
