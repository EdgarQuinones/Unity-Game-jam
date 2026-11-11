using UnityEngine;

[RequireComponent(typeof(EnemyAI), typeof(Rigidbody2D))]
public class DragonFireBreath : MonoBehaviour
{
    private enum BreathState { Waiting, Warning, Burst, Recovery }
    public bool IsAttacking => state != BreathState.Waiting;

    [Header("Timing")]
    [SerializeField] private float timeBetweenBreaths = 4f;
    [SerializeField] private float warningDuration = 1f;
    [SerializeField] private float burstDuration = 0.25f;
    [SerializeField] private float recoveryDuration = 1f;

    [Header("Breath")]
    [SerializeField] private float range = 4f;
    [SerializeField, Range(1f, 180f)] private float angle = 90f;
    [SerializeField] private float damage = 20f;

    [Header("Cone Visual")]
    [SerializeField] private Material coneMaterial;
    [SerializeField] private Color warningColor = new Color(1f, 1f, 0f, 0.5f);
    [SerializeField] private Color fireColor = new Color(1f, 0.25f, 0f, 0.8f);
    [SerializeField] private string sortingLayer = "Default";
    [SerializeField] private int sortingOrder = 5;

    private EnemyAI ai;
    private Rigidbody2D body;
    private EnemyMovement movement;
    private EnemyCombat combat;
    private Karma karma;
    private RunManager runManager;
    private float BreathWait => Mathf.Max(0f, timeBetweenBreaths) /
        (runManager != null && runManager.BossSpeedMultiplier > 0f ? runManager.BossSpeedMultiplier : 1f);
    private DragonCharge charge;
    private Mesh coneMesh;
    private MeshRenderer coneRenderer;
    private Material runtimeMaterial;
    private BreathState state;
    private float stateEndTime;
    private Vector2 lockedDirection;
    private Vector2 lockedOrigin;
    private float lockedRange;
    private float lockedAngle;
    private bool controlsTaken;
    private bool movementWasEnabled;
    private bool combatWasEnabled;

    private void Awake()
    {
        ai = GetComponent<EnemyAI>();
        body = GetComponent<Rigidbody2D>();
        movement = GetComponent<EnemyMovement>();
        combat = GetComponent<EnemyCombat>();
        karma = FindFirstObjectByType<Karma>();
        runManager = FindFirstObjectByType<RunManager>();
        charge = GetComponent<DragonCharge>();

        GameObject visual = new GameObject("Fire Breath Cone");
        visual.transform.SetParent(transform, false);
        MeshFilter filter = visual.AddComponent<MeshFilter>();
        coneRenderer = visual.AddComponent<MeshRenderer>();
        coneMesh = new Mesh { name = "Fire Breath Cone" };
        filter.mesh = coneMesh;
        coneRenderer.sortingLayerName = sortingLayer;
        coneRenderer.sortingOrder = sortingOrder;
        coneRenderer.enabled = false;
        if (coneMaterial != null)
        {
            runtimeMaterial = new Material(coneMaterial);
            coneRenderer.material = runtimeMaterial;
        }
    }

    private void OnEnable()
    {
        state = BreathState.Waiting;
        stateEndTime = Time.time + BreathWait;
    }

    private void Update()
    {
        if (ai.CurrentState == EnemyAI.EnemyState.Stunned || ai.Target == null || !ai.Target.gameObject.activeInHierarchy)
        {
            if (controlsTaken) CancelBreath();
            stateEndTime = Time.time + BreathWait;
            return;
        }

        if (state == BreathState.Waiting && Time.time >= stateEndTime)
            BeginWarning();
        else if (state == BreathState.Warning && Time.time >= stateEndTime)
            BeginBurst();
        else if (state == BreathState.Burst && Time.time >= stateEndTime)
        {
            coneRenderer.enabled = false;
            state = BreathState.Recovery;
            stateEndTime = Time.time + Mathf.Max(0f, recoveryDuration);
        }
        else if (state == BreathState.Recovery && Time.time >= stateEndTime)
            CancelBreath();
    }

    private void BeginWarning()
    {
        if (charge != null && charge.enabled)
        {
            Debug.LogWarning("Disable Dragon Charge while testing Dragon Fire Breath; attack selection is not implemented yet.", this);
            stateEndTime = Time.time + Mathf.Max(1f, timeBetweenBreaths);
            return;
        }
        if (runtimeMaterial == null || range <= 0f)
        {
            Debug.LogWarning("Dragon Fire Breath needs a cone material and positive range.", this);
            stateEndTime = Time.time + Mathf.Max(1f, timeBetweenBreaths);
            return;
        }

        Vector2 direction = (Vector2)ai.Target.position - body.position;
        if (direction.sqrMagnitude < 0.0001f)
        {
            stateEndTime = Time.time + 1f;
            return;
        }

        lockedDirection = direction.normalized;
        lockedOrigin = body.position;
        lockedRange = range;
        lockedAngle = Mathf.Clamp(angle, 1f, 180f);
        movementWasEnabled = movement != null && movement.enabled;
        combatWasEnabled = combat != null && combat.enabled;
        controlsTaken = true;
        if (movementWasEnabled) movement.enabled = false;
        if (combatWasEnabled) combat.enabled = false;
        body.linearVelocity = Vector2.zero;

        BuildCone();
        runtimeMaterial.color = warningColor;
        coneRenderer.enabled = true;
        state = BreathState.Warning;
        stateEndTime = Time.time + Mathf.Max(0f, warningDuration);
        Debug.Log($"{name}: fire breath warning.", this);
    }

    private void BuildCone()
    {
        const int segments = 40;
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];
        float directionAngle = Mathf.Atan2(lockedDirection.y, lockedDirection.x) * Mathf.Rad2Deg;
        for (int i = 0; i <= segments; i++)
        {
            float radians = (directionAngle - lockedAngle * 0.5f + lockedAngle * i / segments) * Mathf.Deg2Rad;
            Vector3 worldPoint = (Vector3)lockedOrigin + new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * lockedRange;
            vertices[i + 1] = coneRenderer.transform.InverseTransformPoint(worldPoint);
            if (i < segments)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 2;
                triangles[i * 3 + 2] = i + 1;
            }
        }
        vertices[0] = coneRenderer.transform.InverseTransformPoint(lockedOrigin);
        coneMesh.Clear();
        coneMesh.vertices = vertices;
        coneMesh.triangles = triangles;
        coneMesh.RecalculateBounds();
    }

    private void BeginBurst()
    {
        runtimeMaterial.color = fireColor;
        Vector2 toPlayer = (Vector2)ai.Target.position - lockedOrigin;
        if (toPlayer.sqrMagnitude <= lockedRange * lockedRange &&
            (toPlayer.sqrMagnitude < 0.0001f || Vector2.Angle(lockedDirection, toPlayer) <= lockedAngle * 0.5f))
        {
            Health playerHealth = ai.Target.GetComponentInParent<Health>();
            if (playerHealth != null)
            {
                float multiplier = karma != null ? Mathf.Lerp(0.5f, 1.5f, karma.CurrentKarma / 100f) : 1f;
                float finalDamage = damage * multiplier;
                playerHealth.TakeDamage(finalDamage);
                Debug.Log($"{name}: fire breath hit player for {finalDamage} damage.", this);
            }
        }
        state = BreathState.Burst;
        stateEndTime = Time.time + Mathf.Max(0f, burstDuration);
        Debug.Log($"{name}: fire breath burst.", this);
    }

    private void CancelBreath()
    {
        if (coneRenderer != null) coneRenderer.enabled = false;
        if (controlsTaken)
        {
            body.linearVelocity = Vector2.zero;
            if (movement != null) movement.enabled = movementWasEnabled;
            if (combat != null) combat.enabled = combatWasEnabled;
            controlsTaken = false;
        }
        state = BreathState.Waiting;
        stateEndTime = Time.time + BreathWait;
    }

    private void OnDisable()
    {
        CancelBreath();
    }

    private void OnDestroy()
    {
        if (coneMesh != null) Destroy(coneMesh);
        if (runtimeMaterial != null) Destroy(runtimeMaterial);
    }
}