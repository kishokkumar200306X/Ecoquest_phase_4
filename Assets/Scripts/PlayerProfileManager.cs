using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; // This line is crucial
using TMPro; // TextMeshPro is used for input fields
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class PlayerProfileManager : MonoBehaviour
{
    public TMP_InputField firstNameField;
    public TMP_InputField lastNameField;
    public TMP_InputField nicField;
    public TMP_InputField usernameField;
    public TMP_InputField mobileNumberField;
    public TMP_InputField emailField;

    public Button updateButton;
    private string token;

    [SerializeField] private Button _mainMenuButton = null;
    [SerializeField] private GameObject _updatedMsg = null;
    [SerializeField] private GameObject _nullWarning = null;

    public string fetched_username;

    private bool isUserRegistered = false;

    public void MainMenuButton()
    {
        SceneManager.LoadScene(2);
    }

    void Start()
    {

        token = PlayerPrefs.GetString("Token", "");
        // token = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJvdmVyc2lnaHRfZzE3IiwiaWF0IjoxNzE2NDYzNTk2LCJleHAiOjE3MTY0OTk1OTZ9.VFidHeocccciiff0TLMByQEWcCspSvwCr6W-lbasXLYMpObl7yqV2yIpIEGJJyUsUL_T71lTcVeE_Fyn7HckEA";
        Debug.Log("Token: " + token);
        if (!string.IsNullOrEmpty(token))
        {
            StartCoroutine(FetchPlayerProfile());
        }
        else
        {
            Debug.LogError("Token is not available.");
        }

        updateButton.onClick.AddListener(SubmitProfileUpdate);
    }

    IEnumerator FetchPlayerProfile()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://20.15.114.131:8080/api/user/profile/view");
        www.SetRequestHeader("Authorization", "Bearer " + token);


        yield return www.SendWebRequest();


        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;

            PlayerProfileResponse response = JsonUtility.FromJson<PlayerProfileResponse>(jsonResponse);

            Fethched_PlayerProfileResponse fethched_response = JsonUtility.FromJson<Fethched_PlayerProfileResponse>(jsonResponse);


            fetched_username = fethched_response.user.username;
            //Fethched_PlayerProfileResponse fethched_response = response;

            //DisplayProfile(response);
            if (response != null && response.user != null)
            {
                //DisplayProfile(response.user);
                if (fethched_response != null && fethched_response.user != null)
                {
                    fetched_username = fethched_response.user.username;
                    Debug.Log("Fetched username: " + fetched_username);
                    DisplayProfile(response.user);
                }
                else
                {
                    if (fethched_response.user != null)
                    {
                        Debug.Log("user not null");
                    }
                    Debug.LogError("Fetched response is invalid.");
                }
            }
            else
            {
                Debug.LogError("Invalid profile response.");
            }
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    void DisplayProfile(PlayerProfile profile)
    {
        // Set the input fields with the profile information
        firstNameField.text = profile.firstname ?? "";
        lastNameField.text = profile.lastname ?? "";
        nicField.text = profile.nic ?? "";
        usernameField.text = fetched_username;
        mobileNumberField.text = profile.phoneNumber ?? "";
        emailField.text = profile.email ?? "";
        //profilePictureUrlField.text = profile.profilePictureUrl ?? ""; // Set the profile picture URL

        // Log the profile information
        /*
        Debug.Log($"Profile Information Received: \n" +
                  $"First Name: {profile.firstname}\n" +
                  $"Last Name: {profile.lastname}\n" +
                  $"NIC: {profile.nic}\n" +
                  $"Username: {profile.username}\n" +
                  $"Mobile Number: {profile.phoneNumber}\n" +
                  $"Email: {profile.email}\n" +
                  $"Profile Picture URL: {profile.profilePictureUrl}");
        */
    }


    public void SubmitProfileUpdate()
    {


        PlayerProfile updatedProfile = new PlayerProfile
        {
            firstname = firstNameField.text,
            lastname = lastNameField.text,
            nic = nicField.text,
            //username = usernameField.text,
            phoneNumber = mobileNumberField.text,
            email = emailField.text,
            //profilePictureUrl = profilePictureUrlField.text // Include profile picture URL
        };


        // Define the dummy data
        //
        PlayerProfile dummyProfile = new PlayerProfile
        {
            firstname = "John",
            lastname = "Doe",
            nic = "200004102260",
            phoneNumber = "1234567890",
            email = "johndoe@example.com"
        };

        if (updatedProfile.firstname == "" || updatedProfile.lastname == "" || updatedProfile.nic == "" || updatedProfile.phoneNumber == "" || updatedProfile.email == "")
        {
            _nullWarning.SetActive(true);
            Debug.Log("Please fill all the fields.");
            return;
        }


        string jsonData = JsonUtility.ToJson(updatedProfile);//updatedProfiledummyProfile

        // Print the JSON data to be sent
        Debug.Log($"JSON Data to be sent for profile update: {jsonData}");

        StartCoroutine(ValidateUserCoroutine(fetched_username, (isRegistered) =>
        {
            isUserRegistered = isRegistered;

            if (!isUserRegistered)
            {
                Debug.Log("User is not registered.");
                RedirectToWebPage();
            }
            else
            {
                Debug.Log("User is registered.");
            }
        }));


        StartCoroutine(UpdatePlayerProfile(jsonData));

    }


    IEnumerator UpdatePlayerProfile(string jsonData)
    {
        UnityWebRequest www = UnityWebRequest.Put("http://20.15.114.131:8080/api/user/profile/update", jsonData);
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        www.uploadHandler.contentType = "application/json";
        www.SetRequestHeader("Authorization", "Bearer " + token);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            _nullWarning.SetActive(false);
            _updatedMsg.SetActive(true);
            Debug.Log("Profile updated successfully.");
            StartCoroutine(DeactivateMessageAfterDelay(3f));
            _mainMenuButton.interactable = true;
        }
        else
        {
            // Improved error handling
            string responseBody = www.downloadHandler?.text;
            Debug.LogError($"Failed to update profile. Server responded with status code: {www.responseCode} and message: {responseBody}");
        }
    }

    private IEnumerator DeactivateMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _updatedMsg.SetActive(false);
    }

    private IEnumerator ValidateUserCoroutine(string inputUsername, System.Action<bool> callback)
    {
        //string url = $"https://ecoquest-420605.el.r.appspot.com//user/getuser/{inputUsername}";
        string url = "https://ecoquest-420605.el.r.appspot.com/user/getuser/{userName}?userName="+inputUsername;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Authorization", "Bearer " + token);

        PlayerPrefs.SetString("Username", inputUsername);
        PlayerPrefs.Save();
        Debug.Log("Sending GET request to: " + url); // Log the URL being requested

        yield return www.SendWebRequest();

        Debug.Log(www.responseCode);
        /*
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("TTTT");
            Debug.LogError($"Error validating registered user: {www.error}");
            yield return false;
        }
        */
        if (www.responseCode == 200) // OK
        {
            _mainMenuButton.interactable = true;

            string jsonResponse = www.downloadHandler.text;
            Debug.Log($"Received response: {jsonResponse}");

            // Deserialize the JSON response
            UserResponse userResponse = JsonUtility.FromJson<UserResponse>(jsonResponse);

            // Extract the username from the response
            string fetchedUsername = userResponse?.userName;

            Debug.Log("User already exisiting");

            yield return true;

        }
        else if (www.responseCode == 404) // Not Found
        {
            _mainMenuButton.interactable = false;
            //Debug.Log("DDDDDD");
            Debug.Log($"User '{inputUsername}' is not registered.");
            RedirectToWebPage();
            yield return false;
        }
        else
        {
            Debug.Log($"Received unexpected response code {www.responseCode}: {www.downloadHandler.text}");
            yield return false;
        }
    }


    [System.Serializable]
    public class UserResponse
    {
        public string userName;
    }


    // Add this method to open a URL in the web browser
    public void RedirectToWebPage()
    {
        string redirect_url = $"https://ecoquest-mcq.vercel.app/eq-home/{fetched_username}";
        Application.OpenURL(redirect_url);

    }





}

[System.Serializable]
public class PlayerProfile
{
    public string firstname;
    public string lastname;
    public string nic;
    //public string username;
    public string phoneNumber;
    public string email;
    //public string profilePictureUrl; // New field for profile picture URL
}

[System.Serializable]
public class Fethched_PlayerProfile
{
    public string firstname;
    public string lastname;
    public string username;
    public string nic;
    public string phoneNumber;
    public string email;
    //public string profilePictureUrl; // New field for profile picture URL
}

[System.Serializable]
public class PlayerProfileResponse
{
    public PlayerProfile user;
}

[System.Serializable]
public class Fethched_PlayerProfileResponse
{
    public Fethched_PlayerProfile user;
}
