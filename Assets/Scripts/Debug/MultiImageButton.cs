using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MultiImageButton : UnityEngine.UI.Button
{

    private Graphic[] m_graphics;
    protected Graphic[] Graphics
    {
        get
        {
            if (m_graphics == null)
            {
                List<Graphic> list = new List<Graphic>();
                targetGraphic.GetComponentsInChildren<Graphic>(list);
                //foreach (Graphic g in list)
                int i = 0;
                while(i < list.Count)
                {
                    if (list[i].CompareTag("IgnoreMultiImageButton"))
                        list.Remove(list[i]);
                    else
                        i++;
                }
                m_graphics = list.ToArray();
            }
            return m_graphics;
        }
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        Color color;
        switch (state)
        {
            case Selectable.SelectionState.Normal:
                color = this.colors.normalColor;
                break;
            case Selectable.SelectionState.Highlighted:
                color = this.colors.highlightedColor;
                break;
            case Selectable.SelectionState.Pressed:
                color = this.colors.pressedColor;
                break;
            case Selectable.SelectionState.Disabled:
                color = this.colors.disabledColor;
                break;
            default:
                color = Color.black;
                break;
        }
        if (base.gameObject.activeInHierarchy)
        {
            switch (this.transition)
            {
                case Selectable.Transition.ColorTint:
                    ColorTween(color * this.colors.colorMultiplier, instant);
                    break;
                default:
                    throw new System.NotSupportedException();
            }
        }
    }

    private void ColorTween(Color targetColor, bool instant)
    {
        if (this.targetGraphic == null)
        {
            return;
        }

        foreach (Graphic g in this.Graphics)
        {
            g.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
        }
    }
}