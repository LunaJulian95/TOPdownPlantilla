
using UnityEngine;
using Unity.Cinemachine;
using System.Threading.Tasks;

public class MapTransitions : MonoBehaviour
{
    [Header("Mapa")]
    [SerializeField] private PolygonCollider2D mapBoundary;

    private CinemachineConfiner2D confiner;

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Teleport
    }

    [Header("Transición")]
    [SerializeField] private Direction direction;
    [SerializeField] private Transform teleportTargetPosition;

    [SerializeField, Range(0, 5)]
    private float amountToMove = 5f;

    private bool isTransitioning = false;

    private void Start()
    {
        confiner = FindAnyObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (isTransitioning)
            return;

        FadeTransition(collision.gameObject);

        if (SoundMusicManager.Instance != null)
            SoundMusicManager.Instance.PlayMusicWithCrossFade(mapBoundary.name);

        MapControllerManual.Instance?.HigtligthArea(mapBoundary.name);
        MapControllerDynamic.Instance?.UpdateCurrentArea(mapBoundary.name);
    }

    private async void FadeTransition(GameObject player)
    {

        isTransitioning = true;

        await ScreenFader.Instance.FadeOut();

        // Cambiar el límite de la cámara.
        if (confiner != null && mapBoundary != null)
        {
            confiner.BoundingShape2D = mapBoundary;
            confiner.InvalidateBoundingShapeCache();
        }

        // Mover al jugador.
        UpdatePlayerPosition(player);

        // Esperar a que Cinemachine actualice la cámara.
        await Task.Yield();
        await Task.Yield();

        await ScreenFader.Instance.FadeIn();

        isTransitioning = false;
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        if (direction == Direction.Teleport)
        {
            if (teleportTargetPosition != null)
                player.transform.position = teleportTargetPosition.position;

            return;
        }

        Vector3 newPosition = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPosition += Vector3.up * amountToMove;
                break;

            case Direction.Down:
                newPosition += Vector3.down * amountToMove;
                break;

            case Direction.Left:
                newPosition += Vector3.left * amountToMove;
                break;

            case Direction.Right:
                newPosition += Vector3.right * amountToMove;
                break;
        }

        player.transform.position = newPosition;
    }
}
