namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Grab Attach Mechanics/VRTK_ChildOfControllerGrabAttachWithoutSnapping")]
    public class VRTK_ChildOfControllerGrabAttachWithoutSnapping : VRTK_ChildOfControllerGrabAttach
    {
        protected override void SnapObjectToGrabToController(GameObject obj)
        {
            obj.transform.SetParent(controllerAttachPoint.transform);
        }
    }
}