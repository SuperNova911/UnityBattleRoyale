/*--------------------------------------------------------
  InnerMantisLODEditorProfessional.cs

  Created by MINGFEN WANG on 13-12-26.
  Copyright (c) 2013 MINGFEN WANG. All rights reserved.
  http://www.mesh-online.net/
--------------------------------------------------------*/
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace MantisLODEditor {
	[CustomEditor(typeof(MantisLODEditorProfessional))]
	class InnerMantisLODEditorProfessional: Editor {
		// run when focused
		[DllImport("MantisLOD")]
		private static extern int create_progressive_mesh(Vector3[] vertex_array, int vertex_count, int[] triangle_array, int triangle_count, Vector3[] normal_array, int normal_count, Color[] color_array, int color_count, Vector2[] uv_array, int uv_count, int protect_boundary, int protect_detail, int protect_symmetry, int protect_normal, int protect_shape);
		[DllImport("MantisLOD")]
		private static extern int get_triangle_list(int index, float goal, int[] triangles, ref int triangle_count);
		[DllImport("MantisLOD")]
		private static extern int delete_progressive_mesh(int index);
		
		private int origin_face_number = 0;
		private int face_number = 0;
		private float quality = 100.0f;
		private float save_quality = 100.0f;
		private bool protect_boundary = true;
		private bool save_protect_boundary = true;
		private bool protect_detail = false;
		private bool save_protect_detail = false;
		private bool protect_symmetry = false;
		private bool save_protect_symmetry = false;
		private bool protect_normal = false;
		private bool save_protect_normal = false;
		private bool protect_shape = true;
		private bool save_protect_shape = true;
		private Mantis_Mesh[] Mantis_Meshes = null;
		private bool show_title = true;
		private bool show_help = true;
		private bool save_show_help = true;

		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			if(target) {
				if(Mantis_Meshes != null) {
					show_title = EditorGUILayout.Foldout(show_title, "Mantis LOD Editor - Professional Edition V3.5");
					if(show_title) {
						// A decent style.  Light grey text inside a border.
						GUIStyle helpStyle = new GUIStyle(GUI.skin.box);
						helpStyle.wordWrap = true;
						helpStyle.alignment = TextAnchor.UpperLeft;
						show_help = EditorGUILayout.Foldout(show_help, show_help?"Hide Help":"Show Help");
						// save all triangle lists as progressive mesh
						if(GUILayout.Button("Save Progressive Mesh", GUILayout.ExpandWidth(true), GUILayout.ExpandWidth(true))) {
							ProgressiveMesh pm = (ProgressiveMesh)ScriptableObject.CreateInstance(typeof(ProgressiveMesh));
							fill_progressive_mesh(pm);
							string filePath = EditorUtility.SaveFilePanelInProject (
								"Save Progressive Mesh",
								((Component)target).gameObject.name + "_Progressive_Mesh.asset",
								"asset",
								"Choose a file location"
								);   
							if(filePath!="") {
								AssetDatabase.CreateAsset(pm, filePath);
								AssetDatabase.SaveAssets();
								AssetDatabase.Refresh();
							}
						}
						if(show_help) {
							GUILayout.Label(
								"When Clicked, save the progressive meshes as a single asset file for runtime use."
								, helpStyle
								, GUILayout.ExpandWidth(true));
						}
						// save current mesh as a lod asset file
						if(GUILayout.Button("Save Current Mesh", GUILayout.ExpandWidth(true), GUILayout.ExpandWidth(true))) {
							foreach (Mantis_Mesh child in Mantis_Meshes) {
								// clone the mesh
								Mesh new_mesh = (Mesh)Instantiate(child.mesh);
								// remove unused vertices
								if(new_mesh.blendShapeCount == 0) {
									shrink_mesh(new_mesh);
								}
								string filePath = EditorUtility.SaveFilePanelInProject (
									"Save Current Mesh",
									new_mesh.name + "_Quality_" + quality.ToString() + ".asset",
									"asset",
									"Choose a file location"
									);   
								if(filePath!="") {
									AssetDatabase.CreateAsset(new_mesh, filePath);
									AssetDatabase.SaveAssets();
									AssetDatabase.Refresh();
								}
							}
						}
						if(show_help) {
							GUILayout.Label(
								"When Clicked, save the meshes of current quality as LOD asset files."
								, helpStyle
								, GUILayout.ExpandWidth(true));
						}
						protect_boundary = EditorGUILayout.Toggle ("Protect Boundary", protect_boundary);
						if(show_help) {
							GUILayout.Label(
								"When checked, all open boundaries will be protected; Otherwise, some smooth parts of open boundaries will be smartly merged. Both way, uv boundaries and material boundaries will be protected."
								, helpStyle
								, GUILayout.ExpandWidth(true));
						}
						protect_detail = EditorGUILayout.Toggle ("More Details", protect_detail);
						if(show_help) {
							GUILayout.Label(
								"When checked, more details will be preserved, toggle it only if when making the highest LOD, otherwise, please leave it unchecked to get best results."
								, helpStyle
								, GUILayout.ExpandWidth(true));
						}
						protect_symmetry = EditorGUILayout.Toggle ("Protect Symmetry", protect_symmetry);
						if(show_help) {
							GUILayout.Label(
								"When checked, all symmetric uv mapping will be preserved, you should check it only if you are making the higher LODs; Otherwise, please leave it unchecked to get best results."
								, helpStyle
								, GUILayout.ExpandWidth(true));
						}
						protect_normal = EditorGUILayout.Toggle ("Protect Hard Edge", protect_normal);
						if(show_help) {
							GUILayout.Label(
								"When checked, all hard edges will be preserved; If you want to get maximum decimation, please leave it unchecked."
								, helpStyle
								, GUILayout.ExpandWidth(true));
						}
						protect_shape = EditorGUILayout.Toggle ("Beautiful Triangles", protect_shape);
						if(show_help) {
							GUILayout.Label(
								"When checked, it generates beautiful triangles, but the result may not be better than the tradition method, if this happens, please leave it unchecked."
								, helpStyle
								, GUILayout.ExpandWidth(true));
						}
						EditorGUILayout.LabelField("Triangles", face_number.ToString() + "/" + origin_face_number.ToString());
						if(show_help) {
							GUILayout.Label(
								"Display current triangle number and total triangle number."
								, helpStyle
								, GUILayout.ExpandWidth(true));
						}
						quality = EditorGUILayout.Slider ("Quality", quality, 0.0f, 100.0f);
						if(show_help) {
							GUILayout.Label(
								"Drag to change the quality, the mesh will change in real time."
								, helpStyle
								, GUILayout.ExpandWidth(true));
						}
						if (GUI.changed) {
							if(save_show_help != show_help) {
								string filename = "mantis_not_show_help";
								if(show_help) {
									if(System.IO.File.Exists(filename)) System.IO.File.Delete(filename);
								} else {
									if(!System.IO.File.Exists(filename)) System.IO.File.Create(filename);
								}
								save_show_help = show_help;
							}
							if(save_protect_boundary != protect_boundary) {
								quality = 100.0f;
								// delete old progressive mesh
								clean_all();
								init_all();
								// create new progressive mesh
								optimize();
								save_protect_boundary = protect_boundary;
							}
							if(save_protect_detail != protect_detail) {
								quality = 100.0f;
								// delete old progressive mesh
								clean_all();
								init_all();
								// create new progressive mesh
								optimize();
								save_protect_detail = protect_detail;
							}
							if(save_protect_symmetry != protect_symmetry) {
								quality = 100.0f;
								// delete old progressive mesh
								clean_all();
								init_all();
								// create new progressive mesh
								optimize();
								save_protect_symmetry = protect_symmetry;
							}
							if(save_protect_normal != protect_normal) {
								quality = 100.0f;
								// delete old progressive mesh
								clean_all();
								init_all();
								// create new progressive mesh
								optimize();
								save_protect_normal = protect_normal;
							}
							if(save_protect_shape != protect_shape) {
								quality = 100.0f;
								// delete old progressive mesh
								clean_all();
								init_all();
								// create new progressive mesh
								optimize();
								save_protect_shape = protect_shape;
							}
							if(save_quality != quality) {
								face_number = 0;
								foreach (Mantis_Mesh child in Mantis_Meshes) {
									// get triangle list by quality value
									if(child.index != -1 && get_triangle_list(child.index, quality, child.out_triangles, ref child.out_count) == 1) {
										if(child.out_count > 0) {
											int counter = 0;
											int mat = 0;
											while(counter < child.out_count) {
												int len = child.out_triangles[counter];
												counter++;
												if(len > 0) {
													int[] new_triangles = new int[len];
													Array.Copy(child.out_triangles, counter, new_triangles, 0, len);
													child.mesh.SetTriangles(new_triangles, mat);
													counter += len;
												} else {
													child.mesh.SetTriangles((int[])null, mat);
												}
												mat++;
											}
											face_number += child.mesh.triangles.Length/3;
											// refresh normals and bounds
											child.mesh.RecalculateNormals();
											child.mesh.RecalculateBounds();
											EditorUtility.SetDirty (target);
										}
									}
								}
								save_quality = quality;
							}
						}
					}
				}
			}
		}
		private void fill_progressive_mesh(ProgressiveMesh pm) {
			int triangle_count = 0;
			int[][][][] temp_triangles;
			temp_triangles = new int[ProgressiveMesh.max_lod_count][][][];
			// max lod count
			triangle_count++;
			for (int lod=0; lod<temp_triangles.Length; lod++) {
				float quality = 100.0f * (temp_triangles.Length - lod) / temp_triangles.Length;
				temp_triangles[lod] = new int[Mantis_Meshes.Length][][];
				// mesh count
				triangle_count++;
				int mesh_count = 0;
				foreach (Mantis_Mesh child in Mantis_Meshes) {
					// get triangle list by quality value
					if(child.index != -1 && get_triangle_list(child.index, quality, child.out_triangles, ref child.out_count) == 1) {
						if(child.out_count > 0) {
							int counter = 0;
							int mat = 0;
							temp_triangles[lod][mesh_count] = new int[child.mesh.subMeshCount][];
							// sub mesh count
							triangle_count++;
							while(counter < child.out_count) {
								int len = child.out_triangles[counter];
								// triangle count
								triangle_count++;
								// triangle list count
								triangle_count += len;
								counter++;
								int[] new_triangles = new int[len];
								Array.Copy(child.out_triangles, counter, new_triangles, 0, len);
								temp_triangles[lod][mesh_count][mat] = new_triangles;
								counter += len;
								mat++;
							}
						} else {
							temp_triangles[lod][mesh_count] = new int[child.mesh.subMeshCount][];
							// sub mesh count
							triangle_count++;
							for (int mat=0; mat<temp_triangles[lod][mesh_count].Length; mat++) {
								temp_triangles[lod][mesh_count][mat] = new int[0];
								// triangle count
								triangle_count++;
							}
						}
					}
					mesh_count++;
				}
			}
			// create fix size array
			pm.triangles = new int[triangle_count];

			// reset the counter
			triangle_count = 0;
			// max lod count
			pm.triangles[triangle_count] = temp_triangles.Length;
			triangle_count++;
			for (int lod=0; lod<temp_triangles.Length; lod++) {
				// mesh count
				pm.triangles[triangle_count] = temp_triangles[lod].Length;
				triangle_count++;
				for (int mesh_count=0; mesh_count<temp_triangles[lod].Length; mesh_count++) {
					// sub mesh count
					pm.triangles[triangle_count] = temp_triangles[lod][mesh_count].Length;
					triangle_count++;
					for (int mat=0; mat<temp_triangles[lod][mesh_count].Length; mat++) {
						// triangle count
						pm.triangles[triangle_count] = temp_triangles[lod][mesh_count][mat].Length;
						triangle_count++;
						Array.Copy(temp_triangles[lod][mesh_count][mat], 0, pm.triangles, triangle_count, temp_triangles[lod][mesh_count][mat].Length);
						// triangle list count
						triangle_count += temp_triangles[lod][mesh_count][mat].Length;
					}
				}
			}
		}
		private void shrink_mesh(Mesh mesh) {
			// get all origin data
			Vector3[] origin_vertices = mesh.vertices;
			Vector3[] vertices = null;
			if(origin_vertices != null && origin_vertices.Length > 0) vertices = new Vector3[origin_vertices.Length];
			BoneWeight[] origin_boneWeights = mesh.boneWeights;
			BoneWeight[] boneWeights = null;
			if(origin_boneWeights != null && origin_boneWeights.Length > 0) boneWeights = new BoneWeight[origin_boneWeights.Length];
			Color[] origin_colors = mesh.colors;
			Color[] colors = null;
			if(origin_colors != null && origin_colors.Length > 0) colors = new Color[origin_colors.Length];
			Color32[] origin_colors32 = mesh.colors32;
			Color32[] colors32 = null;
			if(origin_colors32 != null && origin_colors32.Length > 0) colors32 = new Color32[origin_colors32.Length];
			Vector4[] origin_tangents = mesh.tangents;
			Vector4[] tangents = null;
			if(origin_tangents != null && origin_tangents.Length > 0) tangents = new Vector4[origin_tangents.Length];
			Vector2[] origin_uv = mesh.uv;
			Vector2[] uv = null;
			if(origin_uv != null && origin_uv.Length > 0) uv = new Vector2[origin_uv.Length];
			Vector2[] origin_uv2 = mesh.uv2;
			Vector2[] uv2 = null;
			if(origin_uv2 != null && origin_uv2.Length > 0) uv2 = new Vector2[origin_uv2.Length];
			int[][] origin_triangles = new int[mesh.subMeshCount][];
			for (int i=0; i<mesh.subMeshCount; i++) {
				origin_triangles[i] = mesh.GetTriangles(i);
			}
			// 	make permutation
			Hashtable imap = new Hashtable ();
			int vertex_count = 0;
			for (int i=0; i<mesh.subMeshCount; i++) {
				int[] triangles = mesh.GetTriangles(i);
				for(int j=0; j<triangles.Length; j += 3) {
					if(!imap.Contains(triangles[j])) {
						if(vertices != null) vertices[vertex_count] = origin_vertices[triangles[j]];
						if(boneWeights != null) boneWeights[vertex_count] = origin_boneWeights[triangles[j]];
						if(colors != null) colors[vertex_count] = origin_colors[triangles[j]];
						if(colors32 != null) colors32[vertex_count] = origin_colors32[triangles[j]];
						if(tangents != null) tangents[vertex_count] = origin_tangents[triangles[j]];
						if(uv != null) uv[vertex_count] = origin_uv[triangles[j]];
						if(uv2 != null) uv2[vertex_count] = origin_uv2[triangles[j]];
						imap.Add(triangles[j], vertex_count);
						vertex_count++;
					}
					if(!imap.Contains(triangles[j+1])) {
						if(vertices != null) vertices[vertex_count] = origin_vertices[triangles[j+1]];
						if(boneWeights != null) boneWeights[vertex_count] = origin_boneWeights[triangles[j+1]];
						if(colors != null) colors[vertex_count] = origin_colors[triangles[j+1]];
						if(colors32 != null) colors32[vertex_count] = origin_colors32[triangles[j+1]];
						if(tangents != null) tangents[vertex_count] = origin_tangents[triangles[j+1]];
						if(uv != null) uv[vertex_count] = origin_uv[triangles[j+1]];
						if(uv2 != null) uv2[vertex_count] = origin_uv2[triangles[j+1]];
						imap.Add(triangles[j+1], vertex_count);
						vertex_count++;
					}
					if(!imap.Contains(triangles[j+2])) {
						if(vertices != null) vertices[vertex_count] = origin_vertices[triangles[j+2]];
						if(boneWeights != null) boneWeights[vertex_count] = origin_boneWeights[triangles[j+2]];
						if(colors != null) colors[vertex_count] = origin_colors[triangles[j+2]];
						if(colors32 != null) colors32[vertex_count] = origin_colors32[triangles[j+2]];
						if(tangents != null) tangents[vertex_count] = origin_tangents[triangles[j+2]];
						if(uv != null) uv[vertex_count] = origin_uv[triangles[j+2]];
						if(uv2 != null) uv2[vertex_count] = origin_uv2[triangles[j+2]];
						imap.Add(triangles[j+2], vertex_count);
						vertex_count++;
					}
				}
			}
			// set data back to mesh
			mesh.Clear (false);
			if (vertices != null) {
				Vector3[] new_vertices = new Vector3[vertex_count];
				Array.Copy(vertices, new_vertices, vertex_count);
				mesh.vertices = new_vertices;
			}
			if (boneWeights != null) {
				BoneWeight[] new_boneWeights = new BoneWeight[vertex_count];
				Array.Copy(boneWeights, new_boneWeights, vertex_count);
				mesh.boneWeights = new_boneWeights;
			}
			if (colors != null) {
				Color[] new_colors = new Color[vertex_count];
				Array.Copy(colors, new_colors, vertex_count);
				mesh.colors = new_colors;
			}
			if (colors32 != null) {
				Color32[] new_colors32 = new Color32[vertex_count];
				Array.Copy(colors32, new_colors32, vertex_count);
				mesh.colors32 = new_colors32;
			}
			if (tangents != null) {
				Vector4[] new_tangents = new Vector4[vertex_count];
				Array.Copy(tangents, new_tangents, vertex_count);
				mesh.tangents = new_tangents;
			}
			if (uv != null) {
				Vector2[] new_uv = new Vector2[vertex_count];
				Array.Copy(uv, new_uv, vertex_count);
				mesh.uv = new_uv;
			}
			if (uv2 != null) {
				Vector2[] new_uv2 = new Vector2[vertex_count];
				Array.Copy(uv2, new_uv2, vertex_count);
				mesh.uv2 = new_uv2;
			}
			mesh.subMeshCount = origin_triangles.Length;
			for (int i=0; i<mesh.subMeshCount; i++) {
				int[] new_triangles = new int[origin_triangles[i].Length];
				for(int j=0; j<new_triangles.Length; j += 3) {
					new_triangles[j] = (int)imap[origin_triangles[i][j]];
					new_triangles[j+1] = (int)imap[origin_triangles[i][j+1]];
					new_triangles[j+2] = (int)imap[origin_triangles[i][j+2]];
				}
				mesh.SetTriangles(new_triangles, i);
			}
			// refresh normals and bounds
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}
		private void get_all_meshes() {
			Component[] allFilters = (Component[])((Component)target).gameObject.GetComponentsInChildren (typeof(MeshFilter));
			Component[] allRenderers = (Component[])((Component)target).gameObject.GetComponentsInChildren (typeof(SkinnedMeshRenderer));
			int mesh_count = allFilters.Length + allRenderers.Length;
			if (mesh_count > 0) {
				Mantis_Meshes = new Mantis_Mesh[mesh_count];
				int counter = 0;
				foreach (Component child in allFilters) {
					Mantis_Meshes[counter] = new Mantis_Mesh();
					Mantis_Meshes[counter].mesh = ((MeshFilter)child).sharedMesh;
					counter++;
				}
				foreach (Component child in allRenderers) {
					Mantis_Meshes[counter] = new Mantis_Mesh();
					Mantis_Meshes[counter].mesh = ((SkinnedMeshRenderer)child).sharedMesh;
					counter++;
				}
			}
		}
		void Awake() {
			init_all();
			optimize();
		}
		private void init_all() {
			if (Mantis_Meshes == null) {
				if(target) {
					get_all_meshes();
					if (Mantis_Meshes != null) {
						face_number = 0;
						foreach (Mantis_Mesh child in Mantis_Meshes) {
							int triangle_number = child.mesh.triangles.Length;
							child.origin_triangles = new int[child.mesh.subMeshCount][];
							// out data is large than origin data
							child.out_triangles = new int[triangle_number+child.mesh.subMeshCount];
							for(int i=0; i<child.mesh.subMeshCount; i++) {
								int[] sub_triangles = child.mesh.GetTriangles(i);
								face_number += sub_triangles.Length/3;
								// save origin triangle list
								child.origin_triangles[i] = new int[sub_triangles.Length];
								Array.Copy(sub_triangles, child.origin_triangles[i], sub_triangles.Length);
							}
							child.index = -1;
						}
						origin_face_number = face_number;
						string filename2 = "mantis_not_show_help";
						if(System.IO.File.Exists(filename2)) {
							show_help = false;
							save_show_help = false;
						} else {
							show_help = true;
							save_show_help = true;
						}
					}
				}
			}
		}
		private void optimize() {
			if(target) {
				if (Mantis_Meshes != null) {
					foreach (Mantis_Mesh child in Mantis_Meshes) {
						int triangle_number = child.mesh.triangles.Length;
						Vector3[] vertices = child.mesh.vertices;
						// in data is large than origin data
						int[] triangles = new int[triangle_number+child.mesh.subMeshCount];
						// we need normal data to protect normal boundary
						Vector3[] normals = child.mesh.normals;
						// we need color data to protect color boundary
						Color[] colors = child.mesh.colors;
						// we need uv data to protect uv boundary
						Vector2[] uvs = child.mesh.uv;
						int counter = 0;
						for(int i=0; i<child.mesh.subMeshCount; i++) {
							int[] sub_triangles = child.mesh.GetTriangles(i);
							triangles[counter] = sub_triangles.Length;
							counter++;
							Array.Copy(sub_triangles, 0, triangles, counter, sub_triangles.Length);
							counter += sub_triangles.Length;
						}
						// create progressive mesh
						child.index = create_progressive_mesh(vertices, vertices.Length, triangles, counter, normals, normals.Length, colors, colors.Length, uvs, uvs.Length, protect_boundary?1:0, protect_detail?1:0, protect_symmetry?1:0, protect_normal?1:0, protect_shape?1:0);
					}
				}
			}
		}
		private void clean_all() {
			// restore triangle list
			if (Mantis_Meshes != null) {
				if(target) {
					foreach (Mantis_Mesh child in Mantis_Meshes) {
						if(child.index != -1) {
							for(int i=0; i<child.mesh.subMeshCount; i++) {
								child.mesh.SetTriangles(child.origin_triangles[i], i);
							}
							child.mesh.RecalculateNormals();
							child.mesh.RecalculateBounds();
							// do not need it
							delete_progressive_mesh (child.index);
							child.index = -1;
						}
					}
				}
				Mantis_Meshes = null;
			}
		}
		public void OnDisable() {
			clean_all();
		}
		public void OnDestroy() {
			clean_all ();
		}
	}
}
