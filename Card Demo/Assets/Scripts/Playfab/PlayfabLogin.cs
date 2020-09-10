using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LoginResult = PlayFab.ClientModels.LoginResult;


public class PlayfabLogin : MonoBehaviour
{
    private string userEmail;
    private string userName;
    private string userPassword;
    private string _message;
    public GameObject LoginPanel;

    public enum MyUser
    {
        EMAIL,
        PASSWORD,
        USERNAME
    }

    public void Start()
    {

       
        if (PlayerPrefs.HasKey(MyUser.EMAIL.ToString()) && PlayerPrefs.HasKey(MyUser.PASSWORD.ToString()))
        {
            OnClickLogin();
        }
        else
        {
            LoginPanel.SetActive(true);
        }
    }
    

    private void OnLoginSuccess(LoginResult result)
    {
        PlayerPrefs.SetString(MyUser.EMAIL.ToString(), userEmail);
        PlayerPrefs.SetString(MyUser.PASSWORD.ToString(), userPassword);

        Debug.Log("Congratulations, you made your first successful API call!");
        LoginPanel.SetActive(false);
        GameManager.PlafabID = result.PlayFabId;


        var getUserName = new GetAccountInfoRequest();
        getUserName.PlayFabId = GameManager.PlafabID;
        PlayFabClientAPI.GetAccountInfo(getUserName, OnGetAccountInfoSuccess, OnGetAccountInfoFailure);

        var displayName = new UpdateUserTitleDisplayNameRequest();
        displayName.DisplayName = "Guest";
        PlayFabClientAPI.UpdateUserTitleDisplayName(displayName, OnSetDisplayNameSuccess, OnSetDisplayNameFailure);

    }


    private void OnGetAccountInfoFailure(PlayFabError obj)
    {
        Debug.Log("Get AccountInfo Set Failure");
    }


    private void OnGetAccountInfoSuccess(GetAccountInfoResult obj)
    {
        Debug.Log("Get AccountInfo Set Success");
        Debug.Log("Username :" +obj.AccountInfo.Username);
        GameManager.username = obj.AccountInfo.Username.ToString();
    }
      

    private void OnSetDisplayNameSuccess(UpdateUserTitleDisplayNameResult obj)
    {
        Debug.Log("Username Set Success");
        SceneManager.LoadScene("InGameScene");
    }


    private void OnSetDisplayNameFailure(PlayFabError obj)
    {
        Debug.LogWarning("Username Set Failure");
    }


    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.");
       
    }


    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {

        PlayerPrefs.SetString(MyUser.EMAIL.ToString() , userEmail);
        PlayerPrefs.SetString(MyUser.PASSWORD.ToString() , userPassword);
       
        Debug.Log("Congratulations, your registration is complete");
        
        GameManager.PlafabID = result.PlayFabId;
        string coins = "100";
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {

            Data = new Dictionary<string, string>()
            {
                { GameManager.Datatypes.Coins.ToString() , coins}
            }
        }, SetUserDataSuccess, SetuserDataFailure) ;


    }

   

    private void SetUserDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log("User Data Set Success");
    }



    private void SetuserDataFailure(PlayFabError result)
    {
        Debug.LogWarning("User Data Set Failed");

    }



    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.Log("Something went wrong with your first API call.");
    }


    public void RegisterNewUser()
    {
        Debug.LogError("Registering User");

        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail , Password = userPassword , Username = userName };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest , OnRegisterSuccess , OnRegisterFailure );
    
    }

    
    public void getUserEmail(string useremail)
    {
        userEmail = useremail;
    }
    

    public void getUserPassword(string password)
    {
        userPassword = password;
    }
    
    
    public void getUserName(string username)
    {
        userName = username;
    }


    public void OnClickLogin()
    {
        if (PlayerPrefs.HasKey(MyUser.EMAIL.ToString()) && PlayerPrefs.HasKey(MyUser.PASSWORD.ToString()))
        {
            userEmail = PlayerPrefs.GetString(MyUser.EMAIL.ToString());
            userPassword = PlayerPrefs.GetString(MyUser.PASSWORD.ToString());
            Debug.Log("From PlayerPrefs");
            Debug.Log("User Email   : " +userEmail +"       ; User Password     :" +userPassword);
            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        else
        {
            Debug.Log("User Email   : " + userEmail + "       ; User Password     :" + userPassword);

            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);

        }


    }


    public void LogInWithDesk()
    {
        FB.Init(OnFacebookInitialized);
    }


    private void OnFacebookInitialized()
    {
        SetMessage("Logging into Facebook...");

        // Once Facebook SDK is initialized, if we are logged in, we log out to demonstrate the entire authentication cycle.
        if (FB.IsLoggedIn)
            FB.LogOut();

        // We invoke basic login procedure and pass in the callback to process the result
        FB.LogInWithReadPermissions(null, OnFacebookLoggedIn);
    }


    private void OnFacebookLoggedIn(ILoginResult result)
    {
        // If result has no errors, it means we have authenticated in Facebook successfully
        if (result == null || string.IsNullOrEmpty(result.Error))
        {
            SetMessage("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");

            /*
             * We proceed with making a call to PlayFab API. We pass in current Facebook AccessToken and let it create
             * and account using CreateAccount flag set to true. We also pass the callback for Success and Failure results
             */
            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { CreateAccount = true, AccessToken = AccessToken.CurrentAccessToken.TokenString },
                OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
        }
        else
        {
            // If Facebook authentication failed, we stop the cycle with the message
            SetMessage("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult, true);
        }
    }


    // When processing both results, we just set the message, explaining what's going on.
    private void OnPlayfabFacebookAuthComplete(LoginResult result)
    {
        SetMessage("PlayFab Facebook Auth Complete. Session ticket: " + result.SessionTicket);
    }


    private void OnPlayfabFacebookAuthFailed(PlayFabError error)
    {
        SetMessage("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport(), true);
    }


    public void SetMessage(string message, bool error = false)
    {
        _message = message;
        if (error)
            Debug.LogError(_message);
        else
            Debug.Log(_message);
    }


    public void OnGUI()
    {
        var style = new GUIStyle { fontSize = 40, normal = new GUIStyleState { textColor = Color.white }, alignment = TextAnchor.MiddleCenter, wordWrap = true };
        var area = new Rect(0, 0, Screen.width, Screen.height);
        GUI.Label(area, _message, style);
    }


}
