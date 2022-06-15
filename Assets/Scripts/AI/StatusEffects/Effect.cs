using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Effect
{
    public EffectType type;
    public int level;
    public bool helpful;
    protected readonly Entity owner;
    protected Effect(Entity e){
        owner = e;
    }
    public virtual void Update(){}
    public virtual void End(){}
    protected virtual void Remove(){
        owner.RemoveEffect(type);
    }
    public virtual void Refresh(){}
}
public enum EffectType{
    None,
    TimedEffect,
    Burn,
    Poison,
    Slow
}
