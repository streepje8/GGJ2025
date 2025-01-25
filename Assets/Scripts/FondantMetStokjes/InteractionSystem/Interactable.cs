using FondantMetStokjes.Player;
using UnityEngine;

namespace FondantMetStokjes.InteractionSystem
{
    public abstract class Interactable : MonoBehaviour
    {
        [field: Header("Interactable Settings")]
        [field: SerializeField] public float Range { get; protected set; } = 10f;
        [field: SerializeField] public bool IsInteractable { get; set; } = true;
        [field: SerializeField] public IconGraphicData IconGraphics { get; protected set; }
    
        public virtual ControllerButton InteractionStartButton { get; } = ControllerButton.Cross;
        public abstract void OnInteract(Interactor interactor);

        public virtual void OnEnterRange() { }
        public virtual void OnExitRange() { }

        public bool InRange { get; private set; } = false;
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
        }

        private void Update()
        {
            if (graphicEnabled)
            {
                if (InRange)
                {
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
                    if (graphicT > 0)
                    {
                        graphicT -= Time.deltaTime / IconGraphics.AnimationDuration;
                        UpdateGraphicAnimation(graphicT);
                    }
                    else graphicGameObj.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateGraphicAnimation(float t)
        {
            graphicGameObj.localPosition = Vector3.Lerp(Vector3.zero, IconGraphics.IconOffset, IconGraphics.OffsetAnimation.Evaluate(t));
            graphicGameObj.localScale = Vector3.Lerp(Vector3.zero, IconGraphics.IconScale, IconGraphics.ScaleAnimation.Evaluate(t));
            graphicGameObj.rotation = Quaternion.LookRotation(graphicGameObj.transform.position - Vector3.forward * 10.0f - Vector3.up, Vector3.up);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, Range);
        }
    
        public void SetInRange()
        {
            if (!InRange)
            {
                OnEnterRange();
            }
            inRangeTimer = 0.2f;
            InRange = true;
        }
    }
}