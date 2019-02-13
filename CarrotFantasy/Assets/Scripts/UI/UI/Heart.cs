using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour {

    private float animTime; // 动画时长
    private string resourceName; // 资源池名

    private void OnEnable()
    {
        animTime = 0.433f;
        resourceName = "Img_Heart";
        Invoke("DestroyEffect", animTime);
    }

    private void DestroyEffect()
    {
        GameManager.Instance.PushItem(FactoryType.UI,resourceName, gameObject);
    }
}
