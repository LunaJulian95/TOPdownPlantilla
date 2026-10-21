using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{

    public Image[] tabImages;
    public GameObject[] pages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Activar la primera pestaña por defecto
        OnClickTab(0);
    }

    /// <summary>
    /// Metodo que se llama cada vez que se cambia de pestaña en el menu
    /// </summary>
    /// <param name="tabNumber">Identificador de la pestaña de 0 a 3</param>
    public void OnClickTab( int tabNumber) 
    {
        for (int i = 0; i <this.pages.Length; i++)
        {
            tabImages[i].color = (i  == tabNumber) ? Color.white : Color.gray;
            pages[i].SetActive(false);

        }
        tabImages[tabNumber].color = Color.white;
        pages[tabNumber].SetActive(true);
    }

  
}
