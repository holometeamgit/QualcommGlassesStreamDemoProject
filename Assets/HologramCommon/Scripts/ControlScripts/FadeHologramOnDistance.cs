using UnityEngine;

namespace HoloMeSDK
{
    public class TransparencyHandler : HoloMono
    {
        [SerializeField]
        [Range(-1, 1)]
        float fadeDistance = 0.25f;

        //Should be the same as the clipping plane, how much earlier should the transparency hit 0 before both transforms are on one another
        [SerializeField]
        [Range(-1, 1)]
        float distanceOffset = 0.025f;

        bool isForcedTransparent;

        public Transform FadeTarget { private get; set; }

        public Material HologramMat { private get; set; }

        const string ShaderKey = "_t";

        protected override void OnUpdate()
        {
            if (!isForcedTransparent && FadeTarget != null)
            {
                float distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(FadeTarget.position.x, 0, FadeTarget.position.z)) + distanceOffset;
                float fadeValue = distance / fadeDistance;
                HologramMat.SetFloat(ShaderKey, Mathf.Clamp(fadeValue, 0, 1));
            }
        }

        public void MakeTransparent(bool makeTransparent)
        {
            isForcedTransparent = makeTransparent;
            HologramMat.SetFloat(ShaderKey, makeTransparent ? 0 : 1);
        }
    }
}