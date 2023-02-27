using System.Collections;
using UnityEngine;

public class PlayerAfterImageEffect : MonoBehaviour
{
    [HideInInspector] public Sprite sprite;

    [Header("Fading out")]
    [SerializeField] private float initialAlpha = .8f;
    [SerializeField] private float alphaDecrement = .05f;
    [SerializeField] private float alphaDecrementWaitingTime = .01f;

    private SpriteRenderer SR;
    private Color color;
    private float alpha;

    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        alpha = initialAlpha;
        color = new Color(1, 1, 1, alpha);
        SR.color = color;
        
        SR.sprite = sprite;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        while (alpha > 0) {
            alpha -= alphaDecrement;
            color.a = alpha;
            SR.color = color;
            yield return new WaitForSeconds(alphaDecrementWaitingTime);
        }

        Destroy(this.gameObject);
    }
}
