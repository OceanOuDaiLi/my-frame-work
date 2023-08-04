/// <Licensing>
/// ?2011 (Copyright) Path-o-logical Games, LLC
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <description>
/// PoolManager v2.0
///  - PoolManager.Pools is not a complete implimentation of the IDictionary interface
///    Which enabled:
///        * Much more acurate and clear error handling
///        * Member access protection so you can't run anything you aren't supposed to.
///  - Moved all functions for working with Pools from PoolManager to PoolManager.Pools
///    which enabled shorter names to reduces the length of lines of code.
/// Online Docs: http://docs.poolmanager2.path-o-logical.com
/// </description>
public static class PoolManager
{
    public static readonly SpawnPoolsDict Pools = new SpawnPoolsDict();
}


public class SpawnPoolsDict : IDictionary<string, SpawnPool>
{   
    #region Public Custom Memebers
    /// <summary>
    /// Creates a new GameObject with a SpawnPool Component which registers itself
    /// with the PoolManager.Pools dictionary. The SpawnPool can then be accessed 
    /// directly via the return value of this function or by via the PoolManager.Pools 
    /// dictionary using a 'key' (string : the name of the pool, SpawnPool.poolName).
    /// </summary>
    /// <param name="poolName">
    /// The name for the new SpawnPool. The GameObject will have the word "Pool"
    /// Added at the end.
    /// </param>
    /// <returns>A reference to the new SpawnPool component</returns>
    public SpawnPool Create(string poolName)
    {
        // Cannot request a name with the word "Pool" in it. This would be a 
        //   rundundant naming convention and is a reserved word for GameObject
        //   defaul naming
        string tmpPoolName;
        tmpPoolName = poolName.Replace("Pool", "");
        if (tmpPoolName != poolName)  // Warn if "Pool" was used in poolName
        {
            // Log a warning and continue on with the fixed name
            string msg = string.Format("'{0}' has the word 'Pool' in it. " + 
                   "This word is reserved for GameObject defaul naming. " +
                   "The pool name has been changed to '{1}'", 
                   poolName, tmpPoolName);

            Debug.LogWarning(msg);
            poolName = tmpPoolName;
        }

        if (this.ContainsKey(poolName))
        {
            Debug.Log(string.Format("A pool with the name '{0}' already exists", 
                                    poolName));
            return null;
        }

        // Add "Pool" to the end of the poolName to make a more user-friendly
        //   GameObject name. This gets stripped back out in SpawnPool Awake()
        var groupGO = new GameObject(poolName + "Pool");

        // Note: This will run Awake() to finish init and Add self-add the pool
        var spawnPool = groupGO.AddComponent<SpawnPool>();

        return spawnPool;
    }
    
    
    /// <summary>
    /// Returns a formatted string showing all the pool names
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        // Get a string[] array of the keys for formatting with join()
        var keysArray = new string[this._pools.Count];
        this._pools.Keys.CopyTo(keysArray, 0);

        // Return a comma-sperated list inside square brackets (Pythonesque)
        return string.Format("[{0}]", System.String.Join(", ", keysArray));
    }
    
    
    
    /// <summary>
    /// Destroy an entire SpawnPool, including its GameObject and all children.
    /// You can also just destroy the GameObject directly to achieve the same result.
    /// This is really only here to make it easier when a reference isn't at hand.
    /// </summary>
    /// <param name="spawnPool"></param>
    public bool Destroy(string poolName)
    {
        // Use TryGetValue to avoid KeyNotFoundException.
        //   This is faster than Contains() and then accessing the dictionary
        SpawnPool spawnPool;
        if (!this._pools.TryGetValue(poolName, out spawnPool))
        {
            Debug.LogError(
                string.Format("PoolManager: Unable to destroy '{0}'. Not in PoolManager",
                              poolName));
            return false;
        }

        // The rest of the logic will be handled by OnDestroy() in SpawnPool
        UnityEngine.Object.Destroy(spawnPool.gameObject);
        return true;
    }

    /// <summary>
    /// Destroy ALL SpawnPools, including their GameObjects and all children.
    /// You can also just destroy the GameObjects directly to achieve the same result.
    /// This is really only here to make it easier when a reference isn't at hand.
    /// </summary>
    /// <param name="spawnPool"></param>
    public void DestroyAll()
    {
        foreach (KeyValuePair<string, SpawnPool> pair in this._pools)
            UnityEngine.Object.Destroy(pair.Value);
    }
    #endregion Public Custom Memebers



    #region Dict Functionality
    // Internal (wrapped) dictionary
    private Dictionary<string, SpawnPool> _pools = new Dictionary<string, SpawnPool>();

    /// <summary>
    /// Used internally by SpawnPools to add themseleves on Awake().
    /// Use PoolManager.CreatePool() to create an entirely new SpawnPool GameObject
    /// </summary>
    /// <param name="spawnPool"></param>
    internal void Add(SpawnPool spawnPool)
    {
        // Don't let two pools with the same name be added. See error below for details
        if (this.ContainsKey(spawnPool.poolName))
        {
            Debug.LogError(string.Format("A pool with the name '{0}' already exists. " +
                                            "This should only happen if a SpawnPool with " +
                                            "this name is added to a scene twice.",
                                         spawnPool.poolName));
            return;
        }

        this._pools.Add(spawnPool.poolName, spawnPool);
    }

    // Keeping here so I remember we have a NotImplimented overload (original signature)
    public void Add(string key, SpawnPool value)
    {
        string msg = "SpawnPools add themselves to PoolManager.Pools when created, so " + 
                     "there is no need to Add() them explicitly. Create pools using " +
                     "PoolManager.Pools.Create() or add a SpawnPool component to a " +
                     "GameObject.";
        throw new System.NotImplementedException(msg);
    }


    /// <summary>
    /// Used internally by SpawnPools to remove themseleves on Destroy().
    /// Use PoolManager.DestroyPool() to destroy an entire SpawnPool GameObject.
    /// </summary>
    /// <param name="spawnPool"></param>
    internal bool Remove(SpawnPool spawnPool)
    {
        if (!this.ContainsKey(spawnPool.poolName))
        {
            Debug.LogError(string.Format("PoolManager: Unable to remove '{0}'. " +
                                            "Pool not in PoolManager",
                                        spawnPool.poolName));
            return false;
        }

        this._pools.Remove(spawnPool.poolName);
        return true;
    }

    // Keeping here so I remember we have a NotImplimented overload (original signature)
    public bool Remove(string poolName)
    {
        string msg = "SpawnPools can only be destroyed, not removed and kept alive" +
                     " outside of PoolManager. There are only 2 legal ways to destroy " +
                     "a SpawnPool: Destroy the GameObject directly, if you have a " +
                     "reference, or use PoolManager.Destroy(string poolName).";
        throw new System.NotImplementedException(msg);
    }

    /// <summary>
    /// Get the number of SpawnPools in PoolManager
    /// </summary>
    public int Count { get { return this._pools.Count; } }

    /// <summary>
    /// Returns true if a pool exists with the passed pool name.
    /// </summary>
    /// <param name="poolName">The name to look for</param>
    /// <returns>True if the pool exists, otherwise, false.</returns>
    public bool ContainsKey(string poolName) 
    { 
        return this._pools.ContainsKey(poolName); 
    }

    /// <summary>
    /// Used to get a SpawnPool when the user is not sure if the pool name is used.
    /// This is faster than checking IsPool(poolName) and then accessing Pools][poolName.]
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(string poolName, out SpawnPool spawnPool)
    {
        return this._pools.TryGetValue(poolName, out spawnPool);
    }



    #region Not Implimented
    public bool Contains(KeyValuePair<string, SpawnPool> item)
    {
        string msg = "Use PoolManager.Pools.Contains(string poolName) instead.";
        throw new System.NotImplementedException(msg);
    }

    public SpawnPool this[string key]
    {
        get
        {
            SpawnPool pool;
            try
            {
                pool = this._pools[key];
            }
            catch (KeyNotFoundException)
            {
                string msg = string.Format("A Pool with the name '{0}' not found. " + 
                                            "\nPools={1}",
                                            key, this.ToString());
                throw new KeyNotFoundException(msg);
            }

            return pool;
        }
        set
        {
            string msg = "Cannot set PoolManager.Pools[key] directly. " +
                "SpawnPools add themselves to PoolManager.Pools when created, so " +
                "there is no need to set them explicitly. Create pools using " +
                "PoolManager.Pools.Create() or add a SpawnPool component to a " +
                "GameObject.";
            throw new System.NotImplementedException(msg);
        }
    }

    public ICollection<string> Keys
    {
        get 
        {
            string msg = "If you need this, please request it.";
            throw new System.NotImplementedException(msg);
        }
    }


    public ICollection<SpawnPool> Values
    {
        get
        {
            string msg = "If you need this, please request it.";
            throw new System.NotImplementedException(msg);
        }
    }


    #region ICollection<KeyValuePair<string,SpawnPool>> Members
    private bool IsReadOnly { get { return true; } }
    bool ICollection<KeyValuePair<string, SpawnPool>>.IsReadOnly { get { return true; } }

    public void Add(KeyValuePair<string, SpawnPool> item)
    {
        string msg = "SpawnPools add themselves to PoolManager.Pools when created, so " +
                     "there is no need to Add() them explicitly. Create pools using " +
                     "PoolManager.Pools.Create() or add a SpawnPool component to a " +
                     "GameObject.";
        throw new System.NotImplementedException(msg);
    }

    public void Clear()
    {
        string msg = "Use PoolManager.Pools.DestroyAll() instead.";
        throw new System.NotImplementedException(msg);

    }

    private void CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
    {
        string msg = "PoolManager.Pools cannot be copied";
        throw new System.NotImplementedException(msg);
    }

    void ICollection<KeyValuePair<string, SpawnPool>>.CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
    {
        string msg = "PoolManager.Pools cannot be copied";
        throw new System.NotImplementedException(msg);
    }

    public bool Remove(KeyValuePair<string, SpawnPool> item)
    {
        string msg = "SpawnPools can only be destroyed, not removed and kept alive" +
                     " outside of PoolManager. There are only 2 legal ways to destroy " +
                     "a SpawnPool: Destroy the GameObject directly, if you have a " +
                     "reference, or use PoolManager.Destroy(string poolName).";
        throw new System.NotImplementedException(msg);
    }
    #endregion ICollection<KeyValuePair<string, SpawnPool>> Members
    #endregion Not Implimented




    #region IEnumerable<KeyValuePair<string,SpawnPool>> Members
    public IEnumerator<KeyValuePair<string, SpawnPool>> GetEnumerator()
    {
        return this._pools.GetEnumerator();
    }
    #endregion



    #region IEnumerable Members
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this._pools.GetEnumerator();
    }
    #endregion

    #endregion Dict Functionality

}
