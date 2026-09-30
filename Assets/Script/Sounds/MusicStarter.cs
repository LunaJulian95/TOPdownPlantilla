using UnityEngine;

public class MusicStarter : MonoBehaviour
{
    private void Start()
    {
        SoundMusicManager.Instance.PlayMusicWithCrossFade("Forest");
    }
}
