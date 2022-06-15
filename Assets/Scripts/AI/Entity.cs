using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Percentage hp;
    [SerializeField] private List<Effect> effects;
    private List<Effect> effectsToAdd;
    private List<EffectType> effectsToRemove;
    private int damageTakenThisFrame = 0;
    #region Monobehavior Functions
    protected virtual void Awake() {
        effects = new List<Effect>();
        effectsToAdd = new List<Effect>();
        effectsToRemove = new List<EffectType>();
    }
    protected virtual void FixedUpdate() {
        HandleDebuffs();
        damageTakenThisFrame = 0;
        if (Input.GetKeyDown(KeyCode.S)){
            
        }
    }
    #endregion
    #region Damage
    public virtual void TakeDamage(int damageToTake){
        hp.val -= damageToTake;
        damageTakenThisFrame += damageToTake;
        if (hp.val <= 0){
            Die();
        }
    }
    public virtual void Die(){
        if (damageTakenThisFrame >= hp.max){
            //Gib
        }
        Destroy(gameObject);
    }
    #endregion
    #region Effects
    private void HandleDebuffs() {
        if (effectsToRemove.Count > 0){
            if (effects.Count > 0){
                for (int i = 0; i < effectsToRemove.Count; i++)
                {//Remove effects
                    int effectFound = effects.FindIndex(x => x.type == effectsToRemove[i]);
                    if (effectFound != -1){
                        effects[effectFound].End();
                        effects.RemoveAt(effectFound);
                    }
                }
            }
            effectsToRemove.Clear();
        }
        if (effectsToAdd.Count > 0){
            for (int i = 0; i < effectsToAdd.Count; i++)
            {//Add effects
                int effectFound = effects.FindIndex(x => x.type == effectsToAdd[i].type);
                if (effectFound == -1){
                    effects.Add(new TimedEffect(this, 1));
                } else
                    effects[effectFound].Refresh();
            }
            effectsToAdd.Clear();
        }
        for (int i = 0; i < effects.Count; i++)
        {//Update effects
            effects[i].Update();
        }
    }
    public void AddEffect(Effect effect){
        effectsToAdd.Add(effect);
    }
    public void RemoveEffect(EffectType type){
        effectsToRemove.Add(type);
    }
    public int FindEffect(EffectType type){
        return effects.FindIndex(x => x.type == type);
    }
    public bool HasEffect(EffectType type){
        return effects.Exists(x => x.type == type);
    }
    #endregion
}
