using Fusion;

public struct NetworkInputData : INetworkInput
{
    public float horizontalInput;
    public float verticalInput;
    public NetworkBool isFirePressed;
    public NetworkBool isSecFirePressed;
    public NetworkBool isTrapPressed;
    public NetworkBool isDashPressed;

    public NetworkButtons networkButtons;
}

enum MyButtons
{
    Jump = 0,
}
