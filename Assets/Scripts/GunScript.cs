using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public AudioSource Audio;
    public ParticleSystem gunParticle;
    public AudioClip[] gunSounds;
    Dictionary<string, AudioClip> gunDic;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gunDic = new Dictionary<string, AudioClip>();
        for(int i = 0; i<gunSounds.Length;i++)
        {
            gunDic.Add(gunSounds[i].name, gunSounds[i]);
        }
    }
    public void ShotLogic()
    {
        gunParticle.Play();
        Audio.PlayOneShot(gunDic["ShotgunSFX"]);
    }
}
