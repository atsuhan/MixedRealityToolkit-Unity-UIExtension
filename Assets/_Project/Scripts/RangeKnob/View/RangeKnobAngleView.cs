using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class RangeKnobAngleView : MonoBehaviour
{
    [SerializeField] private float _angleThreshold = 5;
    [SerializeField] private float _valueInterval = 1f;
    [SerializeField] private int _valueDecimalDigit = 0;
    [SerializeField] private float _resetAnimationDuration = 0.5f;

    [Header("AutoMode")]
    [SerializeField] private float _autoModeStartDuration = 1f;
    [SerializeField] private float _autoModeLoopInterval = 0.1f;
    [SerializeField] private float _autoModeValueRate = 0.2f;

    // public
    public float ChangeValue { get; private set; }

    public void OnManipulationStart()
    {
        _isManipulationStarted = true;
    }

    public void OnManipulationEnded()
    {
        _isManipulationStarted = false;
        ResetRotationAsync().Forget();
    }

    // private
    private Vector3 _initialKnobEuler = Vector3.zero;
    private bool _isManipulationStarted = false;
    private ReactiveProperty<bool> _isOverAngleThresholdRp = new ReactiveProperty<bool>(false);
    private CancellationTokenSource _autoModeCancelTokenSrc = null;

    private float _knobAngle => -(Mathf.Repeat(transform.localEulerAngles.z + 180, 360) - 180);
    private bool _isOverAngleThreshold => Mathf.Abs(_knobAngle) >= _angleThreshold;

    private void Start()
    {
        // Initialize private value
        _initialKnobEuler = transform.eulerAngles;

        // Update Stream that set _isOverAngleThresholdRp value
        this.UpdateAsObservable()
            .Select(_ => _isManipulationStarted && _isOverAngleThreshold)
            .Subscribe(isOver => _isOverAngleThresholdRp.Value = isOver);

        // Stream that subscribe when the angle exceeds the threshold.
        _isOverAngleThresholdRp
            .Where(isOver => isOver)
            .Subscribe(_ =>
            {
                // Change value at first
                ChangeValue = GetRoundValue(_valueInterval * Mathf.Sign(_knobAngle));
            })
            .AddTo(this);

        _isOverAngleThresholdRp
            .Where(isOver => isOver)
            .DelayFrame(1)
            .Subscribe(_ =>
            {
                // Reset value after one frame
                ChangeValue = 0;
            })
            .AddTo(this);

        // Stream about auto mode
        _isOverAngleThresholdRp
            .Where(isOver => isOver)
            .Subscribe(_ =>
            {
                // Start auto mode
                _autoModeCancelTokenSrc = new CancellationTokenSource();
                DoAutoModeAsync(_autoModeCancelTokenSrc.Token).Forget();
            })
            .AddTo(this);

        _isOverAngleThresholdRp
            .Where(isOver => !isOver)
            .Subscribe(_ =>
            {
                // Cancel auto mode
                if (_autoModeCancelTokenSrc != null)
                {
                    _autoModeCancelTokenSrc.Cancel();
                    _autoModeCancelTokenSrc = null;
                }
            })
            .AddTo(this);
    }

    private async UniTask DoAutoModeAsync(CancellationToken cancellationToken = default)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_autoModeStartDuration));
        while (true)
        {
            if (cancellationToken.IsCancellationRequested) break;

            ChangeValue = GetRoundValue(_knobAngle * _valueInterval * _autoModeValueRate);
            await UniTask.DelayFrame(1);
            ChangeValue = 0;
            await UniTask.Delay(TimeSpan.FromSeconds(_autoModeLoopInterval));
        }

        ChangeValue = 0;
    }

    private float GetRoundValue(float inputVal)
    {
        return (float) Math.Round(
            inputVal,
            _valueDecimalDigit,
            MidpointRounding.AwayFromZero
        );
    }

    private async UniTaskVoid ResetRotationAsync()
    {
        await transform
            .DOLocalRotate(_initialKnobEuler, _resetAnimationDuration)
            .SetEase(Ease.OutCubic)
            .ToUniTask();
    }
}