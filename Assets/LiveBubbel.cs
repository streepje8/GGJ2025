using System;
using FondantMetStokjes.Interactions.Pickups;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Pickup))]
public class LiveBubbel : MonoBehaviour
{
    private static readonly int Tint = Shader.PropertyToID("_Tint");
    [field: SerializeField]public bool IsImmortal { get; private set; } = true;
    [field: SerializeField]public LayerMask GroundLayer { get; private set; }
    [field: SerializeField] public float FallingSpeed { get; private set; } = 0.5f;
    [field: SerializeField]public UnityEvent OnPop { get; private set; }
    [field: SerializeField]public BubbleKind Kind { get; private set; }

    private Pickup pickup;
    private Material bubbelMat;
    private void Awake()
    {
        var rend = GetComponentInChildren<Renderer>(true);
        bubbelMat = new Material(rend.material);
        rend.sharedMaterial = bubbelMat;
        rend.transform.localScale = Kind.Scale;
        bubbelMat.SetColor(Tint, Kind.Tint);
    }

    private void OnEnable()
    {
        pickup = GetComponent<Pickup>();
        pickup.OnPickup.AddListener(OnPickup);
        pickup.OnDrop.AddListener(OnDrop);
    }

    private void OnDisable()
    {
        pickup.OnPickup.RemoveListener(OnPickup);
        pickup.OnDrop.RemoveListener(OnDrop);
    }
    
    private void OnDrop()
    {
        IsImmortal = false;
        pickup.Range = 1.5f;
    }

    private void OnPickup(Transform display)
    {
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
}
