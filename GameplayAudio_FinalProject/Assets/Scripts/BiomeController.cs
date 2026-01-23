using System.Collections;
using UnityEngine;

public class BiomeController : MonoBehaviour
{
    [Header("Gameobj References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject wwiseHandler;

    private bool music1Running = false;
    private bool music2Running = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        StartCoroutine(BiomeChecker());
    }

    private IEnumerator BiomeChecker()
    {
        RaycastHit hit;
        Debug.Log("running biome checker");
        while (true)
        {
            Physics.Raycast(player.transform.position, -player.transform.up, out hit);
            if (hit.collider.CompareTag("Biome1"))
            {
                Debug.Log("running biome1");

                if (music1Running == false)
                {
                    AkUnitySoundEngine.StopAll(wwiseHandler);
                    AkUnitySoundEngine.SetSwitch("Biomes", "Biome1", wwiseHandler);
                    AkUnitySoundEngine.PostEvent("Biome_Swap", wwiseHandler);
                }

                music2Running = false;
                music1Running = true;

                yield return new WaitForSeconds(10);
            }
            else if (hit.collider.CompareTag("Biome2"))
            {
                Debug.Log("running biome2");
                
                if (music2Running == false)
                {
                    AkUnitySoundEngine.StopAll(wwiseHandler);
                    AkUnitySoundEngine.SetSwitch("Biomes", "Biome2", wwiseHandler);
                    AkUnitySoundEngine.PostEvent("Biome_Swap", wwiseHandler);
                }

                music2Running = true;
                music1Running=false;

                yield return new WaitForSeconds(10);
            }
        }
    }
}
