using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject avatarPrefab;
        public int size;
    }

    #region Singleton
    public static AvatarPooler instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools) 
        {
            Queue<GameObject> objectPool = new Queue<GameObject>(); //for each pool, create a queue full of objects

            for(int i = 0; i < pool.size; i++) //add all objects to the queue
            {
                GameObject obj = PhotonNetwork.Instantiate(pool.avatarPrefab.name, Vector3.zero, Quaternion.identity); //Instantiate(pool.avatarPrefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool); //Add to the dictionary
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation) //taking inactive prefabs and spawning into the world
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject avatarToSpawn = poolDictionary[tag].Dequeue(); //pulls out first element in Queue    
        avatarToSpawn.SetActive(true);
        avatarToSpawn.transform.position = position;
        avatarToSpawn.transform.rotation = rotation;

        IPooledAvatar pooledAvatar = avatarToSpawn.GetComponent<IPooledAvatar>();
        if (pooledAvatar != null)
            pooledAvatar.OnAvatarSpawn();

        poolDictionary[tag].Enqueue(avatarToSpawn); //add object back to queue to use for later;

        return avatarToSpawn;
    }
}
