using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEditor;

public class EnemyScript : MonoBehaviour
{
    public LayerMask obstacleMask;
    public float obstacleCheckDistance = 1f;
    public float viewDistance = 10f;
    public float fieldOfViewAngle = 80f;
    public float hearingRange = 5f;
    Vector3 lastKnownPosition;
    public Camera playerCamera;

    public GameObject playerSphere;
    // as the game becomes more complex, constants should be moved to data files (not PlayerPrefs)

    // MAX_MOVE_DISTANCE is the max speed the seek function can move
    const float MAX_MOVE_DISTANCE = 0.5f;
    // Deceleration factor is like a buffer around the target
    const float DECELERATION_FACTOR = 1f;
    //now variables needed by FixedUpdate
    float moveDistance;
    Vector3 source;
    Vector3 target;
    Vector3 outputVelocity;
    //and those for Seek
    Vector3 directionToTarget;
    Vector3 velocityToTarget;
    //and arrive
    float distanceToTarget;
    float speed;
    // Create an enum to control the movement type of the AI ball-
    // this will allow us to test both seek and arrive in the same script
    public enum MovementType { Idle, Seek, Arrive, Patrol };
    public MovementType movementType;
    // Reference to NavMeshAgent component
    NavMeshAgent navAgent;
    Vector3 destination;
    // Decision Tree control booleans
    public bool isVisible;
    public bool isAudible;
    public bool isClose;
    // Also to the game object we will follow
    public Transform targetObject;
    // Waypoints for Patrol functionality
    int nextIndex;
    public GameObject[] waypoints;
    public float patrolRadius = 10f;
    public float waypointReachDistance = 0.5f;

    Vector3 currentPatrolTarget;
    bool hasPatrolTarget;

    void OnDrawGizmos()
    {
        // seek/arrive
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(currentPatrolTarget, 0.1f);
        Gizmos.DrawLine(transform.position, currentPatrolTarget);

        // patrol
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }

        isVisible = CanSeePlayer();
        isAudible = CanHearPlayer();

        if (isVisible)
        {
            print("Saw player and moving");
            movementType = MovementType.Seek;
            target = playerSphere.transform.position;
        }
        else if (isAudible)
        {
            print("Heard player and moving");
            movementType = MovementType.Seek;
            target = lastKnownPosition;
        }
        else
        {
            movementType = MovementType.Patrol;
        }
        //We multiply by Time.deltaTime to ensure the same distance is achieved across different framerates
        moveDistance = MAX_MOVE_DISTANCE;
        source = transform.position;
        //check to make sure player still exists!
        if (playerSphere != null) {
        target = playerSphere.transform.position + Vector3.up * 0.7f; //fixed height
        }
        else 
        {
        //move to the centre of the game area
        target = Vector3.zero;
        }
        // Run Seek Movement
        if (movementType == MovementType.Seek) {
        outputVelocity = Seek (source, target, moveDistance);
        Debug.Log("Seeking");
        }
        else if (movementType == MovementType.Arrive) {
        outputVelocity = Arrive (source, target);
        Debug.Log("Arriving");
        }
        else if (movementType == MovementType.Idle) {
        outputVelocity = Idle();
        }
        else if (movementType == MovementType.Patrol) {
        outputVelocity = Patrol();
        }
        // Run Arrive Movement
        
        GetComponent<Rigidbody> ().AddForce (outputVelocity, ForceMode.VelocityChange);
    }

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        movementType = MovementType.Patrol;
    }

    private Vector3 Idle()
    {
        print("Idling");
        return Vector3.zero;
    }

    private Vector3 Seek (Vector3 source, Vector3 target, float moveDistance)
    {
        print("Seeking");
        Quaternion targetRotation = Quaternion.LookRotation(target - source);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        Vector3 toTarget = target - source;
        toTarget.y = 0f;
        // Get direction to the target
        directionToTarget = toTarget.normalized;
        // Calculate velocity along this line
        velocityToTarget = moveDistance * directionToTarget;
        // To Calculate the force to the target, subtract the objects current
        // movement from the from the force in the direction of the target
        return velocityToTarget - GetComponent<Rigidbody> ().linearVelocity;
        }
        // The Arrive function is similar to Seek but it also takes into account the distance
        // to the target and slows down as it gets closer to the target

    private Vector3 Arrive (Vector3 source, Vector3 target)
    {
        print("Arriving");
        // Get the distance between source and target
        distanceToTarget = Vector3.Distance (source, target);
        // Get direction to the target
        directionToTarget = Vector3.Normalize (target - source);
        // Calculate current speed
        speed = distanceToTarget / DECELERATION_FACTOR;
        // Use Speed to control deceleration
        velocityToTarget = speed * directionToTarget;
        // To Calculate the force to the target, subtract the objects current
        // Movement from the from the force in the direction of the target
        return velocityToTarget - GetComponent<Rigidbody> ().linearVelocity;
    }


    // Function that loops through waypoints for the Patrol functionality
    public Vector3 NextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned!");
            return transform.position;
        }

        // If close enough to current waypoint, advance
        if (Vector3.Distance(transform.position, waypoints[nextIndex].transform.position) < 0.5f)
        {
            nextIndex = (nextIndex + 1) % waypoints.Length;
        }

        return waypoints[nextIndex].transform.position;
    }

    public Vector3 Patrol()
    {
        //starts checking for targets
        if (!hasPatrolTarget)
        {
            //gets target
            currentPatrolTarget = GetPatrolWaypoints();
            hasPatrolTarget = true;
        }

        //dir etc
        Vector3 direction = (currentPatrolTarget - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, currentPatrolTarget);

        //wall detect raycast
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, distance, LayerMask.GetMask("Wall")))
        {
            print("Hit wall, finding next waypoint");
            currentPatrolTarget = GetPatrolWaypoints();
        }

        if (Vector3.Distance(transform.position, currentPatrolTarget) < waypointReachDistance)
        {
            currentPatrolTarget = GetPatrolWaypoints();
        }

        return Seek(transform.position, currentPatrolTarget, moveDistance);
    }
    
    private Vector3 GetPatrolWaypoints()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        Vector3 point = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        return point;
    }
    
    bool CanSeePlayer()
    {
        if (playerSphere == null) return false;

        Vector3 directionToPlayer = (playerSphere.transform.position - transform.position);
        float distanceToPlayer = directionToPlayer.magnitude;

        // Too far
        if (distanceToPlayer > viewDistance)
            return false;

        // Angle check
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > fieldOfViewAngle * 0.5f)
            return false;

        // Wall check
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f,
            directionToPlayer.normalized,
            distanceToPlayer,
            LayerMask.GetMask("Wall")))
        {
            return false;
        }

        return true;
    }

    bool CanHearPlayer()
    {
        if (playerSphere == null) return false;

        float distance = Vector3.Distance(transform.position, playerSphere.transform.position);

        if (distance <= hearingRange)
        {
            lastKnownPosition = playerSphere.transform.position;
            return true;
        }

        return false;
    }
    
    // Trigger for prox chase
    public void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Player")
        {
            movementType = MovementType.Seek;
            print("Chases player");
        }
    }
}
