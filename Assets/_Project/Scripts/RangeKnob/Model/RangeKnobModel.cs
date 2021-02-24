using UnityEngine;

public class RangeKnobModel : MonoBehaviour
{
    public float Value { get; private set; } = 0f;

    public void AddValue(float additionalVal)
    {
        Value += additionalVal;
    }
}