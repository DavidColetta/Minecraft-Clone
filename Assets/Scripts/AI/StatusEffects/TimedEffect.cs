using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEffect : Effect
{
    protected Timer duration;
    public TimedEffect(Entity e, int duration) : base(e){
        this.duration = new Timer(duration);
    }
    public override void Update() {
        if (duration.IsTriggered()) Remove();
    }
    public override void Refresh()
    {
        duration.Reset();
    }
}
