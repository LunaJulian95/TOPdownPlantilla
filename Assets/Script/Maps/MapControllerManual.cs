using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapControllerManual : MonoBehaviour
{
    public static MapControllerManual Instance { get; set; }

    public GameObject mapParent;

    private List<Image> mapImages;

    public Color highligthColor = Color.gold;

    public Color dimmedColor = new Color(1f, 1f, 1f, 0.5f);

    public RectTransform playerIconTransform;

    private void Awake()
    {
        if(Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            
        }else
        {
            Instance = this;
        }

        mapImages =     mapParent.GetComponentsInChildren<Image>().ToList();


    }

    public void HigtligthArea(string areaName)
    {
        foreach(Image area in  mapImages)
        {
            area.color = dimmedColor;
        }

        Image currentArea = mapImages.Find(area => area.name == areaName);
        if(currentArea != null) 
        {
            currentArea.color = highligthColor;
            playerIconTransform.position = currentArea.GetComponent<RectTransform>().position;
        }
        else 
        {
            Debug.LogWarning($"Area {areaName} not found in map images.");
        }

    }

}
