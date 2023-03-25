using UnityEngine;

public abstract class IndicatorElement<T> : MonoBehaviour
{
    #region FIELDS
    protected T status;
    public virtual T Status
    {
        get
        {
            return status;
        }
        set
        {
            status = value;
            UpdateDisplayStyle();
        }
    }

    #endregion

    protected abstract void UpdateDisplayStyle();
}
