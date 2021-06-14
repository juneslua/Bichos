using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(ControlDialogos))]
public class ControlDialogosEditor : Editor
{
    private static Material _cutout;

    private ReorderableList _lista;
    private SerializedProperty _conversaciones;

    private Dictionary<string, ReorderableList> _listasInternas = new Dictionary<string, ReorderableList>();

    private Texture2D[] _bocadillos = new Texture2D[4];

    void OnEnable()
    {
        var estilo = serializedObject.FindProperty("_estilo");
        if(estilo.objectReferenceValue != null)
            ActualizarBocadillos((Texture2D)estilo.objectReferenceValue);

        _cutout = new Material(Shader.Find("Unlit/Transparent Cutout"));
        _conversaciones = serializedObject.FindProperty("_conversaciones");

        float altoLinea = EditorGUIUtility.singleLineHeight;
        float espaciado = EditorGUIUtility.standardVerticalSpacing * 2f;
        float alturaDialogo = altoLinea * 7.5f;

        // CONVERSACIONES - INICIO
        _lista = new ReorderableList(serializedObject, _conversaciones)
        {
            draggable = true,
            displayAdd = true,
            displayRemove = true,

            // CONVERSACIONES - HEADER
            drawHeaderCallback = rectHeader =>
                EditorGUI.LabelField(rectHeader, "Conversaciones"),

            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty elemeto = _lista.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += EditorGUIUtility.standardVerticalSpacing;

                // CONVERSACION - Nombre
                var nombre = elemeto.FindPropertyRelative("nombre");
                EditorGUI.PropertyField(MedidasLinea(rect), nombre);
                rect.y += altoLinea + espaciado * 2f;

                // COVERSACION - Efectos
                var efectos = elemeto.FindPropertyRelative("efectos");
                EditorGUI.PropertyField(MedidasLinea(rect), efectos);
                rect.y += altoLinea + espaciado * 2f;

                // CONVERSACION - DIALOGOS
                ReorderableList listaInter;
                string llave = elemeto.propertyPath;

                if (!_listasInternas.TryGetValue(llave, out listaInter))
                {
                    // DIALOGOS - INICIO
                    listaInter = new ReorderableList(elemeto.serializedObject, elemeto.FindPropertyRelative("dialogos"))
                    {
                        draggable = true,
                        displayAdd = true,
                        displayRemove = true,

                        // DIALOGOS - HEADER
                        drawHeaderCallback = rectHeader =>
                            EditorGUI.LabelField(rectHeader, "Dialogos (" + nombre.stringValue + ")"),

                        drawElementCallback = (rectInter, indexInter, isActiveInter, isFocusedInter) =>
                        {
                            SerializedProperty elemtoInter = listaInter.serializedProperty.GetArrayElementAtIndex(indexInter);
                            Rect rectTemp;

                            rectInter.x += espaciado;
                            rectInter.y += espaciado;

                            // DIALOGOS - Texturas
                            var dialogo = elemtoInter.FindPropertyRelative("dialogo");
                            var angulo = elemtoInter.FindPropertyRelative("angulo");

                            Texture2D txtDialogo = (Texture2D)dialogo.objectReferenceValue;


                            // DIALOGOS - Texturas -> Cuadrado
                            float medidaTxt = rectInter.height - espaciado * 2f;
                            Rect sqrRect = new Rect(rectInter.x, rectInter.y, medidaTxt, medidaTxt);
                            EditorGUI.DrawRect(sqrRect, new Color(0.75f, 0.75f, 0.75f));

                            // DIALOGOS - Texturas -> Bocadillo 
                            Vector2 dir = new Vector2(Mathf.Cos(angulo.floatValue * Mathf.Deg2Rad), Mathf.Sin(angulo.floatValue * Mathf.Deg2Rad));
                            if (estilo.objectReferenceValue)
                            {
                                Texture2D bocadillo = _bocadillos[0];
                                if (dir.x < 0f && dir.y > 0f)
                                    bocadillo = _bocadillos[1];
                                else if (dir.x < 0f && dir.y < 0f)
                                    bocadillo = _bocadillos[2];
                                else if (dir.x > 0f && dir.y < 0f)
                                    bocadillo = _bocadillos[3];

                                EditorGUI.DrawPreviewTexture(EscalarConcentrico(sqrRect, 0.95f), bocadillo, _cutout, ScaleMode.ScaleToFit);
                            }

                            // DIALOGOS - Texturas -> Dialogo
                            var offset = elemtoInter.FindPropertyRelative("offset");
                            var escala = elemtoInter.FindPropertyRelative("escala");

                            if (txtDialogo)
                            {
                                rectTemp = EscalarConcentrico(sqrRect, 0.95f);
                                Vector2 o = offset.vector2Value * rectTemp.width, s = Vector2.one * rectTemp.width * escala.floatValue;
                                bool xFlip = dir.x < 0f, yFlip = dir.y < 0f;

                                Vector2 p;
                                p.x = (rectTemp.x - s.x * 0.5f) + (xFlip ? rectTemp.width - o.x : o.x);
                                p.y = (rectTemp.y - s.y * 0.5f) + (yFlip ? rectTemp.height - o.y : o.y);
                                rectTemp = new Rect(p, s); 
                                EditorGUI.DrawPreviewTexture(rectTemp, txtDialogo, _cutout, ScaleMode.ScaleToFit);
                            }

                            rectInter.x += medidaTxt + espaciado * 2f;
                            rectInter.y += espaciado;
                            rectInter.width -= medidaTxt + espaciado * 4f;

                            // DIALOGOS - Texturas -> Field
                            EditorGUI.ObjectField(MedidasLinea(rectInter), dialogo, GUIContent.none);
                            rectInter.y += altoLinea + espaciado;

                            // DIALOGOS - Angulo
                            rectTemp = AgregarEtiqueta(MedidasLinea(rectInter), "Angulo", 0.3f);
                            bool auto = elemtoInter.FindPropertyRelative("autoOrientar").boolValue;
                            if (auto) GUI.enabled = false;
                            EditorGUI.Slider (MedidasLinea(rectTemp), angulo, 0f, 360f, GUIContent.none);
                            GUI.enabled = true;
                            rectInter.y += altoLinea + espaciado;

                            // DIALOGOS - Offset
                            GUIContent[] labels = new GUIContent[]
                            {
                                new GUIContent("X"),
                                new GUIContent("Y"),
                            };

                            float[] vals = new float[] { offset.vector2Value.x, offset.vector2Value.y};

                            rectTemp = MedidasLinea(rectInter);
                            rectTemp.width *= 0.5f;

                            EditorGUI.MultiFloatField(rectTemp, labels, vals);
                            Vector2 off = new Vector2(vals[0], vals[1]);
                            off.x = off.x < 0f ? 0f : off.x > 1f ? 1f : off.x;
                            off.y = off.y < 0f ? 0f : off.y > 1f ? 1f : off.y;
                            offset.vector2Value = off;

                            // DIALOGOS - Escala
                            rectTemp = MedidasLinea(rectInter);
                            rectTemp.x += rectTemp.width * 0.52f;
                            rectTemp.width *= 0.48f;

                            rectTemp = AgregarEtiqueta(rectTemp, "Escala", 0.4f);
                            float val = EditorGUI.FloatField(rectTemp, escala.floatValue);
                            escala.floatValue = val < 0f ? 0f : val > 1f ? 1f : val;
                            rectInter.y += altoLinea + espaciado * 2f;

                            // DIALOGOS - Duracion
                            var duracion = elemtoInter.FindPropertyRelative("duracion");
                            rectTemp = AgregarEtiqueta(MedidasLinea(rectInter), "s", 0.03f, 0.01f, false);
                            float nuevoVal = EditorGUI.FloatField(rectTemp, "Duracion", duracion.floatValue);
                            duracion.floatValue = nuevoVal >= 0f ? nuevoVal : 0f;
                            rectInter.y += altoLinea + espaciado * 2f;
                            
                            // DIALOGOS - Opciones
                            String[] leyenda = new string[] { "Jugador", "No esperar", "Auto-Orientar" }; 
                            SerializedProperty[] bools = new SerializedProperty[]
                            {
                                elemtoInter.FindPropertyRelative("dialogoJugador"),
                                elemtoInter.FindPropertyRelative("noEsperar"),
                                elemtoInter.FindPropertyRelative("autoOrientar")
                            };

                            Rect boolRect = MedidasLinea(rectInter);
                            boolRect.width *= 0.28f;
                            for (int i = 0; i < bools.Length; i++)
                            {
                                rectTemp = AgregarEtiqueta(boolRect, leyenda[i], 0.8f, 0f, false);
                                EditorGUI.PropertyField(rectTemp, bools[i], GUIContent.none);
                                boolRect.x += boolRect.width;
                                boolRect.width = rectInter.width * 0.36f;
                            }

                        },

                        // JUGADOR
                        elementHeightCallback = indexInter =>
                        {
                            return alturaDialogo; 
                        }
                    }; // DIALOGOS - FIN

                    _listasInternas[llave] = listaInter;
                }

                listaInter.DoList(new Rect(rect));
            },

            elementHeightCallback = index =>
            {
                var listaInter = _lista.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("dialogos");
                if (listaInter.arraySize > 0)
                    return listaInter.arraySize * alturaDialogo + altoLinea * 6f;
                else
                    return altoLinea * 7.5f;
            }

        }; // CONVERSACIONES - FIN

        Rect MedidasLinea(Rect r) => new Rect(r.x, r.y, r.width, EditorGUIUtility.singleLineHeight);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Space(5f);
        EditorGUILayout.LabelField("Emisor");

        GUILayout.BeginHorizontal();
        var emisor = serializedObject.FindProperty("_emisor");
        GUI.enabled = false;
        EditorGUILayout.PropertyField(emisor, GUIContent.none);

        GUI.enabled = !emisor.objectReferenceValue;

        if (GUILayout.Button("Crear emisor de dialogos"))
        {
            ControlDialogos obj = serializedObject.targetObject as ControlDialogos;

            Transform padre = obj.transform.parent;
            EmisorDialogos emisorComp = padre.GetComponent<EmisorDialogos>();

            if (padre != null && emisorComp == null)
                emisorComp = padre.gameObject.AddComponent<EmisorDialogos>();

            emisor.objectReferenceValue = emisorComp;
        }

        GUI.enabled = emisor.objectReferenceValue;

        GUILayout.EndHorizontal();
        GUILayout.Space(10f);

        var estilo = serializedObject.FindProperty("_estilo");
        EditorGUILayout.PropertyField(estilo);
        GUILayout.Space(10f);

        _lista.DoLayoutList();
        GUILayout.Space(10f);

        GUI.enabled = true;

        EditorGUI.BeginChangeCheck();

        Texture2D txt = (Texture2D)estilo.objectReferenceValue;

        if (txt && txt != _bocadillos[0])
            ActualizarBocadillos(txt);

        EditorGUI.EndChangeCheck();
        
        serializedObject.ApplyModifiedProperties();
    }

    private static Rect EscalarConcentrico(Rect rect, float escala)
    {
        float mEsc = (1 - escala) / 2f;
        rect.x += rect.width * mEsc;
        rect.y += rect.height * mEsc;

        rect.width *= escala;
        rect.height *= escala;

        return rect;
    }

    private void ActualizarBocadillos(Texture2D bocadillo)
    {
        _bocadillos[0] = bocadillo;
        _bocadillos[1] = Espejo(bocadillo);
        _bocadillos[2] = Espejo(Espejo(bocadillo), false);
        _bocadillos[3] = Espejo(bocadillo, false);
    }

    private static Texture2D Espejo(Texture2D textura, bool horizontal = true)
    {
        int w = textura.width, h = textura.height;
        Texture2D txt = new Texture2D(w, h);

        for (int pX = 0; pX < w; pX++)
            for (int pY = 0; pY < textura.width; pY++)
            {
                Color p = textura.GetPixel(pX, pY);
                if (horizontal)
                    txt.SetPixel(w - pX - 1, pY, p);
                else 
                    txt.SetPixel(pX , h - pY - 1, p);
            }

        txt.Apply();
        return txt;
    }

    private static Rect AgregarEtiqueta(Rect rect, string texto, float porcentaje, float offset = 0f, bool izquierda = true)
    {
        Rect propRect = rect;
        rect.x += rect.width * offset;

        rect.width *= porcentaje - offset;
        propRect.width *= 1f - porcentaje;

        propRect.x += izquierda ? rect.width : 0f;
        rect.x += (!izquierda ? propRect.width : 0f);
        

        EditorGUI.LabelField(rect, texto);
        return propRect;
    }
}