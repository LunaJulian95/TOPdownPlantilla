using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject minimap;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(!menuCanvas.activeSelf && PauseController.IsGamePaused) 
            {
                return;//si estamos pausados pero sin abrir el menu
            }
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            minimap.SetActive(!minimap.activeSelf);
            PauseController.SetPause(menuCanvas.activeSelf);
           
        }
    }
}
