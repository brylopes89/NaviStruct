using System.IO;
using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using TMPro;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class NavmeshController : MonoBehaviour
{
    [SerializeField] 
    private float spawnSize;
    [SerializeField] 
    private int spawnAmount;
    [SerializeField]
    private int menuSceneIndex;   
    
    public Transform[] spawnPoints;

    [SerializeField]
    private string[] avatarNames;
    

    private int spawnPicker;

    private void Start()
    {
        spawnPicker = Random.Range(0, spawnPoints.Length);

        SpawnAnimals();
    }

    void SpawnAnimals()
    {                       
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", avatarNames[Random.Range(0, avatarNames.Length)]), spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0);       

        /*var parent = new GameObject("SpawnedCharacters");
        for (int i = 0; i < spawnAmount; i++)
        {
            var value = Random.Range(0, characters.Length);

            Instantiate(characters[value], RandomNavmeshLocation(spawnSize), Quaternion.identity, parent.transform);            
        }*/
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, spawnSize);
    }
}
