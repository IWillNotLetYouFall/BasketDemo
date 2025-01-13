using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBallController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] bouncesSfx;
    [SerializeField] AudioClip[] scoresSfx;
    [SerializeField] new SphereCollider collider;
    
    bool _isNear;
    bool _scored;
    int _bounceCounter;
    
    public void IsNear(bool isNear)
    {
        _isNear = isNear;
        _scored = false;
        _bounceCounter = 0;
        collider.material.bounceCombine = isNear ? PhysicMaterialCombine.Average : PhysicMaterialCombine.Maximum;
    }

    public void ScorePoint()
    {
        _scored = true;
        audioSource.clip = _bounceCounter <= 0 ? scoresSfx[0] : scoresSfx[1];
        audioSource.Play();
    }
    
    void OnCollisionEnter(Collision other)
    {
        if (!_isNear || _scored) return;
    
        _bounceCounter = Math.Min(_bounceCounter, bouncesSfx.Length - 1);
        audioSource.clip = bouncesSfx[_bounceCounter];
        audioSource.Play();
        _bounceCounter++;
    }
}
