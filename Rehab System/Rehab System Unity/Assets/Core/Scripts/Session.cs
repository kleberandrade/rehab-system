using UnityEngine;
using System.Collections;

public class Session : Singleton<User>
{
    public User User { get; set; }
}
