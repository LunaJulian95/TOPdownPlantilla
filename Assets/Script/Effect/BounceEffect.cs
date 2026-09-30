using System.Collections;
using UnityEngine;

public class BounceEffect : MonoBehaviour
{
    public float bounceHeigth = 0.3f;
    public float bounceDuration = 0.4f;
    public float bounceCount = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     public void StartBounce()
    {
        //llamamos a la courrituna
        StartCoroutine(BounceHandler());
    }

   private IEnumerator BounceHandler() 
    {
        Vector3 startPosition = transform.position;
        float localHeigth = bounceHeigth;
        float localDuration = bounceDuration;

        for(int i = 0;  i < bounceCount; i++) 
        {
            //añadir otra currutina para saltar
            yield return StartCoroutine(Bounce(startPosition, localHeigth, localDuration));
            localHeigth *= 0.5f;
            localDuration *= 0.8f;

        }
        transform.position = startPosition;
    }

    private IEnumerator Bounce(Vector3 startPosition,float heigth,float duration) 
    {
        Vector3 peakPosition = startPosition + Vector3.up * heigth;
        float elapsedtime = 0f;

        //Primero movemos el objeto hacia arriba
        while (elapsedtime < duration) 
        {
            transform.position = Vector3.Lerp(startPosition,peakPosition,elapsedtime/duration);
            elapsedtime += Time.deltaTime;
            yield return null;
        }
        elapsedtime = 0f;
        // ahora hacia abajo

        while(elapsedtime < duration) 
        {
            transform.position = Vector3.Lerp(peakPosition, startPosition, elapsedtime);
            elapsedtime += Time.deltaTime;
            yield return null;
        }

        


    }
}
