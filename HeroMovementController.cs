using UnityEngine;

public class HeroMovementController : MonoBehaviour
{
    [SerializeField] private HeroInputController heroInputController;

    [SerializeField] private float forwardMovementSpeed;

    [SerializeField] private float horizontalMovementSpeed;

    [SerializeField] private float horizontalLimitValue;


    private float newPositionX;


    // Update is called once per frame
    void FixedUpdate()
    {
        SetHeroForwardMovement();
        SetHeroHorizontalMovement();
    }

    private void SetHeroForwardMovement()
    {
        transform.Translate(Vector3.down * forwardMovementSpeed * Time.fixedDeltaTime);
    }

    private void SetHeroHorizontalMovement()
    {
        if (heroInputController != null)
        {
            float horizontalInput = heroInputController.HorizontalValue;
            float newPositionX = transform.position.x + horizontalInput * horizontalMovementSpeed * Time.fixedDeltaTime;
            newPositionX = Mathf.Clamp(newPositionX, -horizontalLimitValue, horizontalLimitValue);
            transform.position = new Vector3(newPositionX, transform.position.y, transform.position.z);

            // Karakterin yönünü belirlemek için aþaðýdaki gibi bir kod ekleyebilirsiniz
            if (horizontalInput > 0)
            {
                // Karakter saða doðru hareket ediyor
            }
            else if (horizontalInput < 0)
            {
                // Karakter sola doðru hareket ediyor
            }
        }
    }



}