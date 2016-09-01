using UnityEngine;
using System.Collections;

public class characterscript : MonoBehaviour
{
	//Apparently global variables are called "public" here, and are defined in this area first. Guess this is as good as any place to describe them.
	public Rigidbody physobj;
	//The rigidbody. Why I put A here, I don't know, but it's better to prepare for more rigidbodies even if that's an unlikely occurance.
	public bool movecontrol;
	//Can the player control movement? False if in a vehicle, in the air, etc.
	public float movespeed;
	//How fast the player can accelerate in a turn.
	public Vector3 targetspeed;
	//The speed that will be moved towards.
	public bool setveltick;
	public Vector3 setvelvect;
	//This is the vector that will replace the velocity vector. This will be used if setveltick is TRUE.
	//public float xaccel;
	//public float zaccel;
	//replaced by acceleratevector
	public Vector3 acceleratevector;
	//hard to explain.
	public float speedcap;
	//How fast the player can go max.
	public int jumptimeout;
	//camera stuff
	public Vector3 camrotation;
	public Vector3 lookdirection;

	// Use this for initialization. YEAH DUH UNITY I GET IT
	void Start( )
	{
		physobj = GetComponent<Rigidbody>();
		movecontrol = true;
		movespeed = 15;
		speedcap = 30;
		camrotation = Vector3.zero;
	}

	// Update is called once per frame FIXED UPDATE IS BETTER FOR PHYSICS STUFF!
	void FixedUpdate( )
	{
		//CAMERA
		lookdirection = transform.rotation.eulerAngles;
		lookdirection = new Vector3(lookdirection.x, lookdirection.y + (BoolToInt(Input.GetKey(KeyCode.RightArrow)) - BoolToInt(Input.GetKey(KeyCode.LeftArrow))), lookdirection.z);
		transform.rotation = Quaternion.Euler(lookdirection);


		//MOVE
		//Reset all tick-booleans to their default value.
		setveltick = false;
		//Reset the set velocity to the current velocity. This way, if it's set and an axis doesn't have a new value, it won't suddenly stop dead on that axis.
		setvelvect = physobj.velocity;
		targetspeed = new Vector3(0, 0, 0);

		//Read movement keys
		targetspeed += transform.forward * (BoolToInt(Input.GetKey(KeyCode.W)) - BoolToInt(Input.GetKey(KeyCode.S)));
		targetspeed += transform.right * (BoolToInt(Input.GetKey(KeyCode.D)) - BoolToInt(Input.GetKey(KeyCode.A)));

		//sprinting?
		if ((Input.GetKey(KeyCode.LeftShift)) || (Input.GetKey(KeyCode.RightShift))) {
			speedcap = 15;
		}
		else {
			speedcap = 20;
		}
		//debug draw the raycast
		Debug.DrawRay(transform.position, Vector3.down * 0.6F, Color.white, 0);
		//is the player on the ground?
		if (Physics.Raycast(transform.position, Vector3.down, 1F)) {
			movecontrol = true;
		}
		else {
			movecontrol = false;
		}

		//If the player has movement control, attempt to obtain the target speed on XZ
		if (movecontrol == true) {
			if (((movespeed / physobj.mass) * Time.fixedDeltaTime) >= Mathf.Sqrt(Mathf.Pow((targetspeed.normalized.x * speedcap) - physobj.velocity.x, 2) + Mathf.Pow((targetspeed.normalized.z * speedcap) - physobj.velocity.z, 2))) {
				setvelvect.x = targetspeed.normalized.x * speedcap;
				setvelvect.z = targetspeed.normalized.z * speedcap;
				physobj.velocity = setvelvect;
			}
			else {
				acceleratevector = (targetspeed.normalized * speedcap) - physobj.velocity;
				physobj.AddForce(acceleratevector.normalized.x * movespeed, 0, acceleratevector.normalized.z * movespeed, ForceMode.Acceleration);
			}
		}

		//Jumping timeout
		if (movecontrol) {
			jumptimeout -= 1;
		}
		else {
			jumptimeout = 5;
		}
		//Jumping test
		if ((Input.GetKey(KeyCode.Space)) && (movecontrol) && (jumptimeout <= 0)) {
			physobj.velocity = new Vector3(physobj.velocity.x, physobj.velocity.y + 5, physobj.velocity.z);
			jumptimeout = 10;
		}
	}

	public int BoolToInt(bool inputvar)
	{
		return inputvar ? 1 : 0;
	}

}