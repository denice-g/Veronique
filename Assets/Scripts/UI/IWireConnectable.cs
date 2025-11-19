using UnityEngine;

public interface IWireConnectable
{
    public bool isWireBox => this is WireBox;
    IWireConnectable previousConnectable => null;
    Transform GetTransform();
    Vector3 GetConnectPoint();
}
