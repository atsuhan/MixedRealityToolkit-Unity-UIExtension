using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RangeKnobTextView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _valueTMP;

    public void SetValue(float val)
    {
        _valueTMP.text = val.ToString();
    }
}
