using Unity.VisualScripting;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private Vector3 _screenMousePosition;
    private Vector3 _currentShieldPosition;
    private Vector3 _currentShieldRotation;
    private float _screenToWorldMousePosX;

    void Start()
    {
        _currentShieldRotation = Vector3.zero; // Initialize shield rotation to zero
        _currentShieldPosition.y = transform.position.y;
        _currentShieldPosition.z = transform.position.z;
    }

    void FixedUpdate()
    {
        // Get mouse position in screen coordinates and convert it to world coordinates
        _screenMousePosition = Input.mousePosition;
        _screenMousePosition.z = -Camera.main.transform.position.z;
        _screenToWorldMousePosX = Camera.main.ScreenToWorldPoint(_screenMousePosition).x;

        // Rotate the shield when left mouse button is pressed, within a limit of 15 degrees
        if(Input.GetMouseButton(0))
        {
            if(_currentShieldRotation.y < 15f)
            {
                _currentShieldRotation.y += 1f;
            }
        }

        // Rotate the shield when right mouse button is pressed, within a limit of -15 degrees
        if(Input.GetMouseButton(1))
        {
            if(_currentShieldRotation.y > -15f)
            {
                _currentShieldRotation.y -= 1f;
            }
        }

        // Reset shield rotation if no mouse buttons are pressed
        if(!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            _currentShieldRotation = Vector3.zero;
        }

        // Apply the calculated rotation to the shield
        transform.eulerAngles = _currentShieldRotation;

        // Restrict the shield's X position within a certain range
        if(_screenToWorldMousePosX < -1.45f || _screenToWorldMousePosX > 1.45f) return;

        // Set the shield's X position based on the calculated world mouse X position
        _currentShieldPosition.x = _screenToWorldMousePosX;
        transform.position = _currentShieldPosition;
    }
}

