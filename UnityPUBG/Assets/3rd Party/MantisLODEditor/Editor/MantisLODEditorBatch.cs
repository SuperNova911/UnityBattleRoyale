/*--------------------------------------------------------
  MantisLODEditorBatch.cs

  Created by MINGFEN WANG on 15-12-26.
  Copyright (c) 2013 MINGFEN WANG. All rights reserved.
  http://www.mesh-online.net/
--------------------------------------------------------*/
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace MantisLODEditor {
	public class MantisLODEditorBatch: EditorWindow {
		// run when focused
		[DllImport("MantisLOD")]
		private static extern int create_progressive_mesh(Vector3[] vertex_array, int vertex_count, int[] triangle_array, int triangle_count, Vector3[] normal_array, int normal_count, Color[] color_array, int color_count, Vector2[] uv_array, int uv_count, int protect_boundary, int protect_detail, int protect_symmetry, int protect_normal, int protect_shape);
		[DllImport("MantisLOD")]
		private static extern int get_triangle_list(int index, float goal, int[] triangles, ref int triangle_count);
		[DllImport("MantisLOD")]
		private static extern int delete_progressive_mesh(int index);

		private Mantis_Mesh[] Mantis_Meshes = null;

		// define how to optimize the meshes
		private bool protect_boundary = true;
		private bool protect_detail = false;
		private bool protect_symmetry = false;
		private bool protect_normal = false;
		private bool protect_shape = true;

		private int state = 0;
		private float start_time = 0.0f;
		private List<string> file_name_list = null;
		private int file_name_index = 0;
		private string file_name_hint = null;
		private string message_hint = null;

		[MenuItem("Window/Mantis LOD Editor/Assets/Create/Mantis LOD Editor Batch")]
		static void Init () {
			// Get existing open window or if none, make a new one:
			MantisLODEditorBatch window = (MantisLODEditorBatch)EditorWindow.GetWindow (typeof (MantisLODEditorBatch));
			window.Show();
		}
		
		void OnGUI () {
			GUIStyle helpStyle = new GUIStyle(GUI.skin.box);

			// display the title
			GUI.enabled = true;
			GUILayout.Label(
				"Mantis LOD Editor Batch Edition V3.5"
				, helpStyle
				, GUILayout.ExpandWidth(true));

			// display controls
			GUI.enabled = (state == 0);
			protect_boundary = EditorGUILayout.Toggle ("Protect Boundary", protect_boundary);
			protect_detail = EditorGUILayout.Toggle ("More Details", protect_detail);
			protect_symmetry = EditorGUILayout.Toggle ("Protect Symmetry", protect_symmetry);
			protect_normal = EditorGUILayout.Toggle ("Protect Hard Edge", protect_normal);
			protect_shape = EditorGUILayout.Toggle ("Beautiful Triangles", protect_shape);

			// generate progressive meshes
			if(GUILayout.Button("Batch Generate", GUILayout.ExpandWidth(true), GUILayout.ExpandWidth(true))) {
				if (GetFileList()) {
					message_hint = null;
					// start the batch
					start_update(1);
				}
			}

			// we are still alive
			if(state != 0) {
				EditorGUILayout.LabelField(file_name_hint);
				EditorGUILayout.LabelField("Time Elapse", (Time.realtimeSinceStartup - start_time).ToString("#0.0"));
			}

			// display something
			if (message_hint != null) {
				EditorUtility.DisplayDialog("Message", message_hint, "OK");
				message_hint = null;
/*				GUILayout.Label(
					message_hint
					, helpStyle
					, GUILayout.ExpandWidth(true));*/
			}
		}
		private void start_update(int one_state) {
			state = one_state;
			start_time = Time.realtimeSinceStartup;
			EditorApplication.update += Update;
		}
		private void end_update() {
			state = 0;
			start_time = 0.0f;
			EditorApplication.update -= Update;
		}
		void Update() {
			// we cannot use nice yield method in Editor class, I have to write ugly code:(
			switch(state) {
			case 1: ShowModelName(); break;
			case 2: GenerateProgressiveMesh(); break;
			case 3: ShowResult(); break;
			}
			// show time elapse label in main thread
			Repaint();
		}
		private void ShowModelName() {
			file_name_hint = file_name_list[file_name_index];
			state = 2;
		}
		private void ShowResult() {
			message_hint = "All Done. Totally " + file_name_index.ToString() + " progressive meshes generated into 'Assets/Resources'.";
			end_update();
		}
		private void GenerateProgressiveMesh() {
			try {
				CreateAsset(Path.GetFileNameWithoutExtension(file_name_list[file_name_index]));
			} finally {
			}
			// forward to the next
			file_name_index++;
			// all done
			if (file_name_index == file_name_list.Count) {
				state = 3;
			} else {
				state = 1;
			}
		}
		private bool GetFileList() {
			if (!Directory.Exists("Assets/Resources")) {
				message_hint = "Please create 'Assets/Resources' directory, then put all your 3d models into the directory and try again.";
				return false;
			}
			// Reset file name list and its index
			file_name_list = new List<string>();
			file_name_index = 0;

			// Get file name list of supported 3d models
			List<string> temp_file_name_list = new List<string>(Directory.GetFiles("Assets/Resources"));
			foreach (string filename in temp_file_name_list) {
				string extensionname = Path.GetExtension(filename).ToLower();
				// You may add extensions here
				if (string.Compare(extensionname, ".fbx") == 0 ||
				    string.Compare(extensionname, ".dae") == 0 ||
				    string.Compare(extensionname, ".3ds") == 0 ||
				    string.Compare(extensionname, ".dxf") == 0 ||
				    string.Compare(extensionname, ".obj") == 0 ||
				    string.Compare(extensionname, ".mb") == 0 ||
				    string.Compare(extensionname, ".ma") == 0 ||
				    string.Compare(extensionname, ".c4d") == 0 ||
				    string.Compare(extensionname, ".max") == 0 ||
				    string.Compare(extensionname, ".jas") == 0 ||
				    string.Compare(extensionname, ".lxo") == 0 ||
				    string.Compare(extensionname, ".lws") == 0 ||
				    string.Compare(extensionname, ".blend") == 0) {
					// Add to the list
					file_name_list.Add(filename);
				}
			}
			// No 3d model in the directory
			if (file_name_list.Count == 0) {
				message_hint = "Please put all your 3d models into 'Assets/Resources' and try again.\n\nSupported file extensions:\n.fbx\n.dae\n.3ds\n.dxf\n.obj\n.mb\n.ma\n.c4d\n.max\n.jas\n.lxo\n.lws\n.blend\n\nYou may modify the script(MantisLODEditorPro/Editor/ProgressiveMeshBatch.cs) to support future extensions.";
				return false;
			}
			return true;
		}
		public void CreateAsset(string filename) {
			ProgressiveMesh pm = (ProgressiveMesh)ScriptableObject.CreateInstance(typeof(ProgressiveMesh));

			init_all(filename);
			optimize();
			fill_progressive_mesh(pm);
			clean_all();

			string filePath = "Assets/Resources/" + filename + "_Progressive_Mesh.asset";
			AssetDatabase.CreateAsset(pm, filePath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
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
		private void optimize() {
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
		private void init_all(string filename) {
			get_all_meshes(filename);
			if (Mantis_Meshes != null) {
				foreach (Mantis_Mesh child in Mantis_Meshes) {
					int triangle_number = child.mesh.triangles.Length;
					// out data is large than origin data
					child.out_triangles = new int[triangle_number+child.mesh.subMeshCount];
					child.index = -1;
				}
			}
		}
		private void get_all_meshes(string filename) {
			Mesh[] meshes = Resources.LoadAll<Mesh>(filename);
			int mesh_count = meshes.Length;
			if (mesh_count > 0) {
				Mantis_Meshes = new Mantis_Mesh[mesh_count];
				int counter = 0;
				foreach (Mesh mesh in meshes) {
					Mantis_Meshes[counter] = new Mantis_Mesh();
					Mantis_Meshes[counter].mesh = mesh;
					counter++;
				}
			}
		}
		private void clean_all() {
			if (Mantis_Meshes != null) {
				foreach (Mantis_Mesh child in Mantis_Meshes) {
					if(child.index != -1) {
						Resources.UnloadAsset(child.mesh);
						// do not need it
						delete_progressive_mesh (child.index);
						child.index = -1;
					}
				}
				Mantis_Meshes = null;
			}
		}
	}
}
