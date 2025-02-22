/******************************************************************************
 * Spine Runtimes Software License v2.5
 * 
 * Copyright (c) 2013-2016, Esoteric Software
 * All rights reserved.
 * 
 * You are granted a perpetual, non-exclusive, non-sublicensable, and
 * non-transferable license to use, install, execute, and perform the Spine
 * Runtimes software and derivative works solely for personal or internal
 * use. Without the written permission of Esoteric Software (see Section 2 of
 * the Spine Software License Agreement), you may not (a) modify, translate,
 * adapt, or develop new applications using the Spine Runtimes or otherwise
 * create derivative works or improvements of the Spine Runtimes or (b) remove,
 * delete, alter, or obscure any trademarks or any copyright, trademark, patent,
 * or other intellectual property or proprietary rights notices on or in the
 * Software, including any copy thereof. Redistributions in binary or source
 * form must include this license and terms.
 * 
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES, BUSINESS INTERRUPTION, OR LOSS OF
 * USE, DATA, OR PROFITS) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
 * IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

//#define BAKE_ALL_BUTTON
//#define REGION_BAKING_MESH

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UnityEditor;
using UnityEngine;
using Spine;

namespace Spine.Unity.Editor {
	
	[CustomEditor(typeof(AtlasAsset))]
	public class AtlasAssetInspector : UnityEditor.Editor {
		private SerializedProperty atlasFile, materials;
		private AtlasAsset atlasAsset;

		static List<AtlasRegion> GetRegions (Atlas atlas) {
			FieldInfo regionsField = typeof(Atlas).GetField("regions", BindingFlags.Instance | BindingFlags.NonPublic);
			return (List<AtlasRegion>)regionsField.GetValue(atlas);
		}

		void OnEnable () {
			SpineEditorUtilities.ConfirmInitialization();
			atlasFile = serializedObject.FindProperty("atlasFile");
			materials = serializedObject.FindProperty("materials");
			materials.isExpanded = true;
			atlasAsset = (AtlasAsset)target;
			#if REGION_BAKING_MESH
			UpdateBakedList();
			#endif
		}

		#if REGION_BAKING_MESH
		private List<bool> baked;
		private List<GameObject> bakedObjects;

		void UpdateBakedList () {
			AtlasAsset asset = (AtlasAsset)target;
			baked = new List<bool>();
			bakedObjects = new List<GameObject>();
			if (atlasFile.objectReferenceValue != null) {
				List<AtlasRegion> regions = this.Regions;
				string atlasAssetPath = AssetDatabase.GetAssetPath(atlasAsset);
				string atlasAssetDirPath = Path.GetDirectoryName(atlasAssetPath);
				string bakedDirPath = Path.Combine(atlasAssetDirPath, atlasAsset.name);
				for (int i = 0; i < regions.Count; i++) {
					AtlasRegion region = regions[i];
					string bakedPrefabPath = Path.Combine(bakedDirPath, SpineEditorUtilities.GetPathSafeRegionName(region) + ".prefab").Replace("\\", "/");
					GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(bakedPrefabPath, typeof(GameObject));
					baked.Add(prefab != null);
					bakedObjects.Add(prefab);
				}
			}
		}
		#endif

		static public void UpdateSpriteSlices (Texture texture, Atlas atlas) {
			string texturePath = AssetDatabase.GetAssetPath(texture.GetInstanceID());
			var t = (TextureImporter)TextureImporter.GetAtPath(texturePath);
			t.spriteImportMode = SpriteImportMode.Multiple;
			var spriteSheet = t.spritesheet;
			var sprites = new List<SpriteMetaData>(spriteSheet);

			var regions = AtlasAssetInspector.GetRegions(atlas);
			int textureHeight = texture.height;
			char[] FilenameDelimiter = {'.'};
			int updatedCount = 0;
			int addedCount = 0;

			foreach (var r in regions) {
				int width, height;
				if (r.rotate) {
					width = r.height;
					height = r.width;
				} else {
					width = r.width;
					height = r.height;
				}

				int x = r.x;
				int y = textureHeight - height - r.y;

				string pageName = r.page.name.Split(FilenameDelimiter, StringSplitOptions.RemoveEmptyEntries)[0];
				string textureName = texture.name;
				bool pageMatch = string.Equals(pageName, textureName,StringComparison.Ordinal);
				int spriteIndex = pageMatch ? sprites.FindIndex(
					(s) => string.Equals(s.name, r.name, StringComparison.Ordinal)
				) : -1;
				bool matchFound = spriteIndex >= 0;

				if (matchFound) {
					var s = sprites[spriteIndex];
					s.rect = new Rect(x, y, width, height);
					sprites[spriteIndex] = s;
					updatedCount++;
				} else {
					if (pageMatch) {
						sprites.Add(new SpriteMetaData {
							name = r.name,
							pivot = new Vector2(0.5f, 0.5f),
							rect = new Rect(x, y, width, height)
						});
						addedCount++;
					}
				}
			}

			t.spritesheet = sprites.ToArray();
			EditorUtility.SetDirty(t);
			AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
			EditorGUIUtility.PingObject(texture);
			Debug.Log(string.Format("Applied sprite slices to {2}. {0} added. {1} updated.", addedCount, updatedCount, texture.name));
		}

		override public void OnInspectorGUI () {
			serializedObject.Update();
			atlasAsset = atlasAsset ?? (AtlasAsset)target;
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(atlasFile);

			EditorGUILayout.PropertyField(materials, true);
			if (EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();

			if (materials.arraySize == 0) {
				EditorGUILayout.LabelField(new GUIContent("Error:  Missing materials", SpineEditorUtilities.Icons.warning));
				return;
			}

			for (int i = 0; i < materials.arraySize; i++) {
				SerializedProperty prop = materials.GetArrayElementAtIndex(i);
				Material mat = (Material)prop.objectReferenceValue;
				if (mat == null) {
					EditorGUILayout.LabelField(new GUIContent("Error:  Materials cannot be null", SpineEditorUtilities.Icons.warning));
					return;
				}
			}

			EditorGUILayout.Space();
			if (atlasFile.objectReferenceValue != null) {
				using (new EditorGUILayout.HorizontalScope()) {
					EditorGUILayout.Space();
					if (GUILayout.Button(
						new GUIContent(
							"Apply Regions as Texture Sprite Slices",
							SpineEditorUtilities.Icons.unityIcon,
							"Adds Sprite slices to atlas texture(s). " +
							"Updates existing slices if ones with matching names exist. \n\n" +
							"If your atlas was exported with Premultiply Alpha, " +
							"your SpriteRenderer should use the generated Spine _Material asset (or any Material with a PMA shader) instead of Sprites-Default.")
						, GUILayout.Height(30f))) {
						var atlas = atlasAsset.GetAtlas();
						foreach (var m in atlasAsset.materials)
							UpdateSpriteSlices(m.mainTexture, atlas);
					}
					EditorGUILayout.Space();
				}
			}
			EditorGUILayout.Space();

			#if REGION_BAKING_MESH
			if (atlasFile.objectReferenceValue != null) {
				Atlas atlas = asset.GetAtlas();
				FieldInfo field = typeof(Atlas).GetField("regions", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.NonPublic);
				List<AtlasRegion> regions = (List<AtlasRegion>)field.GetValue(atlas);
				EditorGUILayout.LabelField(new GUIContent("Region Baking", SpineEditorUtilities.Icons.unityIcon));
				EditorGUI.indentLevel++;
				AtlasPage lastPage = null;
				for (int i = 0; i < regions.Count; i++) {
					if (lastPage != regions[i].page) {
						if (lastPage != null) {
							EditorGUILayout.Separator();
							EditorGUILayout.Separator();
						}
						lastPage = regions[i].page;
						Material mat = ((Material)lastPage.rendererObject);
						if (mat != null) {
							GUILayout.BeginHorizontal();
							{
								EditorGUI.BeginDisabledGroup(true);
								EditorGUILayout.ObjectField(mat, typeof(Material), false, GUILayout.Width(250));
								EditorGUI.EndDisabledGroup();
							}
							GUILayout.EndHorizontal();

						} else {
							EditorGUILayout.LabelField(new GUIContent("Page missing material!", SpineEditorUtilities.Icons.warning));
						}
					}
					GUILayout.BeginHorizontal();
					{
						//EditorGUILayout.ToggleLeft(baked[i] ? "" : regions[i].name, baked[i]);
						bool result = baked[i] ? EditorGUILayout.ToggleLeft("", baked[i], GUILayout.Width(24)) : EditorGUILayout.ToggleLeft("    " + regions[i].name, baked[i]);
						if(baked[i]){
							EditorGUILayout.ObjectField(bakedObjects[i], typeof(GameObject), false, GUILayout.Width(250));
						}
						if (result && !baked[i]) {
							//bake
							baked[i] = true;
							bakedObjects[i] = SpineEditorUtilities.BakeRegion(atlasAsset, regions[i]);
							EditorGUIUtility.PingObject(bakedObjects[i]);
						} else if (!result && baked[i]) {
							//unbake
							bool unbakeResult = EditorUtility.DisplayDialog("Delete Baked Region", "Do you want to delete the prefab for " + regions[i].name, "Yes", "Cancel");
							switch (unbakeResult) {
							case true:
								//delete
								string atlasAssetPath = AssetDatabase.GetAssetPath(atlasAsset);
								string atlasAssetDirPath = Path.GetDirectoryName(atlasAssetPath);
								string bakedDirPath = Path.Combine(atlasAssetDirPath, atlasAsset.name);
								string bakedPrefabPath = Path.Combine(bakedDirPath, SpineEditorUtilities.GetPathSafeRegionName(regions[i]) + ".prefab").Replace("\\", "/");
								AssetDatabase.DeleteAsset(bakedPrefabPath);
								baked[i] = false;
								break;
							case false:
								//do nothing
								break;
							}
						}
					}
					GUILayout.EndHorizontal();
				}
				EditorGUI.indentLevel--;

				#if BAKE_ALL_BUTTON
				// Check state
				bool allBaked = true;
				bool allUnbaked = true;
				for (int i = 0; i < regions.Count; i++) {
					allBaked &= baked[i];
					allUnbaked &= !baked[i];
				}

				if (!allBaked && GUILayout.Button("Bake All")) {
					for (int i = 0; i < regions.Count; i++) {
						if (!baked[i]) {
							baked[i] = true;
							bakedObjects[i] = SpineEditorUtilities.BakeRegion(atlasAsset, regions[i]);
						}
					}

				} else if (!allUnbaked && GUILayout.Button("Unbake All")) {
					bool unbakeResult = EditorUtility.DisplayDialog("Delete All Baked Regions", "Are you sure you want to unbake all region prefabs? This cannot be undone.", "Yes", "Cancel");
					switch (unbakeResult) {
					case true:
						//delete
						for (int i = 0; i < regions.Count; i++) {
							if (baked[i]) {
								string atlasAssetPath = AssetDatabase.GetAssetPath(atlasAsset);
								string atlasAssetDirPath = Path.GetDirectoryName(atlasAssetPath);
								string bakedDirPath = Path.Combine(atlasAssetDirPath, atlasAsset.name);
								string bakedPrefabPath = Path.Combine(bakedDirPath, SpineEditorUtilities.GetPathSafeRegionName(regions[i]) + ".prefab").Replace("\\", "/");
								AssetDatabase.DeleteAsset(bakedPrefabPath);
								baked[i] = false;
							}
						}
						break;
					case false:
						//do nothing
						break;
					}

				}
				#endif
				
			}
			#else
			if (atlasFile.objectReferenceValue != null) {
				EditorGUILayout.LabelField("Atlas Regions", EditorStyles.boldLabel);

				var regions = AtlasAssetInspector.GetRegions(atlasAsset.GetAtlas());
				AtlasPage lastPage = null;
				for (int i = 0; i < regions.Count; i++) {
					if (lastPage != regions[i].page) {
						if (lastPage != null) {
							EditorGUILayout.Separator();
							EditorGUILayout.Separator();
						}
						lastPage = regions[i].page;
						Material mat = ((Material)lastPage.rendererObject);
						if (mat != null) {
							
							GUILayout.BeginHorizontal();
							{
								EditorGUI.BeginDisabledGroup(true);
								EditorGUILayout.ObjectField(mat, typeof(Material), false, GUILayout.Width(250));
								EditorGUI.EndDisabledGroup();
								EditorGUI.indentLevel++;
							}
							GUILayout.EndHorizontal();

						} else {
							EditorGUILayout.LabelField(new GUIContent("Page missing material!", SpineEditorUtilities.Icons.warning));
						}
					}
					EditorGUILayout.LabelField(new GUIContent(regions[i].name, SpineEditorUtilities.Icons.image));
				}
				EditorGUI.indentLevel--;
			}
			#endif

			if (serializedObject.ApplyModifiedProperties() ||
				(UnityEngine.Event.current.type == EventType.ValidateCommand && UnityEngine.Event.current.commandName == "UndoRedoPerformed")
			) {
				atlasAsset.Reset();
			}
		}
	}

}
