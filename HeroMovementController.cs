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

            // Karakterin y�n�n� belirlemek i�in a�a��daki gibi bir kod ekleyebilirsiniz
            if (horizontalInput > 0)
            {
                // Karakter sa�a do�ru hareket ediyor
            }
            else if (horizontalInput < 0)
            {
                // Karakter sola do�ru hareket ediyor
            }
        }
    }



}