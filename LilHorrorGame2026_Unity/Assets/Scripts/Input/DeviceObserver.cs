using System;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class DeviceObserver : IObserver<InputEventPtr>, IDisposable
{
    public InputDevice ActiveDevice { get; private set; }
    public event Action ActiveDeviceChanged;
    
    private readonly IDisposable _subscription;

    public DeviceObserver()
    {
        ActiveDevice = InputSystem.devices.FirstOrDefault();
        _subscription = InputSystem.onEvent.Subscribe(this);
    }
    
    public void OnCompleted()
    {
        
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(InputEventPtr value)
    {
        throw new NotImplementedException();
    }

    public void OnNext(InputControl value)
    {
        if (ActiveDevice == value.device)
            return;
        
        ActiveDevice = value.device;
        ActiveDeviceChanged?.Invoke();
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }
}