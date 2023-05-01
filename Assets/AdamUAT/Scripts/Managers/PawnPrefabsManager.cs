using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PawnPrefabsManager;

public class PawnPrefabsManager : MonoBehaviour
{
    public enum Pawns { Default, OrthagonalPawn, PhysicsPawn, BlockyPersonPawn, CylinderPawn}

    [SerializeField]
    private List<PawnPrefabs> pawnPrefabs = new List<PawnPrefabs>();

    public Dictionary<Pawns, GameObject> pawnPrefabsDictionary = new Dictionary<Pawns, GameObject>();

    /// <summary>
    /// The dictionary can't be modified in the editor, so we create a list/struct instead.
    /// However, since dictionaries are faster and look better in code, we convert it at the start of the game.
    /// </summary>
    public void CompileDictionary()
    {
        foreach(PawnPrefabs pawnPrefab in pawnPrefabs)
        {
            pawnPrefabsDictionary.Add(pawnPrefab.pawn, pawnPrefab.pawnPrefab);
        }
    }
}

[System.Serializable]
public struct PawnPrefabs
{
    public PawnPrefabsManager.Pawns pawn;
    public GameObject pawnPrefab;
}
