using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrologueManager : MonoBehaviour
{
    public Button nextButton; // Reference to the Next button
    public TextMeshProUGUI textDisplay; // Reference to the TextMeshProUGUI component

    private AudioSource audioSource; // Reference to the AudioSource component
    private string[] texts = new string[]
    {
    "Aku berjalan melewati hutan yang sunyi dan menemukan rumah itu. Rumah tua ini diwariskan oleh kerabat yang hampir tak kuingat. Tugasku hanya <color=yellow>membersihkannya</color>, menyiapkannya untuk dijual, dan meninggalkannya untuk selamanya.",
    "Ketika aku membuka pintu, bau debu langsung tercium. Lantai berderit di bawah kakiku, dan langkahku bergema di dalam ruangan kosong. Cahaya dari senterku menyinari sudut-sudut gelap, membuat bayangan aneh di dinding.",
    "\"Jangan tinggal sampai malam,\" kata seorang penduduk desa memperingatkan. \"Rumah itu tidak suka tamu, dan mereka bilang <color=yellow>cahaya adalah satu-satunya hal yang bisa membuatnya menjauh.</color>\" Kini, saat bayangan semakin panjang, perasaan tak nyaman mulai muncul.",
    "Aku melihat sebuah jam tanganku, jarumnya berdetak pelan menuju tengah malam. <color=yellow>Jam menunjukkan pukul 03:00 adalah batas waktuku.</color> Aku harus menyelesaikan semuanya sebelum itu, atau mungkin aku tidak akan pernah keluar dari sini dengan selamat."
    };


    private int currentTextIndex = 0; // Keeps track of which text to display

    void Start()
    {
        // Automatically find and initialize components
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found! Please attach an AudioSource component.");
        }

        // Tambahkan ini untuk mencegah AudioSource dihancurkan
        DontDestroyOnLoad(gameObject);

        textDisplay.text = texts[currentTextIndex]; // Display the first text
        nextButton.onClick.AddListener(NextText); // Add listener to the button
    }

    void NextText()
    {
        PlayClickSound(); // Play the click sound

        currentTextIndex++;

        if (currentTextIndex < texts.Length)
        {
            textDisplay.text = texts[currentTextIndex]; // Update the displayed text
        }
        else
        {
            SceneManager.LoadScene("Help"); // Load the next scene when all texts are shown
        }
    }

    private void PlayClickSound()
    {
        if (audioSource != null)
        {
            audioSource.Play(); // Play the sound assigned to the AudioSource
        }
    }
}
