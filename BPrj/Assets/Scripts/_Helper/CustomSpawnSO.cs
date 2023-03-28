using UnityEngine;

[CreateAssetMenu(fileName = "CustomSpawnData", menuName = "ScriptableObjects/CustomSpawnSO")]
public class CustomSpawnSO : ScriptableObject
{
    [field: SerializeField] public bool     Enabled     { get; private set; }
    [field: SerializeField] public string   SceneName   { get; private set; }
    [field: SerializeField] public float    PlayerX     { get; private set; }
    [field: SerializeField] public float    PlayerY     { get; private set; }
}
