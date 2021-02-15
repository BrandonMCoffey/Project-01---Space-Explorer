using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Environments {
public class StarGenerator : MonoBehaviour {
    [Header("Dependencies"), SerializeField]
    private Transform _player = null;
    [SerializeField] private GameObject _star = null;

    [Header("Settings"), SerializeField] private int _chunkRange = 8;
    [SerializeField] private int _chunkSize = 32;
    [SerializeField] private float _delayChunkGeneration = 0.1f;
    [SerializeField] private int _starDensity = 10;
    [SerializeField] private float _noiseDensity = 0.4f;
    [SerializeField] private float _minDepth = 50;
    [SerializeField] private float _maxDepth = 200;
    [SerializeField] private int _seed = 1337;
    [SerializeField] private bool _randomSeed = true;

    private static Dictionary<ChunkPos, StarChunk> _chunks;
    private List<StarChunk> _pooledChunks;
    private List<ChunkPos> _toGenerate;
    private ChunkPos _curChunk;
    private FastNoise _noise;

    private void Awake()
    {
        _chunks = new Dictionary<ChunkPos, StarChunk>();
        _toGenerate = new List<ChunkPos>();
        _pooledChunks = new List<StarChunk>();
        _curChunk = new ChunkPos(-1, -1);
        _noise = new FastNoise(_randomSeed ? Random.Range(0, 1000) : _seed);
        if (_player == null) {
            Debug.Log("[Star Generator] No Player found");
        }
        if (_star == null) {
            Debug.Log("[Star Generator] No Star Prefab found");
        }
    }

    private void Start()
    {
        LoadChunks(true);
    }

    private void Update()
    {
        LoadChunks();
    }

    private void LoadChunks(bool instant = false)
    {
        int curChunkPosX;
        int curChunkPosZ;

        Rigidbody rb = _player.GetComponent<Rigidbody>();
        if (rb != null) {
            curChunkPosX = Mathf.FloorToInt((_player.position + 2 * rb.velocity).x / _chunkSize) * _chunkSize;
            curChunkPosZ = Mathf.FloorToInt((_player.position + 2 * rb.velocity).z / _chunkSize) * _chunkSize;
        } else {
            curChunkPosX = Mathf.FloorToInt(_player.position.x / _chunkSize) * _chunkSize;
            curChunkPosZ = Mathf.FloorToInt(_player.position.z / _chunkSize) * _chunkSize;
        }

        if (_curChunk.X == curChunkPosX && _curChunk.Z == curChunkPosZ && !instant) return;

        _curChunk.X = curChunkPosX;
        _curChunk.Z = curChunkPosZ;

        for (int x = curChunkPosX - _chunkSize * _chunkRange; x <= curChunkPosX + _chunkSize * _chunkRange; x += _chunkSize) {
            for (int z = curChunkPosZ - _chunkSize * _chunkRange; z <= curChunkPosZ + _chunkSize * _chunkRange; z += _chunkSize) {
                ChunkPos chunkPos = new ChunkPos(x, z);

                if (_chunks.ContainsKey(chunkPos) || _toGenerate.Contains(chunkPos)) continue;
                if (instant) {
                    BuildChunk(x, z);
                } else {
                    _toGenerate.Add(chunkPos);
                }
            }
        }

        // Remove chunks that are too far away
        List<ChunkPos> toDestroy = new List<ChunkPos>();
        foreach (var c in _chunks) {
            ChunkPos chunkPos = c.Key;
            if (Mathf.Abs(curChunkPosX - chunkPos.X) > _chunkSize * (_chunkRange + 2) || Mathf.Abs(curChunkPosZ - chunkPos.Z) > _chunkSize * (_chunkRange + 1)) {
                toDestroy.Add(chunkPos);
            }
        }

        /*foreach (ChunkPos chunkPos in _toGenerate) {
            if (Mathf.Abs(curChunkPosX - chunkPos.X) > _chunkSize * (_chunkRange + 1) || Mathf.Abs(curChunkPosZ - chunkPos.Z) > _chunkSize * (_chunkRange + 1)) {
                _toGenerate.Remove(chunkPos);
            }
        }*/

        for (int i = 0; i < _toGenerate.Count; i++) {
            if (Mathf.Abs(curChunkPosX - _toGenerate[i].X) > _chunkSize * (_chunkRange + 1) || Mathf.Abs(curChunkPosZ - _toGenerate[i].Z) > _chunkSize * (_chunkRange + 1)) {
                _toGenerate.Remove(_toGenerate[i--]);
            }
        }

        foreach (ChunkPos chunkPos in toDestroy) {
            _chunks[chunkPos].gameObject.SetActive(false);
            _pooledChunks.Add(_chunks[chunkPos]);
            _chunks.Remove(chunkPos);
        }

        StartCoroutine(DelayBuildChunks());
    }

    private void BuildChunk(int xPos, int zPos)
    {
        StarChunk chunk;
        if (_pooledChunks.Count > 0) {
            chunk = _pooledChunks[0];
            chunk.gameObject.SetActive(true);
            _pooledChunks.RemoveAt(0);
            chunk.transform.position = new Vector3(xPos, 0, zPos);
            chunk.transform.parent = transform;
        } else {
            GameObject chunkObject = new GameObject("StarChunk");
            chunkObject.transform.position = new Vector3(xPos, 0, zPos);
            chunkObject.transform.parent = transform;
            chunk = chunkObject.AddComponent<StarChunk>();
        }

        GenerateStars(chunk, xPos, zPos);

        _chunks.Add(new ChunkPos(xPos, zPos), chunk);
    }

    private void GenerateStars(StarChunk chunk, int x, int z)
    {
        float simplex = _noise.GetSimplex(x * _noiseDensity, z * _noiseDensity);

        int starCount = Mathf.FloorToInt((simplex + 1) * _starDensity);
        for (int i = 0; i < starCount; i++) {
            GameObject star = Instantiate(_star);
            star.transform.position = chunk.transform.position + new Vector3(Random.Range(-_chunkSize, _chunkSize), Random.Range(-_minDepth, -_maxDepth), Random.Range(-_chunkSize, _chunkSize));
            star.transform.parent = chunk.transform;
            chunk.Stars.Add(star);
        }
    }

    private IEnumerator DelayBuildChunks()
    {
        List<ChunkPos> generationList = _toGenerate;
        while (generationList.Count > 0) {
            BuildChunk(generationList[0].X, generationList[0].Z);
            generationList.RemoveAt(0);

            yield return new WaitForSeconds(_delayChunkGeneration);
        }
    }
}

internal struct ChunkPos {
    public int X;
    public int Z;

    public ChunkPos(int x, int z)
    {
        X = x;
        Z = z;
    }
}
}

// Old Generation System
/*
GameObject starContainer = new GameObject("Stars");
starContainer.transform.parent = transform;
for (int i = 0; i < _starAmount; i++) {
    GameObject newStar = Instantiate(_star);
    newStar.transform.parent = starContainer.transform;
    newStar.transform.position = new Vector3(Random.Range(-_generationRange, _generationRange), Random.Range(-_maxDepth, -_minDepth), Random.Range(-_generationRange, _generationRange));

    // DOES NOT WORK IN BUILD
    // UnityEditor.SerializedObject halo = new UnityEditor.SerializedObject(newStar.GetComponent("Halo"));
    // float brightness = Random.Range(254 - _dimEffect, 255);
    // float red = Mathf.Clamp(Random.Range(brightness - _possibleSaturation, brightness + _possibleSaturation) / 255, 0, 1);
    // float green = Mathf.Clamp(Random.Range(brightness - _possibleSaturation, brightness + _possibleSaturation) / 255, 0, 1);
    // float blue = Mathf.Clamp(Random.Range(brightness - _possibleSaturation, brightness + _possibleSaturation) / 255, 0, 1);
    // float alpha = Mathf.Clamp((255 - Random.Range(0, _dimEffect)) / 255, 0, 1);
    // halo.FindProperty("m_Color").colorValue = new Color(red, green, blue, alpha);
    // halo.ApplyModifiedProperties();
}
*/