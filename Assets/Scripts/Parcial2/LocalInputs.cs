using UnityEngine;

public class LocalInputs : MonoBehaviour
{
    private NetworkInputData _networkInputData;

    private bool _isFirePressed;
    private bool _isSecFirePressed;
    private bool _isTrapPressed;
    private bool _isDashPressed;
    
    void Start()
    {
        _networkInputData = new NetworkInputData();
    }

    void Update()
    {
        _networkInputData.horizontalInput = Input.GetAxis("Horizontal");
        _networkInputData.verticalInput = Input.GetAxis("Vertical");

        _isFirePressed |= Input.GetKeyDown(KeyCode.Mouse0);
        _isSecFirePressed |= Input.GetKeyDown(KeyCode.Mouse1);
        _isTrapPressed |= Input.GetKeyDown(KeyCode.Space);
        _isDashPressed |= Input.GetKeyDown(KeyCode.LeftShift);
    }

    public NetworkInputData GetLocalInputs()
    {
        _networkInputData.isFirePressed = _isFirePressed;
        _isFirePressed = false;
        
        _networkInputData.isSecFirePressed = _isSecFirePressed;
        _isSecFirePressed = false;

        _networkInputData.isTrapPressed = _isTrapPressed;
        _isTrapPressed = false;

        _networkInputData.networkButtons.Set(MyButtons.Jump, _isDashPressed);
        _isDashPressed = false;
        
        return _networkInputData;
    }
}
