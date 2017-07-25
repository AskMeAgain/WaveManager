using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave {

    public Vector3 pos;
    public int maxDistance;
    public float scale;

    public enum WaveType {
        GrenadeStrong, GrenadeMiddle, GrenadeLow
    };

    public Wave(WaveType type) {

        if (type == WaveType.GrenadeStrong) {
            this.scale = 1f;
            this.maxDistance = 14;
        } else if (type == WaveType.GrenadeMiddle) {
            this.scale = 0.6f;
            this.maxDistance = 10;
        } else if (type == WaveType.GrenadeLow) {
            this.scale = 0.3f;
            this.maxDistance = 7;
        }
    }

}
