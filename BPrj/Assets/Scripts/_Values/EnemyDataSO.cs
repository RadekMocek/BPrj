using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    [field: Header("Basic values")]
    [field: SerializeField] public int      MaxHealth                               { get; private set; }
    [field: SerializeField] public int      FieldOfView                             { get; private set; }
    [field: SerializeField] public float    ViewDistance                            { get; private set; }
    [field: Header("AttackState")]
    [field: SerializeField] public float    AttackMovementSpeed                     { get; private set; }
    [field: SerializeField] public int      AttackDamage                            { get; private set; }
    [field: Header("DetectingState")]
    [field: SerializeField] public float    DetectionSpeed                          { get; private set; }
    [field: SerializeField] public float    FractionOfViewDistanceToGetSuspicious   { get; private set; }
    [field: Header("InvestigateState")]
    [field: SerializeField] public float    InvestigateMovementSpeed                { get; private set; }
    [field: Header("ChaseState")]
    [field: SerializeField] public float    ChaseMovementSpeed                      { get; private set; }
    [field: Header("LookAroundState")]
    [field: SerializeField] public float    RotationPauseDuration                   { get; private set; }
    [field: Header("PatrolState")]
    [field: SerializeField] public float    PatrolMovementSpeed                     { get; private set; }
}
