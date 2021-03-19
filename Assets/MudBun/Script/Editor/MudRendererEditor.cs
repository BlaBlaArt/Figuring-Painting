/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System.Linq;

using UnityEditor;

namespace MudBun
{
  [CustomEditor(typeof(MudRenderer), true)]
  [CanEditMultipleObjects]
  public class MudRendererEditor : MudRendererBaseEditor
  {
    protected override void LockMesh()
    {
      base.LockMesh();

      foreach (var renderer in targets.Select(x => (MudRenderer) x))
      {
        if (renderer == null)
          continue;

        if (renderer.MeshLocked)
          continue;

        var prevMeshGenerationRenderableMeshMode = renderer.MeshGenerationRenderableMeshMode;
        if (MeshGenerationCreateNewObject.boolValue)
          renderer.MeshGenerationRenderableMeshMode = MudRendererBase.RenderableMeshMode.MeshRenderer;

        if (MeshGenerationCreateCollider.boolValue)
        {
          renderer.AddCollider(renderer.gameObject, false, null, MeshGenerationCreateRigidBody.boolValue);
        }

        renderer.LockMesh(MeshGenerationAutoRigging.boolValue, false, null, (MudRendererBase.UVGenerationMode) MeshGenerationUVGeneration.intValue);

        if (MeshGenerationCreateNewObject.boolValue)
          renderer.MeshGenerationRenderableMeshMode = prevMeshGenerationRenderableMeshMode;

        if (MeshGenerationCreateNewObject.boolValue)
        {
          var clone = Instantiate(renderer.gameObject);
          clone.name = renderer.name + " (Locked Mesh Clone)";

          if (MeshGenerationAutoRigging.boolValue)
          {
            var cloneRenderer = clone.GetComponent<MudRenderer>();
            cloneRenderer.RescanBrushersImmediate();
            cloneRenderer.DestoryAllBrushesImmediate();
          }
          else
          {
            DestroyAllChildren(clone.transform);
          }

          Undo.RegisterCreatedObjectUndo(clone, clone.name);
          DestroyImmediate(clone.GetComponent<MudRenderer>());
          Selection.activeObject = clone;

          renderer.UnlockMesh();
        }
      }
    }
  }
}

