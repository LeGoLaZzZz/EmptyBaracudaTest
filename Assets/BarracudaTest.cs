using Unity.Barracuda;
using UnityEditor;
using UnityEngine;

public class BarracudaTest : MonoBehaviour
{
    [SerializeField] private NNModel modelAsset;
    [SerializeField] private bool isNnPostProcEnabled;
    [SerializeField] private WorkerFactory.Type newWorkerType = WorkerFactory.Type.Auto;

    private IWorker _worker;
    private Model _model;
    private WorkerFactory.Type _currentType;

    private void Start()
    {
        LoadModel();
    }

    private void OnDestroy()
    {
        _worker.Dispose();
    }

    private void LoadModel()
    {
        _model = ModelLoader.Load(modelAsset);
        _currentType = newWorkerType;
        _worker = WorkerFactory.CreateWorker(_currentType, _model);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (isNnPostProcEnabled)
        {
            if (_currentType != newWorkerType)
            {
                _currentType = newWorkerType;
                _worker.Dispose();
                _worker = WorkerFactory.CreateWorker(_currentType, _model);
            }

            Tensor inputTensor = new Tensor(src, channels: 3);
            _worker.Execute(inputTensor);
            Tensor outputTensor = _worker.PeekOutput();
            Graphics.Blit(outputTensor.ToRenderTexture(), dst);
            inputTensor.Dispose();
        }
        else
        {
            Graphics.Blit(src, dst);
        }
    }
}