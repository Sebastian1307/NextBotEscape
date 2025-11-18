using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    public float bobSpeed = 6f;
    public float bobAmount = 0.05f;
    public float swayAmount = 1.5f;

    private float defaultYPos = 0;
    private float timer = 0;

    void Start()
    {
        defaultYPos = transform.localPosition.y;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            timer += Time.deltaTime * bobSpeed;
            float newY = defaultYPos + Mathf.Sin(timer) * bobAmount;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
        else
        {
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultYPos, Time.deltaTime * 5), transform.localPosition.z);
        }

        // Small "sway" random, like found footage games
        float swayX = Mathf.PerlinNoise(Time.time * swayAmount, 0f) - 0.5f;
        float swayY = Mathf.PerlinNoise(0f, Time.time * swayAmount) - 0.5f;
        transform.localRotation = Quaternion.Euler(swayY * 2f, swayX * 2f, 0f);
    }
}
