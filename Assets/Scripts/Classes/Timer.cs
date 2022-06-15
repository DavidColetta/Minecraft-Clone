using UnityEngine;
public class Timer : Clock
{
    private Clock pauseClock = null;
    ///<summary>
    ///Whether or not the timer is paused.
    ///</summary>
    public bool paused{get{return pauseClock != null;} set{SetPause(value);}}
    ///<summary>
    ///Time in seconds from the timer's start until the timer is triggered (Read Only).
    ///When this duration has been reached, triggered functions will return true.
    ///</summary>
    public float duration{get; private set;}
    ///<summary>
    ///Class which will be triggered after the duration has been reached.
    ///Can be triggered multiple times. Can be paused.
    ///</summary>
    public Timer(float duration, bool useUnscaledTime = false) : base(useUnscaledTime){
        SetDuration(duration);
    }
    ///<summary>
    ///Returns true if timer duration has been reached. 
    ///</summary>
    public bool IsTriggered(){
        return TimeElapsed() >= duration;
    }
    ///<summary>
    ///Returns true if timer duration has been reached. 
    ///If the duration has been reached, timer is reset.
    ///</summary>
    public bool IsTriggeredReset(){
        if (TimeElapsed() >= duration){
            Reset();
            return true;
        } else
            return false;
    }
    ///<summary>
    ///Returns true if timer duration has been reached, and if so, jumps the timer forward once by the duration.
    ///If jumpOnlyOnce is set to false, timer will jump until it is no longer triggered.
    ///</summary>
    public bool IsTriggeredJump(bool jumpOnlyOnce = true){
        if (TimeElapsed() >= duration){
            if (jumpOnlyOnce) startTime += duration;
            else startTime += TimesTriggered() * duration;
            return true;
        } else
            return false;
    }
    ///<summary>
    ///Returns the number of times the timer has been triggered, assuming the timer loops.
    ///</summary>
    public int TimesTriggered(){
        return Mathf.FloorToInt(PercentTimeElapsed());;
    }
    ///<summary>
    ///Returns the number of times the timer has been triggered, assuming the timer loops.
    ///If the timer has been triggered at least once, timer is reset.
    ///</summary>
    public int TimesTriggeredReset(){
        int timesTriggered = TimesTriggered();
        if (timesTriggered >= 1) Reset(); 
        return timesTriggered;
    }
    ///<summary>
    ///Returns the number of times the timer has been triggered, assuming the timer loops.
    ///Jumps the timer forward by duration until it is no longer triggered.
    ///</summary>
    public int TimesTriggeredJump(){
        int timesTriggered = TimesTriggered();
        startTime += timesTriggered*duration;
        return timesTriggered;
    }
    public override void Reset(){
        startTime = time;
        if (paused) pauseClock.Reset();
    }
    ///<summary>
    ///Resets the timer and sets the duration.
    ///</summary>
    public void SetDurationReset(float newDuration){
        Reset();
        SetDuration(newDuration);
    }
    ///<summary>
    ///Sets the duration of the timer. When this duration has been reached, triggered functions will return true.
    ///Warning: Can cause trigger(s) to suddenly appear or dissapear if not used right after reset.
    ///</summary>
    public void SetDuration(float newDuration){
        if (newDuration > 0)
            duration = newDuration;
        else Debug.LogError("Duration "+newDuration+" of timer must be greater than 0.");
    }
    ///<summary>
    ///Sets the duration of the timer. The current number of triggers will stay the same,
    ///as will the % progress towards the next trigger.
    ///</summary>
    public void SetDurationStable(float newDuration){
        float percentTimeElapsed = PercentTimeElapsed();
        SetDuration(newDuration);
        SetTriggered(percentTimeElapsed);
    }
    ///<summary>
    ///Sets the number of times the timer has been triggered. Make negative to remove triggers.
    ///If reset is false, these triggers will be applied on top of previous triggers.
    ///</summary>
    public void SetTriggered(float timesTriggered = 1, bool reset = true){
        SetTimeElapsed(duration * timesTriggered, reset);
    }
    public override void SetTimeElapsed(float timeElapsed, bool reset = true){
        if (reset) {
            startTime = time - timeElapsed;
            if (paused) pauseClock.Reset();
        } else 
            startTime -= timeElapsed;
    }
    private void SetPause(bool pause){
        if (pause){
            if (!paused){
                pauseClock = new Clock(unscaled);
            }
        } else {
            if (paused){
                startTime += pauseClock.TimeElapsed();
                pauseClock = null;
            }
        }
    }
    public override float TimeElapsed(){
        if (paused) return time - (startTime + pauseClock.TimeElapsed());
        else return time - startTime;
    }
    ///<summary>
    ///Returns the time in seconds elapsed since the timer has been reset.
    ///If loop is true, returns the time since the previous duration was reached.
    ///</summary>
    public float TimeElapsed(bool loop){
        if (loop) return TimeElapsed() % duration;
        else return TimeElapsed();
    }
    ///<summary>
    ///Returns how close to the duration the timer is, as a percentage.
    ///If loop is true, result will always be less than 1. 
    ///Otherwise, result can be >= 1 if timer has already been triggered.
    ///</summary>
    public float PercentTimeElapsed(bool loop = false){
        return TimeElapsed(loop) / duration;
    }
    ///<summary>
    ///Returns the time in seconds remaining until the duration is reached.
    ///If loop is true, returns the time until the next duration is reached.
    ///</summary>
    public float TimeRemaining(bool loop = false){
        return duration - TimeElapsed(loop);
    }
    ///<summary>
    ///Returns the time in whole seconds elapsed since the timer has been reset.
    ///Count will cap at the duration and not increase further.
    ///</summary>
    public override int CountUp(){
        return Mathf.CeilToInt(Mathf.Min(TimeElapsed(),duration));
    }
    ///<summary>
    ///Returns the time in whole seconds elapsed since the timer has been reset.
    ///If loop is true, returns the time since the previous duration was reached.
    ///Count will cap at the duration and not increase further.
    ///</summary>
    public int CountUp(bool loop){
        return Mathf.CeilToInt(Mathf.Min(TimeElapsed(loop),duration));
    }
    ///<summary>
    ///Returns the time in whole seconds remaining until the duration is reached.
    ///If loop is true, returns the time until the next duration is reached.
    ///Count will stop at 0 and not become negative.
    ///</summary>
    public int CountDown(bool loop = false){
        return Mathf.CeilToInt(Mathf.Max(TimeRemaining(loop),0));
    }
    public override string ToString()
    {
        return TimeElapsed() + " out of "+duration+" seconds elapsed";
    }
}
