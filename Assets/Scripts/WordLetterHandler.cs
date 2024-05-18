using UnityEngine;

public class WordLetterHandler : MonoBehaviour
{
    public Sprite hiddenSprite;  // ������ ��� ������� �����
    public Sprite revealedSprite;  // ������ ��� �������� �����
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = hiddenSprite;  // ���������� ��� ����� ������
    }

    public void RevealLetter()
    {
        spriteRenderer.sprite = revealedSprite;  // ��������� �����
    }
}
