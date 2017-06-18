using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbEventForwarder : MonoBehaviour
{
    public USBCharacter usb_character;


    void FireSpecial()
    {
        usb_character.gameObject.SendMessage("FireSpecial");
    }


    void SlotDropDone()
    {
        usb_character.gameObject.SendMessage("SlotDropDone");
    }

}
