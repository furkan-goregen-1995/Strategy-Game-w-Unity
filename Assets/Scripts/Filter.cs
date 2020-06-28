using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Filter : MonoBehaviour
{

public Mesh shape;
public Material material;

MeshFilter filter;
MeshRenderer render;

MeshCollider coll;

private void Start() {

filter = this.gameObject.AddComponent<MeshFilter>();
filter.mesh=shape;

render = this.gameObject.AddComponent<MeshRenderer>();
render.material = material;

coll = this.gameObject.AddComponent<MeshCollider>();

}
}
