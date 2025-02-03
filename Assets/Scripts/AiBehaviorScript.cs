using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlaySceneManager : MonoBehaviour
{
    [SerializeField] private AudioSource collisionSound;
    [SerializeField] private AudioSource escapeSound;
    public GameObject seeker; 
    public GameObject target;   
    public GameObject obstacle;

    private bool seeking = false;
    private bool fleeing = false;
    private bool arriving = false;
    private bool avoiding = false;


    void Update()
    {
     
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ResetScene();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ResetScene();
            ActivateObjects(new GameObject[] {seeker, target}); 
            seeking = true;
        }

     
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ResetScene();
            ActivateObjects(new GameObject[] {seeker, obstacle});
            fleeing = true;
        }

    
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ResetScene();
            ActivateObjects(new GameObject[] {seeker, target});
            arriving = true;
        }

        // Avoidance behavior when 4 is pressed
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ResetScene();
            ActivateObjects(new GameObject[] {seeker, target, obstacle});
            avoiding = true;
        }

        if (seeking)
        {
            Seek();
        }
        else if (fleeing)
        {
            Flee();
        }
        else if (arriving)
        {
            Arrive();
        }
        else if (avoiding)
        {
            Avoid();
        }

    }

    void ResetScene()
    {
        
        seeker.SetActive(false);
        target.SetActive(false);
        obstacle.SetActive(false);

        seeking = false;
        fleeing = false;
        arriving = false;
        avoiding = false;
    }

    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-8.1f, 8.1f), Random.Range(-4.1f, 4.1f), 0);
    }

    void ActivateObjects(GameObject[] Sprites)
    {
        foreach (GameObject obj in Sprites)
        {
            obj.SetActive(true);
            obj.transform.position = GetRandomPosition();
        }
    }

    void Seek()
    {
        // Move character towards target
        seeker.transform.position = Vector3.MoveTowards(seeker.transform.position, target.transform.position, 5f * Time.deltaTime);

        // Rotate character to face the target
        Vector3 direction = (target.transform.position - seeker.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // swap to degrees
        seeker.transform.rotation = Quaternion.Euler(0, 0, angle); // Rotate around Z-axis

        // Stop if close to target
        if (Vector3.Distance(seeker.transform.position, target.transform.position) < 1.5f)
        {
            seeking = false;
            collisionSound.Play();
        }
    }

    void Flee()
    {
        // Move character away from the obstacle
        Vector3 fleeDirection = (seeker.transform.position - obstacle.transform.position).normalized;
        seeker.transform.position += fleeDirection * 5f * Time.deltaTime;

        // Rotate character to face away from the obstacle 
        float angle = Mathf.Atan2(fleeDirection.y, fleeDirection.x) * Mathf.Rad2Deg;
        seeker.transform.rotation = Quaternion.Euler(0, 0, angle);
        obstacle.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Stop fleeing if far enough from the obstacle
        if (Vector3.Distance(seeker.transform.position, obstacle.transform.position) > 16f)
        {
            fleeing = false;
            escapeSound.Play();
        }

        // Stop if reaching screen boundaries
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(seeker.transform.position);
        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
        {
            fleeing = false;
            escapeSound.Play();
        }
    }
   
    void Arrive()
    {
        float slowdownDistance = 4f;

        // Calculate distance to target
        float distance = Vector3.Distance(seeker.transform.position, target.transform.position);

        // Adjust speed based on distance
        float speed = Mathf.Lerp(0, 5f, distance / slowdownDistance);

        // Move character towards target
        seeker.transform.position = Vector3.MoveTowards(seeker.transform.position, target.transform.position, speed * Time.deltaTime);

        // Rotate character to face the target
        Vector3 direction = (target.transform.position - seeker.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // swap to degrees
        seeker.transform.rotation = Quaternion.Euler(0, 0, angle); // Rotate around Z-axis

      
        if (distance < 1.5f)
        {
            arriving = false;
            collisionSound.Play();
        }
    }

    void Avoid()
    {
        float avoidanceDistance = 2f;

       
        Vector3 direction = (target.transform.position - seeker.transform.position).normalized;


        if (Vector3.Distance(seeker.transform.position, obstacle.transform.position) < avoidanceDistance)
        {
            Vector3 avoidDirection = (seeker.transform.position - obstacle.transform.position).normalized;
            direction += avoidDirection;
            collisionSound.Play();
        }

        seeker.transform.position += direction * 5f * Time.deltaTime;

       
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
        seeker.transform.rotation = Quaternion.Euler(0, 0, angle); 

        
        if (Vector3.Distance(seeker.transform.position, target.transform.position) < 1.5f)
        {
            avoiding = false;
            collisionSound.Play();
        }
    }
}