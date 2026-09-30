using UnityEngine;

public class ExperienceGem : MonoBehaviour
{
    public int xpValue = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLevel playerLevel = other.GetComponent<PlayerLevel>();
            if (playerLevel != null)
            {
                playerLevel.AddXP(xpValue);
            }
            Destroy(gameObject);
        }
    }
}