
using TMPro;
using UnityEditor;
using UnityEngine;

public class ReplaceTMPFont
{
    [MenuItem("Tools/UI/Reemplazar fuente de toda la UI")]
    static void ReplaceFont()
    {
        TMP_FontAsset newFont = Selection.activeObject as TMP_FontAsset;

        if (newFont == null)
        {
            Debug.LogError(
                "Seleccioná primero el Font Asset de Ortoid en Project."
            );
            return;
        }

        TMP_Text[] texts =
            Resources.FindObjectsOfTypeAll<TMP_Text>();

        int count = 0;

        foreach (TMP_Text text in texts)
        {
            if (text == null || !text.gameObject.scene.IsValid())
                continue;

            Undo.RecordObject(text, "Cambiar fuente de UI");

            text.font = newFont;
            EditorUtility.SetDirty(text);

            count++;
        }

        Debug.Log("Fuente cambiada en " + count + " textos.");
    }
}

