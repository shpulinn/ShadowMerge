using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ObjectRotator : MonoBehaviour
{
    [Header("Настройки вращения")]
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;
    
    [Header("Ограничения вращения")]
    [SerializeField] private bool limitVerticalRotation = true;
    [SerializeField] private float minVerticalAngle = -90f;
    [SerializeField] private float maxVerticalAngle = 90f;

    [Header("Сброс")]
    [SerializeField] private Button resetRotationButton;
    [SerializeField] private float resetDuration = .2f;
    
    private Vector3 lastMousePosition;
    private Vector3 lastTouchPosition;
    private bool isRotating = false;
    private float currentVerticalRotation = 0f;

    private void Awake()
    {
        resetRotationButton.onClick.AddListener(ResetRotation);
    }

    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || 
            Application.platform == RuntimePlatform.OSXEditor ||
            Application.platform == RuntimePlatform.LinuxEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.OSXPlayer ||
            Application.platform == RuntimePlatform.LinuxPlayer)
        {
            HandleMouseInput();
        }
        else
        {
            HandleTouchInput();
        }
    }
    
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
        }
        
        if (isRotating && Input.GetMouseButton(0))
        {
            Vector3 deltaPosition = Input.mousePosition - lastMousePosition;
            RotateObject(deltaPosition);
            lastMousePosition = Input.mousePosition;
        }
    }
    
    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                isRotating = true;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isRotating = false;
            }
            else if (touch.phase == TouchPhase.Moved && isRotating)
            {
                Vector3 deltaPosition = (Vector3)touch.position - lastTouchPosition;
                RotateObject(deltaPosition);
                lastTouchPosition = touch.position;
            }
        }
        else
        {
            isRotating = false;
        }
    }
    
    void RotateObject(Vector3 deltaPosition)
    {
        float rotationX = deltaPosition.y * rotationSpeed;
        float rotationY = deltaPosition.x * rotationSpeed;
        
        if (invertX) rotationX = -rotationX;
        if (invertY) rotationY = -rotationY;
        
        if (limitVerticalRotation)
        {
            currentVerticalRotation -= rotationX;
            currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, minVerticalAngle, maxVerticalAngle);
            rotationX = -(currentVerticalRotation - (transform.eulerAngles.x > 180 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x));
        }
        
        transform.Rotate(-rotationX, rotationY, 0, Space.World);
    }
    
    private void ResetRotation()
    {
        // transform.rotation = Quaternion.identity;
        // currentVerticalRotation = 0f;
        transform.DORotate(Vector3.zero, resetDuration)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => currentVerticalRotation = 0f);
    }
    
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }

    private void OnDestroy()
    {
        resetRotationButton.onClick.RemoveListener(ResetRotation);
    }
}