using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TriLibCore;
using TriLibCore.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class LoadModelManager : MonoBehaviour
{
   [SerializeField] private TMP_Text m_ProgressText;
   [SerializeField] private Button m_LoadButton;
   [SerializeField] private ExportModelManager m_ExportModelManager;

   
   
   private void Start()
   {
      m_LoadButton.onClick.AddListener(LoadModel);
      
      
   }


   /// <summary>
   /// The last loaded GameObject.
   /// </summary>
   private GameObject m_LoadedGameObject;


   private void LoadModel()
   {
      var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true);
      var assetLoaderFilePicker = AssetLoaderFilePicker.Create();
      assetLoaderFilePicker.LoadModelFromFilePickerAsync("Select a Model file", OnLoad, OnMaterialsLoad, OnProgress, OnBeginLoad, OnError, null, assetLoaderOptions);
   }
   
   
   /// <summary>
   /// Called when the Model Meshes and hierarchy are loaded.
   /// </summary>
   /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
   /// <param name="assetLoaderContext">The context used to load the Model.</param>
   private void OnLoad(AssetLoaderContext assetLoaderContext)
   {
      if (m_LoadedGameObject != null)
      {
         Destroy(m_LoadedGameObject);
      }
      m_LoadedGameObject = assetLoaderContext.RootGameObject;
      if (m_LoadedGameObject != null)
      {
         Camera.main.FitToBounds(assetLoaderContext.RootGameObject, 2f);
         m_ExportModelManager.m_ObjectToExport = m_LoadedGameObject;
      }
   }
   
   /// <summary>
   /// Called when the Model (including Textures and Materials) has been fully loaded.
   /// </summary>
   /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
   /// <param name="assetLoaderContext">The context used to load the Model.</param>
   private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
   {
      Debug.Log(assetLoaderContext.RootGameObject != null ? "Model fully loaded." : "Model could not be loaded.");
      m_LoadButton.interactable = true;
      m_ProgressText.text = "Model Loaded";
      m_ProgressText.enabled = true;
   }
   
   /// <summary>
   /// Called when the Model loading progress changes.
   /// </summary>
   /// <param name="assetLoaderContext">The context used to load the Model.</param>
   /// <param name="progress">The loading progress.</param>
   private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
   {
      m_ProgressText.text = $"Progress: {progress:P}";
   }
   
   /// <summary>
   /// Called when the the Model begins to load.
   /// </summary>
   /// <param name="filesSelected">Indicates if any file has been selected.</param>
   private void OnBeginLoad(bool filesSelected)
   {
      m_LoadButton.interactable = !filesSelected;
      m_ProgressText.enabled = filesSelected;
   }
   
   /// <summary>
   /// Called when any error occurs.
   /// </summary>
   /// <param name="obj">The contextualized error, containing the original exception and the context passed to the method where the error was thrown.</param>
   private void OnError(IContextualizedError obj)
   {
      Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
   }
}
