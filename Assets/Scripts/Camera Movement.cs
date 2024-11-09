using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;       // Velocidad de movimiento hacia adelante y atrás
    public float verticalSpeed = 3f;   // Velocidad de movimiento vertical (arriba/abajo)
    public float rotationSpeed = 50f;  // Velocidad de rotación izquierda/derecha

    private void Update()
    {
        // Movimiento hacia adelante y atrás con W y S
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // Movimiento en el eje Y con flechas arriba y abajo
        float moveY = 0;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveY = verticalSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveY = -verticalSpeed * Time.deltaTime;
        }

        // Rotación izquierda y derecha con A y D
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        // Actualizar posición de la cámara (movimiento hacia adelante, atrás y arriba/abajo)
        transform.Translate(new Vector3(0, moveY, moveZ), Space.Self);
    }
}