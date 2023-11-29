using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnDistance : MonoBehaviour
{
    private AudioSource _audio;
    [SerializeField] private AudioClip _clip;
    private Transform _playerTransform;
    [SerializeField] private float _maxDistance = 5.0f;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        CheckDistance();
    }

    private void CheckDistance()
    {
        if (_playerTransform != null)
        {
            Vector3 start = transform.position;
            Vector3 end = _playerTransform.position;

            float distance = Vector3.Distance(start, end);

            if (distance <= _maxDistance)
            {
                _audio.PlayOneShot(_clip);
            }
            else
            {
                _audio.Stop();
            }
        }
    }
}