using TMPro;
using UnityEngine;

public class RangeKnobTextView : MonoBehaviour
{
    [SerializeField] private TMP_Text _valueTMP;
    [SerializeField] private string _numberFormat = "f0";

    public void SetValue(float val)
    {
        _valueTMP.text = val.ToString(_numberFormat);
    }
}