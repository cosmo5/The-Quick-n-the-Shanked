using UnityEngine;
using System.Collections;

public class MovingEntity : AI_Entity<MovingEntity> {
    protected Vector3 velocity;
    public Vector3 vehicleHeading;

    protected float mass = 10;
    public float maxSpeed;
    public float maxForce;
    public float rotSpeed;

    
}
