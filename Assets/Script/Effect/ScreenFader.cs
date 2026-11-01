using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] private CinemachineCamera Ccam;

    CinemachineFramingTransposer transposer;
    Vector3 originalDamping;

    private void Awake()
    {
        if(Instance == null)
        
            Instance = this;
        
        else
        
            Destroy(gameObject);

        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        originalDamping = new Vector3(transposer.m_XDamping, transposer.m_YDamping, transposer.m_ZDamping);

    }

    async Task Fade(float targetTransparency)
    {
        float start = canvasGroup.alpha,t = 0;
        while(t< fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetTransparency, t / fadeDuration);
            await Task.Yield();
        }
        canvasGroup.alpha = targetTransparency;
    }

    public async Task FadeOut()
    {
        await Fade(1);//fade to black
        SetDamping(Vector3.zero);
    }

    public async Task FadeIn()
    {
        await Fade(0);//fade to Transparent
        SetDamping(originalDamping);
    }

    void SetDamping(Vector3 d)
    {
        if(!transposer) return;
        transposer.m_XDamping = d.x;
        transposer.m_YDamping = d.y;
        transposer.m_ZDamping = d.z;
    }

    public void NotifyPlayerWarp(Transform player, Vector3 displacement)
    {
        if (vcam == null || player == null)
            return;

        vcam.OnTargetObjectWarped(player, displacement);
    }
}
