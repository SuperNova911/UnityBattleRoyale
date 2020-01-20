/*--------------------------------------------------------
  ProgressiveMeshRuntime.cs

  Created by MINGFEN WANG on 13-12-26.
  Copyright (c) 2013 MINGFEN WANG. All rights reserved.
  http://www.mesh-online.net/
--------------------------------------------------------*/
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace MantisLODEditor {
	class ProgressiveMeshRuntime: MonoBehaviour {
		// Drag a reference or assign it with code
		public ProgressiveMesh progressiveMesh = null;

		// Optional fields
		public Text fpsHint = null;
		public Text lodHint = null;
		public Text triangleHint = null;

		// Clamp lod to [minLod, maxLod]
		public int minLod = 0;
		public int maxLod = ProgressiveMesh.max_lod_count-1;

		private Mesh[] cloned_meshes = null;
		private Mesh[] shared_meshes = null;
		private int current_lod = -1;

		private Component[] allBasicRenderers = null;
		
		// How often to check lod changes, default four times per second.
		// You may increase the value to balance the load if you have hundreds of 3d models in the scene.
		private float UpdateInterval = 0.25f;
		private float currentTimeToInterval = 0.0f;
		private bool culled = false;
		private bool working = false;
		
		#if UNITY_EDITOR
		[MenuItem("Window/Mantis LOD Editor/Component/Runtime/Progressive Mesh Runtime")]
		public static void AddComponent() {
			GameObject SelectedObject = Selection.activeGameObject;
			if (SelectedObject) {
				// Register root object for undo.
				Undo.RegisterCreatedObjectUndo(SelectedObject.AddComponent(typeof(ProgressiveMeshRuntime)), "Add Progressive Mesh Runtime");
			}
		}
		[MenuItem ("Window/Mantis LOD Editor/Component/Runtime/Progressive Mesh Runtime", true)]
		static bool ValidateAddComponent () {
			// Return false if no gameobject is selected.
			return Selection.activeGameObject != null;
		}
		#endif
		void Awake() {
			get_all_meshes();
		}
		// Use this for initialization
		void Start() {
		}
		private float ratio_of_screen() {
			Vector3 min = new Vector3(float.MaxValue,float.MaxValue,float.MaxValue);
			Vector3 max = new Vector3(float.MinValue,float.MinValue,float.MinValue);
			foreach (Component child in allBasicRenderers) {
				Renderer rend = (Renderer)child;
				Vector3 center = rend.bounds.center;
				float radius = rend.bounds.extents.magnitude;
				Vector3[] six_points = new Vector3[6];
				six_points[0] = Camera.main.WorldToScreenPoint(new Vector3(center.x-radius, center.y, center.z));
				six_points[1] = Camera.main.WorldToScreenPoint(new Vector3(center.x+radius, center.y, center.z));
				six_points[2] = Camera.main.WorldToScreenPoint(new Vector3(center.x, center.y-radius, center.z));
				six_points[3] = Camera.main.WorldToScreenPoint(new Vector3(center.x, center.y+radius, center.z));
				six_points[4] = Camera.main.WorldToScreenPoint(new Vector3(center.x, center.y, center.z-radius));
				six_points[5] = Camera.main.WorldToScreenPoint(new Vector3(center.x, center.y, center.z+radius));
				foreach (Vector3 v in six_points) {
					if (v.x < min.x) min.x = v.x;
					if (v.y < min.y) min.y = v.y;
					if (v.x > max.x) max.x = v.x;
					if (v.y > max.y) max.y = v.y;
				}
			}
			float ratio_width = (max.x-min.x)/Camera.main.pixelWidth;
			float ratio_height = (max.y-min.y)/Camera.main.pixelHeight;
			float ratio = (ratio_width > ratio_height) ? ratio_width : ratio_height;
			if (ratio > 1.0f) ratio = 1.0f;
			
			return ratio;
		}
		// Update is called once per frame
		void Update() {
			if (progressiveMesh) {
				currentTimeToInterval -= Time.deltaTime;
				// time out
				if (currentTimeToInterval <= 0.0f) {
					// detect if the game object is visible
					bool visable = false;
					if (!culled) {
						foreach (Component child in allBasicRenderers) {
							if(((Renderer)child).isVisible) visable = true;
							break;
						}
					}
					// we only change levels when the game object had been culled by ourselves or is visable
					if (culled || visable) {
						// we only calculate ratio of screen when the main camera is active in hierarchy
						float ratio = (Camera.main != null && Camera.main.gameObject != null && Camera.main.gameObject.activeInHierarchy) ? ratio_of_screen() : 0.0f;
						// you may change cull condition here
						if (ratio < 0.1f) {
							// cull the game object
							if (!culled) {
								// cull me
								foreach (Component child in allBasicRenderers) {
									((Renderer)child).enabled = false;
								}
								culled = true;
							}
						} else {
							// show the game object
							if (culled) {
								// show me
								foreach (Component child in allBasicRenderers) {
									((Renderer)child).enabled = true;
								}
								culled = false;
							}
							// get lod count
							int max_lod_count = progressiveMesh.triangles[0];
							// set triangle list according to current lod
							int lod = (int)((1.0f-ratio)*max_lod_count);
							// clamp the value
							if (lod > max_lod_count-1) lod = max_lod_count-1;
							// adjust the value by min lod
							if (lod < minLod) lod = minLod;
							// adjust the value by max lod
							if (lod > maxLod) lod = maxLod;
							// lod changed
							if (current_lod != lod) {
								set_triangles(lod);
								current_lod = lod;
							}
						}
					}
					if (fpsHint) {
						int fps = Mathf.RoundToInt(1.0f / Time.smoothDeltaTime);
						fpsHint.text = "FPS: " + fps.ToString();
					}
					//reset timer
					currentTimeToInterval = UpdateInterval + (UnityEngine.Random.value + 0.5f) * currentTimeToInterval;
				}
			}
		}
		private int[] get_triangles_from_progressive_mesh(int lod0, int mesh_count0, int mat0) {
			int triangle_count = 0;
			// max lod count
			int max_lod_count = progressiveMesh.triangles[triangle_count];
			triangle_count++;
			for (int lod=0; lod<max_lod_count; lod++) {
				// max mesh count
				int max_mesh_count = progressiveMesh.triangles[triangle_count];
				triangle_count++;
				for (int mesh_count=0; mesh_count<max_mesh_count; mesh_count++) {
					// max sub mesh count
					int max_sub_mesh_count = progressiveMesh.triangles[triangle_count];
					triangle_count++;
					for (int mat=0; mat<max_sub_mesh_count; mat++) {
						// max triangle count
						int max_triangle_count = progressiveMesh.triangles[triangle_count];
						triangle_count++;
						// here it is
						if(lod == lod0 && mesh_count == mesh_count0 && mat == mat0) {
							int[] new_triangles = new int[max_triangle_count];
							Array.Copy(progressiveMesh.triangles, triangle_count, new_triangles, 0, max_triangle_count);
							return new_triangles;
						}
						// triangle list count
						triangle_count += max_triangle_count;
					}
				}
			}
			return null;
		}
		private void set_triangles(int lod) {
			if(working) {
				int mesh_count = 0;
				int total_triangles_count = 0;
				foreach (Mesh child in cloned_meshes) {
					for(int mat=0; mat<child.subMeshCount; mat++) {
						int [] triangle_list = get_triangles_from_progressive_mesh(lod, mesh_count, mat);
						child.SetTriangles(triangle_list, mat);
						total_triangles_count += triangle_list.Length;
					}
					// time consuming functions, we just comment them here, Unity engine seems automatically update the normals.
					// child.RecalculateNormals();
					// child.RecalculateBounds();
					mesh_count++;
				}
				// update read only status
				if (lodHint) lodHint.text = "Level Of Detail: " + lod.ToString();
				if (triangleHint) triangleHint.text = "Triangle Count: " + (total_triangles_count/3).ToString();
			}
		}
		private void get_all_meshes() {
			if (!working) {
				Component[] allFilters = (Component[])(gameObject.GetComponentsInChildren (typeof(MeshFilter)));
				Component[] allRenderers = (Component[])(gameObject.GetComponentsInChildren (typeof(SkinnedMeshRenderer)));
				int mesh_count = allFilters.Length + allRenderers.Length;
				if (mesh_count > 0) {
					cloned_meshes = new Mesh[mesh_count];
					shared_meshes = new Mesh[mesh_count];
					int counter = 0;
					foreach (Component child in allFilters) {
						// store original shared mesh
						shared_meshes[counter] = ((MeshFilter)child).sharedMesh;
						// clone the shared mesh
						((MeshFilter)child).sharedMesh = Instantiate(((MeshFilter)child).sharedMesh);
						// store cloned mesh
						cloned_meshes[counter] = ((MeshFilter)child).sharedMesh;
						counter++;
					}
					foreach (Component child in allRenderers) {
						// store original shared mesh
						shared_meshes[counter] = ((SkinnedMeshRenderer)child).sharedMesh;
						// clone the shared mesh
						((SkinnedMeshRenderer)child).sharedMesh = Instantiate(((SkinnedMeshRenderer)child).sharedMesh);
						// store cloned mesh
						cloned_meshes[counter] = ((SkinnedMeshRenderer)child).sharedMesh;
						counter++;
					}
				}

				// get all renderers
				allBasicRenderers = (Component[])(gameObject.GetComponentsInChildren (typeof(Renderer)));
				// We use random value to spread the update moment in range [0, UpdateInterval]
				currentTimeToInterval = UnityEngine.Random.value * UpdateInterval;

				// init current lod
				current_lod = -1;

				working = true;
			}
		}
		private void clean_all() {
			if (working) {
				Component[] allFilters = (Component[])(gameObject.GetComponentsInChildren (typeof(MeshFilter)));
				Component[] allRenderers = (Component[])(gameObject.GetComponentsInChildren (typeof(SkinnedMeshRenderer)));
				int mesh_count = allFilters.Length + allRenderers.Length;
				if (mesh_count > 0) {
					int counter = 0;
					foreach (Component child in allFilters) {
						// restore original shared mesh
						((MeshFilter)child).sharedMesh = shared_meshes[counter];
						counter++;
					}
					foreach (Component child in allRenderers) {
						// restore original shared mesh
						((SkinnedMeshRenderer)child).sharedMesh = shared_meshes[counter];
						counter++;
					}
				}
				shared_meshes = null;
				cloned_meshes = null;
				allBasicRenderers = null;

				working = false;
			}
		}
		void OnEnable() {
			Awake();
			Start();
		}
		void OnDisable() {
			clean_all();
		}
		void OnDestroy() {
			clean_all ();
		}
	}
}
