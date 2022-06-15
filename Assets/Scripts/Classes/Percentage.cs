using UnityEngine;
[System.Serializable]
public struct Percentage
{
    [SerializeField] private int _max;
    ///<summary>
    ///The max value of the percentage.
    ///</summary>
    public int max{get {return _max;} set{_max = Mathf.Max(value, 1);}}
    ///<summary>
    ///The value of the percentage.
    ///</summary>
    public int val;
    ///<summary>
    ///The value of the percentage, clamped between 0 and the max.
    ///</summary>
    public int valClamped{get {return Mathf.Clamp(val,0,max);} set{val = Mathf.Clamp(value,0,max);}}
    ///<summary>
    ///The percentage of the value over the max value, on a scale of 0 to 1.
    ///Values can be outside this range if val is greater than max or less than 0.
    ///</summary>
    public float percent{get {return (float)val/max;} set {val = Mathf.RoundToInt(value*max);}}
    ///<summary>
    ///The percentage of the value over the max value, on a scale of 0 to 1.
    ///Automatically clamps values outside this range.
    ///</summary>
    public float percentClamped{get {return Mathf.Clamp01(percent);} set{percent = Mathf.Clamp01(value);}}
    ///<summary>
    ///The percentage of the value over the max value, on a scale of 0 to 100.
    ///Values can be outside this range if val is greater than max or less than 0.
    ///</summary>
    public int percent100{get{return Mathf.RoundToInt(percent*100);} set{percent = (float)(value)/100;}}
    ///<summary>
    ///A class which stores the percentage of value over a max value.
    ///</summary>
    public Percentage(int maxVal, float defaultPercent = 1) : this(){
        max = maxVal;
        percent = defaultPercent;
    }
    ///<summary>
    ///A class which stores the percentage of value over a max value.
    ///</summary>
    public Percentage(int maxVal, int defaultVal) : this(){
        max = maxVal;
        val = defaultVal;
    }
    ///<summary>
    ///Changes the max while keeping the percentage the same.
    ///</summary>
    public void SetMax(int newMax){
        float _percent = percent;
        max = newMax;
        percent = _percent;
    }
    public override string ToString()
    {
        return val+" / "+max+" = "+percent100+"%";
    }
    public static implicit operator int(Percentage _percent){
        return _percent.val;
    }
}
