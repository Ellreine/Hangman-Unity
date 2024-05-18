using UnityEngine;

public class WordLetterHandler : MonoBehaviour
{
    public Sprite hiddenSprite;  // Спрайт для скрытой буквы
    public Sprite revealedSprite;  // Спрайт для открытой буквы
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = hiddenSprite;  // Изначально все буквы скрыты
    }

    public void RevealLetter()
    {
        spriteRenderer.sprite = revealedSprite;  // Открываем букву
    }
}
