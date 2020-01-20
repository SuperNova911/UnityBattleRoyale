/*--------------------------------------------------------
  Mantis_Mesh.cs

  Created by MINGFEN WANG on 13-12-26.
  Copyright (c) 2013 MINGFEN WANG. All rights reserved.
  http://www.mesh-online.net/
--------------------------------------------------------*/
using UnityEngine;
using System.Collections;

namespace MantisLODEditor {
	public class Mantis_Mesh {
		public Mesh mesh;
		public int index;
		public string uuid;
		public int[][] origin_triangles;
		public int[] out_triangles;
		public int out_count;
		public Mantis_Mesh() {
			index = -1;
			uuid = null;
			out_count = 0;
		}
	}
}
