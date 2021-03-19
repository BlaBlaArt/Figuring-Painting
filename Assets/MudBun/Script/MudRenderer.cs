/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace MudBun
{
  [ExecuteInEditMode]
  public class MudRenderer : MudRendererBase
  {
    protected override void OnSharedMaterialChanged(UnityEngine.Object material)
    {
      foreach (var renderer in s_renderers)
      {
        if (renderer.SharedMaterial == material)
          renderer.MarkNeedsCompute();

        foreach (var b in renderer.Brushes)
        {
          var m = b.GetComponent<MudMaterial>();
          if (m != null && m.SharedMaterial != null && m.SharedMaterial == material)
            b.MarkDirty();
        }
      }
    }

    protected override void OnValidate()
    {
      base.OnValidate();

      #if UNITY_EDITOR
      EditorApplication.QueuePlayerLoopUpdate();
      #endif
    }

    private T AddComponentHelper<T>(GameObject go) where T : Component
    {
      var comp = go.GetComponent<T>();
      if (comp == null)
      {
        #if UNITY_EDITOR
        comp = Undo.AddComponent<T>(go);
        #else
        comp = go.AddComponent<T>();
        #endif
      }
      else
      {
        #if UNITY_EDITOR
        Undo.RecordObject(comp, comp.name);
        #endif
      }

      if (m_addedComponents == null)
        m_addedComponents = new HashSet<Type>();

      m_addedComponents.Add(typeof(T));

      return comp;
    }

    private void RemoveComponentHelper<T>(GameObject go) where T : Component
    {
      // if not added, don't remove it
      if (m_addedComponents == null 
          || !m_addedComponents.Contains(typeof(T)))
        return;

      var comp = go.GetComponent<T>();
      if (comp != null)
      {
        #if UNITY_EDITOR
        Undo.DestroyObjectImmediate(comp);
        #else
        Destroy(comp);
        #endif
      }
    }

    public override Mesh AddCollider
    (
      GameObject go, 
      bool async, 
      Mesh mesh = null, 
      bool makeRigidBody = false
    )
    {
      var comp = AddComponentHelper<MeshCollider>(go);
      mesh = GenerateMesh(GeneratedMeshType.Collider, async, mesh);
      comp.sharedMesh = mesh;

      if (makeRigidBody)
      {
        comp.convex = true;
        AddComponentHelper<Rigidbody>(go);
      }

      return mesh;
    }

    public override Mesh AddLockedStandardMesh
    (
      GameObject go, 
      bool autoRigging, 
      bool async, 
      Mesh mesh = null, 
      UVGenerationMode uvGeneration = UVGenerationMode.None
    )
    {
      #if UNITY_EDITOR
      Undo.RecordObject(this, name);
      #endif

      var transformStack = new Stack<Transform>();
      transformStack.Push(transform);
      while (transformStack.Count > 0)
      {
        var t = transformStack.Pop();
        if (t == null)
          continue;
        
        #if UNITY_EDITOR
        Undo.RecordObject(t, t.name);
        #endif

        for (int i = 0; i < t.childCount; ++i)
          transformStack.Push(t.GetChild(i));
      }

      m_doRigging = autoRigging;
      Transform [] aBone;
      mesh = GenerateMesh(GeneratedMeshType.Standard, go.transform, out aBone, async, mesh, uvGeneration);
      m_doRigging = false;

      Material material = ResourcesUtil.DefaultLockedMeshMaterial;

      if (autoRigging)
      {
        var meshRenderer = AddComponentHelper<SkinnedMeshRenderer>(go);
        meshRenderer.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;
        meshRenderer.bones = aBone;
        meshRenderer.rootBone = go.transform;
      }
      else
      {
        var meshFilter = AddComponentHelper<MeshFilter>(go);
        var meshRenderer = AddComponentHelper<MeshRenderer>(go);
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;
      }

      #if UNITY_EDITOR
      EditorApplication.QueuePlayerLoopUpdate();
      #endif

      return mesh;
    }

    private LockMeshIntermediateStateEnum m_lockMeshIntermediateState = LockMeshIntermediateStateEnum.Idle;
    protected override LockMeshIntermediateStateEnum LockMeshIntermediateState => m_lockMeshIntermediateState;

    [SerializeField] [HideInInspector] private HashSet<Type> m_addedComponents;

    public override void LockMesh
    (
      bool autoRigging, 
      bool async, 
      Mesh mesh = null, 
      UVGenerationMode uvGeneration = UVGenerationMode.None
    )
    {
      m_lockMeshIntermediateState = LockMeshIntermediateStateEnum.PreLock;

      #if UNITY_EDITOR
      Undo.RecordObject(this, "Lock Mesh (" + name + ")");
      #endif

      base.LockMesh(autoRigging, async, mesh, uvGeneration);

      #if UNITY_EDITOR
      Undo.FlushUndoRecordObjects();
      #endif

      switch (MeshGenerationRenderableMeshMode)
      {
      case RenderableMeshMode.None:
        break;

      case RenderableMeshMode.Procedural:
        MarkNeedsCompute();
        break;

      case RenderableMeshMode.MeshRenderer:
        AddLockedStandardMesh(gameObject, autoRigging, async, mesh, uvGeneration);
        if (!async)
          DisposeLocalResources();
        break;
      }

      m_lockMeshIntermediateState = LockMeshIntermediateStateEnum.PostLock;
    }

    public override void UnlockMesh()
    {
      m_lockMeshIntermediateState = LockMeshIntermediateStateEnum.PreUnlock;

      #if UNITY_EDITOR
      Undo.RecordObject(this, "Unlock Mesh (" + name + ")");
      #endif

      base.UnlockMesh();

      #if UNITY_EDITOR
      Undo.FlushUndoRecordObjects();
      #endif

      RemoveComponentHelper<MeshCollider>(gameObject);
      RemoveComponentHelper<Rigidbody>(gameObject);
      RemoveComponentHelper<MeshFilter>(gameObject);
      RemoveComponentHelper<MeshRenderer>(gameObject);
      RemoveComponentHelper<SkinnedMeshRenderer>(gameObject);
      RemoveComponentHelper<MudLockedMeshRenderer>(gameObject);
      RemoveComponentHelper<MudStandardMeshRenderer>(gameObject);

      m_lockMeshIntermediateState = LockMeshIntermediateStateEnum.Idle;

      MeshGenerationLockOnStartByEditor = false;

      m_addedComponents = null;
    }

    protected override void GenerateUV(Mesh mesh, UVGenerationMode mode)
    {
      switch (mode)
      {
        case UVGenerationMode.PerTriangleEditorOnly:
        #if UNITY_EDITOR
           mesh.SetUVs(0, Unwrapping.GeneratePerTriangleUV(mesh));
          break;
        #endif
        //case UVGenerationMode.Spherical:
          // TODO
          //break;
      }
    }

    //-------------------------------------------------------------------------

#if UNITY_EDITOR

    protected override void OnEnable()
    {
      base.OnEnable();

      RegisterEditorEvents();
    }

    protected override void OnDisable()
    {
      base.OnDisable();

      UnregisterEditorEvents(); 
    }

    private void OnHierarchyChanged()
    {
      if (MeshLocked)
        return;

      NotifyHierarchyChange();
    }

    private void OnEditorUpdate()
    {
      if (IsAnyMeshGenerationPending)
        EditorApplication.QueuePlayerLoopUpdate();
    }

    private void OnVisibilityChanged()
    {
      bool needsCompute = false;
      foreach (var b in Brushes)
      {
        bool isHidden = SceneVisibilityManager.instance.IsHidden(b.gameObject);
        if (isHidden != b.Hidden)
          needsCompute = true;

        b.Hidden = isHidden;
      }

      if (needsCompute)
      {
        ForceCompute();
        EditorApplication.QueuePlayerLoopUpdate();
      }
    }

    private void OnSceneSaved(UnityEngine.SceneManagement.Scene scene)
    {
      MarkNeedsCompute();
    }

    private void OnUndoPerformed()
    {
      MarkNeedsCompute();
    }

    private void OnBeforeAssemblyReload()
    {
      DisposeGlobalResources();
      DisposeLocalResources();
    }

    private void OnAfterAssemblyReload()
    {

    }

    private void RegisterEditorEvents()
    { 
      EditorApplication.hierarchyChanged += OnHierarchyChanged;
      EditorApplication.update += OnEditorUpdate;
      SceneVisibilityManager.visibilityChanged += OnVisibilityChanged;
      UnityEditor.SceneManagement.EditorSceneManager.sceneSaved += OnSceneSaved;
      Undo.undoRedoPerformed += OnUndoPerformed;
      AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
      AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    private void UnregisterEditorEvents()
    {
      EditorApplication.hierarchyChanged -= OnHierarchyChanged;
      EditorApplication.update -= OnEditorUpdate;
      SceneVisibilityManager.visibilityChanged -= OnVisibilityChanged;
      UnityEditor.SceneManagement.EditorSceneManager.sceneSaved -= OnSceneSaved;
      Undo.undoRedoPerformed -= OnUndoPerformed;
      AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
      AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }

    protected override bool IsEditorBusy()
    {
      if (EditorApplication.isCompiling)
        return true;

      if (EditorApplication.isUpdating)
        return true;

      return false;
    }

    public override void ReloadShaders()
    {
      base.ReloadShaders();

      EditorApplication.QueuePlayerLoopUpdate();
      SceneView.RepaintAll();
    }

    bool BrushesSelected
    {
      get
      {
        foreach (var selectedGo in Selection.gameObjects)
        {
          var b = selectedGo?.GetComponent<MudBrush>();
          if (Brushes.Contains(b))
            return true;
        }

        return false;
      }
    }

    private void OnDrawGizmosSelected()
    {
      if (AlwaysDrawGizmos && BrushesSelected)
        return;

      DrawGizmos();
    }

    private void OnDrawGizmos()
    {
      if (!AlwaysDrawGizmos && !BrushesSelected)
        return;

      DrawGizmos();
    }

    private void DrawGizmos()
    {
      if (IsEditorBusy())
        return;

      if (MeshLocked)
        return;

      Color prevColor = Gizmos.color;
      
      Gizmos.matrix = transform.localToWorldMatrix;

      foreach (var b in Brushes)
      {
        Gizmos.color = GizmosUtil.OutlineDefault;
        b.DrawGizmosRs();
      }

      if (DrawRawBrushBounds)
      {
        Gizmos.color = Color.white;
        foreach (var b in Brushes)
        {
          Aabb bounds = b.Bounds;
          Gizmos.DrawWireCube(bounds.Center, bounds.Extents);
        }
      }

      if (DrawComputeBrushBounds)
      {
        Gizmos.color = Color.yellow;
        m_aabbTree.ForEach(bounds => Gizmos.DrawWireCube(bounds.Center, bounds.Extents));
      }

      if (DrawVoxelNodes)
      {
        Gizmos.color = Color.gray;
        var aNumAllocated = new int[m_numNodesAllocatedBuffer.count];
        m_numNodesAllocatedBuffer.GetData(aNumAllocated);
        int numTotalNodes = aNumAllocated[0];
        var aNode = new VoxelNode[numTotalNodes];
        m_nodePoolBuffer.GetData(aNode);
        var aNodeSize = NodeSizes;
        int iNode = 0;
        for (int depth = 0; depth <= VoxelNodeDepth; ++depth)
        {
          int numNodesInDepth = Mathf.Min(aNumAllocated[depth + 1], aNode.Length);

          if (DrawVoxelNodesDepth >= 0 && depth != DrawVoxelNodesDepth)
          {
            iNode += numNodesInDepth;
            continue;
          }

          float nodeSize = aNodeSize[depth];
          for (int i = 0; i < numNodesInDepth && iNode < aNode.Length; ++i)
          {
            Gizmos.DrawWireCube(aNode[iNode++].Center, DrawVoxelNodesScale * nodeSize * Vector3.one);
          }
        }
      }

      Gizmos.matrix = Matrix4x4.identity;

      if (DrawRenderBounds)
      {
        Gizmos.color = Color.cyan;
        Aabb bounds = RenderBounds;
        Gizmos.DrawWireCube(bounds.Center, bounds.Extents);
      }

      Gizmos.color = prevColor;
    }

#endif
  }
}
