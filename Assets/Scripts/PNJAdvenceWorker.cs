using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PNJAdvanceWorker : MonoBehaviour
{
    public float workDuration = 10f;     // Temps passé dans la ferme ou mine
    public float detectionRange = 40f;   // Portée de détection
    public float cooldownDuration = 20f; // Cooldown après récolte
    private bool isWorking = false;
    private ResourceManager resourceManager;

    private static HashSet<GameObject> occupiedResources = new HashSet<GameObject>();
    private static Dictionary<GameObject, float> resourceCooldowns = new Dictionary<GameObject, float>();

    // Liste des noms exacts de prefab correspondant à des ressources illimitées
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
                    occupiedResources.Add(target);
                    yield return WorkInResource(target);
                    occupiedResources.Remove(target);
                }
                else
                {
                    Debug.Log($"❌ [{gameObject.name}] Aucune ressource disponible à proximité.");
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
            if (!IsResource(obj) || occupiedResources.Contains(obj)) continue;

            if (resourceCooldowns.ContainsKey(obj) && Time.time < resourceCooldowns[obj])
                continue;

            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < minDistance && dist <= detectionRange)
            {
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
            if (obj.name.Equals(res, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    IEnumerator WorkInResource(GameObject resource)
    {
        Vector3 targetPosition = resource.transform.position;
        Debug.Log($"🚜 [{gameObject.name}] Se dirige vers {resource.name}");

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 2f);
            yield return null;
        }

        Debug.Log($"⛏️ [{gameObject.name}] Commence le travail dans {resource.name}");
        isWorking = true;
        float timer = 0f;
        while (timer < workDuration)
        {
            transform.position = targetPosition;
            timer += Time.deltaTime;
            yield return null;
        }

        // Ajoute la ressource en fonction du prefab exact
        switch (resource.name)
        {
            case "Ferme":
                resourceManager.AddFood(20);
                Debug.Log($"✅ [{gameObject.name}] +20 Nourriture récoltée depuis {resource.name}");
                break;
            case "Pierre":
                resourceManager.AddStone(40);
                Debug.Log($"✅ [{gameObject.name}] +40 Pierre récoltée depuis {resource.name}");
                break;
            case "or":
                resourceManager.AddGold(40);
                Debug.Log($"✅ [{gameObject.name}] +20 Or récolté depuis {resource.name}");
                break;
            case "Fer":
                resourceManager.AddIron(40);
                Debug.Log($"✅ [{gameObject.name}] +20 Fer récolté depuis {resource.name}");
                break;
        }

        // Met en cooldown
        resourceCooldowns[resource] = Time.time + cooldownDuration;
        Debug.Log($"⏳ [{resource.name}] est en cooldown pendant {cooldownDuration}s");

        isWorking = false;
    }
}
