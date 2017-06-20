using UnityEngine;
using UnityEngine.UI;

public class ShakeAnimatorController : MonoBehaviour
{
    public GameObject imgRetry;

    private void Start()
    {
        imgRetry.SetActive(false);
    }
    public void OnShakeComplete()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetBool("shouldShake", false);
    }
}
