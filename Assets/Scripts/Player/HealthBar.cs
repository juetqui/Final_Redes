using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Transform _owner;

    private const float HEAD_OFFSET = 1.5F;

    [SerializeField] private Image _myLifeImage;

    public HealthBar SetOwner(Player owner)
    {
        _owner = owner.transform;

        return this;
    }

    public void UpdateLife(float val, float maxHealth)
    {
        _myLifeImage.fillAmount = val / maxHealth;
    }

    public void UpdatePosition()
    {
        if (_owner == null) return;
        transform.position = _owner.position + Vector3.up * HEAD_OFFSET;
    }
}
