/*--------------------------------------------------------
  ProgressiveMesh.cs

  Created by MINGFEN WANG on 13-12-26.
  Copyright (c) 2013 MINGFEN WANG. All rights reserved.
  http://www.mesh-online.net/
--------------------------------------------------------*/
using UnityEngine;
using System.Collections;

namespace MantisLODEditor {
	[System.Serializable]
	public class ProgressiveMesh : ScriptableObject {
		public static int max_lod_count = 20;
		public int[] triangles;
	}
}
