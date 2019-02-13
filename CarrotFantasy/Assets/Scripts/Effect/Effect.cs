using UnityEngine;

public class Effect : MonoBehaviour
{
    public float animTime; // 动画时长
    public string resourceName; // 资源池名

    private void OnEnable()
    {
        Invoke("DestroyEffect", animTime);
    }

    private void DestroyEffect()
    {
        GameController.Instance.PushItem(resourceName, gameObject);
    }
}
