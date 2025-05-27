using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PNJAdvenceWorker : MonoBehaviour
{
    public float workDuration = 10f;     // Temps passé dans la ferme ou mine
    public float detectionRange = 40f;   // Portée de détection
    public float cooldownDuration = 20f; // Cooldown après récolte
    private bool isWorking = false;
    public bool IsWorking => isWorking;  // Permet à d'autres scripts de savoir si on travaille

    private ResourceManager resourceManager;

    private static HashSet<GameObject> occupiedResources = new HashSet<GameObject>();
    private static Dictionary<GameObject, float> resourceCooldowns = new Dictionary<GameObject, float>();

    private readonly string[] resourceNames = { "Ferme", "Pierre", "or", "Fer" };

    void Start()
    {
        resourceManager = ResourceManager.Instance;
        StartCoroutine(WorkCycle());
    }

    IEnumerator WorkCycle()
    {
        while (true)
        {
            if (!isWorking)
            {
                GameObject target = FindNearestResource();

                if (target != null)
                {
                    Debug.Log($"✅ [{gameObject.name}] Ressource trouvée : {target.name}");
                    occupiedResources.Add(target);
                    yield return WorkInResource(target);
                    occupiedResources.Remove(target);
                }
                else
                {
                    Debug.Log($"❌ [{gameObject.name}] Aucune ressource bâtiment détectée à proximité.");
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    GameObject FindNearestResource()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (var obj in allObjects)
        {
            Debug.Log($"🔍 Vérification objet : {obj.name}");

            if (!IsResource(obj))
            {
                Debug.Log($"⛔ {obj.name} n'est pas une ressource bâtiment.");
                continue;
            }

            if (occupiedResources.Contains(obj))
            {
                Debug.Log($"⏳ {obj.name} déjà occupé.");
                continue;
            }

            if (resourceCooldowns.ContainsKey(obj) && Time.time < resourceCooldowns[obj])
            {
                Debug.Log($"🕒 {obj.name} en cooldown.");
                continue;
            }

            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < minDistance && dist <= detectionRange)
            {
                Debug.Log($"🎯 Candidat potentiel : {obj.name} à distance {dist}");
                minDistance = dist;
                nearest = obj;
            }
        }

        return nearest;
    }

    bool IsResource(GameObject obj)
    {
        foreach (var res in resourceNames)
        {
            // 🔥 Modification pour tolérer les suffixes "(Clone)" ou autres
            if (obj.name.StartsWith(res, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    IEnumerator WorkInResource(GameObject resource)
    {
        Vector3 targetPosition = resource.transform.position;
        isWorking = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 2f);
            yield return null;
        }

        float timer = 0f;
        while (timer < workDuration)
        {
            transform.position = targetPosition;
            timer += Time.deltaTime;
            yield return null;
        }

        switch (resource.name)
        {
            case string s when s.StartsWith("Ferme", System.StringComparison.OrdinalIgnoreCase):
                resourceManager.AddFood(20);
                Debug.Log($"🍽️ [{gameObject.name}] +20 Nourriture depuis {resource.name}");
                break;
            case string s when s.StartsWith("Pierre", System.StringComparison.OrdinalIgnoreCase):
                resourceManager.AddStone(40);
                Debug.Log($"🪨 [{gameObject.name}] +40 Pierre depuis {resource.name}");
                break;
            case string s when s.StartsWith("or", System.StringComparison.OrdinalIgnoreCase):
                resourceManager.AddGold(40);
                Debug.Log($"💰 [{gameObject.name}] +40 Or depuis {resource.name}");
                break;
            case string s when s.StartsWith("Fer", System.StringComparison.OrdinalIgnoreCase):
                resourceManager.AddIron(40);
                Debug.Log($"⚙️ [{gameObject.name}] +40 Fer depuis {resource.name}");
                break;
        }

        resourceCooldowns[resource] = Time.time + cooldownDuration;
        isWorking = false;
    }
}