using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public Transform playerBody; // Сюди перетягни об'єкт гравця в інспекторі

    private float xRotation = 0f;

    void Start()
    {
        // Ховаємо курсор під час гри
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Отримуємо рух миші
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Обертання вгору-вниз (з обмеженням)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Обертання гравця вліво-вправо
        playerBody.Rotate(Vector3.up * mouseX);
    }
}