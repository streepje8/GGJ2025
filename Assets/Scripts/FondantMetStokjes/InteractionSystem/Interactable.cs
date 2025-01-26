using FondantMetStokjes.Player;
using UnityEngine;

namespace FondantMetStokjes.InteractionSystem
{
    public abstract class Interactable : MonoBehaviour
    {
        private const float glowAmount = 10.0f;
        [field: Header("Interactable Settings")]
        [field: SerializeField] public float Range { get; set; } = 10f;
        [field: SerializeField] public bool IsInteractable { get; set; } = true;
        [field: SerializeField] public IconGraphicData IconGraphics { get; protected set; }
        [field: SerializeField] public Renderer Renderer { get; protected set; }
        [field: SerializeField] public bool RequireBubble { get; set; }


        public virtual ControllerButton InteractionStartButton { get; } = ControllerButton.Cross;
        public abstract void OnInteract(Interactor interactor);

        public virtual void OnEnterRange() { }
        public virtual void OnExitRange() { }

        public bool InRange { get; private set; } = false;
        private Material[] materials;
        private float inRangeTimer = 0.2f;
        private float graphicT = 0;
        private bool graphicEnabled = false;
        private Transform graphicGameObj;
        private void Awake()
        {
            if (IconGraphics != null) graphicEnabled = true;
            if (graphicEnabled)
            {
                graphicGameObj = Instantiate(IconGraphics.IconObject, transform.position, Quaternion.identity).transform;    
                graphicGameObj.transform.SetParent(transform);
                IconGraphics.ApplyVisual(graphicGameObj, InteractionStartButton);
            }

            if (Renderer != null)
            {
                for (int i = 0; i < Renderer.materials.Length; i++)
                {
                    Renderer.materials[i] = new Material(Renderer.materials[i]);
                }
                materials = Renderer.materials;
            }
        }

        private void Update()
        {
            if (graphicEnabled)
            {
                if (InRange)
                {
                    
                    if (materials != null)
                    {
                        foreach (Material material in materials)
                        {
                            material.SetFloat("_Glow", glowAmount);
                        }
                    }
                    inRangeTimer -= Time.deltaTime;
                    if (inRangeTimer <= 0)
                    {
                        OnExitRange();
                        InRange = false;    
                    }
                    if(!graphicGameObj.gameObject.activeSelf) graphicGameObj.gameObject.SetActive(true);
                    if (graphicT < 1)
                    {
                        graphicT += Time.deltaTime / IconGraphics.AnimationDuration;
                        UpdateGraphicAnimation(graphicT);
                    }
                }
                else
                {
                    if (materials != null)
                    {
                        foreach (Material material in materials)
                        {
                            material.SetFloat("_Glow", 0);
                        }
                    }
                    if (graphicT > 0)
                    {
                        graphicT -= Time.deltaTime / IconGraphics.AnimationDuration;
                        UpdateGraphicAnimation(graphicT);
                    }
                    else graphicGameObj.gameObject.SetActive(false);
                }
            }

            InteractableUpdate();
        }

        public bool InInteraction { get; private set; }
        public Interactor CurrentInteractor { get; private set; }
        public void StartInteraction(Interactor interactor)
        {
            interactor.SetInteractionAnimation(true);
            CurrentInteractor = interactor;
            InInteraction = true;
        }

        public void EndInteraction()
        {
            if(CurrentInteractor != null) CurrentInteractor.SetInteractionAnimation(false);
            else Debug.LogWarning("Interaction was ended but none was started!");
            CurrentInteractor = null;
            InInteraction = false;
        }
        
        public virtual void InteractableUpdate() { }

        private void UpdateGraphicAnimation(float t)
        {
            graphicGameObj.localPosition = Vector3.Lerp(Vector3.zero, IconGraphics.IconOffset, IconGraphics.OffsetAnimation.Evaluate(t));
            graphicGameObj.localScale = Vector3.Lerp(Vector3.zero, IconGraphics.IconScale, IconGraphics.ScaleAnimation.Evaluate(t));
            graphicGameObj.rotation = Quaternion.LookRotation((new Vector3(0,1,-10)-graphicGameObj.transform.position).normalized, Vector3.up);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, Range);
        }
    
        public void SetInRange(bool hasBubble)
        {
            if (!hasBubble && RequireBubble)
                return;
            if (!InRange)
            {
                OnEnterRange();
            }
            inRangeTimer = 0.2f;
            InRange = true;
        }
    }
}