using UnityEngine;

namespace HoloMeSDK
{
    public class LookRotationOneAxis : HoloMono
    {
        public Transform lookTarget;
        float damping = 15.0f;
        float lerpFactorCallibration = 0.5f;
        bool hasAddRotation = true;
        bool disableXRotation;

        protected override void OnUpdate()
        {
            if (lookTarget == null)
                return;

            var lookPos = lookTarget.position - transform.position;
            lookPos.y = 0;
            var rotation = (lookPos != Vector3.zero) ? Quaternion.LookRotation(lookPos) : Quaternion.identity;

            if (disableXRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
            }
            else
            {
                if (hasAddRotation)
                {
                    var addRotation = Quaternion.Euler(new Vector3(-lookTarget.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z));
                    rotation = Quaternion.Lerp(rotation, addRotation, lerpFactorCallibration);
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w), Time.deltaTime * damping);
            }
        }
    }
}
