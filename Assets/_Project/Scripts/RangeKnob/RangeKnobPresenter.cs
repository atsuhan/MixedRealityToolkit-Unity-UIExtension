using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class RangeKnobPresenter : MonoBehaviour
{
    [SerializeField] private RangeKnobModel _rangeKnobModel = null;
    [SerializeField] private RangeKnobView _rangeKnobView = null;
    [SerializeField] private RangeKnobTextView _rangeKnobTextView = null;
    
    void Start()
    {
        _rangeKnobModel
            .ObserveEveryValueChanged(model => model.Value)
            .Subscribe(_rangeKnobTextView.SetValue)
            .AddTo(this);
    }
}
