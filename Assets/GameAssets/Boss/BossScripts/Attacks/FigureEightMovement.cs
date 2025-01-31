using UnityEngine;

public class FigureEightMovement : MonoBehaviour
{
    public float speed = 1.0f; // Speed of movement
    public float scale = 1.0f; // Scale of the figure-eight

    private float time = 0.0f;

    private Vector3 startPos;


    void Start()
    {
        startPos = transform.position;
    }
    void Update()
    {
        // Update the time based on the speed
        time += Time.deltaTime * speed;

        // Parametric equations for a figure-eight (lemniscate)
        float x = Mathf.Sin(time) * scale;
        float z = Mathf.Sin(time) * Mathf.Cos(time) * scale;

        // Update the object's position
        transform.position = new Vector3(startPos.x + x, transform.position.y, startPos.z + z);
    }
}