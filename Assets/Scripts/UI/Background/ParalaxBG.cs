using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ParalaxBG : MonoBehaviour
{
	#region Fields

	[SerializeField] private RectTransform bgImage;
	[SerializeField] private float shiftStrength = 200f;

	#endregion

	public void MoveBG(int direction)
    {
		bgImage.DOLocalMoveX(shiftStrength * direction, 1f);
	}
}
