using UnityEngine;

public class ObservableProp : MonoBehaviour, IObservable
{
    [Header("IObservable")]
    [SerializeField] private string propName;

    public string GetName() => propName;
}
