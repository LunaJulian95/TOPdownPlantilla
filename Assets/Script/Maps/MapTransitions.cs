using UnityEngine;
using Unity.Cinemachine;

public class MapTransitions : MonoBehaviour
{

    [SerializeField] private PolygonCollider2D mapBoundary;
    CinemachineConfiner2D confiner;

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    [SerializeField] Direction direction;

    [SerializeField][Range(0,5)] float amountToMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        confiner = FindAnyObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            confiner.BoundingShape2D = mapBoundary;
            UpdatePlayerPosition(collision.gameObject);
            SoundMusicManager.Instance.PlayMusicWithCrossFade(mapBoundary.name);    

            MapControllerManual.Instance?.HigtligthArea(mapBoundary.name);
            MapControllerDynamic.Instance?.UpdateCurrentArea(mapBoundary.name);
        }
    }

    void UpdatePlayerPosition(GameObject player) 
    {
        Vector3 newposition = player.transform.position;
        switch (this.direction) 
        {
            case Direction.Up:
                newposition += Vector3.up * 5f;
                break;
            case Direction.Down:
                newposition += Vector3.down * 5f;
                break;
            case Direction.Left:
                newposition += Vector3.left * 5f;
                break;
            case Direction.Right:
                newposition += Vector3.right * 5f;
                break;
        }
    }
    


}
