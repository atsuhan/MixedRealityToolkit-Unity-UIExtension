using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class RangeKnobView : MonoBehaviour
{
    [SerializeField] private float _rotationThreshold = 10;
    [SerializeField] private float _autoWaitDuration = 0.5f;
    [SerializeField] private float _speed = 1;
    [SerializeField] private float _resetAnimationDuration = 0.5f;
    
    private Vector3 _initLocalEulerAngle;
    
    // Start is called before the first frame update
    void Start()
    {
        _initLocalEulerAngle = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async UniTaskVoid ResetRotation()
    {
        await transform.DOLocalRotate(_initLocalEulerAngle, _resetAnimationDuration).ToUniTask();
    }
}
