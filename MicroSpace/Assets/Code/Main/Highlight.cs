using ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;

public class Highlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Colors _colors;

    private Color _startColor;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _startColor = _spriteRenderer.color;
        _spriteRenderer.color = _colors
            .MixColors(_colors.Highlighted, _startColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _spriteRenderer.color = _startColor;
    }
}