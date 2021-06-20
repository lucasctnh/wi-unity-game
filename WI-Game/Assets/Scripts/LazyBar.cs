using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LazyBar : MonoBehaviour
{
	public Image fastBar, slowBar;

	private float lerpFactor = 0;
	private float currentStatSlow;

	public void SetBars(Image sBar, Image bar)
	{
		slowBar = sBar;
		fastBar = bar;
	}

	public void UpdateBar(float currentStat, float maxStat)
	{
		if (currentStatSlow != currentStat)
		{
			currentStatSlow = Mathf.Lerp(currentStatSlow, currentStat, lerpFactor);
			lerpFactor += 1.0f * Time.deltaTime;
		}

		fastBar.fillAmount = currentStat/maxStat;
		slowBar.fillAmount = currentStatSlow/maxStat;
	}

	public void ResetLerpFactor()
	{
		lerpFactor = 0;
	}
}
