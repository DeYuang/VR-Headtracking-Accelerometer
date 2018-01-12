#pragma strict

private var mainCam 	: Transform 		= null;
private var	rb			: Rigidbody2D		= null;
private	var	rend		: SpriteRenderer	= null;

function Start () {

	mainCam = Camera.main.transform;
	rb = GetComponent(Rigidbody2D);
	rend = GetComponent(SpriteRenderer);
}


function FixedUpdate (){

	if (Vector3.Distance(transform.position, mainCam.position) >= 15f){
		rb.Sleep();
		rend.enabled = false;
	}
	else
		rend.enabled = true;
}