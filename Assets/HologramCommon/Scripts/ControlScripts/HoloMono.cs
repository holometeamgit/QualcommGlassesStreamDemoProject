using UnityEngine;

public class HoloMono : MonoBehaviour
{
    protected virtual void OnStart()
    {

    }

    void Start()
    {
        OnStart();
    }

    protected virtual void OnUpdate()
    {

    }

    void Update()
    {
        OnUpdate();
    }

    protected virtual void OnBecameVisibleOverride()
    {

    }

    void OnBecameVisible()
    {
        OnBecameVisibleOverride();
    }

    protected virtual void OnBecameInvisibleOverride()
    {

    }

    void OnBecameInvisible()
    {
        OnBecameInvisibleOverride();
    }
}
