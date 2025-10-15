using System;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : AbstractManager<AudioManager>
{
    [Serializable]
    public class ClipInfo
    {
        public string Name;
        public AudioClip Clip;
        public bool Loop;
        public FloatRange Pitch;
    }
    [SerializeField] private List<ClipInfo> clips;
    private Dictionary<string, ClipInfo> config;
    private class SourceInfo
    {
        public GameObject Target;
        public AudioSource Source;
    }
    private List<SourceInfo> infos;
    public override void Init()
    {
        base.Init();

        config = new Dictionary<string, ClipInfo>();
        foreach (var info in clips) config.Add(info.Name, info);
        infos = new List<SourceInfo>();

        Play("BGM", gameObject);
    }
    public void Play(string name, GameObject player)
    {
        SourceInfo curInfo = null;
        foreach (var info in infos)
            if (!info.Source.isPlaying)
            {
                curInfo = info;
                break;
            }
        if (curInfo == null)
        {
            curInfo = new SourceInfo();
            curInfo.Source = new GameObject().AddComponent<AudioSource>();
            curInfo.Source.transform.parent = transform;
            infos.Add(curInfo);
        }
        curInfo.Target = player;
        curInfo.Source.clip = config[name].Clip;
        curInfo.Source.loop = config[name].Loop;
        curInfo.Source.pitch = 1 + config[name].Pitch;
        curInfo.Source.Play();
    }
    public void Stop(string name, GameObject player)
    {
        foreach (var info in infos)
            if (info.Target == player && config[name].Clip == info.Source.clip)
                info.Source.Stop();
    }
    private void FixedUpdate()
    {
        foreach (var info in infos)
            if (info.Target != null && info.Source.isPlaying)
                info.Source.transform.position = info.Target.transform.position;
    }
}
