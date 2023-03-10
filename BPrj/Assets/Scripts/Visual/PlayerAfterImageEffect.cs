using System.Collections;
using UnityEngine;

public class PlayerAfterImageEffect : MonoBehaviour
{
    // Variables set from elsewhere (e.g. script instantiating this object)
    [HideInInspector] public Sprite sprite;     // Sprite to fade out
    [HideInInspector] public Vector2 position;  // Constant position of object

    [Header("Fading out")]
    [SerializeField] private float initialAlpha = .7f;
    [SerializeField] private float alphaDecrement = .06f;
    [SerializeField] private float alphaDecrementWaitingTime = .01f;

    private SpriteRenderer SR;
    private Color color;
    private float alpha;

    // Decrease sprite alpha value (which starts at `initialAlpha`) by `alphaDecrement` every `alphaDecrementWaitingTime` seconds
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

    private void Update()
    {
        this.transform.position = position; // Position needs to be updated so we don't move when parent transform moves
    }
}
