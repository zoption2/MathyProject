using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChangeableStyle
{
    public void SubscribeOnDifficultyModeChanged();
    public void UpdateDisplayStyle(int index);
}
