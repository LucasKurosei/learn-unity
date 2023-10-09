using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelBar : MonoBehaviour
{
    public PlayerControls player;
    public TextMeshProUGUI level;
    public Slider bar;
    public Button btn;

    private Color normalColor;
    private Color disabledColor;

    void Start()
    {
        normalColor = bar.colors.normalColor;
        disabledColor = bar.colors.disabledColor;
    }

    public void SetValue(float i)
    {
        bar.value = i;
    }
    
    public void LevelUp()
    {
        bar.value += 1;
    }
    public void LevelUp(int i)
    {
        level.text = i.ToString();
    }

    public void Activate()
    {
        btn.interactable = true;
        var cb = bar.colors;
        cb.disabledColor = normalColor;
        bar.colors = cb;
    }

    public void Deactivate()
    {
        btn.interactable = false;
        var cb = bar.colors;
        cb.disabledColor = disabledColor;
        bar.colors = cb;
    }
}
