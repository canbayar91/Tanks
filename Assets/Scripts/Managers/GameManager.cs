using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject m_TankPrefab;         
    public TankManager[] m_Tanks;           

    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;       

    private void Start() {

        // Create the delays
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        // Set up the scene
        SpawnAllTanks();
        SetCameraTargets();

        // Start the game
        StartCoroutine(GameLoop());
    }

    private void SpawnAllTanks() {

        // Instantiate all tanks
        for (int i = 0; i < m_Tanks.Length; i++) {
            m_Tanks[i].m_Instance = Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }

    private void SetCameraTargets() {

        // Set the targets to the appropriate tank transform
        Transform[] targets = new Transform[m_Tanks.Length];
        for (int i = 0; i < targets.Length; i++) {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        // Set the targets for camera to follow
        m_CameraControl.m_Targets = targets;
    }

    private IEnumerator GameLoop() {

        // Run the gameplay coroutines where each one waits for the previous one to end
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        // Restart the game or start another round
        if (m_GameWinner != null) { 
            SceneManager.LoadScene(0);
        } else {
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator RoundStarting() {

        // Reset the tanks
        ResetAllTanks();
        DisableTankControl();

        // Reset the camera
        m_CameraControl.SetStartPositionAndSize();

        // Increment the round number
        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        // Wait for 3 seconds to return to the game loop
        yield return m_StartWait;
    }

    private IEnumerator RoundPlaying() {

        // Enable the controls
        EnableTankControl();

        // Clear the message
        m_MessageText.text = string.Empty;

        // Infinitely loop until one tank is left
        while (!OneTankLeft()) {
            yield return null;
        }
    }

    private IEnumerator RoundEnding() {

        // Disable the controls
        DisableTankControl();

        // Get the round winner and update the number of wins
        m_RoundWinner = GetRoundWinner();
        if (m_RoundWinner != null) {
            m_RoundWinner.m_Wins++;
        }

        // Get the game winner and update the message
        m_GameWinner = GetGameWinner();
        m_MessageText.text = EndMessage();

        // Wait for 3 seconds to return to the game loop
        yield return m_EndWait;
    }

    private bool OneTankLeft()  {

        // Count the active tanks
        int numTanksLeft = 0;
        for (int i = 0; i < m_Tanks.Length; i++) {
            if (m_Tanks[i].m_Instance.activeSelf) {
                numTanksLeft++;
            }
        }

        return numTanksLeft <= 1;
    }

    private TankManager GetRoundWinner() {

        // The active tanks is the winner
        for (int i = 0; i < m_Tanks.Length; i++) {
            if (m_Tanks[i].m_Instance.activeSelf) {
                return m_Tanks[i];
            }
                
        }

        // Return nulls if somehow both tanks are destroyed simultenously
        return null;
    }


    private TankManager GetGameWinner()  {

        // Check if any of the tanks reached the score limit
        for (int i = 0; i < m_Tanks.Length; i++) {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin) {
                return m_Tanks[i];
            }
        }

        return null;
    }

    private string EndMessage() {

        // Default message
        string message = "DRAW!";

        // Round winner message
        if (m_RoundWinner != null) {
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";
        }

        // Scores
        message += "\n\n\n\n";
        for (int i = 0; i < m_Tanks.Length; i++) {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        // Game winner message
        if (m_GameWinner != null) {
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";
        }

        return message;
    }

    private void ResetAllTanks() {

        // Reset the tanks
        for (int i = 0; i < m_Tanks.Length; i++) {
            m_Tanks[i].Reset();
        }
    }

    private void EnableTankControl()  {

        // Enable the tank controls
        for (int i = 0; i < m_Tanks.Length; i++) {
            m_Tanks[i].EnableControl();
        }
    }

    private void DisableTankControl() {

        // Disable the tank controls
        for (int i = 0; i < m_Tanks.Length; i++) {
            m_Tanks[i].DisableControl();
        }
    }
}