using UnityEngine;
using static Weapon;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }
    public AudioSource ShootingChannel;

    public AudioClip UZIShot;
    public AudioClip M1911Shot;
    public AudioClip AK47Shot;

    public AudioSource reloadingSoundM1911;
    public AudioSource reloadingSoundAK47;
    public AudioSource reloadingSoundUZI;

    public AudioSource emptyMagazineSoundUZI;

    public AudioSource throwablesChannel;
    public AudioClip grenadeSound;

    public AudioSource playerChannel;
    public AudioClip playerHurt;
    public AudioClip playerDie;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void PlayShootingSound(WeaponModel weapon)
    {
        switch(weapon)
        {
            case WeaponModel.M1911:
                ShootingChannel.PlayOneShot(M1911Shot);
                break;
            case WeaponModel.AK47:
                ShootingChannel.PlayOneShot(AK47Shot);
                break;
            case WeaponModel.UZI:
                ShootingChannel.PlayOneShot(UZIShot);
                break;
        }
    }
    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.M1911:
                reloadingSoundM1911.Play();
                break;
            case WeaponModel.AK47:
                reloadingSoundAK47.Play();
                break;
            case WeaponModel.UZI:
                reloadingSoundUZI.Play();
                break;
        }
    }
}
