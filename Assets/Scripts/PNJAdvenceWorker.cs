using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PNJAdvenceWorker : MonoBehaviour
{
    public float workDuration = 10f;     // Temps passé dans la ferme ou mine
    public float detectionRange = 40f;   // Portée de détection
    public float cooldownDuration = 20f; // Cooldown après récolte
    private bool isWorking = false;
    public bool IsWorking => isWorking;

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
                    //Debug.Log($"❌ [{gameObject.name}] Aucune ressource bâtiment détectée à proximité.");
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
            //Debug.Log($"🔍 Vérification objet : {obj.name}");

            if (!IsResource(obj))
            {
                //Debug.Log($"⛔ {obj.name} n'est pas une ressource bâtiment.");
                continue;
            }

            if (occupiedResources.Contains(obj))
            {
                //Debug.Log($"⏳ {obj.name} déjà occupé.");
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
            if (obj.name.StartsWith(res, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    IEnumerator WorkInResource(GameObject resource)
    {
        Vector3 targetPosition = resource.transform.position;
        Debug.Log($"🚜 [{gameObject.name}] Se dirige vers {resource.name}");

        while (resource != null && Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 2f);
            yield return null;
        }

        if (resource == null)
        {
            Debug.LogWarning($"❌ [{gameObject.name}] La ressource a été détruite avant d'arriver.");
            yield break; // Arrête la coroutine proprement
        }

        Debug.Log($"⛏️ [{gameObject.name}] Commence le travail dans {resource.name}");
        isWorking = true;
        float timer = 0f;
        while (timer < workDuration)
        {
            if (resource == null)
            {
                Debug.LogWarning($"❌ [{gameObject.name}] La ressource a été détruite pendant le travail.");
                isWorking = false;
                yield break;
            }

            transform.position = targetPosition;
            timer += Time.deltaTime;
            yield return null;
        }

        // Ajoute la ressource uniquement si elle existe encore
        if (resource != null)
        {
            switch (resource.name)
            {
                case "Ferme":
                    resourceManager.AddFood(20);
                    break;
                case "Pierre":
                    resourceManager.AddStone(40);
                    break;
                case "or":
                    resourceManager.AddGold(40);
                    break;
                case "Fer":
                    resourceManager.AddIron(40);
                    break;
            }

            resourceCooldowns[resource] = Time.time + cooldownDuration;
            Debug.Log($"⏳ [{resource.name}] est en cooldown pendant {cooldownDuration}s");
        }

        isWorking = false;
    }
}