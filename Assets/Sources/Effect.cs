using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Effect : MonoBehaviour {
    private const string IMPACT_RESOURCE = "Impact";

    private AudioSource _source;

    public void Awake() {
        _source = GetComponent<AudioSource>();
    }

    public static void PlayHitSound(Vector3 position) {
        var resource = Resources.Load<Effect>(IMPACT_RESOURCE);
        var effect = Instantiate<Effect>(resource);
        effect.transform.position = position;
    }

    public void Start() {
        _source.Play();
    }

    public void Update() {
        if (!_source.isPlaying) {
            Destroy(gameObject);
        }
    }
}
