using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;
public class RandomSound : MonoBehaviour
{
    public AudioSource _source;
    public AudioClip[] audioClips;

    void Update()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        // wait a random period of time between 1 - 3 min
        yield return new WaitForSeconds(Random.Range(60,180));
        
        // 0.01% chance to play
        float rand_float = Random.Range(0, 100);
        if (rand_float <= 0.01)
        {
            // pick a random sfx from the list
            AudioClip randomClip = audioClips[Random.Range(0, audioClips.Length)];

            // adjust pan
            float _pan = Random.Range(-1, 1);
            _source.panStereo = _pan;

            // play to mess with players :)
            _source.PlayOneShot(randomClip);
        }
    }
}
