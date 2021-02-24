using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class RangeKnobAngleView : MonoBehaviour
{
    [SerializeField] private float _angleMin = 10f;
    [SerializeField] private float _angleMax = 120f;
    [SerializeField] private float _valueDiff = 1f;
    [SerializeField] private float _resetAnimationDuration = 0.5f;

    [Header("AutoMode")] [SerializeField] private float _autoModeStartDuration = 0.5f;
    [SerializeField] private float _autoModeIntervalMin = 0.01f;
    [SerializeField] private float _autoModeIntervalMax = 0.25f;

    private Vector3 _initialKnobEuler = Vector3.zero;
    private bool _isManipulationStarted = false;
    private ReactiveProperty<bool> _isOverAngleMinRp = new ReactiveProperty<bool>(false);
    private CancellationTokenSource _autoModeCancelTokenSrc = null;

    private float _knobAngle => -Mathf.Clamp(
        Mathf.Repeat(transform.localEulerAngles.z + 180, 360) - 180,
        -_angleMax,
        _angleMax
    );

    private bool _isOverAngleThreshold => Mathf.Abs(_knobAngle) >= _angleMin;

    // Diff Value Observable
    private Subject<float> _changedValueSubject = new Subject<float>();
    public IObservable<float> ChangedValueObservable => _changedValueSubject.AsObservable();

    private void Start()
    {
        // Initialize private value
        _initialKnobEuler = transform.eulerAngles;

        // Update Stream that set _isOverAngleThresholdRp value
        this.UpdateAsObservable()
            .Select(_ => _isManipulationStarted && _isOverAngleThreshold)
            .Subscribe(isOver => _isOverAngleMinRp.Value = isOver);

        // Stream that subscribe when the angle exceeds angle min.
        _isOverAngleMinRp
            .Where(isOver => isOver)
            .Subscribe(_ =>
            {
                // Change value at first
                _changedValueSubject.OnNext(_valueDiff * Mathf.Sign(_knobAngle));
            })
            .AddTo(this);

        // Stream about auto mode
        _isOverAngleMinRp
            .Where(isOver => isOver)
            .Subscribe(_ =>
            {
                // Start auto mode
                _autoModeCancelTokenSrc = new CancellationTokenSource();
                DoAutoModeAsync(_autoModeCancelTokenSrc.Token).Forget();
            })
            .AddTo(this);

        _isOverAngleMinRp
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
            if (cancellationToken.IsCancellationRequested) return;

            _changedValueSubject.OnNext(_valueDiff * Mathf.Sign(_knobAngle));

            float angleRate = 1 - Mathf.InverseLerp(_angleMin, _angleMax, Mathf.Abs(_knobAngle));
            float waitTime = _autoModeIntervalMin + (_autoModeIntervalMax - _autoModeIntervalMin) * angleRate;
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        }
    }

    private async UniTaskVoid ResetRotationAsync()
    {
        await transform
            .DOLocalRotate(_initialKnobEuler, _resetAnimationDuration)
            .SetEase(Ease.OutCubic)
            .ToUniTask();
    }

    // Manipulation Event
    public void OnManipulationStart()
    {
        _isManipulationStarted = true;
    }

    public void OnManipulationEnded()
    {
        _isManipulationStarted = false;
        ResetRotationAsync().Forget();
    }
}