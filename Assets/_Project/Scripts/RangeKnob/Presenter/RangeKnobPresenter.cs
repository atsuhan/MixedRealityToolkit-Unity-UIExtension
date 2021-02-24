using UnityEngine;
using UniRx;

public class RangeKnobPresenter : MonoBehaviour
{
    [Header("Model")] [SerializeField] private RangeKnobModel _rangeKnobModel = null;

    [Header("View")] [SerializeField] private RangeKnobAngleView _rangeKnobAngleView = null;
    [SerializeField] private RangeKnobAudioView _rangeKnobAudioView = null;
    [SerializeField] private RangeKnobTextView _rangeKnobTextView = null;

    void Start()
    {
        // stream update text when model value is changed
        _rangeKnobModel
            .ObserveEveryValueChanged(model => model.Value)
            .Subscribe(val =>
            {
                _rangeKnobTextView.SetValue(val);
                _rangeKnobAudioView.PlayClickSound();
            })
            .AddTo(this);

        // stream update model value from UI
        _rangeKnobAngleView
            .ObserveEveryValueChanged(knobView => knobView.ChangeValue)
            .Where(changeVal => changeVal != 0)
            .Subscribe(changeVal => { _rangeKnobModel.AddValue(changeVal); });
    }
}