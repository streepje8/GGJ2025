using FondantMetStokjes.Interactions.Pickups;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Pickup))]
public class LiveBubble : MonoBehaviour
{
    private static readonly int Tint = Shader.PropertyToID("_Tint");
    [field: SerializeField]public bool IsImmortal { get; private set; } = true;
    [field: SerializeField]public LayerMask GroundLayer { get; private set; }
    [field: SerializeField] public float FallingSpeed { get; private set; } = 0.5f;
    [field: SerializeField]public UnityEvent OnPop { get; private set; }
    [field: SerializeField]public BubbleKind Kind { get; private set; }

    private Pickup pickup;
    private Material bubbelMat;
    private Renderer rend;
    private void Awake()
    {
        rend = GetComponentInChildren<Renderer>(true);
        bubbelMat = new Material(rend.material);
        rend.sharedMaterial = bubbelMat;
        rend.transform.localScale = Kind.Scale;
        bubbelMat.SetColor(Tint, Kind.Tint);
        pickup = GetComponent<Pickup>();
        pickup.OnPickup += OnPickup;
        pickup.OnDrop += OnDrop;
    }

    private void OnDestroy()
    {
        pickup.OnPickup -= OnPickup;
        pickup.OnDrop -= OnDrop;
    }

    public void SwitchKind(BubbleKind newKind)
    {
        Kind = newKind;
        rend.transform.localScale = Kind.Scale;
        bubbelMat.SetColor(Tint, Kind.Tint);
    }
    
    private void OnDrop()
    {
        IsImmortal = false;
        pickup.Range = 1.5f;
    }

    private Transform currentDisplay;
    private void OnPickup(Transform display)
    {
        currentDisplay = display;
        display.localScale = Kind.Scale;
        display.GetComponent<Renderer>().sharedMaterial = bubbelMat;
    }

    private void Update()
    {
        if (IsImmortal) return;
        transform.position += Vector3.down * (FallingSpeed * Time.deltaTime);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, GroundLayer))
        {
            if (hit.distance < 0.01)
            {
                OnPop.Invoke();
                Destroy(gameObject);
            }
        }
    }

    public void MakeImmortal()
    {
        IsImmortal = true;
    }

    public void ToRenderer(MeshRenderer display)
    {
        display.transform.localScale = Kind.Scale;
        display.sharedMaterial = bubbelMat;
    }
}
