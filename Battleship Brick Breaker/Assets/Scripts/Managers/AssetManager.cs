using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : Singleton<AssetManager>
{

    [SerializeField] FloatingText [] floatingTexts; 
    [SerializeField] TextFlash [] fireballText = new TextFlash [2];
    
    // Start is called before the first frame update
    void Start()
    {
        floatingTexts = GetComponentsInChildren<FloatingText>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public TextFlash GetFireballTextFlash(bool player1){
        if(player1){
            return fireballText[0];
        }
        else{
            return fireballText[1];
        }
    }
    public FloatingText GetFloatingText(){
        foreach(FloatingText item in floatingTexts){
            if(!item.IsActive){
                return item;
            }
        }
        return null;
    }
}
