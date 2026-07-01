//using UnityEngine;
//using UnityEditor;
//using Mirror;
//using System;

//public class FindMirrorPrefab : EditorWindow
//{
//    [MenuItem("Tools/Mirror - Find Prefab by Hash-ID")]
//    public static void FindPrefab()
//    {
//        // Deine ID aus dem Debug.Error Log
//        uint targetHash = 4281327350;

//        // 1. Suche in allen registrierten Prefabs des aktuellen NetworkManagers in der Szene
//        NetworkManager manager = FindFirstObjectByType<NetworkManager>();
//        if (manager != null)
//        {
//            foreach (GameObject prefab in manager.spawnPrefabs)
//            {
//                if (prefab != null && prefab.TryGetComponent(out NetworkIdentity identity))
//                {
//                    if (CheckIdentity(identity, targetHash)) return;
//                }
//            }
//        }

//        // 2. Falls nicht im Manager, durchsuche das gesamte Projekt nach allen NetworkIdentities
//        string[] allGuids = AssetDatabase.FindAssets("t:Prefab");
//        foreach (string guid in allGuids)
//        {
//            string path = AssetDatabase.GUIDToAssetPath(guid);
//            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

//            if (prefab != null && prefab.TryGetComponent(out NetworkIdentity identity))
//            {
//                if (CheckIdentity(identity, targetHash)) return;
//            }
//        }

//        Debug.LogWarning($"[Mirror Finder] Kein Prefab mit dem Hash {targetHash} im Projekt gefunden. Möglicherweise wurde das Asset gelöscht oder die ID stammt aus einer anderen Projekt-Version.");
//    }

//    private static bool CheckIdentity(NetworkIdentity identity, uint targetHash)
//    {
//        // Mirror konvertiert die System.Guid intern oft in einen 32-Bit Hash für Netzwerkpakete
//        string assetIdGuidString = identity.assetId.ToString("N");
//        uint currentHash = (uint)assetIdGuidString.GetHashCode(); // Vergleiche Standard-Hash
//        uint stableHash = GetStableHash(assetIdGuidString);       // Vergleiche deterministischen FNV-Hash

//        if (currentHash == targetHash || stableHash == targetHash)
//        {
//            Debug.Log($"🎉 <b>[Mirror Finder] Asset gefunden!</b> Pfad: <color=cyan>{AssetDatabase.GetAssetPath(identity.gameObject)}</color>", identity.gameObject);
//            Selection.activeObject = identity.gameObject; // Hebt das Prefab direkt im Projekt-Tab hervor
//            return true;
//        }
//        return false;
//    }

//    private static uint GetStableHash(string str)
//    {
//        unchecked
//        {
//            uint hash = 2166136261;
//            foreach (char c in str)
//            {
//                hash = (hash ^ c) * 16777619;
//            }
//            return hash;
//        }
//    }
//}
