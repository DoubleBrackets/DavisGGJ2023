using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/Events/Yarn/YarnVariablesEventChannel", fileName = "NewYarnVariablesEventChannel")]
public class YarnVariablesEventChannelSO : 
    GenericEventChannelSO<
        Dictionary<string, float>,
        Dictionary<string, string>,
        Dictionary<string, bool>>
{

}
