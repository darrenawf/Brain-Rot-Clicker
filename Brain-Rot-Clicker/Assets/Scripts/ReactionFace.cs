using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReactionFace : MonoBehaviour
{
    public List<GameObject> faceChildren = new List<GameObject>();
    private int currentFaceIndex = -1;
    private bool isAnimating = false;
    
    // Animation settings
    public float rotationDuration = 0.3f;
    
    void Start()
    {
        // Hide all faces initially
        HideAllFaces();
    }
    
    void HideAllFaces()
    {
        foreach (GameObject face in faceChildren)
        {
            if (face != null)
                face.SetActive(false);
        }
    }
    
    public void ShowRandomFace()
    {
        if (faceChildren.Count == 0 || isAnimating) return;
        
        StartCoroutine(FaceRotationAnimation());
    }
    
    private IEnumerator FaceRotationAnimation()
    {
        isAnimating = true;
        
        // Hide current face if there is one
        if (currentFaceIndex >= 0 && currentFaceIndex < faceChildren.Count)
        {
            faceChildren[currentFaceIndex].SetActive(false);
        }
        
        // Choose a random new face (different from current if possible)
        int newFaceIndex;
        do
        {
            newFaceIndex = Random.Range(0, faceChildren.Count);
        }
        while (newFaceIndex == currentFaceIndex && faceChildren.Count > 1);
        
        currentFaceIndex = newFaceIndex;
        
        // Show the new face
        if (faceChildren[currentFaceIndex] != null)
        {
            faceChildren[currentFaceIndex].SetActive(true);
        }
        
        isAnimating = false;
        yield return null;
    }
    
    // Quick method to show face without animation (for testing)
    public void ShowFaceInstant(int index)
    {
        if (index >= 0 && index < faceChildren.Count && faceChildren[index] != null)
        {
            HideAllFaces();
            faceChildren[index].SetActive(true);
            currentFaceIndex = index;
        }
    }
    
    // Method to check if animation is playing
    public bool IsShowingFace()
    {
        return isAnimating;
    }
    
    // Method to get current face index
    public int GetCurrentFaceIndex()
    {
        return currentFaceIndex;
    }
    
    // Method to hide current face
    public void HideCurrentFace()
    {
        if (currentFaceIndex >= 0 && currentFaceIndex < faceChildren.Count)
        {
            faceChildren[currentFaceIndex].SetActive(false);
            currentFaceIndex = -1;
        }
    }
}