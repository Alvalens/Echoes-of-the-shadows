using System.Collections;
using UnityEngine;

public class GameControllers : MonoBehaviour
{
    public static GameControllers Instance { get; private set; }
    public bool IsControllerConnected { get; private set; } = false;

    private IEnumerator CheckForControllers()
    {
        while (true)
        {
            var controllers = Input.GetJoystickNames();
            bool isControllerValid = false;

            foreach (var controller in controllers)
            {
                if (!string.IsNullOrEmpty(controller)) // Check for valid controller names
                {
                    isControllerValid = true;
                    break;
                }
            }

            if (!IsControllerConnected && isControllerValid)
            {
                IsControllerConnected = true;
                Debug.Log("Controller Connected");
                OnControllerConnected();
            }
            else if (IsControllerConnected && !isControllerValid)
            {
                IsControllerConnected = false;
                Debug.Log("Controller Disconnected");
                OnControllerDisconnected();
            }

            yield return new WaitForSeconds(1f);
        }
    }


    // Event for other scripts to subscribe to
    public delegate void ControllerConnectionHandler();
    public event ControllerConnectionHandler OnControllerConnectedEvent;
    public event ControllerConnectionHandler OnControllerDisconnectedEvent;

    private void OnControllerConnected()
    {
        OnControllerConnectedEvent?.Invoke();
    }

    private void OnControllerDisconnected()
    {
        OnControllerDisconnectedEvent?.Invoke();
    }
}