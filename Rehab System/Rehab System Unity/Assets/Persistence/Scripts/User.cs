using UnityEngine;
using System.Collections;

public class User : MonoBehaviour
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public UserType Type { get; set; }
}

public enum UserType
{
    Administrator,
    Therapist,
    Patient
}