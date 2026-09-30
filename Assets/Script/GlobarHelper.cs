using UnityEngine;

public class GlobarHelper 
{
   public static string  GenerateUniqueID(GameObject g)
    {

        //UI ID
        return$"{g.scene.name}_{g.name}_{g.transform.position.x}_{g.transform.position.y}";
    }
}
