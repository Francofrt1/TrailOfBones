using System;

namespace Assets.Scripts.Interfaces
{
    public interface IStopWheelcart
    {
        event Action<bool> OnBlockWheelcartRequested;
    }
}

