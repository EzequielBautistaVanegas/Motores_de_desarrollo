using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] stepClips;

    [Header("Intervalos")]
    [SerializeField] private float walkInterval = 0.5f;
    [SerializeField] private float runInterval = 0.3f;

    [Header("Velocidad")]
    [SerializeField] private float minMoveSpeed = 0.1f;  
    [SerializeField] private float runSpeedThreshold = 5f; 

    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    private float timer;
    private int lastIndex = -1;
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 delta = transform.position - lastPosition;
        delta.y = 0f;
        float speed = delta.magnitude / Time.deltaTime;
        lastPosition = transform.position;

        if (speed < minMoveSpeed)
        {
            timer = 0f;
            return;
        }

        float interval = speed >= runSpeedThreshold ? runInterval : walkInterval;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            PlayStep();
            timer = interval;
        }
    }

    void PlayStep()
    {
        if (stepClips.Length == 0) return;

        lastIndex = (lastIndex + 1) % stepClips.Length;
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(stepClips[lastIndex]);
    }
}