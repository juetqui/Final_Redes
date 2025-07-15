using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private Transform _targetTransform;

    [SerializeField] private float _height = 20f;       // Altura sobre el jugador
    [SerializeField] private float _smoothSpeed = 10f;  // Velocidad de seguimiento
    [SerializeField] private Vector3 _offset = Vector3.zero; // Offset si querés ajustarlo manualmente

    public void SetTarget(Player player)
    {
        _targetTransform = player.transform;

        // Rotar cámara para que mire hacia abajo
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void LateUpdate()
    {
        if (!_targetTransform) return;

        Vector3 desiredPosition = _targetTransform.position;
        desiredPosition.y += _height;
        desiredPosition += _offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * _smoothSpeed);
    }
}
