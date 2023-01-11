using Miscellaneous;
using UnityEngine;

public class DistanceActivator : MonoBehaviour
{
    private bool _isSatelliteLoaded = true;
    private static readonly float _satelliteUnloadDistance = 200F;

    public void ActivateOrDeactivateChildren()
    {
        if (IsDistantFromFocusedSatellite() && _isSatelliteLoaded)
            SetChildrenActive(false);
        else if (!IsDistantFromFocusedSatellite() && !_isSatelliteLoaded)
            SetChildrenActive(true);
    }

    private void SetChildrenActive(bool state)
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(state);
        _isSatelliteLoaded = state;
    }

    private bool IsDistantFromFocusedSatellite()
    {
        return Vector2.Distance(transform.position, References.FocusedSatellite.position) >
            _satelliteUnloadDistance;
    }
}