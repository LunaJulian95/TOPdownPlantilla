using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapControllerDynamic : MonoBehaviour
{
    [Header("Referencias UI")]
    public RectTransform mapParent;
    public GameObject areaPrefab;
    public RectTransform playerIcon;

    [Header("Colores")]
    public Color defaultColor = Color.grey;
    public Color currentColor = Color.yellow;

    [Header("Mapa")]
    public GameObject mapBounds;
    public PolygonCollider2D initialArea;
    public float mapScale = 10f;

    private PolygonCollider2D[] mapAreas;

    private Dictionary<string, RectTransform> uiAreas =
        new Dictionary<string, RectTransform>();

    public static MapControllerDynamic Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Buscar automáticamente las áreas
        if (mapBounds != null)
        {
            mapAreas =
                mapBounds.GetComponentsInChildren<PolygonCollider2D>(
                    true
                );
        }

        Debug.Log(
            "MapControllerDynamic: Áreas encontradas = " +
            (mapAreas != null ? mapAreas.Length : 0)
        );
    }


    private void Start()
    {
        GenerateMap();
    }


    // =========================================================
    // GENERAR MAPA
    // =========================================================

    public void GenerateMap(
        PolygonCollider2D currentArea = null)
    {
        if (mapParent == null)
        {
            Debug.LogError(
                "MapControllerDynamic: mapParent no está asignado."
            );
            return;
        }

        if (areaPrefab == null)
        {
            Debug.LogError(
                "MapControllerDynamic: areaPrefab no está asignado."
            );
            return;
        }

        if (mapAreas == null || mapAreas.Length == 0)
        {
            Debug.LogError(
                "MapControllerDynamic: No se encontraron PolygonCollider2D."
            );
            return;
        }

        // Si no se indicó un área,
        // usamos initialArea
        if (currentArea == null)
        {
            currentArea = initialArea;
        }

        ClearMap();

        foreach (PolygonCollider2D area in mapAreas)
        {
            if (area == null)
                continue;

            CreateAreaUI(
                area,
                area == currentArea
            );
        }

        if (currentArea != null)
        {
            MoveIconPlayer(currentArea.name);
        }

        Debug.Log(
            "Mapa generado correctamente. Áreas UI: " +
            uiAreas.Count
        );
    }


    // =========================================================
    // LIMPIAR MAPA
    // =========================================================

    private void ClearMap()
    {
        foreach (Transform child in mapParent)
        {
            Destroy(child.gameObject);
        }

        uiAreas.Clear();
    }


    // =========================================================
    // CREAR ÁREA EN LA UI
    // =========================================================

    private void CreateAreaUI(
        PolygonCollider2D area,
        bool isCurrent)
    {
        GameObject areaObject =
            Instantiate(
                areaPrefab,
                mapParent
            );

        RectTransform areaRect =
            areaObject.GetComponent<RectTransform>();

        if (areaRect == null)
        {
            Debug.LogError(
                "areaPrefab no tiene RectTransform."
            );

            Destroy(areaObject);
            return;
        }


        // -----------------------------------------------------
        // POSICIÓN
        // -----------------------------------------------------

        Bounds bounds = area.bounds;

        areaRect.anchoredPosition =
            new Vector2(
                bounds.center.x * mapScale,
                bounds.center.y * mapScale
            );


        // -----------------------------------------------------
        // TAMAÑO
        // -----------------------------------------------------

        areaRect.sizeDelta =
            new Vector2(
                Mathf.Max(
                    bounds.size.x * mapScale,
                    10f
                ),
                Mathf.Max(
                    bounds.size.y * mapScale,
                    10f
                )
            );


        // -----------------------------------------------------
        // COLOR
        // -----------------------------------------------------

        Image image =
            areaObject.GetComponent<Image>();

        if (image != null)
        {
            image.color =
                isCurrent
                    ? currentColor
                    : defaultColor;
        }


        // -----------------------------------------------------
        // NOMBRE
        // -----------------------------------------------------

        areaObject.name =
            "MapArea_" + area.name;


        // -----------------------------------------------------
        // GUARDAR REFERENCIA
        // -----------------------------------------------------

        uiAreas[area.name] =
            areaRect;
    }


    // =========================================================
    // CAMBIAR ÁREA ACTUAL
    // =========================================================

    public void UpdateCurrentArea(
        string newCurrentArea)
    {
        foreach (
            KeyValuePair<
                string,
                RectTransform
            > area in uiAreas)
        {
            Image image =
                area.Value.GetComponent<Image>();

            if (image != null)
            {
                image.color =
                    area.Key == newCurrentArea
                        ? currentColor
                        : defaultColor;
            }
        }

        MoveIconPlayer(
            newCurrentArea
        );
    }


    // =========================================================
    // MOVER ICONO DEL JUGADOR
    // =========================================================

    private void MoveIconPlayer(
        string areaName)
    {
        if (playerIcon == null)
        {
            Debug.LogWarning(
                "MapControllerDynamic: playerIcon no está asignado."
            );

            return;
        }

        if (
            uiAreas.TryGetValue(
                areaName,
                out RectTransform areaRect
            )
        )
        {
            playerIcon.anchoredPosition =
                areaRect.anchoredPosition;
        }
        else
        {
            Debug.LogWarning(
                "No se encontró el área UI: " +
                areaName
            );
        }
    }


    // =========================================================
    // BUSCAR ÁREA POR NOMBRE
    // =========================================================

    public PolygonCollider2D GetAreaByName(
        string areaName)
    {
        if (mapAreas == null)
            return null;

        foreach (
            PolygonCollider2D area
            in mapAreas)
        {
            if (
                area != null &&
                area.name == areaName)
            {
                return area;
            }
        }

        return null;
    }


    // =========================================================
    // CARGAR ÁREA GUARDADA
    // =========================================================

    public void LoadCurrentArea(
        string areaName)
    {
        PolygonCollider2D area =
            GetAreaByName(areaName);

        if (area == null)
        {
            Debug.LogWarning(
                "No existe un área llamada: " +
                areaName
            );

            GenerateMap();

            return;
        }

        GenerateMap(area);
    }
}