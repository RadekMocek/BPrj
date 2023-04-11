using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    [field: SerializeField] public int      MaxHealth               { get; private set; }
    [field: SerializeField] public int      FieldOfView             { get; private set; }
    [field: SerializeField] public float    ViewDistance            { get; private set; }

    [field: Header("LookAroundState")]
    [field: SerializeField] public float    RotationPauseDuration   { get; private set; }
}
