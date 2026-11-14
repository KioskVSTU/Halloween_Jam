using UnityEngine;

public class forCamera : MonoBehaviour
{
    private float targetY;

    void Start()
    {
        // Запоминаем ту высоту, которая стоит у камеры прямо сейчас
        targetY = transform.position.y;
    }

    void LateUpdate()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y = targetY; // Жестко возвращаем Y на место
        transform.position = currentPosition;
    }
}