using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

public class CloudFirestore : MonoBehaviour
{
    FirebaseApp firebaseApp;
    FirebaseFirestore db;

    void Awake()
    {
        firebaseApp = GameObject.FindObjectOfType<FirebaseApp>();
    }

    void OnEnable()
    {
        firebaseApp.OnSetFirebase.AddListener(InitializeData);
    }

    void OnDisable()
    {
        firebaseApp.OnSetFirebase.RemoveListener(InitializeData);
    }

    void InitializeData()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    /// <summary>
    /// Creates a new user in the database with the provided user data.
    /// </summary>
    /// <param name="userData">A dictionary containing user data.</param>
    public void CreateNewUser(Dictionary<string, object> userData)
    {
        DocumentReference userRef = db.Collection("Users").Document(userData["id"].ToString());

        userRef.SetAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {

            }
            else
            {

            }
        });
    }

    /// <summary>
    /// Checks if a user with the given user ID exists in the database.
    /// </summary>
    /// <param name="userId">The ID of the user to check.</param>
    /// <returns>True if the user exists, false otherwise.</returns>
    async public Task<bool> CheckUserExists(string userId)
    {
        DocumentReference userRef = db.Collection("Users").Document(userId);

        DocumentSnapshot snapshot = await userRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            LoginController loginController = FindObjectOfType<LoginController>();
            loginController.CurrentGenderUser = (GenderUser)Enum.Parse(typeof(GenderUser), snapshot.GetValue<string>("gender"));
            return true;
        }

        return false;
    }
}