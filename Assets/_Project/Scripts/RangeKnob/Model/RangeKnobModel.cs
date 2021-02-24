using UnityEngine;

public class RangeKnobModel : MonoBehaviour
{
    public float Value { get; private set; }

    public void AddValue(float additionalVal)
    {
        Value += additionalVal;
    }
}