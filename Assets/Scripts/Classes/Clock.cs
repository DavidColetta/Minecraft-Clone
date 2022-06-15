using UnityEngine;

public class Clock
{
    protected float startTime;
    ///<summary>
    ///True if timer is timeScale-independant.
    ///</summary>
    public readonly bool unscaled;
    protected float time{get{if (unscaled) return Time.unscaledTime; else return Time.time;}}
    ///<summary>
    ///Class which keeps track of how long it has been since it has been reset.
    ///</summary>
    public Clock(bool useUnscaledTime = false){
        unscaled = useUnscaledTime;
        Reset();
    }
    ///<summary>
    ///Restarts the clock. TimeElapsed will be 0.
    ///</summary>
    public virtual void Reset(){
        startTime = time;
    }
    ///<summary>
    ///Sets the time elapsed. If reset is false, this will added on top of previous time elapsed.
    ///</summary>
    public virtual void SetTimeElapsed(float timeElapsed, bool reset = true){
        if (reset)
            startTime = time - timeElapsed;
        else 
            startTime -= timeElapsed;
    }
    ///<summary>
    ///Returns the time in seconds elapsed since it has been reset.
    ///</summary>
    public virtual float TimeElapsed(){
        return time - startTime;
    }
    ///<summary>
    ///Returns the time in whole seconds elapsed since it has been reset.
    ///</summary>
    public virtual int CountUp(){
        return Mathf.CeilToInt(TimeElapsed());
    }
    public override string ToString()
    {
        return TimeElapsed() + " seconds elapsed";
    }
    public static implicit operator float(Clock _clock){
        return _clock.TimeElapsed();
    }
}
