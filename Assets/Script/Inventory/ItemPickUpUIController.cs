using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUpUIController : MonoBehaviour
{
    public static ItemPickUpUIController Instance { get; private set; }

    public GameObject popupPrefab;
    public int maxPopups = 5;
    public float popupDuration = 2f;

    private readonly Queue<GameObject> activePopups = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogError("Multiple instances of ItemPickUpUIController detected. There should only be one instance in the scene.");
            Destroy(gameObject);
        }
    }

    public void ShowItemPickUp(string itemName, Sprite itemSprite)
    {
        // Instanciamos el popup
        GameObject popup = Instantiate(popupPrefab, transform);

        // Nombre del objeto
        TMP_Text text = popup.GetComponentInChildren<TMP_Text>();

        if (text != null)
        {
            text.text = itemName;
        }

        // Icono
        Transform iconTransform = popup.transform.Find("Item Icon");

        if (iconTransform != null)
        {
            Image image = iconTransform.GetComponent<Image>();

            if (image != null)
            {
                image.sprite = itemSprite;
            }
        }
        else
        {
            Debug.LogError("Item Icon not found in the popup prefab.");
        }

        // Agregamos el popup a la cola
        activePopups.Enqueue(popup);

        // Si superamos el máximo, destruimos el más antiguo
        if (activePopups.Count > maxPopups)
        {
            GameObject oldestPopup = activePopups.Dequeue();

            if (oldestPopup != null)
            {
                Destroy(oldestPopup);
            }
        }

        // Fade out
        StartCoroutine(FadeOutAndDestroyPopUp(popup));
    }

    private IEnumerator FadeOutAndDestroyPopUp(GameObject popup)
    {
        yield return new WaitForSeconds(popupDuration);

        if (popup == null)
        {
            yield break;
        }

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup not found in popup prefab.");
            Destroy(popup);
            yield break;
        }

        for (float timePassed = 0f; timePassed < 1f; timePassed += Time.deltaTime)
        {
            if (popup == null)
            {
                yield break;
            }

            canvasGroup.alpha = 1f - timePassed;

            yield return null;
        }

        Destroy(popup);
    }
}