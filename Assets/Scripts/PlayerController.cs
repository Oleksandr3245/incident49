using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 2f;
    public float gravity = -19.62f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrain = 25f;
    public float staminaRegen = 20f;

    [Header("UI References")]
    public Image staminaFillImage;
    public CanvasGroup staminaCanvasGroup; // Сюди перетягни StaminaBackground
    public float fadeSpeed = 5f; // Швидкість появи/зникнення

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentHeight;
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina;

        // Початково ховаємо шкалу
        if (staminaCanvasGroup != null) staminaCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // Присідання
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        currentHeight = Mathf.Lerp(controller.height, isCrouching ? crouchingHeight : standingHeight, Time.deltaTime * 10f);
        controller.height = currentHeight;

        // Перевірка руху
        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool isRunning = isShiftPressed && isMoving && isGrounded && !isCrouching && currentStamina > 0;

        // Логіка витрати/регену
        if (isRunning)
        {
            currentStamina -= staminaDrain * Time.deltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegen * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // ОНОВЛЕННЯ UI
        if (staminaFillImage != null)
        {
            float ratio = currentStamina / maxStamina;
            // Змінюємо масштаб по осі X. 1 - повна шкала, 0 - пуста.
            // Оскільки масштаб змінюється відносно центру (Pivot), вона буде звужуватись до середини.
            staminaFillImage.transform.localScale = new Vector3(ratio, 1f, 1f);
        }

        // ЛОГІКА ЗНИКНЕННЯ (Fade In / Fade Out)
        if (staminaCanvasGroup != null)
        {
            // Показуємо, якщо затиснуто Shift АБО якщо стаміна ще не повна
            bool shouldShow = isShiftPressed || currentStamina < maxStamina;
            float targetAlpha = shouldShow ? 1f : 0f;

            staminaCanvasGroup.alpha = Mathf.Lerp(staminaCanvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }

        // Рух
        float currentSpeed = isRunning ? runSpeed : (isCrouching ? crouchSpeed : walkSpeed);
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Стрибок
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}